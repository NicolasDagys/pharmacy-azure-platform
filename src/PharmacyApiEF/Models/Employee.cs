using System;
using System.Collections.Generic;

namespace PharmacyApiEF.Models;

public partial class Employee
{
    public string UserEmp { get; set; } = null!;

    public string NameEmp { get; set; } = null!;

    public string PassHashEmp { get; set; } = null!;

    public string RoleEmp { get; set; } = null!;
}
