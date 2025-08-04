using System;
using System.ComponentModel.DataAnnotations;

// 帳號主檔
public partial class Sys_AccountsDto
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
	[Display(Name = "帳號")]
    public string AccountID { get; set; } = null!;

	[Display(Name = "帳號狀態")]
    public string? AccountStatus { get; set; }

	[Display(Name = "姓名")]
    public string? FullName { get; set; }

	[Display(Name = "電子信箱")]
    public string? EMail { get; set; }

	[Display(Name = "建立人員")]
    public string? CreaterName { get; set; }
	[Display(Name = "修改人員")]
    public string? UpdaterName { get; set; }
}
