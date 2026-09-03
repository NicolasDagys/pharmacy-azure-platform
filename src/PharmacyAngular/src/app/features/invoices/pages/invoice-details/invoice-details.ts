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

import { InvoiceService } from '../../services/invoice.service';

import { InvoiceDetails } from '../../models/invoice-details';

@Component({

    selector: 'app-invoice-details',

    standalone: true,

    imports: [
        CommonModule,
        RouterLink
    ],

    templateUrl: './invoice-details.html',

    styleUrl: './invoice-details.css'

})
export class InvoiceDetailsComponent implements OnInit {

    private route = inject(ActivatedRoute);

    private router = inject(Router);

    private invoiceService = inject(InvoiceService);

    invoice?: InvoiceDetails;

    loading = true;

    ngOnInit(): void {

        const numbInv = this.route.snapshot.paramMap.get('numbInv');

        if (!numbInv) {

            this.router.navigate(['/invoices']);

            return;

        }

        this.invoiceService.getById(Number(numbInv))
            .subscribe({

                next: invoice => {

                    this.invoice = invoice;

                    this.loading = false;

                },

                error: err => {

                    console.error(err);

                    this.loading = false;

                    this.router.navigate(['/invoices']);

                }

            });

    }

}