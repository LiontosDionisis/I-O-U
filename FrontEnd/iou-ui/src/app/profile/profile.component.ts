import { Component } from '@angular/core';
import { UserDTO, UserService } from '../services/user.service.service';
import { AvatarType, getAvatarUrl } from '../Models/avatar';
import { CurrentUserDTO } from '../Models/current-user.dto';
import { NgIf } from '@angular/common';
import { UpdateUserDto } from '../Models/updateUser.dto';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatIconRegistry } from '@angular/material/icon';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [NgIf, FormsModule, MatButtonModule, MatIconModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent {
  user?: CurrentUserDTO
  avatarUrl: string = '';
  avatars: AvatarType[] = [AvatarType.Cat, AvatarType.Dog, AvatarType.Panda, AvatarType.Penguin];
  showUsernameInput = false;
  showEmailInput = false;

  newUsername: string = '';
  newEmail: string = '';

  constructor(private userService: UserService,private matIconRegistry: MatIconRegistry,
    private domSanitizer: DomSanitizer){
      this.matIconRegistry.setDefaultFontSetClass('material-icons');
    }

    //TODO: Fix Username and email becoming null after updating one of them
    
  ngOnInit(): void {
    this.userService.getCurrentUser().subscribe(user => {
      this.user = user;
      this.avatarUrl = getAvatarUrl(user.avatar);
    });
  }

  updateUsername(userId: number, newUsername: string) {
    const dto: UpdateUserDto = { username: newUsername };
    this.userService.updateUser(userId, dto).subscribe({
      next: res => {
        console.log('Username updated');
        if (this.user) this.user.username = newUsername;
        this.showUsernameInput = false;
      },
      error: err => console.log('Fail to update username')
    });
  }

  updateEmail(userId: number, newEmail: string) {
    const dto: UpdateUserDto = { email: newEmail };
    this.userService.updateUser(userId, dto).subscribe({
      next: res => {
        console.log('Email updated');
        if (this.user) this.user.email = newEmail;
        this.showEmailInput = false;
      },
      error: err => console.log('Fail to update email')
    });
  }


  getAvatarUrl = getAvatarUrl;

}
