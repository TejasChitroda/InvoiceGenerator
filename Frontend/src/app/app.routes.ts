import { Routes } from '@angular/router';
import { Customer } from './features/customer/customer';
import { Category } from './features/category/category';


export const routes: Routes = [

    {
        path: 'customer', component: Customer
    },
    {
        path: 'category', component: Category
    }
    
];
