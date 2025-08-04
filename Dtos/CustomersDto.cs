using System;
using System.ComponentModel.DataAnnotations;

// 客戶主檔
public partial class CustomersDto
{
	[Display(Name = "建立日期")]
    public DateTime Create_Date { get; set; }

	[Display(Name = "建立帳號")]
    public string Creater { get; set; } = null!;

	[Display(Name = "修改日期")]
    public DateTime? Update_Date { get; set; }

	[Display(Name = "修改帳號")]
    public string? Updater { get; set; }

	[Display(Name = "客戶代號")]
    public string Customer_ID { get; set; } = null!;

	[Display(Name = "客戶名稱")]
    public string? Customer_Name { get; set; }

	[Display(Name = "客戶狀態")]
    public string? Customer_Status { get; set; }

	[Display(Name = "客戶電子信箱")]
    public string? Customer_EMail { get; set; }

	[Display(Name = "客戶地址")]
    public string? Customer_Address { get; set; }

	[Display(Name = "建立人員")]
    public string? CreaterName { get; set; }
	[Display(Name = "修改人員")]
    public string? UpdaterName { get; set; }
}
