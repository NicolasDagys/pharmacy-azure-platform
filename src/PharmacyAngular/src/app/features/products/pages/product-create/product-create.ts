import {

    Component,

    OnInit,

    inject

} from '@angular/core';

import {

    Router

} from '@angular/router';

import {

    forkJoin

} from 'rxjs';

import {

    ProductFormComponent

} from '../../components/product-form/product-form';

import {

    ProductService

} from '../../services/product.service';

import {

    CategoryService

} from '../../../categories/services/category.service';

import {

    Category

} from '../../../categories/models/category';

import {

    Product

} from '../../models/product';

import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

@Component({

    selector: 'app-product-create',

    standalone: true,

    imports: [

        ProductFormComponent,

        CommonModule

    ],

    templateUrl: './product-create.html',

    styleUrl: './product-create.css'

})
export class ProductCreateComponent implements OnInit {

    private productService = inject(ProductService);

    private categoryService = inject(CategoryService);

    private toastr = inject(ToastrService);

    private router = inject(Router);

    categories: Category[] = [];

    presentationTypes: string[] = [];

    loading = true;

    ngOnInit(): void {

        forkJoin({

            categories: this.categoryService.getCategories({ page: 1, pageSize: 1000 }),

            presentations: this.productService.getPresentationTypes()

        })

            .subscribe({

                next: data => {

                    this.categories = data.categories;

                    this.presentationTypes = data.presentations;

                    this.loading = false;

                },

                error: err => {

                    console.error(err);

                    this.loading = false;

                }

            });

    }

    save(product: Product): void {

        this.productService
            .createProduct(product)
            .subscribe({

                next: () => {

                    this.toastr.success(

                        'Product created successfully.',

                        'Success'

                    );

                    this.router.navigate([
                        '/products'
                    ]);

                },

                error: err => {

                    console.error(err);

                    let message = 'Unable to create product.';

                    if (typeof err.error === 'string') {

                        message = err.error;

                    }
                    else if (err.error?.errors) {

                        const firstError = Object.values(err.error.errors)
                            .flat()[0];

                        if (firstError) {

                            message = String(firstError);

                        }

                    }
                    else if (err.error?.message) {

                        message = err.error.message;

                    }

                    this.toastr.error(

                        message,

                        'Error'

                    );

                }

            });

    }
    goBack(): void {

    this.router.navigate([
        '/products'
    ]);

}

}