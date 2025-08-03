using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MvcERPTest01.Models;

namespace MvcERPTest01.Models;

// 訂單詳細頁可新增訂單明細用
public class OrdersViewModel
{
	// 採購主檔 DTO（顯示用）
    public OrdersDto OrdersInfo { get; set; } = new();
	// 採購主檔（表單用）
    public Orders OrdersForm { get; set; } = new();
	// 採購明細（表單用）
    public Orders_Detail DetailForm { get; set; } = new();
	// 已有明細清單（table 顯示）
    public List<Orders_DetailDto> DetailList { get; set; } = new();
}
