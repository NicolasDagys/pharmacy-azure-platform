using Microsoft.EntityFrameworkCore;
using PharmacyApiEF.DTOs;
using PharmacyApiEF.Models;
using PharmacyApiEF.Services.Interfaces;

namespace PharmacyApiEF.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly PharmacyContext _context;

        public DashboardService(PharmacyContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto> GetDashboard()
        {
            return new DashboardDto
            {
                //public int ActiveProducts { get; set; }
                ActiveProducts = await GetActiveProducts(),

                //public List<TopProductDto> TopSellingProducts { get; set; } = new();
                TopSellingProducts = await GetTopSellingProducts(),

                //public int ProductsExpiringSoon { get; set; }
                ProductsExpiringSoon = await GetProductsExpiringSoon(),

                //public int OutOfStockProducts { get; set; }
                OutOfStockProducts = await GetOutOfStockProducts(),

                //public int TotalCustomers { get; set; }
                TotalCustomers = await GetTotalCustomers(),

                //public int TotalInvoices { get; set; }
                TotalInvoices = await GetTotalInvoices(),

                //public int PendingInvoices { get; set; }
                PendingInvoices = await GetPendingInvoices(),

                //public decimal MonthlyRevenue { get; set; }
                MonthlyRevenue = await GetMonthlyRevenue(),

                //public int LowStockProducts { get; set; }
                LowStockProducts = await GetLowStockProducts(),

                //public int DeliveredOrders { get; set; }
                DeliveredOrders = await GetDeliveredOrders()
            };
        }

        //=========================================================
        // Products
        //=========================================================

        private async Task<int> GetActiveProducts()
        {
            return await _context.Products
                .CountAsync(p => p.Active);
        }

        private async Task<int> GetProductsExpiringSoon()
        {
            return await _context.Products
                .CountAsync(p =>
                    p.Active &&
                    p.ExpDateProd <= DateOnly.FromDateTime(DateTime.Today.AddDays(30)));
        }

        private async Task<int> GetOutOfStockProducts()
        {
            return await _context.Products
                .CountAsync(p =>
                    p.Active &&
                    p.StockQty == 0);
        }

        private async Task<int> GetLowStockProducts()
        {
            return await _context.Products
                .CountAsync(p =>
                    p.Active &&
                    p.StockQty < 5);
        }

        //=========================================================
        // Customers
        //=========================================================

        private async Task<int> GetTotalCustomers()
        {
            return await _context.Customers.CountAsync();
        }

        //=========================================================
        // Invoices
        //=========================================================

        private async Task<int> GetTotalInvoices()
        {
            return await _context.Invoices.CountAsync();
        }

        private async Task<decimal> GetMonthlyRevenue()
        {
            return await _context.Invoices
                .Where(i =>
                    i.DateInv.Month == DateTime.Today.Month &&
                    i.DateInv.Year == DateTime.Today.Year)
                .SumAsync(i => (decimal?)i.TotalInv)
                ?? 0;
        }

        //=========================================================
        // Assignments
        //=========================================================

        private async Task<int> GetPendingInvoices()
        {
            return await _context.Assignments
                .GroupBy(a => a.NumbInv)
                .Select(g => g
                    .OrderByDescending(a => a.DateTimeStatus)
                    .First())
                .CountAsync(a => a.NumbSta != 4);
        }

        private async Task<int> GetDeliveredOrders()
        {
            return await _context.Assignments
                .GroupBy(a => a.NumbInv)
                .Select(g => g
                    .OrderByDescending(a => a.DateTimeStatus)
                    .First())
                .CountAsync(a => a.NumbSta == 4);
        }

        //=========================================================
        // Top Selling Products
        //=========================================================

        private async Task<List<TopProductDto>> GetTopSellingProducts()
        {
            return await _context.InvoiceLines
                .GroupBy(l => new
                {
                    l.CodProd,
                    l.CodProdNavigation.NameProd
                })
                .Select(g => new TopProductDto
                {
                    CodProd = g.Key.CodProd,
                    NameProd = g.Key.NameProd,
                    QuantitySold = g.Sum(x => x.Cant)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(5)
                .ToListAsync();
        }
    }

}
