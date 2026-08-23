import { HttpInterceptorFn } from '@angular/common/http';

import { inject } from '@angular/core';

import { TokenStorageService } from '../../features/auth/services/token-storage.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {

    const tokenStorage = inject(TokenStorageService);

    const token = tokenStorage.getToken();

    if (!token) {

        return next(req);

    }

    const cloned = req.clone({

        setHeaders: {

            Authorization: `Bearer ${token}`

        }

    });

    return next(cloned);

};