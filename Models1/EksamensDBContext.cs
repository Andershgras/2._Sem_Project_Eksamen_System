using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models1;

public partial class EksamensDBContext : DbContext
{
    string? connectionString = null;
    public EksamensDBContext(IConfiguration conf)
    {
        connectionString = conf.GetConnectionString("EksamensDBConection");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        //options.UseSqlServer (@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=RegistrationDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        //Finder appsettings.json connection string
        options.UseSqlServer(connectionString);
    }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomsToExam> RoomsToExams { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentsToClass> StudentsToClasses { get; set; }

    public virtual DbSet<StudentsToExam> StudentsToExams { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<TeachersToExam> TeachersToExams { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=mssql11.unoeuro.com;Initial Catalog=andershgras_dk_db_eksamenproject;User ID=andershgras_dk;Password=dBFybtpRwacg3rG9zhm5;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Class__CB1927A06216F48D");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.ExamId).HasName("PK__Exam__297521A7813E58EB");

            entity.Property(e => e.IsFinalExam).HasDefaultValue(false);
            entity.Property(e => e.IsReExam).HasDefaultValue(false);

            entity.HasOne(d => d.Class).WithMany(p => p.Exams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Exam__ClassID__44CA3770");

            entity.HasOne(d => d.ReExam).WithMany(p => p.InverseReExam).HasConstraintName("FK__Exam__ReExamID__45BE5BA9");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__Room__32863919C5E8573E");
        });

        modelBuilder.Entity<RoomsToExam>(entity =>
        {
            entity.HasKey(e => e.RoomExamId).HasName("PK__RoomsToE__910471D9E4363F92");

            entity.HasOne(d => d.Exam).WithMany(p => p.RoomsToExams).HasConstraintName("FK__RoomsToEx__ExamI__56E8E7AB");

            entity.HasOne(d => d.Room).WithMany(p => p.RoomsToExams).HasConstraintName("FK__RoomsToEx__RoomI__55F4C372");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Student__32C52A79D4378E3D");
        });

        modelBuilder.Entity<StudentsToClass>(entity =>
        {
            entity.HasKey(e => e.StudentClassId).HasName("PK__Students__2FF1216749CA654A");

            entity.HasOne(d => d.Class).WithMany(p => p.StudentsToClasses).HasConstraintName("FK__StudentsT__Class__498EEC8D");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentsToClasses).HasConstraintName("FK__StudentsT__Stude__489AC854");
        });

        modelBuilder.Entity<StudentsToExam>(entity =>
        {
            entity.HasKey(e => e.StudentExamId).HasName("PK__Students__C5794956714E4DF5");

            entity.HasOne(d => d.Exam).WithMany(p => p.StudentsToExams).HasConstraintName("FK__StudentsT__ExamI__4D5F7D71");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentsToExams).HasConstraintName("FK__StudentsT__Stude__4C6B5938");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("PK__Teacher__EDF25944054A720C");
        });

        modelBuilder.Entity<TeachersToExam>(entity =>
        {
            entity.HasKey(e => e.TeacherExamId).HasName("PK__Teachers__5A1FB6FE5D9C3D1D");

            entity.HasOne(d => d.Exam).WithMany(p => p.TeachersToExams).HasConstraintName("FK__TeachersT__ExamI__5224328E");

            entity.HasOne(d => d.Teacher).WithMany(p => p.TeachersToExams).HasConstraintName("FK__TeachersT__Teach__51300E55");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
