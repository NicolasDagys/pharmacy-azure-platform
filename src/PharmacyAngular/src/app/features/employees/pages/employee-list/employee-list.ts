import { Component, OnInit, inject } from '@angular/core';

import { CommonModule } from '@angular/common';

import { FormsModule } from '@angular/forms';

import {

    RouterModule,

    RouterLink

} from '@angular/router';

import * as bootstrap from 'bootstrap';

import { ToastrService } from 'ngx-toastr';

import { Employee } from '../../models/employee';
import { EmployeeService } from '../../services/employee.service';

import { ConfirmationDialogComponent } from '../../../../shared/components/confirmation-dialog/confirmation-dialog';

@Component({

    selector: 'app-employee-list',

    standalone: true,

    imports: [

        CommonModule,

        FormsModule,

        RouterModule,

        RouterLink,

        ConfirmationDialogComponent

    ],

    templateUrl: './employee-list.html',

    styleUrl: './employee-list.css'

})
export class EmployeeListComponent implements OnInit {

    private employeeService = inject(EmployeeService);

    private toastr = inject(ToastrService);

    employees: Employee[] = [];

    selectedEmployee?: Employee;

    loading = false;

    error = false;

    search = '';

    page = 1;

    pageSize = 10;

    sortBy = 'nameEmp';

    descending = false;

    ngOnInit(): void {

        this.loadEmployees();

    }

    loadEmployees(): void {

        this.loading = true;

        this.error = false;

        this.employeeService.getEmployees({

            page: this.page,

            pageSize: this.pageSize,

            search: this.search,

            sortBy: this.sortBy,

            descending: this.descending

        }).subscribe({

            next: response => {

                this.employees = response;

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

        this.loadEmployees();

    }

    previousPage(): void {

        if (this.page > 1) {

            this.page--;

            this.loadEmployees();

        }

    }

    nextPage(): void {

        this.page++;

        this.loadEmployees();

    }

    openDelete(employee: Employee): void {

        this.selectedEmployee = employee;

        const modalElement = document.getElementById('confirmationModal');

        if (!modalElement) {

            return;

        }

        const modal = new bootstrap.Modal(modalElement);

        modal.show();

    }

    deleteEmployee(): void {

        if (!this.selectedEmployee) {

            return;

        }

        this.employeeService.deleteEmployee(

            this.selectedEmployee.userEmp

        ).subscribe({

            next: () => {

    this.toastr.success(

        `Employee ${this.selectedEmployee?.nameEmp} deleted successfully.`,

        'Success'

    );

    this.selectedEmployee = undefined;

    this.loadEmployees();

},

            error: err => {

                const message =

                    typeof err.error === 'string'

                        ? err.error

                        : 'Unexpected error';

                if (err.status === 404) {

                    this.toastr.error(

                        'Employee not found.',

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

}