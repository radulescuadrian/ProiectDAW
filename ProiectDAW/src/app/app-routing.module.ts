import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './pages/dashboard/home/home.component';
import { LoginComponent } from './pages/account/login/login.component';
import { RegisterComponent } from './pages/account/register/register.component';
import { AuthGuard } from './services/auth.guard';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: '', component: HomeComponent, canActivate: [AuthGuard]},
  { path: "**", redirectTo: "/" }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
