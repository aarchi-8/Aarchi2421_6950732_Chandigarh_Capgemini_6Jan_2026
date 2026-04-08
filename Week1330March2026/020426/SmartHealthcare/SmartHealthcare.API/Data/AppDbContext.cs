using Microsoft.EntityFrameworkCore;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<DoctorSpecialization> DoctorSpecializations { get; set; }
        public DbSet<PrescriptionMedicine> PrescriptionMedicines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── 1. User ──────────────────────────────────────────
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Role).HasDefaultValue("Patient");
            });

            // ── 2. One-to-One: User → Patient ────────────────────
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(p => p.PatientId);

                entity.HasOne(p => p.User)
                      .WithOne(u => u.Patient)
                      .HasForeignKey<Patient>(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── 3. One-to-One: User → Doctor ─────────────────────
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasKey(d => d.DoctorId);

                entity.HasOne(d => d.User)
                      .WithOne(u => u.Doctor)
                      .HasForeignKey<Doctor>(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(d => d.ConsultationFee)
                      .HasColumnType("decimal(10,2)");
            });

            // ── 4. Many-to-Many: Doctor ↔ Specialization ─────────
            modelBuilder.Entity<DoctorSpecialization>(entity =>
            {
                entity.HasKey(ds => new { ds.DoctorId, ds.SpecializationId });

                entity.HasOne(ds => ds.Doctor)
                      .WithMany(d => d.DoctorSpecializations)
                      .HasForeignKey(ds => ds.DoctorId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ds => ds.Specialization)
                      .WithMany(s => s.DoctorSpecializations)
                      .HasForeignKey(ds => ds.SpecializationId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── 5. One-to-Many: Doctor → Appointments ────────────
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(a => a.AppointmentId);

                entity.HasOne(a => a.Doctor)
                      .WithMany(d => d.Appointments)
                      .HasForeignKey(a => a.DoctorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Patient)
                      .WithMany(p => p.Appointments)
                      .HasForeignKey(a => a.PatientId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(a => a.Status)
                      .HasConversion<string>();
            });

            // ── 6. One-to-One: Appointment → Prescription ────────
            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.HasKey(p => p.PrescriptionId);

                entity.HasOne(p => p.Appointment)
                      .WithOne(a => a.Prescription)
                      .HasForeignKey<Prescription>(p => p.AppointmentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── 7. Many-to-Many: Prescription ↔ Medicine ─────────
            modelBuilder.Entity<PrescriptionMedicine>(entity =>
            {
                entity.HasKey(pm => new { pm.PrescriptionId, pm.MedicineId });

                entity.HasOne(pm => pm.Prescription)
                      .WithMany(p => p.PrescriptionMedicines)
                      .HasForeignKey(pm => pm.PrescriptionId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pm => pm.Medicine)
                      .WithMany(m => m.PrescriptionMedicines)
                      .HasForeignKey(pm => pm.MedicineId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── 8. Seed Data ──────────────────────────────────────
            modelBuilder.Entity<Specialization>().HasData(
                new Specialization { SpecializationId = 1, Name = "Cardiology", Description = "Heart specialist" },
                new Specialization { SpecializationId = 2, Name = "Neurology", Description = "Brain & nerve specialist" },
                new Specialization { SpecializationId = 3, Name = "Orthopedics", Description = "Bone & joint specialist" },
                new Specialization { SpecializationId = 4, Name = "Dermatology", Description = "Skin specialist" },
                new Specialization { SpecializationId = 5, Name = "Pediatrics", Description = "Child specialist" }
            );

            modelBuilder.Entity<Medicine>().HasData(
                new Medicine { MedicineId = 1, Name = "Paracetamol", Category = "Analgesic", Description = "Pain & fever relief" },
                new Medicine { MedicineId = 2, Name = "Amoxicillin", Category = "Antibiotic", Description = "Bacterial infections" },
                new Medicine { MedicineId = 3, Name = "Ibuprofen", Category = "Anti-inflammatory", Description = "Pain & inflammation" },
                new Medicine { MedicineId = 4, Name = "Cetirizine", Category = "Antihistamine", Description = "Allergy relief" },
                new Medicine { MedicineId = 5, Name = "Metformin", Category = "Antidiabetic", Description = "Type 2 diabetes" }
            );
        }
    }
}