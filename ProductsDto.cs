using System;
using System.ComponentModel.DataAnnotations;

public class ProductsDto
{
	[Display(Name = "建立日期")]
    public DateTime Create_Date { get; set; }
	[Display(Name = "建立帳號")]
    public string Creater { get; set; } = null!;
	[Display(Name = "修改日期")]
    public DateTime? Update_Date { get; set; }
	[Display(Name = "修改帳號")]
    public string? Updater { get; set; }
	[Display(Name = "產品代號")]
    public string Product_ID { get; set; } = null!;
	[Display(Name = "產品名稱")]
    public string? Product_Name { get; set; }
	[Display(Name = "產品狀態")]
    public string? Product_Status { get; set; }
	[Display(Name = "價格")]
    public decimal? Price { get; set; }

	[Display(Name = "建立人員")]
    public string? CreaterName { get; set; }
	[Display(Name = "修改人員")]
    public string? UpdaterName { get; set; }
}