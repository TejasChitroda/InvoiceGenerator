import { CustomerModel } from './customer.model';
import { InvoiceDetail } from './invoiceDetail.model';

export interface Invoice {
  id: number;
  customerId: number;
  invoiceDate: Date;
  subTotal: number;
  taxTotal: number;
  grandTotal: number;
  customer?: CustomerModel;                // optional
  invoiceDetails?: InvoiceDetail[];   // optional list
}