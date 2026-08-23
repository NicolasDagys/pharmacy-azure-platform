import {
    Component,
    EventEmitter,
    Output,
    inject
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
    FormBuilder,
    ReactiveFormsModule,
    Validators
} from '@angular/forms';

import { CreateEmployee } from '../../models/create-employee';

@Component({

    selector: 'app-employee-create-form',

    standalone: true,

    imports: [
        CommonModule,
        ReactiveFormsModule
    ],

    templateUrl: './employee-create-form.html',

    styleUrl: './employee-create-form.css'

})
export class EmployeeCreateFormComponent {

    private fb = inject(FormBuilder);

    @Output()

    save = new EventEmitter<CreateEmployee>();

    @Output()
    back = new EventEmitter<void>();

    readonly roles = [

        'Admin',
        'Pharmacist',
        'Sales'

    ];

    form = this.fb.group({

        userEmp: [

            '',

            [
                Validators.required,
                Validators.minLength(3),
                Validators.maxLength(50)
            ]

        ],

        nameEmp: [

            '',

            [
                Validators.required,
                Validators.minLength(3),
                Validators.maxLength(50)
            ]

        ],

        password: [

            '',

            [

                Validators.required,

                Validators.minLength(8),

                Validators.pattern(

                    /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$/

                )

            ]

        ],

        roleEmp: [

            '',

            Validators.required

        ]

    });

    submit(): void {

        if (this.form.invalid) {

            this.form.markAllAsTouched();

            return;

        }

        this.save.emit(

            this.form.getRawValue() as CreateEmployee

        );

    }
    goBack(): void {

        this.back.emit();

    }

}