import { isPlatformBrowser } from '@angular/common';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { LoginResponse, User } from './user.services';

@Injectable({
  providedIn: 'root',
})
export class MyGlobalObject {
  private readonly USER_KEY = 'user_data';

  private user: Data = {
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
  };

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

  public setUserData(user: LoginResponse): void {
    this.user = {
      firstName: user.firtName,
      email: user.email,
      lastName: user.lastName,
      phone: user.phone,
    };

    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem(this.USER_KEY, JSON.stringify(this.user));
    }
  }

  public getUserName(): string {
    if (isPlatformBrowser(this.platformId)) {
      const user = localStorage.getItem(this.USER_KEY);
      return user ? JSON.parse(user).name : '';
    }
    return this.user.firstName;
  }

  public clearUserData(): void {
    this.user.firstName = '';
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem(this.USER_KEY);
    }
  }
}

export interface Data {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
}
