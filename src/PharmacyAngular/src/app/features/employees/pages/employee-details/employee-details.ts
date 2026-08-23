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

import { EmployeeService } from '../../services/employee.service';

import { Employee } from '../../models/employee';

@Component({

    selector: 'app-employee-details',

    standalone: true,

    imports: [
        CommonModule,
        RouterLink
    ],

    templateUrl: './employee-details.html',

    styleUrl: './employee-details.css'

})
export class EmployeeDetailsComponent implements OnInit {

    private route = inject(ActivatedRoute);

    private router = inject(Router);

    private employeeService = inject(EmployeeService);

    employee?: Employee;

    loading = true;

    currentUser = '';

    canChangePassword = false;

    ngOnInit(): void {
        console.log('EmployeeDetails iniciado');
        const userEmp =
            this.route.snapshot.paramMap.get('userEmp');


        if (!userEmp) {

            this.router.navigate(['/employees']);

            return;

        }
        this.currentUser = this.employeeService.getCurrentUsername() ?? '';

        console.log('currentUser:', this.currentUser);
console.log('route user:', userEmp);

        this.canChangePassword =
            this.currentUser === userEmp;

        this.employeeService
            .getById(userEmp)
            .subscribe({

                next: employee => {

                    this.employee = employee;

                    this.loading = false;

                },

                error: err => {

                    console.error(err);

                    this.loading = false;

                    this.router.navigate(['/employees']);

                }

            });


    }

}