using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcERPTest01.Models;

// 產品主檔
public partial class Products
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
	[Display(Name = "產品代號")]
    public string Product_ID { get; set; } = null!;

	[Display(Name = "產品名稱")]
    public string? Product_Name { get; set; }

	[Display(Name = "產品狀態")]
    public string? Product_Status { get; set; }

	[Display(Name = "價格")]
    public decimal? Price { get; set; }
}
