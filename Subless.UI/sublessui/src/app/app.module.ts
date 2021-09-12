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
import { AuthInterceptor, AuthModule, LogLevel } from 'angular-auth-oidc-client';
import { environment } from '../environments/environment';
import { CreatorstatsComponent } from './creatorstats/creatorstats.component';
import { ReactiveFormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { IdComponent } from './id/id.component';
import { PayoneerComponent } from './payoneer/payoneer.component';
import { PartnerprofileComponent } from './partnerprofile/partnerprofile.component';
import { MatToolbarModule } from '@angular/material/toolbar';
import { UnauthorizedInterceptor } from './services/auth.interceptor';

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
    PayoneerComponent,
    PartnerprofileComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    MatToolbarModule,
    FormsModule,
    AuthModule.forRoot({
      config: {
        authority: environment.authority,
        redirectUrl: window.location.origin + "/login",
        postLogoutRedirectUri: window.location.origin + "/login",
        clientId: environment.clientId,
        scope: 'openid',
        responseType: 'code',
        silentRenew: true,
        useRefreshToken: true,
        logLevel: LogLevel.Error,
        secureRoutes: ['/api/Creator', '/api/Checkout/', '/api/Authorization', '/api/Partner', '/api/Admin'],
      },
    }),
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: UnauthorizedInterceptor, multi: true },
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  exports: [
    MatSelectModule
  ]
})
export class AppModule { }
