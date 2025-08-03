using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MvcERPTest01.Models;

namespace MvcERPTest01.Models;

public class LoginViewModel
{
    [Required]
    public string AccountID { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
