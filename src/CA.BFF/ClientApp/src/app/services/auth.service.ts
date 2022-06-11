import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private isLoggedIn = false

  user = null;

  constructor(private http: HttpClient) { }

  async getSession() {
    const session = await this.http.get<any>('/account/session').toPromise();
    if (session?.username) {
      this.isLoggedIn = true;
      this.user = session.username;
    } else {
      this.isLoggedIn = false;
      this.user = null;
    }
  }

  isAuthenticated() {
    return this.isLoggedIn;
  }
}
