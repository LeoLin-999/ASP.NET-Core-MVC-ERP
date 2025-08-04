using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcERPTest01.Models;

// 供應商主檔
public partial class Suppliers
{
	[ScaffoldColumn(false)]
	[Display(Name = "建立日期")]
    public DateTime Create_Date { get; set; }

    [ScaffoldColumn(false)]
	[Display(Name = "建立人員")]
    public string Creater { get; set; } = null!;

	[Display(Name = "修改日期")]
    public DateTime? Update_Date { get; set; }

	[Display(Name = "修改人員")]
    public string? Updater { get; set; }

    [Key]
	[Display(Name = "供應商代號")]
    public string Supplier_ID { get; set; } = null!;

	[Display(Name = "供應商名稱")]
    public string? Supplier_Name { get; set; }

	[Display(Name = "供應商狀態")]
    public string? Supplier_Status { get; set; }

	[Display(Name = "供應商電子信箱")]
    public string? Supplier_EMail { get; set; }

	[Display(Name = "供應商地址")]
    public string? Supplier_Address { get; set; }
}
