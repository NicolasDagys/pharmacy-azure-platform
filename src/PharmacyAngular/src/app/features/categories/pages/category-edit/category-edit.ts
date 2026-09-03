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

import { ToastrService } from 'ngx-toastr';

import {
    CategoryFormComponent
} from '../../components/category-form/category-form';

import {
    CategoryService
} from '../../services/category.service';

import {
    Category
} from '../../models/category';


@Component({

    selector: 'app-category-edit',

    standalone: true,

    imports: [

        CommonModule,

        CategoryFormComponent

    ],

    templateUrl: './category-edit.html',

    styleUrl: './category-edit.css'

})
export class CategoryEditComponent implements OnInit {

    private route = inject(ActivatedRoute);

    private router = inject(Router);

    private categoryService = inject(CategoryService);

    private toastr = inject(ToastrService);

    category?: Category;

    loading = true;

    ngOnInit(): void {

        const codeCat = this.route.snapshot.paramMap.get('codeCat');

        if (!codeCat) {

            this.router.navigate(['/categories']);

            return;

        }

        this.categoryService.getById(codeCat)

            .subscribe({

                next: category => {

                    this.category = category;

                    this.loading = false;

                },

                error: err => {

                    console.error(err);

                    this.loading = false;

                    this.toastr.error(

                        'Unable to load category.',

                        'Error'

                    );

                }

            });

    }

    save(category: Category): void {

        if (!this.category) {

            return;

        }

        this.categoryService.updateCategory(

            this.category.codeCat,

            category

        ).subscribe({

            next: () => {

                this.toastr.success(

                    'Category updated successfully.',

                    'Success'

                );

                this.router.navigate([

                    '/categories'

                ]);

            },

            error: err => {

                if (err.status === 409) {

                    this.toastr.warning(
                        err.error,
                        'Warning'
                    );

                    return;

                }

                const message =

                    typeof err.error === 'string'
                        ? err.error
                        : 'Unable to update category.';

                this.toastr.error(
                    message,
                    'Error'
                );

            }

        });

    }
    goBack(): void {

    this.router.navigate([
        '/categories'
    ]);

}

}