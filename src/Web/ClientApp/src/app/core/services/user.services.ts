import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class UserServices {
  private url: string = 'http://localhost:5252/api/user';

  constructor(private http: HttpClient) {}

  login(credential: LoginRequest): Observable<Response<LoginResponse>> {
    {
      return this.http.post<Response<LoginResponse>>(
        `${this.url}/login`,
        credential
      );
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
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
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

export interface LoginResponse {
  email: string;
  firtName: string;
  lastName: string;
  phone: string;
  token: string;
}

interface LoginRequest {
  email: string;
  password: string;
}
