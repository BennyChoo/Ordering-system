using Microsoft.EntityFrameworkCore;

namespace QFD.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        public virtual DbSet<Table> Table { get; set; }

        public virtual DbSet<TableList> TableList { get; set; }
        public virtual DbSet<TableQrCodeList> TableQrCodeList { get; set; }
        public virtual DbSet<TableDetail> TableDetail { get; set; }
        public virtual DbSet<TableQrCode> TableQrCode { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductDetail> ProductDetail { get; set; }
        public virtual DbSet<ProductList> ProductList { get; set; }
        public virtual DbSet<ProductPrice> ProductPrice { get; set; }
        public virtual DbSet<ProductCategory> ProductCategory { get; set; }
        public virtual DbSet<AttachmentSummary> AttachmentSummary { get; set; }

        public virtual DbSet<Attachment> Attachment { get; set; }
        public virtual DbSet<AttachmentType> AttachmentType { get; set; }
        public virtual DbSet<AttachmentEntityMap> AttachmentEntityMap { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attachment>(entity =>
            {
                entity.Property(e => e.Comment).HasMaxLength(200);
                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.FilePath)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<AttachmentEntityMap>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.DeletedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Attachment)
                    .WithMany(p => p.AttachmentEntityMap)
                    .HasForeignKey(d => d.AttachmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AttachmentEntityMap_Attachment");
                entity.HasOne(d => d.EntityType)
                    .WithMany(p => p.AttachmentEntityMap)
                    .HasForeignKey(d => d.EntityTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AttachmentEntityMap_EntityType");
            });

            modelBuilder.Entity<AttachmentType>(entity =>
            {
                entity.HasKey(e => e.AttachmentTypeId);

            });

            modelBuilder.Entity<AttachmentSummary>(entity => { entity.HasKey(e => e.FileId); });

        }



    }
}
