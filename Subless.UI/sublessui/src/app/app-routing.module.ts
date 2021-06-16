import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RegisterPaymentComponent } from './register-payment/register-payment.component';
import { AuthGuard } from './services/auth.guard';
import { UserprofileComponent } from './userprofile/userprofile.component';

const routes: Routes = [
  { path: 'register-payment', component: RegisterPaymentComponent, canActivate: [AuthGuard] },
  { path: 'user-profile', component: UserprofileComponent, canActivate: [AuthGuard] },
  { path: 'login', component: LoginComponent },

];

@NgModule({
  imports: [RouterModule.forRoot(routes, { enableTracing: true })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
