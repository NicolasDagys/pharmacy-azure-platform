import { Component } from '@angular/core';

import { RouterOutlet } from '@angular/router';

import { SidebarComponent } from '../../shared/components/sidebar/sidebar';

import { TopbarComponent } from '../../shared/components/topbar/topbar';

@Component({

    selector:'app-main-layout',

    standalone:true,

    imports:[

        RouterOutlet,

        SidebarComponent,

        TopbarComponent

    ],

    templateUrl:'./main-layout.html',

    styleUrl:'./main-layout.css'

})
export class MainLayoutComponent{

}