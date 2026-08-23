import { Injectable, inject } from '@angular/core';

import { HttpClient, HttpParams } from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';

import { Category } from '../models/category';

import { QueryParameters } from '../../../core/models/query-parameters';

import { buildQueryParams } from '../../../core/utils/http-query';

@Injectable({

    providedIn: 'root'

})
export class CategoryService {

    private http = inject(HttpClient);

    private api = environment.apiUrl + '/categories';

    getCategories(query: QueryParameters): Observable<Category[]> {

        return this.http.get<Category[]>(

        this.api,

        {
            params: buildQueryParams(query)
        }

    );

    }

    getById(codeCat: string): Observable<Category> {

        return this.http.get<Category>(

            `${this.api}/${codeCat}`

        );

    }

    createCategory(category: Category): Observable<any> {

        return this.http.post(

            this.api,

            category

        );

    }

    updateCategory(

        codeCat: string,

        category: Category

    ): Observable<any> {

        return this.http.put(

            `${this.api}/${codeCat}`,

            category

        );

    }

    deleteCategory(codeCat: string): Observable<any> {

        return this.http.delete(

            `${this.api}/${codeCat}`

        );

    }

}