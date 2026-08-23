using System;
using System.Collections.Generic;

namespace PharmacyApiEF.Models;

public partial class Category
{
    public string CodeCat { get; set; } = null!;

    public string NameCat { get; set; } = null!;

    public bool Active { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
