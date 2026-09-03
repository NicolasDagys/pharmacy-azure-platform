namespace PharmacyApiEF.DTOs
{
    public class ProductDto
    {
        public string CodProd { get; set; }

        public string NameProd { get; set; }

        public decimal PriceProd { get; set; }

        public DateOnly ExpDateProd { get; set; }

        public string PresentationTypeProd { get; set; }

        public int SizeProd { get; set; }

        public string CodeCat { get; set; }

        public int StockQty { get; set; }
    }
}
