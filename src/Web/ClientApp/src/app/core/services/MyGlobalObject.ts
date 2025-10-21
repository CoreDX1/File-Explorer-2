import { isPlatformBrowser } from "@angular/common";
import { Inject, Injectable, PLATFORM_ID } from "@angular/core";

@Injectable({
  providedIn: "root",
})
export class MyGlobalObject {
  private readonly USER_KEY = "user_data";

  private user: Data = {
    name: "",
  };

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

  public setUserName(name: string): void {
    this.user.name = name;
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem(this.USER_KEY, JSON.stringify(this.user));
    }
  }

  public getUserName(): string {
    if (isPlatformBrowser(this.platformId)) {
      const user = localStorage.getItem(this.USER_KEY);
      return user ? JSON.parse(user).name : "";
    }
    return this.user.name;
  }

  public clearUserData(): void {
    this.user.name = "";
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem(this.USER_KEY);
    }
  }
}

export interface Data {
  name: string;
}
