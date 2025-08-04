using System;
using System.ComponentModel.DataAnnotations;

// 採購主檔
public partial class PurchaseDto
{
	[Display(Name = "建立日期")]
    public DateTime Create_Date { get; set; }

	[Display(Name = "建立帳號")]
    public string Creater { get; set; } = null!;

	[Display(Name = "修改日期")]
    public DateTime? Update_Date { get; set; }

	[Display(Name = "修改帳號")]
    public string? Updater { get; set; }

    [Key]
	[Display(Name = "採購單號")]
    public string Purchase_ID { get; set; } = null!;

	[Display(Name = "供應商代號")]
    public string? Supplier_ID { get; set; }

	[Display(Name = "供應商名稱")]
    public string? Supplier_Name { get; set; }

	[Display(Name = "倉庫代號")]
    public string? Warehouse_ID { get; set; }

	[Display(Name = "進貨倉庫")]
    public string? Warehouse_Name { get; set; }

	[Display(Name = "採購日期")]
    public DateTime? Purchase_Date { get; set; }

	[Display(Name = "採購狀態")]
    public string? Purchase_Status { get; set; }

	[Display(Name = "採購金額")]
    public decimal? Total_Amount { get; set; }

	[Display(Name = "到貨日期")]
    public DateTime? Arrival_Date { get; set; }

	[Display(Name = "建立人員")]
    public string? CreaterName { get; set; }
	[Display(Name = "修改人員")]
    public string? UpdaterName { get; set; }
}
