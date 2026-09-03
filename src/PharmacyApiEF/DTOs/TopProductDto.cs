namespace PharmacyApiEF.DTOs
{
    public class TopProductDto
    {
        public string CodProd { get; set; } = null!;

        public string NameProd { get; set; } = null!;

        public int QuantitySold { get; set; }
    }
}
