using System;
using System.Collections.Generic;

namespace PharmacyApiEF.Models;

public partial class Product
{
    public string CodProd { get; set; } = null!;

    public string NameProd { get; set; } = null!;

    public decimal PriceProd { get; set; }

    public DateOnly ExpDateProd { get; set; }

    public string PresentationTypeProd { get; set; } = null!;

    public int SizeProd { get; set; }

    public string CodeCat { get; set; } = null!;

    public int StockQty { get; set; }

    public bool Active { get; set; }

    public virtual Category CodeCatNavigation { get; set; } = null!;

    public virtual ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
}
