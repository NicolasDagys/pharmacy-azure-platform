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

    RouterModule,

    RouterLink

} from '@angular/router';

import * as bootstrap from 'bootstrap';

import {

    ToastrService

} from 'ngx-toastr';

import {

    CustomerService

} from '../../services/customer.service';

import {

    Customer

} from '../../models/customer';

import {

    ConfirmationDialogComponent

} from '../../../../shared/components/confirmation-dialog/confirmation-dialog';
import { TokenStorageService } from '../../../auth/services/token-storage.service';

@Component({

    selector: 'app-customer-list',

    standalone: true,

    imports: [

        CommonModule,

        FormsModule,

        RouterModule,

        RouterLink,

        ConfirmationDialogComponent

    ],

    templateUrl: './customer-list.html',

    styleUrl: './customer-list.css'

})
export class CustomerListComponent implements OnInit {

    private customerService = inject(CustomerService);

    private toastr = inject(ToastrService);

    private tokenStorage = inject(TokenStorageService);

    customers: Customer[] = [];

    selectedCustomer?: Customer;

    loading = false;

    error = false;

    search = '';

    page = 1;

    pageSize = 10;

    sortBy = 'NameCus';

    descending = false;

    ngOnInit(): void {

        this.loadCustomers();

    }

    loadCustomers(): void {

        this.loading = true;

        this.error = false;

        this.customerService.getCustomers({

            page: this.page,

            pageSize: this.pageSize,

            search: this.search,

            sortBy: this.sortBy,

            descending: this.descending

        }).subscribe({

            next: response => {

                this.customers = response;

                this.loading = false;

            },

            error: err => {

                console.error(err);

                this.error = true;

                this.loading = false;

            }

        });

    }

    sort(column: string): void {

        if (this.sortBy === column) {

            this.descending = !this.descending;

        }

        else {

            this.sortBy = column;

            this.descending = false;

        }

        this.loadCustomers();

    }

    nextPage(): void {

        this.page++;

        this.loadCustomers();

    }

    previousPage(): void {

        if (this.page > 1) {

            this.page--;

            this.loadCustomers();

        }

    }

    openDelete(customer: Customer): void {

        this.selectedCustomer = customer;

        const modalElement = document.getElementById(

            'confirmationModal'

        );

        if (!modalElement) {

            return;

        }

        const modal = new bootstrap.Modal(

            modalElement

        );

        modal.show();

    }

    deleteCustomer(): void {

        if (!this.selectedCustomer) {

            return;

        }

        this.customerService.deleteCustomer(

            this.selectedCustomer.idCus

        ).subscribe({

            next: () => {

                this.toastr.success(

                    `Customer ${this.selectedCustomer?.nameCus} deleted successfully.`,

                    'Success'

                );

                this.customers = this.customers.filter(

                    c => c.idCus !== this.selectedCustomer?.idCus

                );

                this.selectedCustomer = undefined;

            },

            error: err => {

                const message =

                    typeof err.error === 'string'

                        ? err.error

                        : 'Unexpected error';

                if (err.status === 404) {

                    this.toastr.error(

                        'Customer not found.',

                        'Error'

                    );

                }

                else {

                    this.toastr.error(

                        message,

                        'Error'

                    );

                }

                console.error(err);

            }

        });

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

get canDelete(): boolean {

    return this.role === 'Admin';

}

}