using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcERPTest01.Models;

// 採購主檔
public partial class Purchase
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
	[Display(Name = "採購單號")]
    public string Purchase_ID { get; set; } = null!;

	[Display(Name = "供應商代號")]
    public string? Supplier_ID { get; set; }

	[Display(Name = "倉庫代號")]
    public string? Warehouse_ID { get; set; }

	[DataType(DataType.Date)]
    public DateTime? Purchase_Date { get; set; }

	[Display(Name = "採購狀態")]
    public string? Purchase_Status { get; set; }

	[Display(Name = "採購金額")]
    public decimal? Total_Amount { get; set; }

	[DataType(DataType.Date)]
	[Display(Name = "到貨日期")]
    public DateTime? Arrival_Date { get; set; }
}
