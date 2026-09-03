import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({

    selector: 'app-confirmation-dialog',

    standalone: true,

    templateUrl: './confirmation-dialog.html',

    styleUrl: './confirmation-dialog.css'

})
export class ConfirmationDialogComponent {

    @Input()

    title = 'Confirmation';

    @Input()

    message = '';

    @Output()

    confirmed = new EventEmitter<void>();

    confirm(): void {

        this.confirmed.emit();

    }

}