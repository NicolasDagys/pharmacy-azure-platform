using System;
using System.Collections.Generic;

namespace PharmacyApiEF.Models;

public partial class Customer
{
    public string IdCus { get; set; } = null!;

    public string NameCus { get; set; } = null!;

    public string? PaymentTokenCus { get; set; }

    public string MailCus { get; set; } = null!;

    public string? PhoneCus { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
