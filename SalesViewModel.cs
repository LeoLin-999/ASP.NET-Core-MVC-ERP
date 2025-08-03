using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MvcERPTest01.Models;

namespace MvcERPTest01.Models;

// 銷貨單詳細頁可顯示銷貨明細用
public class SalesViewModel
{
	// 銷貨主檔 DTO（顯示用）
    public SalesDto SalesInfo { get; set; } = new();
	// 銷貨主檔（表單用）
    public Sales SalesForm { get; set; } = new();
	// 銷貨明細清單（table 顯示）
    public List<Sales_DetailDto> DetailList { get; set; } = new();
}
