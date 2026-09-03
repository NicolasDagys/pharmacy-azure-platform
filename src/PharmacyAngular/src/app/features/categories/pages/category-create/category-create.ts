import {

    Component,

    OnInit,

    inject

} from '@angular/core';

import { CommonModule } from '@angular/common';

import { Router } from '@angular/router';

import { ToastrService } from 'ngx-toastr';

import { CategoryFormComponent } from '../../components/category-form/category-form';

import { CategoryService } from '../../services/category.service';

import { Category } from '../../models/category';

@Component({

    selector: 'app-category-create',

    standalone: true,

    imports: [

        CommonModule,

        CategoryFormComponent

    ],

    templateUrl: './category-create.html',

    styleUrl: './category-create.css'

})

export class CategoryCreateComponent implements OnInit {

    private categoryService = inject(CategoryService);

    private toastr = inject(ToastrService);

    private router = inject(Router);

    loading = false;

    ngOnInit(): void {

    }

    save(category: Category): void {

        this.categoryService.createCategory(category)

            .subscribe({

                next: () => {

                    this.toastr.success(

                        'Category created successfully.',

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
                            : 'Unable to create category.';

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