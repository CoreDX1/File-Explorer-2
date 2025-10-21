import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class UserServices {
  private url: string = "http://localhost:5252/api/user";

  constructor(private http: HttpClient) {}

  login(credential: LoginRequest): Observable<Response<User>> {
    {
      return this.http.post<Response<User>>(`${this.url}/login`, credential);
    }
  }
}

export interface Response<T> {
  message: string;
  success: boolean;
  data: T;
}

export interface User {
  id: number;
  username: string;
  email: string;
  passwordHash: string;
  fullName: string;
  storageQuotaBytes: number;
  createdAt: string;
  updatedAt: string;
  isActive: boolean;
  lastLoginAt: string;
  trackingState: number;
  modifiedProperties: any;
}

interface LoginResponse {
  email: string;
  fullName: string;
  token: string;
}

interface LoginRequest {
  email: string;
  password: string;
}
