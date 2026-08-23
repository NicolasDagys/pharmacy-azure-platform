using System;
using System.Collections.Generic;

namespace PharmacyApiEF.Models;

public partial class Assignment
{
    public int NumbInv { get; set; }

    public int NumbSta { get; set; }

    public DateTime DateTimeStatus { get; set; }

    public virtual Invoice NumbInvNavigation { get; set; } = null!;

    public virtual OrderStatus NumbStaNavigation { get; set; } = null!;
}
