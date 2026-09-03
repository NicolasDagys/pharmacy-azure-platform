namespace PharmacyApiEF.DTOs
{
    public class InvoiceDetailsDto
    {
        public int NumbInv { get; set; }

        public DateOnly DateInv { get; set; }

        public string ShipmentAddressInv { get; set; } = null!;

        public decimal TotalInv { get; set; }

        public string IdCus { get; set; } = null!;

        public string CustomerName { get; set; } = null!;

        public string Status { get; set; } = null!;

        public List<InvoiceLineDetailDto> InvoiceLines { get; set; }
            = new();
    }
}
