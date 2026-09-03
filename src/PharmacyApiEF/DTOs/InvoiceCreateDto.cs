namespace PharmacyApiEF.DTOs
{
    public class InvoiceCreateDto
    {
        public string ShipmentAddressInv { get; set; } = null!;
        public string IdCus { get; set; } = null!;
        public List<InvoiceLineDto> InvoiceLines { get; set; }
            = new();
    }
}
