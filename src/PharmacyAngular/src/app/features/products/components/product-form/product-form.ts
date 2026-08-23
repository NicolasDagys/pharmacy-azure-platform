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

import { Category } from '../../../categories/models/category';

import { Product } from '../../models/product';

import { Router } from '@angular/router';

@Component({

    selector: 'app-product-form',

    standalone: true,

    imports: [

        CommonModule,

        ReactiveFormsModule

    ],

    templateUrl: './product-form.html',

    styleUrl: './product-form.css'

})
export class ProductFormComponent implements OnChanges {

    private fb = inject(FormBuilder);
    private router = inject(Router);

    @Input()

    product?: Product;

    @Input()

    categories: Category[] = [];

    @Input()

    presentationTypes: string[] = [];

    @Input()

    editMode = false;

    @Input()

    buttonText = 'Save Product';

    @Output()

    save = new EventEmitter<Product>();

    @Output()
back = new EventEmitter<void>();

    form = this.fb.group({

        codProd: [

            '',

            [
                Validators.required,
                Validators.pattern(/^[A-Z]{3}[0-9]{7}$/)
            ]

        ],

        nameProd: [

            '',

            [
                Validators.required,
                Validators.minLength(3),
                Validators.maxLength(50)
            ]

        ],

        priceProd: [

            0,

            [
                Validators.required,
                Validators.min(0.01)
            ]

        ],

        expDateProd: [

            '',

            Validators.required

        ],

        presentationTypeProd: [

            '',

            Validators.required

        ],

        codeCat: [

            '',

            Validators.required

        ],

        sizeProd: [

            0,

            [
                Validators.required,
                Validators.min(1)
            ]

        ],

        stockQty: [

            0,

            [
                Validators.required,
                Validators.min(1)
            ]

        ]

    });

    ngOnChanges(changes: SimpleChanges): void {

        if (changes['product'] && this.product) {

            this.form.patchValue({

                codProd: this.product.codProd,

                nameProd: this.product.nameProd,

                priceProd: this.product.priceProd,

                expDateProd: this.product.expDateProd,

                presentationTypeProd:
                    this.product.presentationTypeProd,

                codeCat: this.product.codeCat,

                sizeProd: this.product.sizeProd,

                stockQty: this.product.stockQty

            });

            if (this.editMode) {

                this.form.controls.codProd.disable();

            }

        }

    }

    submit(): void {

        if (this.form.invalid) {

            this.form.markAllAsTouched();

            return;

        }

        this.save.emit(

            this.form.getRawValue() as Product

        );

    }
    goBack(): void {

    this.back.emit();

}

}