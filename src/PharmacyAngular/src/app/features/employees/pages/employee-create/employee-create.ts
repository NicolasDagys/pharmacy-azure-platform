import {

    Component,

    inject

} from '@angular/core';

import { CommonModule } from '@angular/common';

import {

    Router

} from '@angular/router';

import { ToastrService } from 'ngx-toastr';

import { EmployeeCreateFormComponent } from '../../components/employeeCreateFormComponent/employee-create-form';
import { EmployeeService } from '../../services/employee.service';
import { CreateEmployee } from '../../models/create-employee';

@Component({

    selector: 'app-employee-create',

    standalone: true,

    imports: [

        CommonModule,

        EmployeeCreateFormComponent

    ],

    templateUrl: './employee-create.html',

    styleUrl: './employee-create.css'

})
export class EmployeeCreateComponent {

    private employeeService = inject(EmployeeService);

    private toastr = inject(ToastrService);

    private router = inject(Router);

    save(employee: CreateEmployee): void {

        this.employeeService.createEmployee(employee)

            .subscribe({

                next: () => {

                    this.toastr.success(

                        'Employee created successfully.',

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
                            : 'Unable to create employee.';

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