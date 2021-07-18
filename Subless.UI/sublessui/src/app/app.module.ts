import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RegisterPaymentComponent } from './register-payment/register-payment.component';
import { LoginComponent } from './login/login.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { UserprofileComponent } from './userprofile/userprofile.component';
import { CreatorprofileComponent } from './creatorprofile/creatorprofile.component';
import { LogoutComponent } from './logout/logout.component';
import { LoggedOutComponent } from './logged-out/logged-out.component';
import { FormsModule } from '@angular/forms';
import { AuthInterceptor, AuthModule, LogLevel } from 'angular-auth-oidc-client';



@NgModule({
  declarations: [
    AppComponent,
    RegisterPaymentComponent,
    LoginComponent,
    UserprofileComponent,
    CreatorprofileComponent,
    LogoutComponent,
    LoggedOutComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    AuthModule.forRoot({
      //TODO: make these dynamic
      config: {
        authority: 'https://cognito-idp.us-east-1.amazonaws.com/us-east-1_vbXfe749W',
        redirectUrl: window.location.origin + "/login",
        postLogoutRedirectUri: window.location.origin + "/login",
        clientId: '6a4425t6hjaerp2nndqo3el3d1',
        scope: 'openid',
        responseType: 'code',
        silentRenew: true,
        useRefreshToken: true,
        logLevel: LogLevel.Debug,
        secureRoutes: ['/api/Creator', '/api/Checkout/', '/api/Authorization'],
      },
    }),
  ],
  providers: [{ provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },],
  bootstrap: [AppComponent]
})
export class AppModule { }
