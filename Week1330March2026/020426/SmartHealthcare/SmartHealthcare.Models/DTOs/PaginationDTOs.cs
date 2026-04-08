using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Models.DTOs
{
    /// <summary>
    /// Pagination request parameters
    /// </summary>
    public class PaginationParams
    {
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        public string? SortBy { get; set; }
        public bool IsDescending { get; set; } = false;
    }

    /// <summary>
    /// Generic pagination response wrapper
    /// </summary>
    public class PaginatedResult<T>
    {
        public List<T> Data { get; set; } = new();
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        public PaginatedResult() { }

        public PaginatedResult(List<T> data, int totalCount, int pageNumber, int pageSize)
        {
            Data = data;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            HasPreviousPage = pageNumber > 1;
            HasNextPage = pageNumber < TotalPages;
        }
    }

    /// <summary>
    /// Doctor filtering parameters
    /// </summary>
    public class DoctorFilterParams : PaginationParams
    {
        public int? SpecializationId { get; set; }
        public string? SearchTerm { get; set; }
        public bool? IsAvailable { get; set; }
        public decimal? MinConsultationFee { get; set; }
        public decimal? MaxConsultationFee { get; set; }
    }

    /// <summary>
    /// Patient filtering parameters
    /// </summary>
    public class PatientFilterParams : PaginationParams
    {
        public string? SearchTerm { get; set; }
        public int? Gender { get; set; }
    }

    /// <summary>
    /// Appointment filtering parameters
    /// </summary>
    public class AppointmentFilterParams : PaginationParams
    {
        public int? DoctorId { get; set; }
        public int? PatientId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? Status { get; set; }
    }
}
