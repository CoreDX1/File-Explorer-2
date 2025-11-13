import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class UserServices {
  private url: string = "http://localhost:5252/api/v1";

  constructor(private http: HttpClient) {}

  login(credential: LoginRequest): Observable<Response<LoginResponse>> {
    {
      return this.http.post<Response<LoginResponse>>(
        `${this.url}/auth/login`,
        credential,
      );
    }
  }
}

export enum HttpStatus {
  OK = 200,
  CREATE = 201,
  BAD_REQUEST = 400,
  UNAUTHORIZED = 401,
  NOT_FOUND = 404,
  INTERNAL_SERVER_ERROR = 500,
}

export interface Response<T> {
  message: string;
  metadata: {
    statusCode: number;
    message: string;
  };
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
  firstName: string;
  lastName: string;
  phone: string;
  token: string;
}

interface LoginRequest {
  email: string;
  password: string;
}
