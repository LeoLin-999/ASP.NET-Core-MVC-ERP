using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcERPTest01.Models;

// 採購明細
public partial class Purchase_Detail
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

	[Display(Name = "採購單號")]
    public string Purchase_ID { get; set; } = null!;

	[Display(Name = "產品代號")]
    public string Product_ID { get; set; } = null!;

	[Display(Name = "採購數量")]
    public int? Purchase_Qty { get; set; }

	[Display(Name = "採購金額")]
    public decimal? Purchase_Amount { get; set; }

	[Display(Name = "採購小計")]
    public decimal? Purchase_SubTotal { get; set; }
}
