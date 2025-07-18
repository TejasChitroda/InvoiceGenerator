import { ProductModel } from './product.model';

export interface ProductPrice {
  id: number;
  productId: number;
  price: number;
  effectiveFrom: Date;
  effectiveTo?: Date;           // Optional because it can be null in C#
  product?: ProductModel;   
}