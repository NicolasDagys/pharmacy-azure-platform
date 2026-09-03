using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PharmacyApiEF.Models;

public partial class PharmacyContext : DbContext
{
    public PharmacyContext()
    {
    }

    public PharmacyContext(DbContextOptions<PharmacyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceLine> InvoiceLines { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(e => new { e.NumbInv, e.NumbSta });

            entity.ToTable("Assignment");

            entity.Property(e => e.DateTimeStatus).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.NumbInvNavigation).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.NumbInv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Assignment_Invoice");

            entity.HasOne(d => d.NumbStaNavigation).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.NumbSta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Assignment_OrderStatus");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CodeCat);

            entity.ToTable("Category");

            entity.Property(e => e.CodeCat)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.NameCat)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.IdCus);

            entity.ToTable("Customer");

            entity.Property(e => e.IdCus)
                .HasMaxLength(9)
                .IsUnicode(false);
            entity.Property(e => e.MailCus)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NameCus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PaymentTokenCus)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneCus)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.UserEmp);

            entity.ToTable("Employee");

            entity.Property(e => e.UserEmp)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NameEmp)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PassHashEmp)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RoleEmp)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.NumbInv);

            entity.ToTable("Invoice");

            entity.Property(e => e.DateInv).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdCus)
                .HasMaxLength(9)
                .IsUnicode(false);
            entity.Property(e => e.ShipmentAddressInv)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TotalInv).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdCusNavigation).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.IdCus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoice_Customer");
        });

        modelBuilder.Entity<InvoiceLine>(entity =>
        {
            entity.HasKey(e => new { e.NumbInv, e.CodProd }).HasName("PK_Line");

            entity.ToTable("InvoiceLine");

            entity.Property(e => e.CodProd)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.CodProdNavigation).WithMany(p => p.InvoiceLines)
                .HasForeignKey(d => d.CodProd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Line_Product");

            entity.HasOne(d => d.NumbInvNavigation).WithMany(p => p.InvoiceLines)
                .HasForeignKey(d => d.NumbInv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Line_Invoice");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.NumbSta);

            entity.ToTable("OrderStatus");

            entity.Property(e => e.NumbSta).ValueGeneratedNever();
            entity.Property(e => e.NameSta)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.CodProd);

            entity.ToTable("Product");

            entity.Property(e => e.CodProd)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.CodeCat)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.NameProd)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.PresentationTypeProd)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.PriceProd).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.CodeCatNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.CodeCat)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Category");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
