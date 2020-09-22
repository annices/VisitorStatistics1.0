using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace VisitorStatistics.Models
{
    public partial class VisitorStatisticsContext : DbContext
    {
        public VisitorStatisticsContext()
        {
        }

        public VisitorStatisticsContext(DbContextOptions<VisitorStatisticsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<VsAdminVisit> VsAdminVisits { get; set; }
        public virtual DbSet<VsAdmin> VsAdmins { get; set; }
        public virtual DbSet<VsAppUrl> VsAppUrls { get; set; }
        public virtual DbSet<VsApplication> VsApplications { get; set; }
        public virtual DbSet<VsVisitor> VsVisitors { get; set; }
        public virtual DbSet<VsVisit> VsVisits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VsAdminVisit>(entity =>
            {
                entity.ToTable("VS_AdminVisits");

                entity.HasIndex(e => new { e.AdminId, e.VisitId })
                    .HasName("Unique_AdminVisit")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AdminId).HasColumnName("AdminID");

                entity.Property(e => e.VisitId).HasColumnName("VisitID");

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.VsAdminVisits)
                    .HasForeignKey(d => d.AdminId)
                    .HasConstraintName("FK__VS_AdminV__Admin__20C1E124");

                entity.HasOne(d => d.Visit)
                    .WithMany(p => p.VsAdminVisits)
                    .HasForeignKey(d => d.VisitId)
                    .HasConstraintName("FK__VS_AdminV__Visit__21B6055D");
            });

            modelBuilder.Entity<VsAdmin>(entity =>
            {
                entity.ToTable("VS_Admins");

                entity.HasIndex(e => e.Email)
                    .HasName("UQ__VS_Admin__A9D10534384C10A6")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Firstname)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Lastname)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VsAppUrl>(entity =>
            {
                entity.ToTable("VS_AppURLs");

                entity.HasIndex(e => new { e.AppId, e.RegisteredUrl })
                    .HasName("Unique_AppURL")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AppId).HasColumnName("AppID");

                entity.Property(e => e.RegisteredUrl)
                    .IsRequired()
                    .HasColumnName("RegisteredURL")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.App)
                    .WithMany(p => p.VsAppUrls)
                    .HasForeignKey(d => d.AppId)
                    .HasConstraintName("FK__VS_AppURL__AppID__1B0907CE");
            });

            modelBuilder.Entity<VsApplication>(entity =>
            {
                entity.ToTable("VS_Applications");

                entity.HasIndex(e => e.Name)
                    .HasName("UQ__VS_Appli__737584F6E8CEE1D3")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VsVisitor>(entity =>
            {
                entity.ToTable("VS_Visitors");

                entity.HasIndex(e => e.Ipaddress)
                    .HasName("UQ__VS_Visit__F0C25BE088624418")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AppId).HasColumnName("AppID");

                entity.Property(e => e.Agent)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DeleteDate).HasColumnType("date");

                entity.Property(e => e.HostName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.IsIgnored).HasColumnName("IsIgnored");

                entity.Property(e => e.Ipaddress)
                    .IsRequired()
                    .HasColumnName("IPAddress")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.HasOne(d => d.App)
                    .WithMany(p => p.VsVisitors)
                    .HasForeignKey(d => d.AppId)
                    .HasConstraintName("FK__VS_Visito__AppID__182C9B23");
            });

            modelBuilder.Entity<VsVisit>(entity =>
            {
                entity.ToTable("VS_Visits");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.RefererUrl)
                    .IsRequired()
                    .HasColumnName("RefererURL")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.VisitTime).HasColumnType("datetime");

                entity.Property(e => e.VisitUrl)
                    .IsRequired()
                    .HasColumnName("VisitURL")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.VisitorId).HasColumnName("VisitorID");

                entity.HasOne(d => d.Visitor)
                    .WithMany(p => p.VsVisits)
                    .HasForeignKey(d => d.VisitorId)
                    .HasConstraintName("FK__VS_Visits__Visit__1DE57479");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
