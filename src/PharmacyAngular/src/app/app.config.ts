import { ApplicationConfig } from '@angular/core';

import { provideRouter } from '@angular/router';

import {

provideHttpClient,

withInterceptors

}
from '@angular/common/http';

import { provideAnimations } from '@angular/platform-browser/animations';

import { routes } from './app.routes';

import { authInterceptor } from './core/interceptors/auth.interceptor';
import { provideToastr } from 'ngx-toastr';

export const appConfig: ApplicationConfig = {

providers: [

provideRouter(routes),

provideAnimations(),
provideToastr({

            positionClass: 'toast-bottom-right',

            preventDuplicates: true,

            timeOut: 3000

        }),

provideHttpClient(

withInterceptors([

authInterceptor

])

)

]

};