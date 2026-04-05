import { Component } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { Router } from "@angular/router";
import { CommonModule } from "@angular/common";
import {
  Response,
  LoginResponse,
  UserServices,
  HttpStatus,
  RegisterRequest,
} from "../../core/services/user.services";
import { MyGlobalObject } from "../../core/services/MyGlobalObject";
import { AuthService } from "../../core/services/auth.service";

@Component({
  selector: "app-login",
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: "./login.component.html",
})
export class LoginComponent {
  // Login fields
  email: string = "";
  password: string = "";
  
  // Register fields
  firstName: string = "";
  lastName: string = "";
  phone: string = "";
  confirmPassword: string = "";
  
  // UI state
  isValid: boolean = true;
  isRegisterMode: boolean = false;
  isSubmitting: boolean = false;
  successMessage: string = "";

  constructor(
    private auth: UserServices,
    private router: Router,
    private globalObject: MyGlobalObject,
    private authService: AuthService,
  ) {}

  toggleMode(): void {
    this.isRegisterMode = !this.isRegisterMode;
    this.isValid = true;
    this.successMessage = "";
    this.clearForms();
  }

  clearForms(): void {
    this.email = "";
    this.password = "";
    this.firstName = "";
    this.lastName = "";
    this.phone = "";
    this.confirmPassword = "";
  }

  onLogin(): void {
    this.isSubmitting = true;
    this.isValid = true;

    if (this.email && this.password) {
      this.auth
        .login({ email: this.email, password: this.password })
        .subscribe({
          next: (response: Response<LoginResponse>) => {
            this.isSubmitting = false;
            if (response.metadata.statusCode !== 200) {
              this.isValid = false;
              return;
            }
            console.log("Login successful:", response);

            this.authService.login(response.data.accessToken);
            this.globalObject.setUserData(response.data);
            this.router.navigate(["/explorer"]);
          },
          error: (error: any): void => {
            this.isSubmitting = false;
            console.error("Login failed:", error);
            this.isValid = false;
          },
        });
    } else {
      this.isSubmitting = false;
      this.isValid = false;
    }
  }

  onRegister(): void {
    this.isSubmitting = true;
    this.isValid = true;
    this.successMessage = "";

    // Validation
    if (!this.firstName || !this.lastName || !this.email || !this.password) {
      this.isSubmitting = false;
      this.isValid = false;
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.isSubmitting = false;
      this.isValid = false;
      return;
    }

    const registerRequest: RegisterRequest = {
      firstName: this.firstName,
      lastName: this.lastName,
      email: this.email,
      phone: this.phone,
      password: this.password,
    };

    this.auth.register(registerRequest).subscribe({
      next: (response: Response<LoginResponse>) => {
        this.isSubmitting = false;
        if (response.metadata.statusCode === 201) {
          this.successMessage = "Registration successful! You are now logged in.";
          
          // Auto login after registration
          this.authService.login(response.data.accessToken);
          this.globalObject.setUserData(response.data);
          
          setTimeout(() => {
            this.router.navigate(["/explorer"]);
          }, 1500);
        } else {
          this.isValid = false;
        }
      },
      error: (error: any): void => {
        this.isSubmitting = false;
        console.error("Registration failed:", error);
        this.isValid = false;
        
        // Show detailed error message from backend
        if (error.error && error.error.metadata && error.error.metadata.message) {
          alert("Registration failed: " + error.error.metadata.message);
        }
      },
    });
  }
}
