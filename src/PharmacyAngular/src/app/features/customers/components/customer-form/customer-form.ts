import {
    Component,
    EventEmitter,
    Input,
    Output,
    inject,
    OnChanges,
    SimpleChanges
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
    FormBuilder,
    ReactiveFormsModule,
    Validators
} from '@angular/forms';

import { Customer } from '../../models/customer';

@Component({

    selector: 'app-customer-form',

    standalone: true,

    imports: [
        CommonModule,
        ReactiveFormsModule
    ],

    templateUrl: './customer-form.html',

    styleUrl: './customer-form.css'

})
export class CustomerFormComponent implements OnChanges {

    private fb = inject(FormBuilder);

    @Input()

    customer?: Customer;

    @Input()

    editMode = false;

    @Input()

    buttonText = 'Save Customer';

    @Output()

    save = new EventEmitter<Customer>();

    @Output()
    back = new EventEmitter<void>();

    form = this.fb.group({

        idCus: [
            '',
            [
                Validators.required,
                Validators.pattern(/^[0-9]{6,7}-[0-9]$/)
            ]
        ],

        nameCus: [
            '',
            [
                Validators.required,
                Validators.minLength(3)
            ]
        ],

        mailCus: [
            '',
            [
                Validators.required,
                Validators.email
            ]
        ],

        phoneCus: [
            '',
            [
                Validators.pattern('^[0-9]+$')
            ]
        ]

    });

    ngOnChanges(changes: SimpleChanges): void {

        if (changes['customer'] && this.customer) {

            this.form.patchValue({

                idCus: this.customer.idCus,

                nameCus: this.customer.nameCus,

                mailCus: this.customer.mailCus,

                phoneCus: this.customer.phoneCus ?? ''

            });

            if (this.editMode) {

                this.form.controls.idCus.disable();

            }

        }

    }

    submit(): void {

        if (this.form.invalid) {

            this.form.markAllAsTouched();

            return;

        }

        this.save.emit(

            this.form.getRawValue() as Customer

        );

    }
    goBack(): void {

        this.back.emit();

    }

}