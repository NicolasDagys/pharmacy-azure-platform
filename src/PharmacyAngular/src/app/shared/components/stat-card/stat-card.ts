import { Component, Input } from '@angular/core';

@Component({

    selector: 'app-stat-card',

    standalone: true,

    templateUrl: './stat-card.html',

    styleUrl: './stat-card.css'

})
export class StatCardComponent {

    @Input()

    title = '';

    @Input()

    value: string | number = '';

}