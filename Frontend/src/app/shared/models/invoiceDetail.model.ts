import { Invoice } from './invoice.model';
import { Product } from './product.model';

export interface InvoiceDetail {
  id: number;
  invoiceId: number;
  productId: number;
  quantity: number;
  rate: number;
  subTotal: number;
  tax: number;
  total: number;
  grandTotal: number;
  invoice?: Invoice;     // optional navigation
  product?: Product;     // optional navigation
}