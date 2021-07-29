import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CreatorprofileComponent } from './creatorprofile/creatorprofile.component';
import { IdComponent } from './id/id.component';
import { LoggedOutComponent } from './logged-out/logged-out.component';
import { LoginComponent } from './login/login.component';
import { RegisterPaymentComponent } from './register-payment/register-payment.component';
import { AuthGuard } from './services/auth.guard';
import { UserprofileComponent } from './userprofile/userprofile.component';

const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'register-payment', component: RegisterPaymentComponent, canActivate: [AuthGuard] },
  { path: 'user-profile', component: UserprofileComponent, canActivate: [AuthGuard] },
  { path: 'creator-profile', component: CreatorprofileComponent, canActivate: [AuthGuard] },
  { path: 'id', component: IdComponent, canActivate: [AuthGuard] },
  { path: 'login', component: LoginComponent },
  { path: 'logged-out', component: LoggedOutComponent },

];

@NgModule({
  imports: [RouterModule.forRoot(routes, { enableTracing: true })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
