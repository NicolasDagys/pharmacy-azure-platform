import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product';
import { RouterModule } from '@angular/router';
import { RouterLink } from '@angular/router';
import { ConfirmationDialogComponent } from '../../../../shared/components/confirmation-dialog/confirmation-dialog';
import { ToastrService } from 'ngx-toastr';
import * as bootstrap from 'bootstrap';
import { TokenStorageService } from '../../../auth/services/token-storage.service';

@Component({

    selector: 'app-product-list',

    standalone: true,

    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        RouterLink,
        ConfirmationDialogComponent
    ],

    templateUrl: './product-list.html',

    styleUrl: './product-list.css'

})
export class ProductListComponent implements OnInit {

    private productService = inject(ProductService);
    private toastr = inject(ToastrService);
    private tokenStorage = inject(TokenStorageService);

    products: Product[] = [];

    selectedProduct?: Product;

    loading = false;

    error = false;

    search = '';

    page = 1;

    pageSize = 10;

    sortBy = 'NameProd';

    descending = false;

    ngOnInit(): void {

        this.loadProducts();

    }

    loadProducts(): void {

        this.loading = true;

        this.error = false;

        this.productService.getProducts({

            page: this.page,

            pageSize: this.pageSize,

            search: this.search,

            sortBy: this.sortBy,

            descending: this.descending

        }).subscribe({

            next: response => {

                this.products = response;

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

        this.loadProducts();

    }

    nextPage(): void {

        this.page++;

        this.loadProducts();

    }

    previousPage(): void {

        if (this.page > 1) {

            this.page--;

            this.loadProducts();

        }

    }
    openDelete(product: Product): void {

        this.selectedProduct = product;

        const modalElement = document.getElementById('confirmationModal');

        if (!modalElement) {

            return;

        }

        const modal = new bootstrap.Modal(modalElement);

        modal.show();

    }
    deleteProduct(): void {

        if (!this.selectedProduct) {

            return;

        }

        this.productService.deleteProduct(

            this.selectedProduct.codProd

        ).subscribe({

            next: () => {

                this.toastr.success(

                    `Product ${this.selectedProduct?.nameProd} deleted successfully.`,

                    'Success'

                );

                this.products = this.products.filter(

                    p => p.codProd !== this.selectedProduct?.codProd

                );

                this.selectedProduct = undefined;

            },

            error: err => {

                const message =

                    typeof err.error === 'string'

                        ? err.error

                        : 'Unexpected error';

                if (err.status === 400) {

                    this.toastr.warning(

                        message,

                        'Warning'

                    );

                }

                else if (err.status === 404) {

                    this.toastr.error(

                        'Product not found.',

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

    get role(): string {

        const token = this.tokenStorage.getToken();

        if (!token) {
            return '';
        }

        const payload = JSON.parse(atob(token.split('.')[1]));

        return payload[
            'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
        ] ?? '';

    }

    get canManageProducts(): boolean {

        return this.role === 'Admin' ||
            this.role === 'Pharmacist';

    }

    

}