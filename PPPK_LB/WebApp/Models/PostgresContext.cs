using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Models;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Appointmenttype> Appointmenttypes { get; set; }

    public virtual DbSet<Attachment> Attachments { get; set; }

    public virtual DbSet<Disease> Diseases { get; set; }

    public virtual DbSet<Medicalrecord> Medicalrecords { get; set; }

    public virtual DbSet<Medicine> Medicines { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Prescription> Prescriptions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("User Id=postgres.jrbdmqdprsughrxbnbzk;Password=LeoB06022004Ena;Server=aws-0-eu-central-1.pooler.supabase.com;Port=5432;Database=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("appointmenttypeenum", new[] { "GP", "KRV", "X-RAY", "CT", "MR", "ULTRA", "EKG", "ECHO", "EYE", "DERM", "DENTA", "MAMMO", "NEURO" })
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn", "phone" })
            .HasPostgresEnum("auth", "one_time_token_type", new[] { "confirmation_token", "reauthentication_token", "recovery_token", "email_change_token_new", "email_change_token_current", "phone_change_token" })
            .HasPostgresEnum("pgsodium", "key_status", new[] { "default", "valid", "invalid", "expired" })
            .HasPostgresEnum("pgsodium", "key_type", new[] { "aead-ietf", "aead-det", "hmacsha512", "hmacsha256", "auth", "shorthash", "generichash", "kdf", "secretbox", "secretstream", "stream_xchacha20" })
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "pgjwt")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("graphql", "pg_graphql")
            .HasPostgresExtension("pgsodium", "pgsodium")
            .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Appointmentid).HasName("appointments_pkey");

            entity.ToTable("appointments");

            entity.Property(e => e.Appointmentid).HasColumnName("appointmentid");
            entity.Property(e => e.Appointmentdate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("appointmentdate");
            entity.Property(e => e.Appointmenttypeid).HasColumnName("appointmenttypeid");
            entity.Property(e => e.Patientid).HasColumnName("patientid");

            entity.HasOne(d => d.Appointmenttype).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.Appointmenttypeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("appointments_appointmenttypeid_fkey");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.Patientid)
                .HasConstraintName("appointments_patientid_fkey");
        });

        modelBuilder.Entity<Appointmenttype>(entity =>
        {
            entity.HasKey(e => e.Appointmenttypeid).HasName("appointmenttypes_pkey");

            entity.ToTable("appointmenttypes");

            entity.Property(e => e.Appointmenttypeid).HasColumnName("appointmenttypeid");
            entity.Property(e => e.Appointmenttypename).HasColumnName("appointmenttypename");
        });

        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(e => e.Attachmentid).HasName("attachments_pkey");

            entity.ToTable("attachments");

            entity.Property(e => e.Attachmentid).HasColumnName("attachmentid");
            entity.Property(e => e.Appointmentid).HasColumnName("appointmentid");
            entity.Property(e => e.Filename)
                .HasMaxLength(255)
                .HasColumnName("filename");
            entity.Property(e => e.Filepath)
                .HasMaxLength(255)
                .HasColumnName("filepath");
            entity.Property(e => e.Uploadedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("uploadedat");

            entity.HasOne(d => d.Appointment).WithMany(p => p.Attachments)
                .HasForeignKey(d => d.Appointmentid)
                .HasConstraintName("attachments_appointmentid_fkey");
        });

        modelBuilder.Entity<Disease>(entity =>
        {
            entity.HasKey(e => e.Diseaseid).HasName("disease_pkey");

            entity.ToTable("diseases");

            entity.Property(e => e.Diseaseid).HasColumnName("diseaseid");
            entity.Property(e => e.Diseasename).HasColumnName("diseasename");
        });

        modelBuilder.Entity<Medicalrecord>(entity =>
        {
            entity.HasKey(e => e.Recordid).HasName("medicalrecords_pkey");

            entity.ToTable("medicalrecords");

            entity.Property(e => e.Recordid).HasColumnName("recordid");
            entity.Property(e => e.Diseaseid).HasColumnName("diseaseid");
            entity.Property(e => e.Enddate).HasColumnName("enddate");
            entity.Property(e => e.Patientid).HasColumnName("patientid");
            entity.Property(e => e.Startdate).HasColumnName("startdate");

            entity.HasOne(d => d.Disease).WithMany(p => p.Medicalrecords)
                .HasForeignKey(d => d.Diseaseid)
                .HasConstraintName("medicalrecords_diseaseid_fkey");

            entity.HasOne(d => d.Patient).WithMany(p => p.Medicalrecords)
                .HasForeignKey(d => d.Patientid)
                .HasConstraintName("medicalrecords_patientid_fkey");
        });

        modelBuilder.Entity<Medicine>(entity =>
        {
            entity.HasKey(e => e.Medicineid).HasName("medicine_pkey");

            entity.ToTable("medicines");

            entity.Property(e => e.Medicineid).HasColumnName("medicineid");
            entity.Property(e => e.Medicinename).HasColumnName("medicinename");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Patientid).HasName("patients_pkey");

            entity.ToTable("patients");

            entity.HasIndex(e => e.Oib, "patients_oib_key").IsUnique();

            entity.Property(e => e.Patientid).HasColumnName("patientid");
            entity.Property(e => e.Dateofbirth).HasColumnName("dateofbirth");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .HasColumnName("firstname");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .HasColumnName("gender");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Oib)
                .HasMaxLength(11)
                .HasColumnName("oib");
        });

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasKey(e => e.Prescriptionid).HasName("prescriptions_pkey");

            entity.ToTable("prescriptions");

            entity.Property(e => e.Prescriptionid).HasColumnName("prescriptionid");
            entity.Property(e => e.Medicineid).HasColumnName("medicineid");
            entity.Property(e => e.Patientid).HasColumnName("patientid");

            entity.HasOne(d => d.Medicine).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.Medicineid)
                .HasConstraintName("prescriptions_medicineid_fkey");

            entity.HasOne(d => d.Patient).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.Patientid)
                .HasConstraintName("prescriptions_patientid_fkey");
        });
        modelBuilder.HasSequence<int>("seq_schema_version", "graphql").IsCyclic();

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
