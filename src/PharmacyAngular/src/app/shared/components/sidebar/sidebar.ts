import { Component, inject } from '@angular/core';

import { RouterLink, RouterLinkActive } from '@angular/router';

import { TokenStorageService } from '../../../features/auth/services/token-storage.service';
import { CommonModule } from '@angular/common';

@Component({

    selector: 'app-sidebar',

    standalone: true,

    imports: [

        RouterLink,
        CommonModule,
        RouterLinkActive

    ],

    templateUrl: './sidebar.html',

    styleUrl: './sidebar.css'

})
export class SidebarComponent {

    private tokenStorage = inject(TokenStorageService);

    get role(): string {

        const token = this.tokenStorage.getToken();

        if (!token) {
            return '';
        }

        const payload = JSON.parse(atob(token.split('.')[1]));

        return payload[
            'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
        ] ?? '';
    }

    get isAdmin(): boolean {

    return this.role === 'Admin';

}

get isSales(): boolean {

    return this.role === 'Sales';

}

get isPharmacist(): boolean {

    return this.role === 'Pharmacist';

}

get canViewCustomers(): boolean {

    return this.role === 'Admin' ||
           this.role === 'Sales';

}

get canViewEmployees(): boolean {

    return this.role === 'Admin';

}

}