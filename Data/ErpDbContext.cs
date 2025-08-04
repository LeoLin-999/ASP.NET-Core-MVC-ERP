using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcERPTest01.Models;

    public class ErpDbContext : DbContext
    {
        public ErpDbContext (DbContextOptions<ErpDbContext> options)
            : base(options)
        {
        }

        public DbSet<MvcERPTest01.Models.Sys_Accounts> Sys_Accounts { get; set; } = default!;

public DbSet<MvcERPTest01.Models.Customers> Customers { get; set; } = default!;

public DbSet<MvcERPTest01.Models.Suppliers> Suppliers { get; set; } = default!;

public DbSet<MvcERPTest01.Models.Warehouse> Warehouse { get; set; } = default!;

public DbSet<MvcERPTest01.Models.Inventory> Inventory { get; set; } = default!;

public DbSet<MvcERPTest01.Models.Products> Products { get; set; } = default!;

public DbSet<MvcERPTest01.Models.Purchase> Purchase { get; set; } = default!;

public DbSet<MvcERPTest01.Models.Purchase_Detail> Purchase_Detail { get; set; } = default!;

public DbSet<MvcERPTest01.Models.Orders> Orders { get; set; } = default!;

public DbSet<MvcERPTest01.Models.Orders_Detail> Orders_Detail { get; set; } = default!;

public DbSet<MvcERPTest01.Models.Sales> Sales { get; set; } = default!;

public DbSet<MvcERPTest01.Models.Sales_Detail> Sales_Detail { get; set; } = default!;
    }
