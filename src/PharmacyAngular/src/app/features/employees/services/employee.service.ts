import { Injectable, inject } from '@angular/core';

import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';

import { Employee } from '../models/employee';

import { QueryParameters } from '../../../core/models/query-parameters';

import { CreateEmployee } from '../models/create-employee';

import { UpdateEmployee } from '../models/update-employee';

import { ChangePassword } from '../models/change-password';

import { ChangeRole } from '../models/change-role';

import { buildQueryParams } from '../../../core/utils/http-query';

import { TokenStorageService } from '../../../features/auth/services/token-storage.service';

@Injectable({

    providedIn: 'root'

})
export class EmployeeService {

    private http = inject(HttpClient);

    private tokenStorage = inject(TokenStorageService);

    private api = environment.apiUrl + '/employees';

    getEmployees(query: QueryParameters): Observable<Employee[]> {

        return this.http.get<Employee[]>(

            this.api,

            {

                params: buildQueryParams(query)

            }

        );

    }

    getById(userEmp: string): Observable<Employee> {

        return this.http.get<Employee>(

            `${this.api}/${userEmp}`

        );

    }

    createEmployee(employee: CreateEmployee): Observable<any> {

        return this.http.post(

            this.api,

            employee

        );

    }

    updateEmployee(

        userEmp: string,

        employee: UpdateEmployee

    ): Observable<any> {

        return this.http.put(

            `${this.api}/${userEmp}`,

            employee

        );

    }

    changePassword(

        password: ChangePassword

    ): Observable<any> {

        return this.http.put(

            `${this.api}/change-password`,

            password

        );

    }

    changeRole(

        userEmp: string,

        role: ChangeRole

    ): Observable<any> {

        return this.http.put(

            `${this.api}/${userEmp}/role`,

            role

        );

    }

    deleteEmployee(userEmp: string): Observable<any> {

        return this.http.delete(

            `${this.api}/${userEmp}`

        );

    }

    getCurrentUsername(): string | null {

        const token = this.tokenStorage.getToken();

        if (!token) {

            return null;

        }

        const payload = JSON.parse(atob(token.split('.')[1]));

        console.log(payload);

        return (
            payload.sub ??
            payload.unique_name ??
            payload.name ??
            payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ??
            null
        );

    }

}