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
    FormBuilder,
    ReactiveFormsModule,
    Validators
} from '@angular/forms';

import { ToastrService } from 'ngx-toastr';

import { EmployeeService } from '../../services/employee.service';

import { Employee } from '../../models/employee';

import { ChangeRole } from '../../models/change-role';

@Component({
    selector: 'app-employee-change-role',

    standalone: true,

    imports: [
        CommonModule,
        ReactiveFormsModule,
        RouterLink
    ],

    templateUrl: './employee-change-role.html',

    styleUrl: './employee-change-role.css'
})
export class EmployeeChangeRoleComponent implements OnInit {

    private fb = inject(FormBuilder);

    private route = inject(ActivatedRoute);

    private router = inject(Router);

    private employeeService = inject(EmployeeService);

    private toastr = inject(ToastrService);

    employee?: Employee;

    readonly roles = [
        'Admin',
        'Pharmacist',
        'Sales'
    ];

    form = this.fb.group({
        roleEmp: [
            '',
            Validators.required
        ]
    });

    ngOnInit(): void {

        const userEmp = this.route.snapshot.paramMap.get('userEmp');

        if (!userEmp) {

            this.router.navigate(['/employees']);

            return;
        }

        this.employeeService
            .getById(userEmp)
            .subscribe({

                next: employee => {

                    this.employee = employee;

                    this.form.patchValue({

                        roleEmp: employee.roleEmp

                    });

                },

                error: err => {

                    console.error(err);

                    this.router.navigate(['/employees']);

                }

            });

    }

    submit(): void {

        if (this.form.invalid || !this.employee) {

            this.form.markAllAsTouched();

            return;
        }

        const request: ChangeRole = {

            roleEmp: this.form.getRawValue().roleEmp!

        };

        this.employeeService
            .changeRole(
                this.employee.userEmp,
                request
            )
            .subscribe({

                next: () => {

                    this.toastr.success(
                        'Role updated successfully.',
                        'Success'
                    );

                    this.router.navigate([
                        '/employees/details',
                        this.employee!.userEmp
                    ]);

                },

                error: err => {

                    console.error(err);

                    this.toastr.error(
                        'Unable to update role.',
                        'Error'
                    );

                }

            });

    }

}