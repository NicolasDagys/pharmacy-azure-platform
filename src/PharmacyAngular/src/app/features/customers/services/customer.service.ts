import { Injectable, inject } from '@angular/core';

import {
    HttpClient
} from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';

import { Customer } from '../models/customer';

import { QueryParameters } from '../../../core/models/query-parameters';

import { buildQueryParams } from '../../../core/utils/http-query';

import { CustomerLookup } from '../models/customer-lookup';

@Injectable({

    providedIn: 'root'

})
export class CustomerService {

    private http = inject(HttpClient);

    private api = environment.apiUrl + '/customers';

    getCustomers(
        query: QueryParameters
    ): Observable<Customer[]> {

        return this.http.get<Customer[]>(

            this.api,

            {
                params: buildQueryParams(query)
            }

        );

    }

    getById(
        idCus: string
    ): Observable<Customer> {

        return this.http.get<Customer>(

            `${this.api}/${idCus}`

        );

    }

    createCustomer(
        customer: Customer
    ): Observable<any> {

        return this.http.post(

            this.api,

            customer

        );

    }

    updateCustomer(
        idCus: string,
        customer: Customer
    ): Observable<any> {

        return this.http.put(

            `${this.api}/${idCus}`,

            customer

        );

    }

    deleteCustomer(
        idCus: string
    ): Observable<any> {

        return this.http.delete(

            `${this.api}/${idCus}`

        );

    }

    getLookup(): Observable<CustomerLookup[]> {

        return this.http.get<CustomerLookup[]>(

            `${this.api}/lookup`

        );

    }

}