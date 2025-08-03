using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MvcERPTest01.Models;

public partial class ERPTEST01Context : DbContext
{
    public ERPTEST01Context()
    {
    }

    public ERPTEST01Context(DbContextOptions<ERPTEST01Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Customers> Customers { get; set; }

    public virtual DbSet<Inventory> Inventory { get; set; }

    public virtual DbSet<Orders> Orders { get; set; }

    public virtual DbSet<Orders_Detail> Orders_Detail { get; set; }

    public virtual DbSet<Products> Products { get; set; }

    public virtual DbSet<Purchase> Purchase { get; set; }

    public virtual DbSet<Purchase_Detail> Purchase_Detail { get; set; }

    public virtual DbSet<Sales> Sales { get; set; }

    public virtual DbSet<Sales_Detail> Sales_Detail { get; set; }

    public virtual DbSet<Suppliers> Suppliers { get; set; }

    public virtual DbSet<Sys_Accounts> Sys_Accounts { get; set; }

    public virtual DbSet<Warehouse> Warehouse { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=ERPTEST01;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customers>(entity =>
        {
            entity.HasKey(e => e.Customer_ID);

            entity.Property(e => e.Customer_ID)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Create_Date).HasColumnType("datetime");
            entity.Property(e => e.Creater)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Customer_Address).HasMaxLength(200);
            entity.Property(e => e.Customer_EMail).HasMaxLength(200);
            entity.Property(e => e.Customer_Name).HasMaxLength(100);
            entity.Property(e => e.Customer_Status)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.Update_Date).HasColumnType("datetime");
            entity.Property(e => e.Updater)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.Inventory_ID);

            entity.Property(e => e.Inventory_ID)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Create_Date).HasColumnType("datetime");
            entity.Property(e => e.Creater)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ExpirationDate).HasColumnType("datetime");
            entity.Property(e => e.Inventory_Status)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.LastStockInDate).HasColumnType("datetime");
            entity.Property(e => e.LastStockOutDate).HasColumnType("datetime");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Product_ID)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Update_Date).HasColumnType("datetime");
            entity.Property(e => e.Updater)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Warehouse_ID)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Orders>(entity =>
        {
            entity.HasKey(e => e.Orders_ID).HasName("PK_Oeders");

            entity.Property(e => e.Orders_ID)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Create_Date).HasColumnType("datetime");
            entity.Property(e => e.Creater)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Customer_ID)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Delivery_Date).HasColumnType("datetime");
            entity.Property(e => e.Orders_Date).HasColumnType("datetime");
            entity.Property(e => e.Orders_Status)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.Orders_Type)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.Total_Amount).HasColumnType("numeric(15, 2)");
            entity.Property(e => e.Update_Date).HasColumnType("datetime");
            entity.Property(e => e.Updater)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Orders_Detail>(entity =>
        {
            entity.HasKey(e => e.SequenceID).HasName("PK_OedersDetail");

            entity.Property(e => e.Create_Date).HasColumnType("datetime");
            entity.Property(e => e.Creater)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Orders_Amount).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.Orders_ID)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Warehouse_ID)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Orders_SubTotal).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.Product_ID)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Update_Date).HasColumnType("datetime");
            entity.Property(e => e.Updater)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Products>(entity =>
        {
            entity.HasKey(e => e.Product_ID);

            entity.Property(e => e.Product_ID)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Create_Date).HasColumnType("datetime");
            entity.Property(e => e.Creater)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.Product_Name).HasMaxLength(100);
            entity.Property(e => e.Product_Status)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.Update_Date).HasColumnType("datetime");
            entity.Property(e => e.Updater)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => e.Purchase_ID);

            entity.Property(e => e.Purchase_ID)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Arrival_Date).HasColumnType("datetime");
            entity.Property(e => e.Create_Date).HasColumnType("datetime");
            entity.Property(e => e.Creater)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Purchase_Date).HasColumnType("datetime");
            entity.Property(e => e.Purchase_Status)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.Supplier_ID)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Warehouse_ID)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Total_Amount).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.Update_Date).HasColumnType("datetime");
            entity.Property(e => e.Updater)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Purchase_Detail>(entity =>
        {
            entity.HasKey(e => e.SequenceID).HasName("PK_PurchaseDetail");

            entity.Property(e => e.Create_Date).HasColumnType("datetime");
            entity.Property(e => e.Creater)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Product_ID)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Purchase_Amount).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.Purchase_ID)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Purchase_SubTotal).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.Update_Date).HasColumnType("datetime");
            entity.Property(e => e.Updater)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Sales>(entity =>
        {
            entity.HasKey(e => e.Sales_ID);

            entity.Property(e => e.Sales_ID)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Orders_ID)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Create_Date).HasColumnType("datetime");
            entity.Property(e => e.Creater)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Sales_Date).HasColumnType("datetime");
            entity.Property(e => e.Sales_Status)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.Shipping_Date).HasColumnType("datetime");
            entity.Property(e => e.Total_Amount).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.Update_Date).HasColumnType("datetime");
            entity.Property(e => e.Updater)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Sales_Detail>(entity =>
        {
            entity.HasKey(e => e.SequenceID).HasName("PK_SalesDetail");

            entity.Property(e => e.Create_Date).HasColumnType("datetime");
            entity.Property(e => e.Creater)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Product_ID)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Sales_Amount).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.Sales_ID)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Sales_SubTotal).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.Update_Date).HasColumnType("datetime");
            entity.Property(e => e.Updater)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Suppliers>(entity =>
        {
            entity.HasKey(e => e.Supplier_ID);

            entity.Property(e => e.Supplier_ID)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Create_Date).HasColumnType("datetime");
            entity.Property(e => e.Creater)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Supplier_Address).HasMaxLength(200);
            entity.Property(e => e.Supplier_EMail).HasMaxLength(200);
            entity.Property(e => e.Supplier_Name).HasMaxLength(100);
            entity.Property(e => e.Supplier_Status)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.Update_Date).HasColumnType("datetime");
            entity.Property(e => e.Updater)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Sys_Accounts>(entity =>
        {
            entity.HasKey(e => e.AccountID);

            entity.Property(e => e.AccountID).HasMaxLength(10);
            entity.Property(e => e.AccountStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Create_Date).HasColumnType("datetime");
            entity.Property(e => e.Creater).HasMaxLength(10);
            entity.Property(e => e.EMail).HasMaxLength(200);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Update_Date).HasColumnType("datetime");
            entity.Property(e => e.Updater).HasMaxLength(10);
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.Warehouse_ID);

            entity.Property(e => e.Warehouse_ID)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Create_Date).HasColumnType("datetime");
            entity.Property(e => e.Creater)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Update_Date).HasColumnType("datetime");
            entity.Property(e => e.Updater)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Warehouse_Name).HasMaxLength(100);
            entity.Property(e => e.Warehouse_Status)
                .HasMaxLength(1)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
