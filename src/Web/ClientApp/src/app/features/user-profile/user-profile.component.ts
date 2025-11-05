import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Data, MyGlobalObject } from '../../core/services/MyGlobalObject';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  standalone: true,
  imports: [CommonModule, FormsModule],
})
export class UserProfileComponent {
  public editableUser: Data = {
    email: '',
    firstName: '',
    lastName: '',
    phone: '',
  };

  constructor(private globalObjet: MyGlobalObject) {}

  ngOnInit(): void {
    this.editableUser = { ...this.globalObjet.getUserData() };
  }

  saveChanges() {
    this.globalObjet.setUserData(this.editableUser);
    console.log('Datos guardados', this.editableUser);
  }
}
