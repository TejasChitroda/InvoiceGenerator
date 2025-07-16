import { Product } from './product.model';

export interface CategoryModel {
  id?: number;
  name: string;
  description?: string;    // Optional field
  products?: Product[];    // Optional - only if you want nested products when loading a category
}