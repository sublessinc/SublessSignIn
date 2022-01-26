import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RegisterPaymentComponent } from './register-payment/register-payment.component';
import { LoginComponent } from './login/login.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { UserprofileComponent } from './userprofile/userprofile.component';
import { CreatorprofileComponent } from './creatorprofile/creatorprofile.component';
import { NavComponent } from './nav/nav.component';
import { LoggedOutComponent } from './logged-out/logged-out.component';
import { FormsModule } from '@angular/forms';
import { CreatorstatsComponent } from './creatorstats/creatorstats.component';
import { ReactiveFormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { IdComponent } from './id/id.component';
import { PartnerprofileComponent } from './partnerprofile/partnerprofile.component';
import { UnauthorizedInterceptor } from './services/auth.interceptor';
import { StopNavGuard } from './stop-nav.guard';
import { ThemeModule } from './theme/theme.module';
import { PartnerTestComponent } from './partner-test/partner-test.component';
import { AccountSettingsComponent } from './account-settings/account-settings.component';
import { PayoutsettingsComponent } from './payoutsettings/payoutsettings.component';
import { CreatorAccountSettingsComponent } from './creator-account-settings/creator-account-settings.component';
import { UserAccountSettingsComponent } from './user-account-settings/user-account-settings.component';

@NgModule({
  declarations: [
    AppComponent,
    RegisterPaymentComponent,
    LoginComponent,
    UserprofileComponent,
    CreatorprofileComponent,
    NavComponent,
    LoggedOutComponent,
    CreatorstatsComponent,
    IdComponent,
    PartnerprofileComponent,
    PartnerTestComponent,
    AccountSettingsComponent,
    PayoutsettingsComponent,
    CreatorAccountSettingsComponent,
    UserAccountSettingsComponent
  ],
  imports: [
    ThemeModule,
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: UnauthorizedInterceptor, multi: true },
    StopNavGuard
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  exports: [
    MatSelectModule
  ]
})
export class AppModule { }
