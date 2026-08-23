namespace PharmacyApiEF.DTOs
{
    public class InvoiceLineDetailDto
    {
        public string CodProd { get; set; } = null!;

        public string ProductName { get; set; } = null!;

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal SubTotal { get; set; }
    }
}
