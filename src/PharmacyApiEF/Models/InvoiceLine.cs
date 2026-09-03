using System;
using System.Collections.Generic;

namespace PharmacyApiEF.Models;

public partial class InvoiceLine
{
    public int NumbInv { get; set; }

    public string CodProd { get; set; } = null!;

    public int Cant { get; set; }

    public decimal UnitPrice { get; set; }

    public virtual Product CodProdNavigation { get; set; } = null!;

    public virtual Invoice NumbInvNavigation { get; set; } = null!;
}
