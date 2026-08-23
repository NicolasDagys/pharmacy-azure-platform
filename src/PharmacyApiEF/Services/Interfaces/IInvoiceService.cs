using PharmacyApiEF.DTOs;

namespace PharmacyApiEF.Services.Interfaces
{
    public interface IInvoiceService
    {
        string? ValidateInvoice(InvoiceCreateDto dto); //we are useing CreateInvoiceDto instead of InvoiceDto because we want to validate the data before creating the invoice
        Task<bool> CustomerExists(string idCus);

        Task<bool> ProductsExist(List<InvoiceLineDto> lines);

        Task<bool> HasEnoughStock(List<InvoiceLineDto> lines);

        Task<bool> InvoiceExists(int numbInv);

        Task<int?> GetStatusId(string statusName);

        Task<bool> IsValidStatusTransition(int numbInv, int newStatus);

    }
}
