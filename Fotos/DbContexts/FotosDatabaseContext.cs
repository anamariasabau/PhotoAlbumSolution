using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Fotos.Model;

namespace Fotos.DbContexts
{
    public partial class FotosDatabaseContext : DbContext
    {
        public FotosDatabaseContext()
        {
        }

        public FotosDatabaseContext(DbContextOptions<FotosDatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ImageThumbnails> ImageThumbnails { get; set; }
        public virtual DbSet<Images> Images { get; set; }
        public virtual DbSet<Thumbnails> Thumbnails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ImageThumbnails>(entity =>
            {
                entity.HasKey(e => new { e.ImageId, e.ThumbnailId });

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.ThumbnailId).HasColumnName("thumbnail_id");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.ImageThumbnails)
                    .HasForeignKey(d => d.ImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Thumbnail)
                    .WithMany(p => p.ImageThumbnails)
                    .HasForeignKey(d => d.ThumbnailId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Images>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Colorspace).HasColumnName("colorspace");

                entity.Property(e => e.DatetimeModified).HasColumnName("datetime_modified");

                entity.Property(e => e.DatetimeOriginal).HasColumnName("datetime_original");

                entity.Property(e => e.ExposureIme)
                    .HasColumnName("exposure_ime")
                    .HasColumnType("bigint");

                entity.Property(e => e.Fnumber).HasColumnName("fnumber");

                entity.Property(e => e.ImageLength)
                    .HasColumnName("image_length")
                    .HasColumnType("bigint");

                entity.Property(e => e.ImageName)
                    .IsRequired()
                    .HasColumnName("image_name");

                entity.Property(e => e.ImagePath)
                    .IsRequired()
                    .HasColumnName("image_path");

                entity.Property(e => e.ImageType)
                    .IsRequired()
                    .HasColumnName("image_type");

                entity.Property(e => e.Make).HasColumnName("make");

                entity.Property(e => e.Model).HasColumnName("model");

                entity.Property(e => e.Software).HasColumnName("software");

                entity.Property(e => e.Xresolution)
                    .HasColumnName("xresolution")
                    .HasColumnType("bigint");

                entity.Property(e => e.Yresolution)
                    .HasColumnName("yresolution")
                    .HasColumnType("bigint");
            });

            modelBuilder.Entity<Thumbnails>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedTime)
                    .IsRequired()
                    .HasColumnName("created_time");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.DimensionsUnit)
                    .IsRequired()
                    .HasColumnName("dimensions_unit");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasColumnName("file_name");

                entity.Property(e => e.Height)
                    .HasColumnName("height")
                    .HasColumnType("bigint");

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasColumnName("path");

                entity.Property(e => e.ThumbnailImage)
                    .HasColumnName("thumbnail_image")
                    .HasColumnType("image");

                entity.Property(e => e.Width)
                    .HasColumnName("width")
                    .HasColumnType("bigint");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
