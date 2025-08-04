using System;
using System.ComponentModel.DataAnnotations;

// 銷貨主檔
public partial class SalesDto
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
	[Display(Name = "銷貨單號")]
    public string Sales_ID { get; set; } = null!;

	[Display(Name = "訂單編號")]
    public string Orders_ID { get; set; }

	[Display(Name = "銷貨日期")]
    public DateTime? Sales_Date { get; set; }

	[Display(Name = "銷貨狀態")]
    public string? Sales_Status { get; set; }

	[Display(Name = "銷貨金額")]
    public decimal? Total_Amount { get; set; }

	[Display(Name = "出貨日期")]
    public DateTime? Shipping_Date { get; set; }

	[Display(Name = "建立人員")]
    public string? CreaterName { get; set; }
	[Display(Name = "修改人員")]
    public string? UpdaterName { get; set; }
}
