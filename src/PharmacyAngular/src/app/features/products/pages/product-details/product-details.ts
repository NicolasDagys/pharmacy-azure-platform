import {
    Component,
    OnInit,
    inject
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
    ActivatedRoute,
    Router,
    RouterLink
} from '@angular/router';

import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../../categories/services/category.service';

import { Product } from '../../models/product';
import { Category } from '../../../categories/models/category';

@Component({

    selector: 'app-product-details',

    standalone: true,

    imports: [
        CommonModule,
        RouterLink
    ],

    templateUrl: './product-details.html',

    styleUrl: './product-details.css'

})
export class ProductDetailsComponent implements OnInit {

    private route = inject(ActivatedRoute);

    private router = inject(Router);

    private productService = inject(ProductService);

    private categoryService = inject(CategoryService);

    product?: Product;

    category?: Category;

    loading = true;

    ngOnInit(): void {

        const codProd = this.route.snapshot.paramMap.get('codProd');

        if (!codProd) {

            this.router.navigate(['/products']);

            return;

        }

        this.productService.getById(codProd)

            .subscribe({

                next: product => {

                    this.product = product;

                    this.loadCategory(product.codeCat);

                },

                error: err => {

                    console.error(err);

                    this.loading = false;

                    this.router.navigate(['/products']);

                }

            });

    }

    private loadCategory(codeCat: string): void {

        this.categoryService.getById(codeCat)

            .subscribe({

                next: category => {

                    this.category = category;

                    this.loading = false;

                },

                error: err => {

                    console.error(err);

                    this.loading = false;

                }

            });

    }

}