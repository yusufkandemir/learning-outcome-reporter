using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace server.Models
{
    public partial class OutcomeReportingContext : DbContext
    {
        public OutcomeReportingContext()
        {
        }

        public OutcomeReportingContext(DbContextOptions<OutcomeReportingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AssignmentResult> AssignmentResults { get; set; }
        public virtual DbSet<AssignmentTaskOutcome> AssignmentTaskOutcome { get; set; }
        public virtual DbSet<AssignmentTaskResult> AssignmentTaskResults { get; set; }
        public virtual DbSet<AssignmentTask> AssignmentTasks { get; set; }
        public virtual DbSet<Assignment> Assignments { get; set; }
        public virtual DbSet<CourseInfo> CourseInfos { get; set; }
        public virtual DbSet<CourseResult> CourseResults { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Outcome> Outcomes { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AssignmentResult>(entity =>
            {
                entity.ToTable("assignment_results");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");

                entity.Property(e => e.CourseResultId).HasColumnName("course_result_id");

                entity.HasOne(d => d.Assignment)
                    .WithMany(p => p.AssignmentResults)
                    .HasForeignKey(d => d.AssignmentId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_assignment_results_assignment_id");

                entity.HasOne(d => d.CourseResult)
                    .WithMany(p => p.AssignmentResults)
                    .HasForeignKey(d => d.CourseResultId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_assignment_results_course_result_id");
            });

            modelBuilder.Entity<AssignmentTaskOutcome>(entity =>
            {
                entity.ToTable("assignment_task_outcome");

                entity.HasKey(e => new { e.AssignmentTaskId, e.OutcomeId });

                entity.Property(e => e.AssignmentTaskId).HasColumnName("assignment_task_id");

                entity.Property(e => e.OutcomeId).HasColumnName("outcome_id");

                entity.HasOne(d => d.AssignmentTask)
                    .WithMany(p => p.AssignmentTaskOutcomes)
                    .HasForeignKey(d => d.AssignmentTaskId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_assignment_task_outcome_assignment_task_id");

                entity.HasOne(d => d.Outcome)
                    .WithMany()
                    .HasForeignKey(d => d.OutcomeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_assignment_task_outcome_outcome_id");
            });

            modelBuilder.Entity<AssignmentTaskResult>(entity =>
            {
                entity.ToTable("assignment_task_results");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AssignmentResultId).HasColumnName("assignment_result_id");

                entity.Property(e => e.AssignmentTaskId).HasColumnName("assignment_task_id");

                entity.Property(e => e.Grade)
                    .HasColumnName("grade")
                    .HasColumnType("decimal(5, 2)");

                entity.HasOne(d => d.AssignmentResult)
                    .WithMany(p => p.AssignmentTaskResults)
                    .HasForeignKey(d => d.AssignmentResultId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_assignment_task_results_assignment_result_id");

                entity.HasOne(d => d.AssignmentTask)
                    .WithMany(p => p.AssignmentTaskResults)
                    .HasForeignKey(d => d.AssignmentTaskId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_assignment_task_results_assignment_task_id");
            });

            modelBuilder.Entity<AssignmentTask>(entity =>
            {
                entity.ToTable("assignment_tasks");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");

                entity.Property(e => e.Number).HasColumnName("number");

                entity.Property(e => e.Weight)
                    .HasColumnName("weight")
                    .HasColumnType("decimal(3, 3)");

                entity.HasOne(d => d.Assignment)
                    .WithMany(p => p.AssignmentTasks)
                    .HasForeignKey(d => d.AssignmentId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_assignment_tasks_assignment_id");
            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.ToTable("assignments");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasMaxLength(10)
                    .HasConversion<string>();

                entity.Property(e => e.Weight)
                    .HasColumnName("weight")
                    .HasColumnType("decimal(3, 3)");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_assignments_course_id");
            });

            modelBuilder.Entity<CourseInfo>(entity =>
            {
                entity.ToTable("course_infos");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("code")
                    .HasMaxLength(7);

                entity.Property(e => e.Credit).HasColumnName("credit");

                entity.Property(e => e.DepartmentId).HasColumnName("department_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(64);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.CourseInfos)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_course_infos_department_id");
            });

            modelBuilder.Entity<CourseResult>(entity =>
            {
                entity.ToTable("course_results");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.StudentId)
                    .IsRequired()
                    .HasColumnName("student_id")
                    .HasMaxLength(10);

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.CourseResults)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_course_results_course_id");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.CourseResults)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_course_results_student_id");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("courses");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.CourseInfoId).HasColumnName("course_info_id");

                entity.Property(e => e.Semester)
                    .IsRequired()
                    .HasColumnName("semester")
                    .HasMaxLength(10)
                    .HasConversion<string>();

                entity.Property(e => e.Year).HasColumnName("year");

                entity.HasOne(d => d.CourseInfo)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.CourseInfoId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_courses_course_info_id");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("departments");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Outcome>(entity =>
            {
                entity.ToTable("outcomes");

                entity
                    .HasDiscriminator<string>("Type")
                    .HasValue<ProgramOutcome>("program")
                    .HasValue<LearningOutcome>("learning");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("code");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(255);

                entity.Property("Type")
                    .IsRequired()
                    .HasColumnName("type")
                    .HasMaxLength(8);
            });

            modelBuilder.Entity<ProgramOutcome>(entity =>
            {
                entity.Property(e => e.DepartmentId).HasColumnName("department_id");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Outcomes)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_outcomes_department_id");
            });

            modelBuilder.Entity<LearningOutcome>(entity =>
            {
                entity.Property(e => e.CourseInfoId).HasColumnName("course_info_id");

                entity.HasOne(d => d.CourseInfo)
                    .WithMany(p => p.Outcomes)
                    .HasForeignKey(d => d.CourseInfoId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_outcomes_course_info_id");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("students");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(10);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnName("full_name")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DepartmentId).HasColumnName("department_id");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(255);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(255);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_users_department_id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
