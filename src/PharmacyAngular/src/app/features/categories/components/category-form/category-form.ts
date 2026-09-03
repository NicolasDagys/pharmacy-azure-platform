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

import { Category } from '../../models/category';

@Component({

    selector: 'app-category-form',

    standalone: true,

    imports: [
        CommonModule,
        ReactiveFormsModule
    ],

    templateUrl: './category-form.html',

    styleUrl: './category-form.css'

})
export class CategoryFormComponent implements OnChanges {

    private fb = inject(FormBuilder);

    @Input()

    category?: Category;

    @Input()

    editMode = false;

    @Input()

    buttonText = 'Save Category';

    @Output()

    save = new EventEmitter<Category>();
    @Output()
    back = new EventEmitter<void>();

    form = this.fb.group({

        codeCat: [

            '',

            [
                Validators.required,
                Validators.pattern(/^[A-Z]{3}[0-9]{3}$/)
            ]

        ],

        nameCat: [

            '',

            [
                Validators.required,
                Validators.minLength(3),
                Validators.maxLength(50)
            ]

        ]

    });

    ngOnChanges(changes: SimpleChanges): void {

        if (changes['category'] && this.category) {

            this.form.patchValue({

                codeCat: this.category.codeCat,

                nameCat: this.category.nameCat

            });

            if (this.editMode) {

                this.form.controls.codeCat.disable();

            }

        }

    }

    submit(): void {

        if (this.form.invalid) {

            this.form.markAllAsTouched();

            return;

        }

        this.save.emit(

            this.form.getRawValue() as Category

        );

    }
    goBack(): void {

        this.back.emit();

    }

}