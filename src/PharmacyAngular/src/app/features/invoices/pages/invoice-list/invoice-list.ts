import {
    Component,
    OnInit,
    inject
} from '@angular/core';

import {
    CommonModule
} from '@angular/common';

import {
    FormsModule
} from '@angular/forms';

import {
    RouterLink
} from '@angular/router';

import {
    ToastrService
} from 'ngx-toastr';

import {
    InvoiceService
} from '../../services/invoice.service';


import {Invoice} from '../../models/invoice';
import { TokenStorageService } from '../../../auth/services/token-storage.service';

@Component({

    selector: 'app-invoice-list',

    standalone: true,

    imports: [
        CommonModule,
        FormsModule,
        RouterLink
    ],

    templateUrl: './invoice-list.html',

    styleUrl: './invoice-list.css'

})
export class InvoiceListComponent implements OnInit {

    private invoiceService =
        inject(InvoiceService);

    private toastr =
        inject(ToastrService);
        private tokenStorage = inject(TokenStorageService);

    invoices: Invoice[] = [];

    loading = false;

    error = false;

    search = '';

    page = 1;

    pageSize = 10;

    sortBy = 'DateInv';

    descending = true;

    ngOnInit(): void {

        this.loadInvoices();

    }

    loadInvoices(): void {

        this.loading = true;

        this.error = false;

        this.invoiceService
            .getInvoices({

                page: this.page,

                pageSize: this.pageSize,

                search: this.search,

                sortBy: this.sortBy,

                descending: this.descending

            })
            .subscribe({

                next: invoices => {

                    this.invoices = invoices;

                    this.loading = false;

                },

                error: err => {

                    console.error(err);

                    this.loading = false;

                    this.error = true;

                    this.toastr.error(

                        'Unable to load invoices.',

                        'Error'

                    );

                }

            });

    }

    sort(column: string): void {

        if (this.sortBy === column) {

            this.descending =
                !this.descending;

        }
        else {

            this.sortBy = column;

            this.descending = false;

        }

        this.loadInvoices();

    }

    nextPage(): void {

        this.page++;

        this.loadInvoices();

    }

    previousPage(): void {

        if (this.page > 1) {

            this.page--;

            this.loadInvoices();

        }

    }

    get role(): string {

        const token = this.tokenStorage.getToken();

        if (!token) {
            return '';
        }

        const payload = JSON.parse(atob(token.split('.')[1]));

        return payload[
            'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
        ] ?? '';

    }

    get canManageInvoices(): boolean {

    return this.role === 'Admin'
        || this.role === 'Sales';

}

}