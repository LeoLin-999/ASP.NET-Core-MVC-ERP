using System;
using System.ComponentModel.DataAnnotations;

// 銷貨明細
public partial class Sales_DetailDto
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
	[Display(Name = "流水號")]
    public int SequenceID { get; set; }

	[Display(Name = "銷貨單號")]
    public string Sales_ID { get; set; } = null!;

	[Display(Name = "產品代號")]
    public string Product_ID { get; set; } = null!;

	[Display(Name = "產品名稱")]
    public string Product_Name { get; set; } = "";

	[Display(Name = "產品數量")]
    public int? Sales_Qty { get; set; }

	[Display(Name = "產品單價")]
    public decimal? Sales_Amount { get; set; }

	[Display(Name = "小計金額")]
    public decimal? Sales_SubTotal { get; set; }

	[Display(Name = "建立人員")]
    public string? CreaterName { get; set; }
	[Display(Name = "修改人員")]
    public string? UpdaterName { get; set; }
}
