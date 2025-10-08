import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login.component.html',
})
export class LoginComponent {
  email: string = '';
  password: string = '';

  constructor(private auth: AuthService, private router: Router){}

  onLogin() {
    console.log('Email:', this.email, 'Password:', this.password);

    if(this.email && this.password){
      this.auth.login('fake-login');
      this.router.navigate(['/explorer']);
    }
  }
}
