import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AccountSettingsComponent } from './account-settings/account-settings.component';
import { CreatorprofileComponent } from './creatorprofile/creatorprofile.component';
import { IdComponent } from './id/id.component';
import { LoggedOutComponent } from './logged-out/logged-out.component';
import { LoginComponent } from './login/login.component';
import { PartnerprofileComponent } from './partnerprofile/partnerprofile.component';
import { PayoutsettingsComponent } from './payoutsettings/payoutsettings.component';
import { RegisterPaymentComponent } from './register-payment/register-payment.component';
import { AuthGuard } from './services/auth.guard';
import { StopNavGuard } from './stop-nav.guard';
import { UserprofileComponent } from './userprofile/userprofile.component';

const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'register-payment', component: RegisterPaymentComponent, canActivate: [AuthGuard] },
  { path: 'account-settings', component: AccountSettingsComponent, canActivate: [AuthGuard] },
  { path: 'change-plan', component: RegisterPaymentComponent, canActivate: [AuthGuard] },
  { path: 'user-profile', component: UserprofileComponent, canActivate: [AuthGuard] },
  { path: 'creator-profile', component: CreatorprofileComponent, canActivate: [AuthGuard] },
  { path: 'payout-settings', component: PayoutsettingsComponent, canActivate: [AuthGuard] },
  { path: 'payout-setup', component: PayoutsettingsComponent, canActivate: [AuthGuard] },
  { path: 'partner-profile', component: PartnerprofileComponent, canActivate: [AuthGuard], canDeactivate: [StopNavGuard] },
  { path: 'id', component: IdComponent, canActivate: [AuthGuard] },
  { path: 'login', component: LoginComponent },
  { path: 'logged-out', component: LoggedOutComponent },

];

@NgModule({
  imports: [RouterModule.forRoot(routes, { enableTracing: true })],
  exports: [RouterModule]
})
export class AppRoutingModule { }

