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
import { AuthInterceptor } from './services/auth.interceptor';


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
    FormsModule
  ],
  providers: [{ provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }],
  bootstrap: [AppComponent]
})
export class AppModule { }
