import {

    Component,

    OnInit,

    inject

} from '@angular/core';

import {

    CommonModule

} from '@angular/common';

import {

    ActivatedRoute,

    Router

} from '@angular/router';

import {

    CustomerService

} from '../../services/customer.service';

import {

    CustomerFormComponent

} from '../../components/customer-form/customer-form';

import {

    Customer

} from '../../models/customer';

import {

    ToastrService

} from 'ngx-toastr';

@Component({

    selector: 'app-customer-edit',

    standalone: true,

    imports: [

        CommonModule,

        CustomerFormComponent

    ],

    templateUrl: './customer-edit.html',

    styleUrl: './customer-edit.css'

})
export class CustomerEditComponent implements OnInit {

    //tempral
    constructor() { console.log('CustomerEditComponent loaded'); }

    private route = inject(ActivatedRoute);

    private router = inject(Router);

    private customerService = inject(CustomerService);

    private toastr = inject(ToastrService);

    customer?: Customer;

    loading = true;

    ngOnInit(): void {

        //temporal
        console.log(this.route.snapshot.paramMap.get('id'));
        console.log(this.route.snapshot.paramMap.get('idCus'));


        const idCus =

            this.route.snapshot.paramMap.get('idCus');

        if (!idCus) {

            this.router.navigate([

                '/customers'

            ]);

            return;

        }

        this.customerService.getById(idCus)

            .subscribe({

                next: customer => {

                    this.customer = customer;

                    this.loading = false;

                },

                error: err => {

                    console.error(err);

                    this.loading = false;

                    this.router.navigate([

                        '/customers'

                    ]);

                }

            });

    }

    save(customer: Customer): void {

        if (!this.customer) {

            return;

        }

        this.customerService.updateCustomer(

            this.customer.idCus,

            customer

        ).subscribe({

            next: () => {

                this.toastr.success(

                    'Customer updated successfully.',

                    'Success'

                );

                this.router.navigate([

                    '/customers'

                ]);

            },

            error: err => {

                const message =

                    typeof err.error === 'string'
                        ? err.error
                        : 'Unable to update customer.';

                this.toastr.error(
                    message,
                    'Error'
                );

            }

        });

    }
    goBack(): void {

    this.router.navigate([
        '/customers'
    ]);

}

}