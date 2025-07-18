import { Component } from '@angular/core';
import { ProductService } from '../../core/services/product.service';
import { ProductModel } from '../../shared/models/product.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CategoryModel } from '../../shared/models/category.model';
import { CategoryService } from '../../core/services/category.service';
import { Router, RouterModule } from '@angular/router';
import { routes } from '../../app.routes';

@Component({
  selector: 'app-product',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './product.html',
  styleUrl: './product.css'
})
export class Product {

  products: ProductModel[] = [];

  productData: ProductModel = {
    id: 0,
    name: '',
    description: '',
    taxPercentage: 0,
    categoryId: 0,
  }
  categories: CategoryModel[] = [];
  isEdit: boolean = false;  

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    private router: Router
  ) {}

  ngOnInit() {
    console.log('Product component initialized');
    this.getProducts();
    this.loadCategories();
  }

  getProducts() {
    this.productService.getAllProducts().subscribe(products => {
      this.products = products;
      console.log('Products fetched:', this.products);
    });
  }

   loadCategories() {
    this.categoryService.getAllCategories().subscribe(data => {
      this.categories = data;
    });
  }

  submitProduct() {
    if (this.isEdit) {
      this.productService.updateProduct(this.productData.id!, this.productData).subscribe(updatedProduct => {
        console.log('Product updated:', updatedProduct);
        this.resetForm();
      }, error => {
        console.error('Error updating product:', error);
      });
    } else {
      this.productService.addProduct(this.productData).subscribe(newProduct => {
        console.log('Product added:', newProduct);
        this.resetForm();
      }, error => {
        console.error('Error adding product:', error);
      });
    }
    this.getProducts();
  }

  resetForm() {
    this.productData = {
      id: 0,
      name: '',
      description: '',
      taxPercentage: 0,
      categoryId: 0,
    };
    this.isEdit = false; // Reset to add mode
  } 

  getById(id: number) {
    this.productService.getById(id).subscribe(product => {
      this.productData = product;
      this.isEdit = true; // Set to edit mode
    }, error => {
      console.error('Error fetching product by ID:', error);
    });
  }

  deleteProductById(id: number) {
    this.productService.deleteProduct(id).subscribe(() => {
      console.log('Product deleted successfully');
      this.getProducts(); // Refresh the product list
    }, error => {
      console.error('Error deleting product:', error);
    });
  }

  editForm(product: ProductModel) {
    this.productData = { ...product };
    this.isEdit = true; // Set to edit mode
  }

  setPrice()
  {
    // here i want to redicet to ProductPriceComponent with productId
    this.router.navigate(['/product-price', this.productData.id]);
  }

}
