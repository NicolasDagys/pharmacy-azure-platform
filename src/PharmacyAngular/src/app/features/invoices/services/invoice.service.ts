import { Injectable, inject } from '@angular/core';

import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';

import { Invoice } from '../models/invoice';

import { CreateInvoice } from '../models/create-invoice';

import { QueryParameters } from '../../../core/models/query-parameters';

import { buildQueryParams } from '../../../core/utils/http-query';

import { InvoiceDetails } from '../models/invoice-details';

import { ChangeStatus } from '../models/change-status';

@Injectable({

    providedIn: 'root'

})
export class InvoiceService {

    private http = inject(HttpClient);

    private api = environment.apiUrl + '/Invoices';

    getInvoices(
        query: QueryParameters
    ): Observable<Invoice[]> {

        return this.http.get<Invoice[]>(

            this.api,

            {
                params: buildQueryParams(query)
            }

        );

    }

    getById(
        numbInv: number
    ): Observable<InvoiceDetails> {

        return this.http.get<InvoiceDetails>(
        `${this.api}/${numbInv}`
    );

    }

    createInvoice(
        invoice: CreateInvoice
    ): Observable<any> {

        return this.http.post(

            this.api,

            invoice

        );

    }

    changeStatus(
    numbInv: number,
    status: ChangeStatus
): Observable<any> {

    return this.http.put(

        `${this.api}/${numbInv}/status`,

        status

    );

}

}