using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MvcERPTest01.Models;

namespace MvcERPTest01.Models;

// 採購單詳細頁可新增採購明細用
public class PurchaseDetailViewModel
{
	// 採購主檔 DTO（顯示用）
    public PurchaseDto PurchaseInfo { get; set; } = new();
	// 採購主檔（表單用）
    public Purchase PurchaseForm { get; set; } = new();
	// 採購明細（表單用）
    public Purchase_Detail DetailForm { get; set; } = new();
	// 已有明細清單（table 顯示）
    public List<Purchase_Detail> DetailList { get; set; } = new();
}
