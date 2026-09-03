import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { LoginRequest } from '../../models/login-request';


@Component({
    selector: 'app-login',
    standalone: true,
    imports: [ReactiveFormsModule],
    templateUrl: './login.html',
    styleUrl: './login.css'
})
export class LoginComponent {

    private authService = inject(AuthService);
    private router = inject(Router);
    private fb = inject(FormBuilder);

    loginForm = this.fb.group({

        userEmp: ['', Validators.required],

        password: ['', Validators.required]

    });

    login(): void {

    if (this.loginForm.invalid) {

        this.loginForm.markAllAsTouched();

        return;

    }

    this.authService.login(

        this.loginForm.getRawValue() as LoginRequest

    ).subscribe({

        next: () => {

            this.router.navigate(['/dashboard']);

        },

        error: () => {

            alert('Invalid username or password.');

        }

    });

}

}