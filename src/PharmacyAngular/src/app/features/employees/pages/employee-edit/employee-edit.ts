import {

    Component,

    OnInit,

    inject

} from '@angular/core';

import { CommonModule } from '@angular/common';

import {

    ActivatedRoute,

    Router

} from '@angular/router';

import {

    EmployeeService

} from '../../services/employee.service';

import {

    EmployeeEditFormComponent

} from '../../components/employeeEditFormComponent/employee-edit-form';

import {

    Employee

} from '../../models/employee';

import {

    UpdateEmployee

} from '../../models/update-employee';

import { ToastrService } from 'ngx-toastr';

@Component({

    selector: 'app-employee-edit',

    standalone: true,

    imports: [

        CommonModule,

        EmployeeEditFormComponent

    ],

    templateUrl: './employee-edit.html',

    styleUrl: './employee-edit.css'

})
export class EmployeeEditComponent implements OnInit {

    private route = inject(ActivatedRoute);

    private router = inject(Router);

    private employeeService = inject(EmployeeService);

    private toastr = inject(ToastrService);

    employee?: Employee;

    loading = true;

    ngOnInit(): void {

        const userEmp =
            this.route.snapshot.paramMap.get('userEmp');

        if (!userEmp) {

            this.router.navigate([

                '/employees'

            ]);

            return;

        }

        this.employeeService.getById(userEmp)

            .subscribe({

                next: employee => {

                    this.employee = employee;

                    this.loading = false;

                },

                error: err => {

                    console.error(err);

                    this.loading = false;

                }

            });

    }

    save(employee: UpdateEmployee): void {

        if (!this.employee) {

            return;

        }

        this.employeeService.updateEmployee(

            this.employee.userEmp,

            employee

        ).subscribe({

            next: () => {

                this.toastr.success(

                    'Employee updated successfully.',

                    'Success'

                );

                this.router.navigate([

                    '/employees'

                ]);

            },

            error: err => {

                const message =

                    typeof err.error === 'string'
                        ? err.error
                        : 'Unable to update employee.';

                this.toastr.error(
                    message,
                    'Error'
                );

            }

        });

    }
    goBack(): void {

    this.router.navigate([
        '/employees'
    ]);

}

}