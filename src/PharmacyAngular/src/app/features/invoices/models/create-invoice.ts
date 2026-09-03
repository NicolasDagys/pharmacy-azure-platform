import { InvoiceLine } from './invoice-line';

export interface CreateInvoice {

    shipmentAddressInv: string;

    idCus: string;

    invoiceLines: InvoiceLine[];

}