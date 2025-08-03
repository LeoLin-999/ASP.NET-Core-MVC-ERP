using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcERPTest01.Models;

// 訂單主檔
public partial class Orders
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
	[Display(Name = "訂單編號")]
    public string Orders_ID { get; set; } = null!;

	[Display(Name = "客戶代號")]
    public string? Customer_ID { get; set; }

	[DataType(DataType.Date)]
	[Display(Name = "訂單日")]
    public DateTime? Orders_Date { get; set; }

	[Display(Name = "訂單狀態")]
    public string? Orders_Status { get; set; }

	[Display(Name = "訂單類型")]
    public string? Orders_Type { get; set; }

	[Display(Name = "訂單金額")]
    public decimal? Total_Amount { get; set; }

	[DataType(DataType.Date)]
	[Display(Name = "送貨日期")]
    public DateTime? Delivery_Date { get; set; }
}
