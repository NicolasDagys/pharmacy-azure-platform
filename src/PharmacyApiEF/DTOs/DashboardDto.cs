using PharmacyApiEF.Models;

namespace PharmacyApiEF.DTOs
{
    public class DashboardDto
    {
        public int ActiveProducts { get; set; }
        public List<TopProductDto> TopSellingProducts { get; set; }
    = new();
        public int ProductsExpiringSoon { get; set; }

        public int OutOfStockProducts { get; set; } //en esta despues tengo que hacer alerta en azure
        public int TotalCustomers { get; set; }
        public int TotalInvoices { get; set; }
        public int PendingInvoices { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int LowStockProducts { get; set; }
        public int DeliveredOrders { get; set; }

        
    }
}
