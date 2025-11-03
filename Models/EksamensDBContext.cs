using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models;

public partial class EksamensDBContext : DbContext
{
    string? connectionString = null;
    public EksamensDBContext(IConfiguration conf)
    {
        connectionString = conf.GetConnectionString("EksamensDBCornection");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        //options.UseSqlServer (@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=RegistrationDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        //Finder appsettings.json connection string
        options.UseSqlServer(connectionString);
    }


    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<ClassSubject> ClassSubjects { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<ExaminationTeacher> ExaminationTeachers { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentClass> StudentClasses { get; set; }

    public virtual DbSet<StudentExam> StudentExams { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=mssql11.unoeuro.com;Initial Catalog=andershgras_dk_db_eksamenproject;User ID=andershgras_dk;Password=dBFybtpRwacg3rG9zhm5;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Class__CB1927A0459CDFAC");
        });

        modelBuilder.Entity<ClassSubject>(entity =>
        {
            entity.HasKey(e => e.ClassSubjectId).HasName("PK__ClassSub__79A973399E3407F1");

            entity.HasOne(d => d.Class).WithMany(p => p.ClassSubjects).HasConstraintName("FK_ClassSubject_Class");

            entity.HasOne(d => d.Subject).WithMany(p => p.ClassSubjects).HasConstraintName("FK_ClassSubject_Subject");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.ExamId).HasName("PK__Exam__297521A767594786");

            entity.Property(e => e.IsReExam).HasDefaultValue(false);

            entity.HasOne(d => d.Class).WithMany(p => p.Exams)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Exam_Class");

            entity.HasOne(d => d.Room).WithMany(p => p.Exams)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Exam_Room");

            entity.HasOne(d => d.Subject).WithMany(p => p.Exams)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Exam_Subject");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Exams)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Exam_Teacher");
        });

        modelBuilder.Entity<ExaminationTeacher>(entity =>
        {
            entity.HasKey(e => e.ExaminationTeacherId).HasName("PK__Examinat__50759235931F1BCF");

            entity.HasOne(d => d.Exam).WithMany(p => p.ExaminationTeachers).HasConstraintName("FK_ExaminationTeacher_Exam");

            entity.HasOne(d => d.Teacher).WithMany(p => p.ExaminationTeachers).HasConstraintName("FK_ExaminationTeacher_Teacher");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__Room__3286391939FD5A33");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Student__32C52A793BE317C1");
        });

        modelBuilder.Entity<StudentClass>(entity =>
        {
            entity.HasKey(e => e.StudentClassId).HasName("PK__StudentC__2FF121679E358C88");

            entity.HasOne(d => d.Class).WithMany(p => p.StudentClasses).HasConstraintName("FK_StudentClass_Class");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentClasses).HasConstraintName("FK_StudentClass_Student");
        });

        modelBuilder.Entity<StudentExam>(entity =>
        {
            entity.HasKey(e => e.StudentExamId).HasName("PK__StudentE__C57949568B69BBD0");

            entity.HasOne(d => d.Exam).WithMany(p => p.StudentExams).HasConstraintName("FK_StudentExam_Exam");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentExams).HasConstraintName("FK_StudentExam_Student");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__Subject__AC1BA388284A5582");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("PK__Teacher__EDF259443B1C1561");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
