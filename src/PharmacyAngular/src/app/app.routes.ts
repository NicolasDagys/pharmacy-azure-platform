import { Routes } from '@angular/router';

import { LoginComponent } from './features/auth/pages/login/login';
import { DashboardComponent } from './features/dashboard/pages/dashboard/dashboard';

import { MainLayoutComponent } from './layouts/main-layout/main-layout';

import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [

    {
        path: 'login',
        component: LoginComponent
    },

    {
        path: '',
        component: MainLayoutComponent,
        canActivate: [authGuard],

        children: [

            {
                path: '',
                redirectTo: 'dashboard',
                pathMatch: 'full'
            },

            {
                path: 'dashboard',
                component: DashboardComponent
            },

            // ==================================================
            // PRODUCTS
            // ==================================================

            {
                path: 'products',
                loadComponent: () =>
                    import('./features/products/pages/product-list/product-list')
                        .then(m => m.ProductListComponent)
            },

            {
                path: 'products/create',
                canActivate: [roleGuard],
                data: {
                    roles: ['Admin', 'Pharmacist']
                },
                loadComponent: () =>
                    import('./features/products/pages/product-create/product-create')
                        .then(m => m.ProductCreateComponent)
            },

            {
                path: 'products/edit/:codProd',
                canActivate: [roleGuard],
                data: {
                    roles: ['Admin', 'Pharmacist']
                },
                loadComponent: () =>
                    import('./features/products/pages/product-edit/product-edit')
                        .then(m => m.ProductEditComponent)
            },

            {
                path: 'products/details/:codProd',
                loadComponent: () =>
                    import('./features/products/pages/product-details/product-details')
                        .then(m => m.ProductDetailsComponent)
            },

            // ==================================================
            // CATEGORIES
            // ==================================================

            {
                path: 'categories',
                loadComponent: () =>
                    import('./features/categories/pages/category-list/category-list')
                        .then(m => m.CategoryListComponent)
            },

            {
                path: 'categories/create',
                canActivate: [roleGuard],
                data: {
                    roles: ['Admin']
                },
                loadComponent: () =>
                    import('./features/categories/pages/category-create/category-create')
                        .then(m => m.CategoryCreateComponent)
            },

            {
                path: 'categories/edit/:codeCat',
                canActivate: [roleGuard],
                data: {
                    roles: ['Admin']
                },
                loadComponent: () =>
                    import('./features/categories/pages/category-edit/category-edit')
                        .then(m => m.CategoryEditComponent)
            },

            // ==================================================
            // CUSTOMERS
            // ==================================================

            {
                path: 'customers',
                canActivate: [roleGuard],
                data: {
                    roles: ['Admin', 'Sales']
                },
                loadComponent: () =>
                    import('./features/customers/pages/customer-list/customer-list')
                        .then(m => m.CustomerListComponent)
            },

            {
                path: 'customers/create',
                canActivate: [roleGuard],
                data: {
                    roles: ['Admin', 'Sales']
                },
                loadComponent: () =>
                    import('./features/customers/pages/customer-create/customer-create')
                        .then(m => m.CustomerCreateComponent)
            },

            {
                path: 'customers/edit/:idCus',
                canActivate: [roleGuard],
                data: {
                    roles: ['Admin', 'Sales']
                },
                loadComponent: () =>
                    import('./features/customers/pages/customer-edit/customer-edit')
                        .then(m => m.CustomerEditComponent)
            },

            {
                path: 'customers/details/:idCus',
                canActivate: [roleGuard],
                data: {
                    roles: ['Admin', 'Sales']
                },
                loadComponent: () =>
                    import('./features/customers/pages/customer-details/customer-details')
                        .then(m => m.CustomerDetailsComponent)
            },

            // ==================================================
            // EMPLOYEES
            // ==================================================

            {
                path: 'employees',
                canActivate: [roleGuard],
                data: {
                    roles: ['Admin']
                },
                loadComponent: () =>
                    import('./features/employees/pages/employee-list/employee-list')
                        .then(m => m.EmployeeListComponent)
            },

            {
                path: 'employees/create',
                canActivate: [roleGuard],
                data: {
                    roles: ['Admin']
                },
                loadComponent: () =>
                    import('./features/employees/pages/employee-create/employee-create')
                        .then(m => m.EmployeeCreateComponent)
            },

            {
                path: 'employees/edit/:userEmp',
                canActivate: [roleGuard],
                data: {
                    roles: ['Admin']
                },
                loadComponent: () =>
                    import('./features/employees/pages/employee-edit/employee-edit')
                        .then(m => m.EmployeeEditComponent)
            },

            {
                path: 'employees/details/:userEmp',
                canActivate: [roleGuard],
                data: {
                    roles: ['Admin']
                },
                loadComponent: () =>
                    import('./features/employees/pages/employee-details/employee-details')
                        .then(m => m.EmployeeDetailsComponent)
            },

            {
                path: 'employees/change-role/:userEmp',
                canActivate: [roleGuard],
                data: {
                    roles: ['Admin']
                },
                loadComponent: () =>
                    import('./features/employees/pages/employee-change-role/employee-change-role')
                        .then(m => m.EmployeeChangeRoleComponent)
            },

            {
                path: 'employees/change-password',
                loadComponent: () =>
                    import('./features/employees/pages/employee-change-password/employee-change-password')
                        .then(m => m.EmployeeChangePasswordComponent)
            },

            // ==================================================
            // INVOICES
            // ==================================================

            {
                path: 'invoices',
                loadComponent: () =>
                    import('./features/invoices/pages/invoice-list/invoice-list')
                        .then(m => m.InvoiceListComponent)
            },

            {
                path: 'invoices/create',
                canActivate: [roleGuard],
                data: {
                    roles: ['Admin', 'Sales']
                },
                loadComponent: () =>
                    import('./features/invoices/pages/invoice-create/invoice-create')
                        .then(m => m.InvoiceCreateComponent)
            },

            {
                path: 'invoices/details/:numbInv',
                loadComponent: () =>
                    import('./features/invoices/pages/invoice-details/invoice-details')
                        .then(m => m.InvoiceDetailsComponent)
            },

            {
                path: 'invoices/status/:numbInv',
                canActivate: [roleGuard],
                data: {
                    roles: ['Admin', 'Sales', 'Pharmacist']
                },
                loadComponent: () =>
                    import('./features/invoices/pages/invoice-status/invoice-status')
                        .then(m => m.InvoiceStatusComponent)
            },
            {
                path: 'inventory/chat',
                loadComponent: () =>
                    import('./features/assistant/pages/assistant-page/assistant-page')
                        .then(m => m.AssistantPageComponent)
            }

        ]
    },

    {
        path: '**',
        redirectTo: 'login'
    }

];