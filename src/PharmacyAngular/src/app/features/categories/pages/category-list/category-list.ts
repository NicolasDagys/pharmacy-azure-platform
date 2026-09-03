import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, RouterModule } from '@angular/router';
import * as bootstrap from 'bootstrap';
import { ToastrService } from 'ngx-toastr';
import { TokenStorageService } from '../../../auth/services/token-storage.service';
import { Category } from '../../models/category';
import { CategoryService } from '../../services/category.service';
import { ConfirmationDialogComponent } from '../../../../shared/components/confirmation-dialog/confirmation-dialog';

@Component({
    selector: 'app-category-list',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        RouterLink,
        ConfirmationDialogComponent
    ],
    templateUrl: './category-list.html',
    styleUrl: './category-list.css'
})
export class CategoryListComponent implements OnInit {

    private categoryService = inject(CategoryService);
    private toastr = inject(ToastrService);
    private tokenStorage = inject(TokenStorageService);

    categories: Category[] = [];

    selectedCategory?: Category;

    loading = false;

    error = false;

    search = '';

    page = 1;

    pageSize = 10;

    sortBy = 'nameCat';

    descending = false;

    canManageCategories = false;

    ngOnInit(): void {

    this.loadCategories();

    const token =
        this.tokenStorage.getToken();

    if (!token) {

        this.canManageCategories = false;

        return;

    }

    const payload =
        JSON.parse(atob(token.split('.')[1]));

    const role =
        payload[
            'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
        ];

    this.canManageCategories =
        role === 'Admin';

}

    loadCategories(): void {

        this.loading = true;

        this.error = false;

        this.categoryService.getCategories({

            page: this.page,

            pageSize: this.pageSize,

            search: this.search,

            sortBy: this.sortBy,

            descending: this.descending

        }).subscribe({

            next: response => {

                this.categories = response;

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

        this.loadCategories();

    }

    previousPage(): void {

        if (this.page > 1) {

            this.page--;

            this.loadCategories();

        }

    }

    nextPage(): void {

        this.page++;

        this.loadCategories();

    }

    openDelete(category: Category): void {

        this.selectedCategory = category;

        const modalElement = document.getElementById('confirmationModal');

        if (!modalElement) {

            return;

        }

        const modal = new bootstrap.Modal(modalElement);

        modal.show();

    }

    deleteCategory(): void {

        if (!this.selectedCategory) {

            return;

        }

        this.categoryService.deleteCategory(

            this.selectedCategory.codeCat

        ).subscribe({

            next: () => {

                this.toastr.success(

                    `Category ${this.selectedCategory?.nameCat} deleted successfully.`,

                    'Success'

                );

                this.categories = this.categories.filter(

                    c => c.codeCat !== this.selectedCategory?.codeCat

                );

                this.selectedCategory = undefined;

            },

            error: err => {

                const message =

                    typeof err.error === 'string'

                        ? err.error

                        : 'Unexpected error';

                if (err.status === 404) {

                    this.toastr.error(

                        'Category not found.',

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

}