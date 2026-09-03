import {Component,OnInit,inject} from '@angular/core';

import { CommonModule } from '@angular/common';

import {ActivatedRoute,Router,RouterLink} from '@angular/router';

import {FormBuilder,ReactiveFormsModule,Validators} from '@angular/forms';

import { ToastrService } from 'ngx-toastr';

import { EmployeeService } from '../../services/employee.service';

import { ChangePassword } from '../../models/change-password';

@Component({

    selector: 'app-employee-change-password',

    standalone: true,

    imports: [CommonModule,ReactiveFormsModule,RouterLink],

    templateUrl: './employee-change-password.html',

    styleUrl: './employee-change-password.css'

})
export class EmployeeChangePasswordComponent implements OnInit {

    private route = inject(ActivatedRoute);

    private router = inject(Router);

    private fb = inject(FormBuilder);

    private toastr = inject(ToastrService);

    private employeeService = inject(EmployeeService);

    username = '';

    form = this.fb.group({

        currentPassword: [

            '',

            Validators.required

        ],

        newPassword: [

            '',

            [

                Validators.required,

                Validators.minLength(8),

                Validators.pattern(
                    /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$/
                )

            ]

        ]

    });

    ngOnInit(): void {

        

    }

    save(): void {

        if (this.form.invalid) {

            this.form.markAllAsTouched();

            return;

        }

        const request: ChangePassword = {

            currentPassword:
                this.form.getRawValue().currentPassword!,

            newPassword:
                this.form.getRawValue().newPassword!

        };

        this.employeeService
            .changePassword(request)
            .subscribe({

                next: () => {

                    this.toastr.success(

                        'Password updated successfully.',

                        'Success'

                    );

                    this.router.navigate(['/dashboard']);

                },

                error: err => {

                    console.error(err);

                    this.toastr.error(

                        'Unable to update password.',

                        'Error'

                    );

                }

            });

    }

}