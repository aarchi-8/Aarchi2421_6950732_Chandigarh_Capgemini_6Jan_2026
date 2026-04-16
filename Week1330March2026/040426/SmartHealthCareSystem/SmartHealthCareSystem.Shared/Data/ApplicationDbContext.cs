using Microsoft.EntityFrameworkCore;
using SmartHealthCareSystem.Shared.Models;

namespace SmartHealthCareSystem.Shared.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Bill> Bills { get; set; }

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     base.OnModelCreating(modelBuilder);

        //     // One-to-One: User -> Doctor
        //     modelBuilder.Entity<Doctor>()
        //         .HasOne(d => d.User)
        //         .WithOne(u => u.Doctor)
        //         .HasForeignKey<Doctor>(d => d.UserId)
        //         .OnDelete(DeleteBehavior.Restrict);

        //     // One-to-Many: Department -> Doctors
        //     modelBuilder.Entity<Doctor>()
        //         .HasOne(d => d.Department)
        //         .WithMany(dep => dep.Doctors)
        //         .HasForeignKey(d => d.DepartmentId)
        //         .OnDelete(DeleteBehavior.Cascade);

        //     // One-to-Many: User (Patient) -> Appointments
        //     modelBuilder.Entity<Appointment>()
        //         .HasOne(a => a.Patient)
        //         .WithMany(u => u.Appointments)
        //         .HasForeignKey(a => a.PatientId)
        //         .OnDelete(DeleteBehavior.Restrict);

        //     // One-to-Many: Doctor -> Appointments
        //     modelBuilder.Entity<Appointment>()
        //         .HasOne(a => a.Doctor)
        //         .WithMany(d => d.Appointments)
        //         .HasForeignKey(a => a.DoctorId)
        //         .OnDelete(DeleteBehavior.Restrict);

        //     // One-to-One: Appointment -> Prescription
        //     modelBuilder.Entity<Prescription>()
        //         .HasOne(p => p.Appointment)
        //         .WithOne(a => a.Prescription)
        //         .HasForeignKey<Prescription>(p => p.AppointmentId)
        //         .OnDelete(DeleteBehavior.Cascade);

        //     // One-to-One: Appointment -> Bill
        //     modelBuilder.Entity<Bill>()
        //         .HasOne(b => b.Appointment)
        //         .WithOne(a => a.Bill)
        //         .HasForeignKey<Bill>(b => b.AppointmentId)
        //         .OnDelete(DeleteBehavior.Cascade);

        //     // Decimal precision configuration
        //     modelBuilder.Entity<Bill>()
        //         .Property(b => b.ConsultationFee)
        //         .HasPrecision(18, 2);

        //     modelBuilder.Entity<Bill>()
        //         .Property(b => b.MedicineCharges)
        //         .HasPrecision(18, 2);

        //     modelBuilder.Entity<Bill>()
        //         .Property(b => b.TotalAmount)
        //         .HasPrecision(18, 2);

        //     modelBuilder.Entity<Department>()
        //         .HasData(
        //             new Department { DepartmentId = 1, DepartmentName = "Cardiology", Description = "Heart related treatments" },
        //             new Department { DepartmentId = 2, DepartmentName = "Neurology", Description = "Brain and nervous system" },
        //             new Department { DepartmentId = 3, DepartmentName = "Orthopedics", Description = "Bone and joint treatments" }
        //         );
        // }
    }
}
