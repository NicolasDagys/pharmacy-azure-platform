import {
    inject
} from '@angular/core';

import {
    ActivatedRouteSnapshot,
    CanActivateFn,
    Router
} from '@angular/router';

import { TokenStorageService } from '../../features/auth/services/token-storage.service';

export const roleGuard: CanActivateFn = (
    route: ActivatedRouteSnapshot
) => {

    const router = inject(Router);

    const tokenStorage =
        inject(TokenStorageService);

    const token =
        tokenStorage.getToken();

    if (!token) {

        router.navigate(['/login']);

        return false;

    }

    const payload =
        JSON.parse(atob(token.split('.')[1]));

    const role =
        payload[
            'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
        ];

    const allowedRoles =
        route.data?.['roles'] as string[];

    if (
        role &&
        allowedRoles?.includes(role)
    ) {

        return true;

    }

    router.navigate(['/dashboard']);

    return false;

};