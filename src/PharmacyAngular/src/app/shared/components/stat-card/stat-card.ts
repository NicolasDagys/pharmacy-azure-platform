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

    private readonly icons: Record<string, string> = {

        'Active Products': 'inventory_2',

        'Customers': 'groups',

        'Monthly Revenue': 'payments',

        'Pending Invoices': 'pending_actions',

        'Low Stock': 'trending_down',

        'Out Of Stock': 'remove_shopping_cart',

        'Expiring Soon': 'event_busy',

        'Delivered Orders': 'local_shipping'

    };

    get icon(): string {

        return this.icons[this.title] ?? 'analytics';

    }

}