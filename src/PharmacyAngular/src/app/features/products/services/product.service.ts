import { Injectable, inject } from '@angular/core';

import { HttpClient, HttpParams } from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';

import { Product } from '../models/product';

import { QueryParameters } from '../../../core/models/query-parameters';


import { buildQueryParams } from '../../../core/utils/http-query';

import { ProductLookup } from '../models/product-lookup';

@Injectable({

    providedIn: 'root'

})
export class ProductService {

    private http = inject(HttpClient);

    private api = environment.apiUrl + '/products';

    getProducts(query: QueryParameters): Observable<Product[]> {

        return this.http.get<Product[]>(

        this.api,

        {
            params: buildQueryParams(query)
        }

    );

    }

    getPresentationTypes(): Observable<string[]> {

        return this.http.get<string[]>(

            `${this.api}/presentation-types`

        );

    }

    getById(codProd: string): Observable<Product> {

        return this.http.get<Product>(

            `${this.api}/${codProd}`

        );

    }

    createProduct(product: Product): Observable<any> {

        return this.http.post(

            this.api,

            product

        );

    }

    updateProduct(

        codProd: string,

        product: Product

    ): Observable<any> {

        return this.http.put(

            `${this.api}/${codProd}`,

            product

        );

    }

    deleteProduct(codProd: string): Observable<any> {

        return this.http.delete(

            `${this.api}/${codProd}`

        );

    }
    getLookup(): Observable<ProductLookup[]> {

    return this.http.get<ProductLookup[]>(

        `${this.api}/lookup`

    );

}

}