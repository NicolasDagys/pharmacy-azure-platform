import {
    Component,
    EventEmitter,
    Input,
    Output,
    OnChanges,
    SimpleChanges,
    inject
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
    FormBuilder,
    ReactiveFormsModule,
    Validators
} from '@angular/forms';

import { Employee } from '../../models/employee';

import { UpdateEmployee } from '../../models/update-employee';

@Component({

    selector: 'app-employee-edit-form',

    standalone: true,

    imports: [

        CommonModule,

        ReactiveFormsModule

    ],

    templateUrl: './employee-edit-form.html',

    styleUrl: './employee-edit-form.css'

})
export class EmployeeEditFormComponent implements OnChanges {

    private fb = inject(FormBuilder);

    @Input()

    employee?: Employee;

    @Output()

    save = new EventEmitter<UpdateEmployee>();

    @Output()
back = new EventEmitter<void>();

    form = this.fb.group({

        nameEmp: [

            '',

            [

                Validators.required,

                Validators.minLength(3),

                Validators.maxLength(50)

            ]

        ]

    });

    ngOnChanges(changes: SimpleChanges): void {

        if (changes['employee'] && this.employee) {

            this.form.patchValue({

                nameEmp: this.employee.nameEmp

            });

        }

    }

    submit(): void {

        if (this.form.invalid) {

            this.form.markAllAsTouched();

            return;

        }

        this.save.emit(

            this.form.getRawValue() as UpdateEmployee

        );

    }
    goBack(): void {

    this.back.emit();

}

}