using AutoMapper;
using SmartHealthCareSystem.Shared.Models;
using SmartHealthCareSystem.API.Repositories;
using SmartHealthCareSystem.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using SmartHealthCareSystem.Shared.Data;

namespace SmartHealthCareSystem.API.Services
{
    public class BillService : IBillService
    {
        private readonly IRepository<Bill> _billRepository;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public BillService(IRepository<Bill> billRepository, IMapper mapper, ApplicationDbContext context)
        {
            _billRepository = billRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<BillDto>> GetAllBillsAsync()
        {
            var bills = await _context.Bills
                .Include(b => b.Appointment)
                    .ThenInclude(a => a.Patient)
                .Include(b => b.Appointment)
                    .ThenInclude(a => a.Doctor)
                        .ThenInclude(d => d.User)
                .Include(b => b.Appointment)
                    .ThenInclude(a => a.Prescription)
                .ToListAsync();
            return _mapper.Map<IEnumerable<BillDto>>(bills);
        }

        public async Task<BillDto?> GetBillByIdAsync(int id)
        {
            var bill = await _context.Bills
                .Include(b => b.Appointment)
                    .ThenInclude(a => a.Patient)
                .Include(b => b.Appointment)
                    .ThenInclude(a => a.Doctor)
                        .ThenInclude(d => d.User)
                .Include(b => b.Appointment)
                    .ThenInclude(a => a.Prescription)
                .FirstOrDefaultAsync(b => b.BillId == id);
            return _mapper.Map<BillDto?>(bill);
        }

        public async Task<BillDto> CreateBillAsync(CreateBillDto createBillDto)
        {
            var bill = _mapper.Map<Bill>(createBillDto);
            bill.TotalAmount = createBillDto.ConsultationFee + createBillDto.MedicineCharges;
            await _billRepository.AddAsync(bill);
            await _billRepository.SaveChangesAsync();
            return _mapper.Map<BillDto>(bill);
        }

        public async Task<BillDto?> UpdateBillAsync(int id, UpdateBillDto updateBillDto)
        {
            var bill = await _billRepository.GetByIdAsync(id);
            if (bill == null) return null;

            _mapper.Map(updateBillDto, bill);
            bill.TotalAmount = updateBillDto.ConsultationFee + updateBillDto.MedicineCharges;
            await _billRepository.UpdateAsync(bill);
            await _billRepository.SaveChangesAsync();
            return _mapper.Map<BillDto?>(bill);
        }

        public async Task<bool> DeleteBillAsync(int id)
        {
            var bill = await _billRepository.GetByIdAsync(id);
            if (bill == null) return false;

            await _billRepository.DeleteAsync(id);
            await _billRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<BillDto>> GetBillsByAppointmentIdAsync(int appointmentId)
        {
            var bills = await _billRepository.GetAllAsync();
            var appointmentBills = bills.Where(b => b.AppointmentId == appointmentId);
            return _mapper.Map<IEnumerable<BillDto>>(appointmentBills);
        }

        public async Task<IEnumerable<BillDto>> GetBillsByPatientIdAsync(int patientId)
        {
            var bills = await _context.Bills
                .Include(b => b.Appointment)
                    .ThenInclude(a => a.Patient)
                .Include(b => b.Appointment)
                    .ThenInclude(a => a.Doctor)
                        .ThenInclude(d => d.User)
                .Include(b => b.Appointment)
                    .ThenInclude(a => a.Prescription)
                .Where(b => b.Appointment.PatientId == patientId)
                .ToListAsync();
            return _mapper.Map<IEnumerable<BillDto>>(bills);
        }

        public async Task<bool> MarkBillsAsPaidAsync(int patientId)
        {
            var bills = await _context.Bills
                .Include(b => b.Appointment)
                .Where(b => b.Appointment.PatientId == patientId && b.PaymentStatus == "Unpaid")
                .ToListAsync();

            foreach (var bill in bills)
            {
                bill.PaymentStatus = "Paid";
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
