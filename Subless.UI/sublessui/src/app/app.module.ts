import { NgModule, CUSTOM_ELEMENTS_SCHEMA, ErrorHandler } from '@angular/core';
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
import { IntegrationtestComponent } from './integrationtest/integrationtest.component';
import { WarnDialogComponent } from './warn-dialog/warn-dialog.component';
import { ErrorPageComponent } from './error-page/error-page.component';
import { ErrorInterceptor } from './error-handling/error-interceptor.interceptor';
import { GlobalerrorhandlerService } from './error-handling/globalerrorhandler.service';
import { TermsComponent } from './terms/terms.component';
import { CreatorTermsComponent } from './creator-terms/creator-terms.component';
import { PartnerTermsComponent } from './partner-terms/partner-terms.component';
import { RecentActivityComponent } from './recent-activity/recent-activity.component';
import { TopContentComponent } from './top-content/top-content.component';
import { DateFormatter } from './services/dateformatter.service';
import { PartnerstatsComponent } from './partnerstats/partnerstats.component';
import { SessionRenewComponent } from './session-renew/session-renew.component';
import { CookieModule } from 'ngx-cookie';
import { UserTopContentComponent } from './user-top-content/user-top-content.component';
import { UserRecentActivityComponent } from './user-recent-activity/user-recent-activity.component';

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
    UserAccountSettingsComponent,
    IntegrationtestComponent,
    WarnDialogComponent,
    ErrorPageComponent,
    TermsComponent,
    CreatorTermsComponent,
    PartnerTermsComponent,
    RecentActivityComponent,
    TopContentComponent,
    PartnerstatsComponent,
    SessionRenewComponent,
    UserTopContentComponent,
    UserRecentActivityComponent
  ],
  imports: [
    ThemeModule,
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    CookieModule.withOptions()
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: UnauthorizedInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    { provide: ErrorHandler, useClass: GlobalerrorhandlerService },
    StopNavGuard
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  exports: [
    MatSelectModule
  ]
})
export class AppModule { }
