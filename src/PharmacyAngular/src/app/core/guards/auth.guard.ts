import { inject } from '@angular/core';

import { CanActivateFn, Router } from '@angular/router';

import { TokenStorageService } from '../../features/auth/services/token-storage.service';

export const authGuard: CanActivateFn = () => {

    const tokenStorage = inject(TokenStorageService);

    const router = inject(Router);

    if (tokenStorage.getToken()) {

        return true;

    }

    router.navigate(['/login']);

    return false;

};