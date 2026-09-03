import {

    Component,

    inject

} from '@angular/core';

import {

    Router

} from '@angular/router';

import {

    CustomerFormComponent

} from '../../components/customer-form/customer-form';

import {

    CustomerService

} from '../../services/customer.service';


import {

    ToastrService

} from 'ngx-toastr';
import { Customer } from '../../models/customer';

@Component({

    selector: 'app-customer-create',

    standalone: true,

    imports: [

        CustomerFormComponent

    ],

    templateUrl: './customer-create.html',

    styleUrl: './customer-create.css'

})
export class CustomerCreateComponent {

    private customerService = inject(CustomerService);

    private toastr = inject(ToastrService);

    private router = inject(Router);

    save(customer: Customer): void {

        this.customerService.createCustomer(customer)

            .subscribe({

                next: () => {

                    this.toastr.success(

                        'Customer created successfully.',

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
                            : 'Unable to create customer.';

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