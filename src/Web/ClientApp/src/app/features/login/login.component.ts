import { Component } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { Router } from "@angular/router";
import { CommonModule } from "@angular/common";
import { UserServices } from "../../core/services/user.services";

@Component({
  selector: "app-login",
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: "./login.component.html",
})
export class LoginComponent {
  email: string = "";
  password: string = "";
  isValid: boolean = true;

  constructor(
    private auth: UserServices,
    private router: Router,
  ) {}

  onLogin() {
    console.log("Email:", this.email, "Password:", this.password);

    if (this.email && this.password) {
      this.auth
        .login({ email: this.email, password: this.password })
        .subscribe({
          next: (response) => {
            console.log("Login successful:", response);

            if (!response.success) {
              this.isValid = false;
            } else {
              this.router.navigate(["/explorer"]);
            }
          },
          error: (error) => {
            console.error("Login failed:", error);
            this.isValid = false;
          },
        });
    }
  }
}
