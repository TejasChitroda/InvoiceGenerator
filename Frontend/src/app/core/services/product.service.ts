import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ProductModel } from "../../shared/models/product.model";

@Injectable({
  providedIn: "root",
})
export class ProductService {
    private apiUrl = '/api/product';
  constructor(private http: HttpClient) {}

    getAllProducts() {
        return this.http.get<ProductModel[]>(this.apiUrl);
    }

    addProduct(product: ProductModel) {
        return this.http.post<ProductModel>(this.apiUrl, { ...product });
    }
    updateProduct(id: number, product: ProductModel) {
        return this.http.put<ProductModel>(`${this.apiUrl}/${id}`, { ...product });
    }
    getById(id: number) {
        return this.http.get<ProductModel>(`${this.apiUrl}/${id}`);
    }
    deleteProduct(id: number) {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
}