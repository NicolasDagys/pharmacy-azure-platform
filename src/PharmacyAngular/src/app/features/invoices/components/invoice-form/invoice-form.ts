import {
    Component,
    EventEmitter,
    Input,
    OnInit,
    Output
} from '@angular/core';

import {
    CommonModule
} from '@angular/common';

import {
    FormsModule
} from '@angular/forms';

import {
    RouterLink
} from '@angular/router';

import { CreateInvoice } from '../../models/create-invoice';
import { InvoiceLine } from '../../models/invoice-line';

import { CustomerLookup } from '../../../customers/models/customer-lookup';
import { ProductLookup } from '../../../products/models/product-lookup';

@Component({

    selector: 'app-invoice-form',

    standalone: true,

    imports: [
        CommonModule,
        FormsModule,
        RouterLink,
    ],

    templateUrl: './invoice-form.html',

    styleUrl: './invoice-form.css'

})
export class InvoiceFormComponent implements OnInit {

    @Input()
    customers: CustomerLookup[] = [];

    @Input()
    products: ProductLookup[] = [];

    @Input()
    buttonText = 'Save';

    @Output()
    save =
        new EventEmitter<CreateInvoice>();

    invoice: CreateInvoice = {

        shipmentAddressInv: '',

        idCus: '',

        invoiceLines: []

    };

    selectedProduct = '';

    quantity = 1;

    ngOnInit(): void {

    }

    addLine(): void {

        if (!this.selectedProduct)
            return;

        if (this.quantity <= 0)
            return;

        const existing =
            this.invoice.invoiceLines.find(
                l => l.codProd === this.selectedProduct
            );

        if (existing) {

            existing.quantity += this.quantity;

        }
        else {

            this.invoice.invoiceLines.push({

                codProd: this.selectedProduct,

                quantity: this.quantity

            });

        }

        this.selectedProduct = '';

        this.quantity = 1;

    }

    removeLine(index: number): void {

        this.invoice.invoiceLines.splice(index, 1);

    }

    getProductName(code: string): string {

        return this.products.find(
            p => p.codProd === code
        )?.nameProd ?? code;

    }

    getUnitPrice(code: string): number {

        return this.products.find(
            p => p.codProd === code
        )?.priceProd ?? 0;

    }

    getLineTotal(line: InvoiceLine): number {

        return this.getUnitPrice(line.codProd) *
            line.quantity;

    }

    getInvoiceTotal(): number {

        return this.invoice.invoiceLines
            .reduce(
                (total, line) =>
                    total + this.getLineTotal(line),
                0
            );

    }

    submit(): void {

        if (this.invoice.invoiceLines.length === 0)
            return;

        this.save.emit(this.invoice);

    }

}