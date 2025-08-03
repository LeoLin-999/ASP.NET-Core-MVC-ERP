using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcERPTest01.Models;

// 庫存主檔
public partial class Inventory
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
	[Display(Name = "庫存代號")]
    public string Inventory_ID { get; set; } = null!;

	[Display(Name = "產品代號")]
    public string? Product_ID { get; set; }

	[Display(Name = "倉庫代號")]
    public string? Warehouse_ID { get; set; }

	[Display(Name = "庫存狀態")]
    public string? Inventory_Status { get; set; }

	[Display(Name = "庫存數量")]
    public int? Quantity { get; set; }

	[Display(Name = "最小庫存層級")]
    public int? MinStockLevel { get; set; }

	[Display(Name = "最大庫存層級")]
    public int? MaxStockLevel { get; set; }

	[Display(Name = "最後入庫日期")]
    public DateTime? LastStockInDate { get; set; }

	[Display(Name = "最後出庫日期")]
    public DateTime? LastStockOutDate { get; set; }

	[Display(Name = "到期日")]
    public DateTime? ExpirationDate { get; set; }

	[Display(Name = "備註說明")]
    public string? Notes { get; set; }
}
