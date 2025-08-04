using System;
using System.ComponentModel.DataAnnotations;

// 倉庫主檔
public partial class WarehouseDto
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
	[Display(Name = "倉庫代號")]
    public string Warehouse_ID { get; set; } = null!;

	[Display(Name = "倉庫名稱")]
    public string? Warehouse_Name { get; set; }

	[Display(Name = "倉庫狀態")]
    public string? Warehouse_Status { get; set; }

	[Display(Name = "目前儲存量")]
    public int? Total_Quantity { get; set; }

	[Display(Name = "最大儲存量")]
    public int? Max_Quantity { get; set; }

	[Display(Name = "建立人員")]
    public string? CreaterName { get; set; }
	[Display(Name = "修改人員")]
    public string? UpdaterName { get; set; }
}
