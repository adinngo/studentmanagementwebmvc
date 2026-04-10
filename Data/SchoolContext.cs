using Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options)
        {

        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
        public DbSet<CourseAssignment> CourseAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Instructor>().ToTable("Instructor");
            modelBuilder.Entity<OfficeAssignment>().ToTable("OfficeAssignment");
            modelBuilder.Entity<CourseAssignment>().ToTable("CourseAssignment");

            modelBuilder.Entity<CourseAssignment>()
                .HasKey(c => new { c.CourseID, c.InstructorID });

            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.CourseID, e.StudentID })
                .IsUnique();

            // Cấu hình quan hệ giữa Course và Enrollment
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseID)
                .OnDelete(DeleteBehavior.Restrict); // <--- CHÍNH LÀ DÒNG NÀY

            // Nếu bạn có bảng CourseAssignment, cũng nên làm tương tự
            modelBuilder.Entity<CourseAssignment>()
                .HasOne(ca => ca.Course)
                .WithMany(c => c.CourseAssignments)
                .HasForeignKey(ca => ca.CourseID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
