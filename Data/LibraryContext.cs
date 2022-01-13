using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Library_KP.Models;
using Library_KP.Models.ReadersModel;
using Library_KP.Models.BooksModel;
using Library_KP.Models.TerminalModel;

#nullable disable

namespace Library_KP.Data
{
    public partial class LibraryContext : DbContext
    {
        public LibraryContext()
        {
        }

        public LibraryContext(DbContextOptions<LibraryContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Partition> Partitions { get; set; }
        public virtual DbSet<Reader> Readers { get; set; }
        public virtual DbSet<Terminal> Terminals { get; set; }
        public virtual DbSet<ZaprosNew> ZaprosNews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.RegistrationId)
                    .HasName("PK_RegistrationID");

                entity.Property(e => e.Author)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Автора нет')");

                entity.HasOne(d => d.PartitionNameNavigation)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.PartitionName)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_BOOK_PartitionID");
            });

            modelBuilder.Entity<Partition>(entity =>
            {
                entity.Property(e => e.NamePartition).HasDefaultValueSql("('Отсутствует')");
            });

            modelBuilder.Entity<Reader>(entity =>
            {
                entity.HasKey(e => e.NumberTicket)
                    .HasName("PK_NumberTicket");

                entity.Property(e => e.HomeAddress).HasDefaultValueSql("('Домашний адрес отсутствует')");
            });

            modelBuilder.Entity<Terminal>(entity =>
            {
                entity.HasOne(d => d.NumberTicketsNavigation)
                    .WithMany(p => p.Terminals)
                    .HasForeignKey(d => d.NumberTickets)
                    .HasConstraintName("FK_TERMINAL_NumberTicket");

                entity.HasOne(d => d.RegistrationBook)
                    .WithMany(p => p.Terminals)
                    .HasForeignKey(d => d.RegistrationBookId)
                    .HasConstraintName("FK_TERMINAL_RegistrationID");
            });

            modelBuilder.Entity<ZaprosNew>(entity =>
            {
                entity.ToView("ZaprosNew");

                entity.Property(e => e.Author).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
