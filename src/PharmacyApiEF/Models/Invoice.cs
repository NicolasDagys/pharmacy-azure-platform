using System;
using System.Collections.Generic;

namespace PharmacyApiEF.Models;

public partial class Invoice
{
    public int NumbInv { get; set; }

    public DateOnly DateInv { get; set; }

    public string ShipmentAddressInv { get; set; } = null!;

    public decimal TotalInv { get; set; }

    public string IdCus { get; set; } = null!;

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual Customer IdCusNavigation { get; set; } = null!;

    public virtual ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
}
