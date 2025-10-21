import { Component } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { Router } from "@angular/router";
import { CommonModule } from "@angular/common";
import {
  Response,
  User,
  UserServices,
} from "../../core/services/user.services";
import { MyGlobalObject } from "../../core/services/MyGlobalObject";

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
    private globalObject: MyGlobalObject,
  ) {}

  onLogin(): void {
    console.log("Email:", this.email, "Password:", this.password);

    if (this.email && this.password) {
      this.auth
        .login({ email: this.email, password: this.password })
        .subscribe({
          next: (response: Response<User>) => {
            console.log("Login successful:", response);

            if (!response.success) {
              this.isValid = false;
            } else {
              this.globalObject.setUserName(response.data.fullName);
              this.router.navigate(["/explorer"]);
            }
          },
          error: (error: any): void => {
            console.error("Login failed:", error);
            this.isValid = false;
          },
        });
    }
  }
}
