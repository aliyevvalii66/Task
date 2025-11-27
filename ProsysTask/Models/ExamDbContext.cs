using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ProsysTask.Models
{
    public class ExamDbContext : DbContext
    {
        public ExamDbContext(DbContextOptions<ExamDbContext> options)
            : base(options) { }

        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Exam> Exams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Lessons
            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.HasKey(e => e.LessonCode);
                entity.Property(e => e.LessonCode).HasMaxLength(3).IsFixedLength();
                entity.Property(e => e.LessonName).HasMaxLength(30).IsRequired();
                entity.Property(e => e.Grade).IsRequired();
                entity.Property(e => e.TeacherName).HasMaxLength(20);
                entity.Property(e => e.TeacherSurname).HasMaxLength(20);
            });

            // Students
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.StudentNumber);
                entity.Property(e => e.FirstName).HasMaxLength(30).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(30).IsRequired();
                entity.Property(e => e.Grade).IsRequired();
            });

            // Exams
            modelBuilder.Entity<Exam>(entity =>
            {
                entity.HasKey(e => new { e.LessonCode, e.StudentNumber }); 
                entity.Property(e => e.Score).IsRequired();
                entity.Property(e => e.ExamDate).IsRequired();

                entity.HasOne(e => e.Lesson)
                      .WithMany()
                      .HasForeignKey(e => e.LessonCode)
                      .OnDelete(DeleteBehavior.Restrict); 

                entity.HasOne(e => e.Student)
                      .WithMany()
                      .HasForeignKey(e => e.StudentNumber)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

    }
}
