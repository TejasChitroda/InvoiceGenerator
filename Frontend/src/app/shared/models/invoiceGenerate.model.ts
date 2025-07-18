import { CustomerModel } from "./customer.model";
import { InvoiceDetail } from "./invoiceDetail.model";

export interface InvoiceGenerate {
  id: number;
  invoiceDate: string;
  customerId: number;
  grantTotal: number;
  customer: CustomerModel;
  invoiceDetails: InvoiceDetail[];
}
 