import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { ISettings } from '../models/ISettings';
import { ITokenResponse } from '../models/ITokenResponse';
import { IRedirect } from '../models/IRedirect';


@Injectable({
  providedIn: 'root'
})
export class AuthorizationService {
  private baseURI: string = '';
  private redirectURI: string = '';
  private logoutURI: string = '';
  private code_verifier: string = '';
  constructor(
    private httpClient: HttpClient,
    private router: Router
  ) {
    this.baseURI = location.protocol + '//' + window.location.hostname + (location.port ? ':' + location.port : '');
    this.redirectURI = this.baseURI + "/login";
    this.logoutURI = this.baseURI + "/login";
  }

  public getSettings() {
    return this.httpClient.get<ISettings>('/api/Authorization/settings');
  }

  getToken(code: string) {
    this.getSettings().subscribe({
      next: (settings) => {
        var headers = new HttpHeaders().set('Content-Type', 'application/x-www-form-urlencoded');
        this.httpClient.post<ITokenResponse>
          (settings.issuerUrl
            + "/oauth2/token?grant_type=authorization_code&client_id=" + settings.appClientId
            + "&code_verifier=" + this.code_verifier
            + "&redirect_uri=" + this.redirectURI
            + "&code=" + code, null, { headers: headers }).subscribe({
              next: (tokenResponse: ITokenResponse) => {
                sessionStorage.setItem("id_token", tokenResponse.id_token);
                sessionStorage.setItem("access_token", tokenResponse.access_token);
                this.redirect();
              }
            });
      }
    });
  }

  redirect() {
    const authHeader = "Bearer " + sessionStorage.getItem('id_token');
    const activation = sessionStorage.getItem('activation');
    var headers = new HttpHeaders().set('Authorization', authHeader).set("Activation", activation ?? '');
    if (activation) {
      sessionStorage.setItem('activation', '');
    }
    this.httpClient.get<IRedirect>('/api/Authorization/redirect', { headers: headers }).subscribe({
      next: (redirectResponse: IRedirect) => {
        switch (redirectResponse.redirectionPath) {
          case 1:
            this.router.navigate(['register-payment']);
            break;
          case 2:
            this.router.navigate(['user-profile']);
            break;
          case 3:
            this.router.navigate(['creator-profile']);
            break;
          default: {
            break;
          }
        }
      }
    });
  }

  redirectToLogout() {
    this.getSettings().subscribe({
      next: (settings) => {
        window.location.replace(
          settings.issuerUrl
          + "/logout?response_type=code&client_id="
          + settings.appClientId
          + "&logout_uri="
          + this.logoutURI
        );
      }
    })
  }

  decodePayload(payload: string) {
    const cleanedPayload = payload.replace(/-/g, '+').replace(/_/g, '/');
    const decodedPayload = atob(cleanedPayload)
    const uriEncodedPayload = Array.from(decodedPayload).reduce((acc, char) => {
      const uriEncodedChar = ('00' + char.charCodeAt(0).toString(16)).slice(-2)
      return `${acc}%${uriEncodedChar}`
    }, '')
    const jsonPayload = decodeURIComponent(uriEncodedPayload);

    return JSON.parse(jsonPayload)
  }

  //Parse JWT Payload
  parseJWTPayload(token: string) {
    const [header, payload, signature] = token.split('.');
    const jsonPayload = this.decodePayload(payload)

    return jsonPayload
  };

  //Parse JWT Header
  parseJWTHeader(token: string) {
    const [header, payload, signature] = token.split('.');
    const jsonHeader = this.decodePayload(header)

    return jsonHeader
  };

  //Generate a Random String
  getRandomString() {
    const randomItems = new Uint32Array(28);
    crypto.getRandomValues(randomItems);
    const binaryStringItems = randomItems.map(dec => +`0${dec.toString(16).substr(-2)}`);
    return binaryStringItems.reduce((acc, item) => `${acc}${item}`, '');
  }

  //Encrypt a String with SHA256
  async encryptStringWithSHA256(str: string) {
    const PROTOCOL = 'SHA-256'
    const textEncoder = new TextEncoder();
    const encodedData = textEncoder.encode(str);
    return crypto.subtle.digest(PROTOCOL, encodedData);
  }

  //Convert Hash to Base64-URL
  hashToBase64url(arrayBuffer: ArrayBuffer) {
    const items = new Uint8Array(arrayBuffer)
    const stringifiedArrayHash = items.reduce((acc, i) => `${acc}${String.fromCharCode(i)}`, '')
    const decodedHash = btoa(stringifiedArrayHash)

    const base64URL = decodedHash.replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
    return base64URL
  }

  public async getLoginLink() {
    this.getSettings().subscribe({
      next: async (settings) => {
        // Create random "state"
        var state = this.getRandomString();
        sessionStorage.setItem("pkce_state", state);

        // Create PKCE code verifier
        var code_verifier = this.getRandomString();
        sessionStorage.setItem("code_verifier", code_verifier);

        // Create code challenge
        var arrayHash = await this.encryptStringWithSHA256(code_verifier);
        var code_challenge = this.hashToBase64url(arrayHash);
        sessionStorage.setItem("code_challenge", code_challenge)

        // Redirtect user-agent to /authorize endpoint
        window.location.href = settings.issuerUrl + "/oauth2/authorize?response_type=code&state=" + state
          + "&client_id=" + settings.appClientId
          + "&redirect_uri=" + this.redirectURI
          + "&scope=openid&code_challenge_method=S256&code_challenge=" + code_challenge;
      }
    });
  }

  public async processAuthCode(code: string, state: string) {
    // Verify state matches
    if (sessionStorage.getItem("pkce_state") != state) {
      //TODO: error handling 
      alert("Invalid state");
    } else {

      // Fetch OAuth2 tokens from Cognito
      this.code_verifier = sessionStorage.getItem('code_verifier') ?? '';
      this.getToken(code);
    }
  }
}

