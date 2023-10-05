using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CSLibrary.Data.Models;

public partial class MfRadbContext : DbContext
{
    public MfRadbContext()
    {
    }

    public MfRadbContext(DbContextOptions<MfRadbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CardEvent> CardEvents { get; set; }

    public virtual DbSet<EventsType> EventsTypes { get; set; }

    public virtual DbSet<PayType> PayTypes { get; set; }

    public virtual DbSet<Place> Places { get; set; }

    public virtual DbSet<Point> Points { get; set; }

    public virtual DbSet<Qrevent> Qrevents { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseLazyLoadingProxies().UseSqlServer(AppConfig.Instance.DbConnectionString.ToString());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CardEvent>(entity =>
        {
            entity.HasIndex(e => e.PointId, "IX_CardEvents_PointId");

            entity.HasIndex(e => e.TypeId, "IX_CardEvents_TypeId");

            entity.Property(e => e.Card).HasMaxLength(50);
            entity.Property(e => e.Dt).HasColumnType("datetime");

            entity.HasOne(d => d.Point).WithMany(p => p.CardEvents)
                .HasForeignKey(d => d.PointId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CardEvents_Points");

            entity.HasOne(d => d.Type).WithMany(p => p.CardEvents)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CardEvents_EventsTypes");
        });

        modelBuilder.Entity<EventsType>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<PayType>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Place>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Point>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Qrevent>(entity =>
        {
            entity.ToTable("QREvents");

            entity.HasIndex(e => e.PayId, "IX_QREvents_PayId");

            entity.HasIndex(e => e.PointId, "IX_QREvents_PointId");

            entity.HasIndex(e => e.TypeId, "IX_QREvents_TypeId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Dt).HasColumnType("datetime");
            entity.Property(e => e.Fn)
                .HasMaxLength(16)
                .HasColumnName("FN");
            entity.Property(e => e.Fp)
                .HasMaxLength(16)
                .HasColumnName("FP");
            entity.Property(e => e.Sum).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Pay).WithMany(p => p.Qrevents)
                .HasForeignKey(d => d.PayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QREvents_PayTypes");

            entity.HasOne(d => d.Point).WithMany(p => p.Qrevents)
                .HasForeignKey(d => d.PointId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QREvents_Points");

            entity.HasOne(d => d.Type).WithMany(p => p.Qrevents)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QREvents_EventsTypes");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.PlaceId, "IX_Users_PlaceId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Before).HasColumnType("datetime");
            entity.Property(e => e.Card).HasMaxLength(50);
            entity.Property(e => e.Dt).HasColumnType("datetime");
            entity.Property(e => e.Id1).HasMaxLength(50);
            entity.Property(e => e.Id2).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Name2).HasMaxLength(50);
            entity.Property(e => e.Surname).HasMaxLength(50);

            entity.HasOne(d => d.Place).WithMany(p => p.Users)
                .HasForeignKey(d => d.PlaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Places");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
