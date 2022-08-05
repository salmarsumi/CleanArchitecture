import { BrowserModule } from '@angular/platform-browser';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { AuthService } from './services/auth.service';
import { httpInterceptorProviders } from './http-interceptors';
import { AuditComponent } from './audit/audit.component';
import { AccessComponent } from './access/access.component';

export function initApp(auth: AuthService) {

  return () => {
    return auth.getSession();
  }
}

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    FetchDataComponent,
    AuditComponent,
    AccessComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'weather', component: FetchDataComponent },
      { path: 'audit', component: AuditComponent },
      { path: 'access', component: AccessComponent }
    ])
  ],
  providers: [
    httpInterceptorProviders,
    { provide: APP_INITIALIZER, useFactory: initApp, multi: true, deps: [AuthService] }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
