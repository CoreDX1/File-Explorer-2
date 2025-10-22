import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MyGlobalObject } from '../../core/services/MyGlobalObject';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  standalone: true,
  imports: [CommonModule],
})
export class UserProfileComponent {
  constructor(private globalObjet: MyGlobalObject) {}

  get userName(): string {
    return this.globalObjet.getUserName();
  }
}
