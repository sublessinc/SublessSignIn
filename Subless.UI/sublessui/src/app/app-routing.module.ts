import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RegisterPaymentComponent } from './register-payment/register-payment.component';
import { AuthGuard } from './services/auth.guard';

const routes: Routes = [
  { path: 'register-payment', component: RegisterPaymentComponent, canActivate: [AuthGuard] },
  { path: 'login', component: LoginComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
