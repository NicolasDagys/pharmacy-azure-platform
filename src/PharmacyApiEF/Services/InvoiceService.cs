using Microsoft.EntityFrameworkCore;
using PharmacyApiEF.DTOs;
using PharmacyApiEF.Models;
using System.Text.RegularExpressions;

namespace PharmacyApiEF.Services.Interfaces;

public class InvoiceService : IInvoiceService
{
    private readonly PharmacyContext _context;

    public InvoiceService(PharmacyContext context)
    {
        _context = context;
    }

    public string? ValidateInvoice(InvoiceCreateDto dto)
    {
        if (dto == null)
            return "Invoice is required.";

        if (string.IsNullOrWhiteSpace(dto.IdCus))
            return "Customer is required.";

        if (string.IsNullOrWhiteSpace(dto.ShipmentAddressInv))
            return "Shipment address is required.";

        if (dto.ShipmentAddressInv.Trim().Length < 5)
            return "Shipment address is too short.";

        if (dto.ShipmentAddressInv.Length > 50)
            return "Shipment address cannot exceed 50 characters.";

        if (dto.InvoiceLines == null || !dto.InvoiceLines.Any())
            return "Invoice must contain at least one line.";

        foreach (var line in dto.InvoiceLines)
        {
            if (string.IsNullOrWhiteSpace(line.CodProd))
                return "Product code is required.";

            if (line.Quantity <= 0)
                return "Quantity must be greater than zero.";

            if (!Regex.IsMatch(
                    line.CodProd,
                    @"^[A-Z]{3}[0-9]{7}$"))
            {
                return $"Invalid product code: {line.CodProd}";
            }
        }

        if (dto.InvoiceLines
            .GroupBy(l => l.CodProd)
            .Any(g => g.Count() > 1))
        {
            return "Duplicate products are not allowed.";
        }

        return null;
    }

    public async Task<bool> CustomerExists(string idCus)
    {
        return await _context.Customers
            .AnyAsync(c => c.IdCus == idCus);
    }

    public async Task<bool> ProductsExist(List<InvoiceLineDto> lines)
    {
        /*foreach (var line in lines)
        {
            bool exists = await _context.Products
                .AnyAsync(p =>
                    p.CodProd == line.CodProd &&
                    p.Active);

            if (!exists)
                return false;
        }*/

        //to avoid multiple database calls, we can get all the product codes from the lines and check if they exist in the database in a single query
        var productCodes = lines
    .Select(l => l.CodProd)
    .Distinct()
    .ToList();

        int existingProducts = await _context.Products
            .CountAsync(p =>
                productCodes.Contains(p.CodProd) &&
                p.Active);

        return existingProducts == productCodes.Count;
    }

    public async Task<bool> HasEnoughStock(List<InvoiceLineDto> lines)
    {

        var requestedCodes = lines
        .Select(l => l.CodProd)
        .Distinct()
        .ToList();

        var products = await _context.Products
            .Where(p => requestedCodes.Contains(p.CodProd))
            .ToDictionaryAsync(
                p => p.CodProd,
                p => p.StockQty);

        if (products.Count != requestedCodes.Count)
            return false;

        foreach (var line in lines)
        {
            if (products[line.CodProd] < line.Quantity)
                return false;
        }

        return true;
    }

    public async Task<bool> InvoiceExists(int numbInv)
    {
        return await _context.Invoices
            .AnyAsync(i => i.NumbInv == numbInv);
    }

    public async Task<int?> GetStatusId(string statusName)
    {
        var status = await _context.OrderStatuses
            .FirstOrDefaultAsync(s =>
                s.NameSta == statusName);

        return status?.NumbSta;
    }

    public async Task<bool> IsValidStatusTransition(
        int numbInv,
        int newStatus)
    {
        var currentStatus = await _context.Assignments
            .Where(a => a.NumbInv == numbInv)
            .OrderByDescending(a => a.DateTimeStatus)
            .Select(a => a.NumbSta)
            .FirstOrDefaultAsync();

        if (currentStatus == 0)
            return false;

        return newStatus == currentStatus + 1;
    }
}
