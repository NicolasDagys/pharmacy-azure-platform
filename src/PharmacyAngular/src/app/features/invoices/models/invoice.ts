import { InvoiceLine } from './invoice-line';

export interface Invoice {

    numbInv: number;

    dateInv: string;

    shipmentAddressInv: string;

    totalInv: number;

    idCus: string;

    status: string;

    invoiceLines: InvoiceLine[];

}