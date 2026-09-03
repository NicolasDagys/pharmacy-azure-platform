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

import { CustomerService } from '../../services/customer.service';
import { Customer } from '../../models/customer';

@Component({

    selector: 'app-customer-details',

    standalone: true,

    imports: [
        CommonModule,
        RouterLink
    ],

    templateUrl: './customer-details.html',

    styleUrl: './customer-details.css'

})
export class CustomerDetailsComponent implements OnInit {

    private route = inject(ActivatedRoute);

    private router = inject(Router);

    private customerService = inject(CustomerService);

    customer?: Customer;

    loading = true;

    ngOnInit(): void {

        const idCus =
            this.route.snapshot.paramMap.get('idCus');

        if (!idCus) {

            this.router.navigate(['/customers']);

            return;

        }

        this.customerService
            .getById(idCus)
            .subscribe({

                next: customer => {

                    this.customer = customer;

                    this.loading = false;

                },

                error: err => {

                    console.error(err);

                    this.loading = false;

                    this.router.navigate(['/customers']);

                }

            });

    }

}