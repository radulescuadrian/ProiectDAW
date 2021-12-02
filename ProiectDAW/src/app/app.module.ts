import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './components/account/login/login.component';
import { RegisterComponent } from './components/account/register/register.component';
import { ProfileComponent } from './components/account/profile/profile.component';
import { GetProductsComponent } from './components/product/get-products/get-products.component';
import { GetProductComponent } from './components/product/get-product/get-product.component';
import { UpdateProductComponent } from './components/product/update-product/update-product.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    ProfileComponent,
    GetProductsComponent,
    GetProductComponent,
    UpdateProductComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
