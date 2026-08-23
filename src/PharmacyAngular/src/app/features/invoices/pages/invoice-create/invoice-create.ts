import {
    Component,
    OnInit,
    inject
} from '@angular/core';

import {
    CommonModule
} from '@angular/common';

import {
    Router
} from '@angular/router';

import {
    forkJoin
} from 'rxjs';

import {
    ToastrService
} from 'ngx-toastr';

import {
    InvoiceFormComponent
} from '../../components/invoice-form/invoice-form';

import {
    InvoiceService
} from '../../services/invoice.service';

import {
    CustomerService
} from '../../../customers/services/customer.service';

import {
    ProductService
} from '../../../products/services/product.service';

import {
    CustomerLookup
} from '../../../customers/models/customer-lookup';

import {
    ProductLookup
} from '../../../products/models/product-lookup';

import {
    CreateInvoice
} from '../../models/create-invoice';

@Component({

    selector: 'app-invoice-create',

    standalone: true,

    imports: [
        CommonModule,
        InvoiceFormComponent
    ],

    templateUrl: './invoice-create.html',

    styleUrl: './invoice-create.css'

})
export class InvoiceCreateComponent implements OnInit {

    private invoiceService =
        inject(InvoiceService);

    private customerService =
        inject(CustomerService);

    private productService =
        inject(ProductService);

    private toastr =
        inject(ToastrService);

    private router =
        inject(Router);

    customers: CustomerLookup[] = [];

    products: ProductLookup[] = [];

    loading = true;

    ngOnInit(): void {

        forkJoin({

            customers:
                this.customerService.getLookup(),

            products:
                this.productService.getLookup()

        })
        .subscribe({

            next: data => {

                this.customers =
                    data.customers;

                this.products =
                    data.products;

                this.loading = false;

            },

            error: err => {

                console.error(err);

                this.loading = false;

                this.toastr.error(
                    'Unable to load required data.',
                    'Error'
                );

            }

        });

    }

    save(invoice: CreateInvoice): void {

        this.invoiceService
            .createInvoice(invoice)
            .subscribe({

                next: (response: any) => {

                    this.toastr.success(

                        `Invoice #${response.invoiceNumber} created successfully.`,

                        'Success'

                    );

                    this.router.navigate([
                        '/invoices'
                    ]);

                },

                error: err => {

                    const message =

                        typeof err.error === 'string'

                            ? err.error

                            : 'Unable to create invoice.';

                    this.toastr.error(

                        message,

                        'Error'

                    );

                }

            });

    }

}