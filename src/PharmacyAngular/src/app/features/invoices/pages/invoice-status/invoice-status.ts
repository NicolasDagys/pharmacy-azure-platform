import {
    Component,
    OnInit,
    inject
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
    ActivatedRoute,
    Router,
    RouterLink
} from '@angular/router';

import {
    FormsModule
} from '@angular/forms';

import { ToastrService } from 'ngx-toastr';

import { InvoiceService } from '../../services/invoice.service';

import { InvoiceDetails } from '../../models/invoice-details';

@Component({

    selector: 'app-invoice-status',

    standalone: true,

    imports: [
        CommonModule,
        FormsModule,
        RouterLink
    ],

    templateUrl: './invoice-status.html',

    styleUrl: './invoice-status.css'

})
export class InvoiceStatusComponent implements OnInit {

    private route = inject(ActivatedRoute);

    private router = inject(Router);

    private toastr = inject(ToastrService);

    private invoiceService =
        inject(InvoiceService);

    invoice?: InvoiceDetails;

    loading = true;

    selectedStatus = 0;

    readonly statuses = [

    { numbSta: 1, nameSta: 'Preparation' },

    { numbSta: 2, nameSta: 'Dispatch' },

    { numbSta: 3, nameSta: 'Shipment' },

    { numbSta: 4, nameSta: 'Delivered' }

];

    ngOnInit(): void {

        const numbInv =
            this.route.snapshot.paramMap.get('numbInv');

        if (!numbInv) {

            this.router.navigate(['/invoices']);

            return;

        }

        this.invoiceService
            .getById(Number(numbInv))
            .subscribe({

                next: invoice => {

                    this.invoice = invoice;

                    this.loading = false;

                },

                error: err => {

                    console.error(err);

                    this.router.navigate(['/invoices']);

                }

            });

    }

    save(): void {

        if (!this.invoice)
            return;

        if (this.selectedStatus === 0)
            return;

        this.invoiceService
            .changeStatus(
                this.invoice.numbInv,
                {
                    numbSta: this.selectedStatus
                }
            )
            .subscribe({

                next: () => {

                    this.toastr.success(

                        'Status updated successfully.',

                        'Success'

                    );

                    this.router.navigate([

                        '/invoices/details',

                        this.invoice?.numbInv

                    ]);

                },

                error: err => {

                    console.error(err);

                    this.toastr.error(

                        typeof err.error === 'string'
                            ? err.error
                            : 'Unable to update status.',

                        'Error'

                    );

                }

            });

    }

}