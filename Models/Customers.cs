using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcERPTest01.Models;

// 客戶主檔
public partial class Customers
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
}
