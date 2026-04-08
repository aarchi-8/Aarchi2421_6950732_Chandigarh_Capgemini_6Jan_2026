using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartHealthcare.API.Data;
using SmartHealthcare.API.Helpers;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.Models.DTOs;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly JwtHelper _jwtHelper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, IPatientRepository patientRepository, 
            IDoctorRepository doctorRepository, AppDbContext context,
            IMapper mapper, JwtHelper jwtHelper, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _context = context;
            _mapper = mapper;
            _jwtHelper = jwtHelper;
            _logger = logger;
        }

        public async Task<AuthResponseDTO?> LoginAsync(LoginDTO loginDTO)
        {
            _logger.LogInformation("Login attempt for {Email}", loginDTO.Email);

            var user = await _userRepository.GetByEmailAsync(loginDTO.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.PasswordHash))
            {
                _logger.LogWarning("Failed login attempt for {Email}", loginDTO.Email);
                return null;
            }

            var token = _jwtHelper.GenerateToken(user);
            var refreshToken = _jwtHelper.GenerateRefreshToken();
            var refreshTokenExpiry = _jwtHelper.GetRefreshTokenExpiry();

            // Store refresh token in database
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = refreshTokenExpiry;
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("Successful login for {Email}", loginDTO.Email);

            return new AuthResponseDTO
            {
                Token = token,
                RefreshToken = refreshToken,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                Expiry = _jwtHelper.GetExpiry()
            };
        }

        /// <summary>
        /// Refresh the JWT token using a valid refresh token
        /// </summary>
        public async Task<AuthResponseDTO?> RefreshTokenAsync(string refreshToken)
        {
            _logger.LogInformation("Token refresh attempt");

            var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);
            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                _logger.LogWarning("Invalid or expired refresh token");
                return null;
            }

            var newToken = _jwtHelper.GenerateToken(user);
            var newRefreshToken = _jwtHelper.GenerateRefreshToken();
            var newRefreshTokenExpiry = _jwtHelper.GetRefreshTokenExpiry();

            // Update refresh token
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = newRefreshTokenExpiry;
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("Token refreshed successfully for {Email}", user.Email);

            return new AuthResponseDTO
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                Expiry = _jwtHelper.GetExpiry()
            };
        }

        public async Task<UserDTO?> RegisterAsync(RegisterDTO registerDTO)
        {
            if (await _userRepository.EmailExistsAsync(registerDTO.Email))
            {
                _logger.LogWarning("Registration failed - email already exists: {Email}", registerDTO.Email);
                return null;
            }

            var user = new User
            {
                FullName = registerDTO.FullName,
                Email = registerDTO.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password),
                Role = registerDTO.Role,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _userRepository.CreateAsync(user);
            _logger.LogInformation("New user registered: {Email}, UserId: {UserId}, Role: {Role}", 
                registerDTO.Email, created.UserId, registerDTO.Role);

            // Create patient profile if user registered as Patient
            if (registerDTO.Role.ToLower() == "patient")
            {
                try
                {
                    var patient = new Patient
                    {
                        UserId = created.UserId,
                        Phone = "+1234567890", // Provide valid phone
                        DateOfBirth = DateTime.Now.AddYears(-30), // Provide valid DOB
                        Gender = Models.Enums.Gender.Male,
                        Address = "Not Provided"
                    };
                    var patientResult = await _patientRepository.CreateAsync(patient);
                    _logger.LogInformation("Patient profile created successfully - PatientId: {PatientId}, UserId: {UserId}", 
                        patientResult.PatientId, created.UserId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating patient profile for UserId: {UserId}", created.UserId);
                    // Don't throw - user account is created even if patient profile fails
                }
            }

            // Create doctor profile if user registered as Doctor
            if (registerDTO.Role.ToLower() == "doctor")
            {
                try
                {
                    var doctor = new Doctor
                    {
                        UserId = created.UserId,
                        Phone = "+1234567890", // Provide valid phone
                        Qualification = "Not Specified",
                        ExperienceYears = 0,
                        ConsultationFee = 500, // Default consultation fee
                        IsAvailable = true
                    };
                    var doctorResult = await _doctorRepository.CreateAsync(doctor);
                    
                    // Add default specialization (General)
                    var generalSpec = await _context.Specializations.FirstOrDefaultAsync(s => s.Name.ToLower() == "general");
                    if (generalSpec != null)
                    {
                        _context.DoctorSpecializations.Add(new DoctorSpecialization
                        {
                            DoctorId = doctorResult.DoctorId,
                            SpecializationId = generalSpec.SpecializationId
                        });
                        await _context.SaveChangesAsync();
                    }
                    
                    _logger.LogInformation("Doctor profile created successfully - DoctorId: {DoctorId}, UserId: {UserId}", 
                        doctorResult.DoctorId, created.UserId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating doctor profile for UserId: {UserId}", created.UserId);
                    // Don't throw - user account is created even if doctor profile fails
                }
            }

            return _mapper.Map<UserDTO>(created);
        }
    }
}