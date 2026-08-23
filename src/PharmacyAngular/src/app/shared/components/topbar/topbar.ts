import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { TokenStorageService } from '../../../features/auth/services/token-storage.service';

@Component({
    selector: 'app-topbar',
    standalone: true,
    imports: [RouterLink],
    templateUrl: './topbar.html',
    styleUrl: './topbar.css'
})
export class TopbarComponent {

    private tokenStorage = inject(TokenStorageService);
    private router = inject(Router);

    get fullName(): string {

        const token = this.tokenStorage.getToken();

        if (!token) {
            return '';
        }

        const payload = JSON.parse(atob(token.split('.')[1]));

        return payload.FullName ?? '';
    }

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
    
    logout(): void {

        this.tokenStorage.clear();

        this.router.navigate(['/login']);

    }

}