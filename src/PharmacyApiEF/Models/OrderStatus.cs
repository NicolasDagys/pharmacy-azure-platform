using System;
using System.Collections.Generic;

namespace PharmacyApiEF.Models;

public partial class OrderStatus
{
    public int NumbSta { get; set; }

    public string NameSta { get; set; } = null!;

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}
