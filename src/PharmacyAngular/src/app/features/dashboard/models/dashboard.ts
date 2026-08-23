import { TopProduct } from './top-product';

export interface Dashboard {

    activeProducts: number;

    topSellingProducts: TopProduct[];

    productsExpiringSoon: number;

    outOfStockProducts: number;

    totalCustomers: number;

    totalInvoices: number;

    pendingInvoices: number;

    monthlyRevenue: number;

    lowStockProducts: number;

    deliveredOrders: number;

}