import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { CookieService } from '../services/CookieService';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  isExpanded = false;
  token = '';

  constructor(private authService: AuthService, private cookies: CookieService) {
    // get the antiforgery token
    this.token = this.cookies.getCookie('__Host-CSRF-Token');
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  isAuthenticated() {
    return this.authService.isAuthenticated();
  }
}
