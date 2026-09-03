import {
    Component,
    OnInit,
    inject
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
    ActivatedRoute,
    Router
} from '@angular/router';

import {
    forkJoin
} from 'rxjs';

import { ProductService } from '../../services/product.service';

import { CategoryService } from '../../../categories/services/category.service';

import { ProductFormComponent } from '../../components/product-form/product-form';

import { Product } from '../../models/product';

import { Category } from '../../../categories/models/category';

import { ToastrService } from 'ngx-toastr';

@Component({
    selector: 'app-product-edit',
    standalone: true,
    imports: [
        CommonModule,
        ProductFormComponent
    ],
    templateUrl: './product-edit.html',
    styleUrl: './product-edit.css'
})
export class ProductEditComponent implements OnInit {

    private route = inject(ActivatedRoute);

    private router = inject(Router);

    private productService = inject(ProductService);

    private categoryService = inject(CategoryService);

    private toastr = inject(ToastrService);

    product?: Product;

    categories: Category[] = [];

    presentationTypes: string[] = [];

    loading = true;

    ngOnInit(): void {

        const codProd =
            this.route.snapshot.paramMap.get('codProd');

        if (!codProd) {

            this.router.navigate(['/products']);

            return;

        }

        forkJoin({

            product:
                this.productService.getById(codProd),

            categories:
                this.categoryService.getCategories({ page: 1, pageSize: 100 }),

            presentations:
                this.productService.getPresentationTypes()

        }).subscribe({

            next: data => {

                this.product =
                    data.product;

                this.categories =
                    data.categories;

                this.presentationTypes =
                    data.presentations;

                this.loading = false;

            },

            error: err => {

                console.error(err);

                this.loading = false;

            }

        });

    }

    save(product: Product): void {

        if (!this.product) {

            return;

        }

        this.productService
            .updateProduct(
                this.product.codProd,
                product
            )
            .subscribe({

                next: () => {

                    this.toastr.success(

                        'Product updated successfully.',

                        'Success'

                    );

                    this.router.navigate([
                        '/products'
                    ]);

                },

                error: err => {

                    console.error(err);

                    const message =

                        typeof err.error === 'string'

                            ? err.error

                            : err.error?.message

                            ?? 'Unable to update product.';

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