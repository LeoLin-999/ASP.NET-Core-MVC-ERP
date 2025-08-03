using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcERPTest01.Models;

// 銷貨主檔
public partial class Sales
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
	[Display(Name = "銷貨單號")]
    public string Sales_ID { get; set; } = null!;

	[Display(Name = "訂單編號")]
    public string Orders_ID { get; set; }

	[Display(Name = "銷貨日期")]
	[DataType(DataType.Date)]
    public DateTime? Sales_Date { get; set; }

	[Display(Name = "銷貨狀態")]
    public string? Sales_Status { get; set; }

	[Display(Name = "銷貨金額")]
    public decimal? Total_Amount { get; set; }

	[Display(Name = "出貨日期")]
	[DataType(DataType.Date)]
    public DateTime? Shipping_Date { get; set; }
}
