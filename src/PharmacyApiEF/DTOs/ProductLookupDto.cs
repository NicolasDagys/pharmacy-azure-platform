namespace PharmacyApiEF.DTOs
{
    public class ProductLookupDto
    {
        public string CodProd { get; set; } = null!;

        public string NameProd { get; set; } = null!;

        public decimal PriceProd { get; set; }
    }
}
