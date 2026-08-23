using PharmacyApiEF.DTOs.Common;
using PharmacyApiEF.Models;

namespace PharmacyApiEF.Extensions
{
    public static class QueryableExtensions
    {
        /// Applies pagination to any IQueryable.
        
        public static IQueryable<T> ApplyPagination<T>(
            this IQueryable<T> query,
            QueryParameters parameters)
        {
            parameters.PageNumber =
                Math.Max(parameters.PageNumber, 1);

            parameters.PageSize =
                Math.Clamp(parameters.PageSize, 1, 100);

            return query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize);
        }

        /// Applies product search.
        public static IQueryable<Product> ApplyProductSearch(
    this IQueryable<Product> query,
    QueryParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.Search))
                return query;

            return query.Where(p =>

                p.NameProd.Contains(parameters.Search)

                ||

                p.CodProd.Contains(parameters.Search)

                ||

                p.CodeCat.Contains(parameters.Search));
        }

        /// Applies product sorting.
        public static IQueryable<Product> ApplyProductSorting(
    this IQueryable<Product> query,
    QueryParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.SortBy))
                return query.OrderBy(p => p.NameProd);

            switch (parameters.SortBy.Trim().ToLower())
            {
                case "codprod":

                    return parameters.Desc
                        ? query.OrderByDescending(p => p.CodProd)
                        : query.OrderBy(p => p.CodProd);

                case "nameprod":

                    return parameters.Desc
                        ? query.OrderByDescending(p => p.NameProd)
                        : query.OrderBy(p => p.NameProd);

                case "priceprod":

                    return parameters.Desc
                        ? query.OrderByDescending(p => p.PriceProd)
                        : query.OrderBy(p => p.PriceProd);

                case "stockqty":

                    return parameters.Desc
                        ? query.OrderByDescending(p => p.StockQty)
                        : query.OrderBy(p => p.StockQty);

                case "expdateprod":

                    return parameters.Desc
                        ? query.OrderByDescending(p => p.ExpDateProd)
                        : query.OrderBy(p => p.ExpDateProd);

                default:

                    return query.OrderBy(p => p.NameProd);
            }
        }

        public static IQueryable<Category> ApplyCategorySearch(this IQueryable<Category> query,
    QueryParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.Search))
                return query;

            string search = parameters.Search.Trim();

            return query.Where(c =>
                c.CodeCat.Contains(search) ||
                c.NameCat.Contains(search));
        }

        public static IQueryable<Category> ApplyCategorySorting(
    this IQueryable<Category> query,
    QueryParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.SortBy))
            {
                return query.OrderBy(c => c.NameCat);
            }

            switch (parameters.SortBy.Trim().ToLower())
            {
                case "namecat":

                    return parameters.Desc
                        ? query.OrderByDescending(c => c.NameCat)
                        : query.OrderBy(c => c.NameCat);

                case "codecat":

                    return parameters.Desc
                        ? query.OrderByDescending(p => p.CodeCat)
                        : query.OrderBy(p => p.CodeCat);

                default:

                    return query.OrderBy(c => c.NameCat);
            }
        }

        public static IQueryable<Customer> ApplyCustomerSearch(this IQueryable<Customer> query,
QueryParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.Search))
                return query;

            string search = parameters.Search.Trim();

            return query.Where(c =>

                c.IdCus.Contains(search)

                ||

                c.NameCus.Contains(search)

                ||

                c.MailCus.Contains(search));
        }

        public static IQueryable<Customer> ApplyCustomerSorting(this IQueryable<Customer> query,
QueryParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.SortBy))
                return query.OrderBy(c => c.NameCus);

            switch (parameters.SortBy.Trim().ToLower())
            {
                case "namecus":

                    return parameters.Desc
                        ? query.OrderByDescending(c => c.NameCus)
                        : query.OrderBy(c => c.NameCus);

                case "mailcus":

                    return parameters.Desc
                        ? query.OrderByDescending(c => c.MailCus)
                        : query.OrderBy(c => c.MailCus);

                case "idcus":

                    return parameters.Desc
                        ? query.OrderByDescending(c => c.IdCus)
                        : query.OrderBy(c => c.IdCus);

                default:

                    return query.OrderBy(c => c.NameCus);
            }
        }
        public static IQueryable<Employee> ApplyEmployeeSearch(
    this IQueryable<Employee> query,
    QueryParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.Search))
                return query;

            string search = parameters.Search.Trim();

            return query.Where(e =>

                e.UserEmp.Contains(search)

                ||

                e.NameEmp.Contains(search)

                ||

                e.RoleEmp.Contains(search));
        }

        public static IQueryable<Employee> ApplyEmployeeSorting(
            this IQueryable<Employee> query,
            QueryParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.SortBy))
                return query.OrderBy(e => e.NameEmp);

            switch (parameters.SortBy.Trim().ToLower())
            {
                case "nameemp":

                    return parameters.Desc
                        ? query.OrderByDescending(e => e.NameEmp)
                        : query.OrderBy(e => e.NameEmp);

                case "useremp":

                    return parameters.Desc
                        ? query.OrderByDescending(e => e.UserEmp)
                        : query.OrderBy(e => e.UserEmp);

                case "roleemp":

                    return parameters.Desc
                        ? query.OrderByDescending(e => e.RoleEmp)
                        : query.OrderBy(e => e.RoleEmp);

                default:

                    return query.OrderBy(e => e.NameEmp);
            }
        }
        public static IQueryable<Invoice> ApplyInvoiceSearch(this IQueryable<Invoice> query,
QueryParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.Search))
                return query;

            string search = parameters.Search.Trim();

            return query.Where(i =>

                i.ShipmentAddressInv.Contains(search)

                ||

                i.IdCus.Contains(search)

                ||

                i.NumbInv.ToString().Contains(search));
        }

        public static IQueryable<Invoice> ApplyInvoiceSorting(
    this IQueryable<Invoice> query,
    QueryParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.SortBy))
            {
                return query.OrderByDescending(i => i.DateInv);
            }

            switch (parameters.SortBy.Trim().ToLower())
            {
                case "numbinv":

                    return parameters.Desc
                        ? query.OrderByDescending(i => i.NumbInv)
                        : query.OrderBy(i => i.NumbInv);

                case "dateinv":

                    return parameters.Desc
                        ? query.OrderByDescending(i => i.DateInv)
                        : query.OrderBy(i => i.DateInv);

                case "idcus":

                    return parameters.Desc
                        ? query.OrderByDescending(i => i.IdCus)
                        : query.OrderBy(i => i.IdCus);
                case "status":

                    return parameters.Desc

                        ? query.OrderByDescending(i =>
                            i.Assignments
                                .OrderByDescending(a => a.DateTimeStatus)
                                .Select(a => a.NumbSta)
                                .FirstOrDefault())

                        : query.OrderBy(i =>
                            i.Assignments
                                .OrderByDescending(a => a.DateTimeStatus)
                                .Select(a => a.NumbSta)
                                .FirstOrDefault());

                case "totalinv":

                    return parameters.Desc
                        ? query.OrderByDescending(i => i.TotalInv)
                        : query.OrderBy(i => i.TotalInv);

                default:

                    return query.OrderByDescending(i => i.DateInv);
            }
        }

    }
}
