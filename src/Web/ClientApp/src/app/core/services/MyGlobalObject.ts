import { Injectable } from "@angular/core";

@Injectable({
  providedIn: "root",
})
export class MyGlobalObject {
  private user: Data = {
    name: "",
  };

  constructor() {}

  public setUserName(name: string): void {
    this.user.name = name;
  }

  public getUserName(): string {
    return this.user.name;
  }
}

export interface Data {
  name: string;
}
