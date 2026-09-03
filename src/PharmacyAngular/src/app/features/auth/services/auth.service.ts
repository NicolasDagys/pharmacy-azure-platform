import { Injectable, inject } from '@angular/core';

import { HttpClient } from '@angular/common/http';

import { Observable, tap } from 'rxjs';

import { environment } from '../../../../environments/environment';

import { LoginRequest } from '../models/login-request';

import { LoginResponse } from '../models/login-response';

import { TokenStorageService } from './token-storage.service';

@Injectable({

    providedIn: 'root'

})
export class AuthService {

    private http = inject(HttpClient);

    private tokenStorage = inject(TokenStorageService);

    private api = environment.apiUrl;

    login(request: LoginRequest): Observable<LoginResponse> {

        return this.http.post<LoginResponse>(

            `${this.api}/auth/login`,

            request

        ).pipe(

            tap(response => {

                this.tokenStorage.saveToken(

                    response.token

                );

            })

        );

    }

}