import { isPlatformBrowser } from '@angular/common';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

  isLoggedIn(): boolean {
    if(isPlatformBrowser(this.platformId)){
        return !!localStorage.getItem('token');
    }
    return false;
  }

  login(token: string) {
    if(isPlatformBrowser(this.platformId)){
        localStorage.setItem('token', token);
    }
  }

  logout() {
    if(isPlatformBrowser(this.platformId)){
        localStorage.removeItem('token');
    }
  }
}
