using AutoMapper;
using SmartHealthCareSystem.Shared.Models;
using SmartHealthCareSystem.API.Repositories;
using SmartHealthCareSystem.Shared.DTOs;

namespace SmartHealthCareSystem.API.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IRepository<Department> _departmentRepository;
        private readonly IMapper _mapper;

        public DepartmentService(IRepository<Department> departmentRepository, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync()
        {
            var departments = await _departmentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
        }

        public async Task<DepartmentDto?> GetDepartmentByIdAsync(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            return _mapper.Map<DepartmentDto?>(department);
        }

        public async Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto createDepartmentDto)
        {
            var department = _mapper.Map<Department>(createDepartmentDto);
            await _departmentRepository.AddAsync(department);
            await _departmentRepository.SaveChangesAsync();
            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task<DepartmentDto?> UpdateDepartmentAsync(int id, UpdateDepartmentDto updateDepartmentDto)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null) return null;

            _mapper.Map(updateDepartmentDto, department);
            await _departmentRepository.UpdateAsync(department);
            await _departmentRepository.SaveChangesAsync();
            return _mapper.Map<DepartmentDto?>(department);
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null) return false;

            await _departmentRepository.DeleteAsync(id);
            await _departmentRepository.SaveChangesAsync();
            return true;
        }
    }
}
