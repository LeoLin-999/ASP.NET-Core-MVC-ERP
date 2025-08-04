using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcERPTest01.Models;

// 銷貨明細
public partial class Sales_Detail
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
	[Display(Name = "流水號")]
    public int SequenceID { get; set; }

	[Display(Name = "銷貨單號")]
    public string Sales_ID { get; set; } = null!;

	[Display(Name = "產品代號")]
    public string Product_ID { get; set; } = null!;

	[Display(Name = "產品數量")]
    public int? Sales_Qty { get; set; }

	[Display(Name = "產品單價")]
    public decimal? Sales_Amount { get; set; }

	[Display(Name = "小計金額")]
    public decimal? Sales_SubTotal { get; set; }
}
