using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models;

public partial class MovieDBContext : DbContext
{
    public MovieDBContext()
    {
    }

    public MovieDBContext(DbContextOptions<MovieDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EksaminationsUndervise> EksaminationsUndervises { get; set; }

    public virtual DbSet<Elever> Elevers { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<Fag> Fags { get; set; }

    public virtual DbSet<Hold> Holds { get; set; }

    public virtual DbSet<Lokale> Lokales { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<Underviser> Undervisers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=mssql11.unoeuro.com;Initial Catalog=andershgras_dk_db_eksamenproject;User ID=andershgras_dk;Password=dBFybtpRwacg3rG9zhm5;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EksaminationsUndervise>(entity =>
        {
            entity.HasKey(e => new { e.ExamId, e.UnderviserId }).HasName("PK__Eksamina__AB614231798B511C");

            entity.HasOne(d => d.Exam).WithMany(p => p.EksaminationsUndervises).HasConstraintName("FK_EksaminationsUndervise_Exam");

            entity.HasOne(d => d.Underviser).WithMany(p => p.EksaminationsUndervises)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EksaminationsUndervise_Underviser");
        });

        modelBuilder.Entity<Elever>(entity =>
        {
            entity.HasKey(e => e.ElevId).HasName("PK__Elever__4AE80D63D3519114");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.ExamId).HasName("PK__Exam__297521C75DEB807B");

            entity.HasOne(d => d.Fag).WithMany(p => p.Exams)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Exam_Fag");

            entity.HasOne(d => d.Hold).WithMany(p => p.Exams)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Exam_Hold");

            entity.HasOne(d => d.Lokale).WithMany(p => p.Exams)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Exam_Lokale");

            entity.HasOne(d => d.Underviser).WithMany(p => p.Exams)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Exam_Underviser");

            entity.HasMany(d => d.Elevs).WithMany(p => p.Exams)
                .UsingEntity<Dictionary<string, object>>(
                    "ElevEksaman",
                    r => r.HasOne<Elever>().WithMany()
                        .HasForeignKey("ElevId")
                        .HasConstraintName("FK_ElevExam_Elev"),
                    l => l.HasOne<Exam>().WithMany()
                        .HasForeignKey("ExamId")
                        .HasConstraintName("FK_ElevExam_Exam"),
                    j =>
                    {
                        j.HasKey("ExamId", "ElevId").HasName("PK__ELEV_EKS__0DDBA17724BFB73D");
                        j.ToTable("ELEV_EKSAMEN");
                        j.IndexerProperty<int>("ExamId").HasColumnName("ExamID");
                        j.IndexerProperty<int>("ElevId").HasColumnName("ElevID");
                    });
        });

        modelBuilder.Entity<Fag>(entity =>
        {
            entity.HasKey(e => e.FagId).HasName("PK__Fag__9A31300B430E6CC1");
        });

        modelBuilder.Entity<Hold>(entity =>
        {
            entity.HasKey(e => e.HoldId).HasName("PK__Hold__6E24D9C42AE420FB");

            entity.HasMany(d => d.Elevs).WithMany(p => p.Holds)
                .UsingEntity<Dictionary<string, object>>(
                    "ElevHold",
                    r => r.HasOne<Elever>().WithMany()
                        .HasForeignKey("ElevId")
                        .HasConstraintName("FK_ElevHold_Elev"),
                    l => l.HasOne<Hold>().WithMany()
                        .HasForeignKey("HoldId")
                        .HasConstraintName("FK_ElevHold_Hold"),
                    j =>
                    {
                        j.HasKey("HoldId", "ElevId").HasName("PK__ELEV_HOL__4A8A5AF40EEDDD84");
                        j.ToTable("ELEV_HOLD");
                        j.IndexerProperty<int>("HoldId").HasColumnName("HoldID");
                        j.IndexerProperty<int>("ElevId").HasColumnName("ElevID");
                    });

            entity.HasMany(d => d.Fags).WithMany(p => p.Holds)
                .UsingEntity<Dictionary<string, object>>(
                    "HoldFag",
                    r => r.HasOne<Fag>().WithMany()
                        .HasForeignKey("FagId")
                        .HasConstraintName("FK_HoldFag_Fag"),
                    l => l.HasOne<Hold>().WithMany()
                        .HasForeignKey("HoldId")
                        .HasConstraintName("FK_HoldFag_Hold"),
                    j =>
                    {
                        j.HasKey("HoldId", "FagId").HasName("PK__HOLD_FAG__C787C9249D8E94C8");
                        j.ToTable("HOLD_FAG");
                        j.IndexerProperty<int>("HoldId").HasColumnName("HoldID");
                        j.IndexerProperty<int>("FagId").HasColumnName("FagID");
                    });
        });

        modelBuilder.Entity<Lokale>(entity =>
        {
            entity.HasKey(e => e.LokaleId).HasName("PK__Lokale__1C7789B24F589146");
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.MovieId).HasName("PK__Movies__4BD2943AAF2324FA");

            entity.Property(e => e.MovieId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Underviser>(entity =>
        {
            entity.HasKey(e => e.UnderviserId).HasName("PK__Undervis__21463969F57A2088");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
