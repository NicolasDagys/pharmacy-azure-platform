import { InvoiceLineDetail } from './invoice-line-detail';

export interface InvoiceDetails {

    numbInv: number;

    dateInv: string;

    shipmentAddressInv: string;

    totalInv: number;

    idCus: string;

    customerName: string;

    status: string;

    invoiceLines: InvoiceLineDetail[];

}