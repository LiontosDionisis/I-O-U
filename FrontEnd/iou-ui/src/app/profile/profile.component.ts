import { Component } from '@angular/core';
import { UserDTO, UserService } from '../services/user.service.service';
import { AvatarType, getAvatarUrl } from '../Models/avatar';
import { CurrentUserDTO } from '../Models/current-user.dto';
import { NgIf, NgFor } from '@angular/common';
import { UpdateUserDto } from '../Models/updateUser.dto';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatIconRegistry } from '@angular/material/icon';
import { DomSanitizer } from '@angular/platform-browser';
import { UpdateUsernameDto } from '../Models/UpdateUsernameDto';
import { UpdateEmailDto } from '../Models/updateEmailDto';
import { RouterLink, RouterOutlet } from '@angular/router';
import { UpdateAvatarDto } from '../Models/UpdateAvatarDto';
import { UpdatePasswordDto } from '../Models/UpdatePasswordDto';



@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [NgIf, FormsModule, MatButtonModule, MatIconModule, RouterLink, RouterOutlet, NgFor],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent {
  user?: CurrentUserDTO
  avatarUrl: string = '';
  avatars: AvatarType[] = [AvatarType.Cat, AvatarType.Dog, AvatarType.Panda, AvatarType.Penguin];
  showUsernameInput = false;
  showEmailInput = false;
  showAvatarInput = false;
  showPasswordInput = false;

  newUsername: string = '';
  newEmail: string = '';
  oldPassword: string = '';
  password: string = '';

  constructor(private userService: UserService,private matIconRegistry: MatIconRegistry,
    private domSanitizer: DomSanitizer){
      this.matIconRegistry.setDefaultFontSetClass('material-icons');
    }

    
  ngOnInit(): void {
    this.avatars = [AvatarType.Cat, AvatarType.Dog, AvatarType.Panda, AvatarType.Penguin];
    this.userService.getCurrentUser().subscribe(user => {
      this.user = user;
      this.avatarUrl = getAvatarUrl(user.avatar);
    });
  }

  updateUsername(userId: number, newUsername: string) {
    const dto: UpdateUsernameDto = { username: newUsername };
    this.userService.updateUsername(userId, dto).subscribe({
      next: res => {
        console.log('Username updated');
        if (this.user) this.user.username = newUsername;
        this.showUsernameInput = false;
      },
      error: err => console.log('Fail to update username')
    });
  }

  updateEmail(userId: number, newEmail: string) {
    const dto: UpdateEmailDto = { email: newEmail };
    this.userService.updateEmail(userId, dto).subscribe({
      next: res => {
        console.log('Email updated');
        if (this.user) this.user.email = newEmail;
        this.showEmailInput = false;
      },
      error: err => console.log('Fail to update email')
    });
  }

  updateAvatar(userId: number, newAvatar: number){
    const dto: UpdateAvatarDto = {avatarType: newAvatar};
    this.userService.updateAvatar(userId, dto).subscribe({
      next: res => {
        console.log("Avatar updated!")
      },
      error: err => console.log("Failed to change avatar.")
    });
  }

  updatePassword(userId: number, oldPassword: string, password: string){
    const dto: UpdatePasswordDto = {currentPassword: oldPassword, newPassword: password}
    this.userService.updatePassword(userId, dto).subscribe({
      next: res => {
        console.log("Password changed!");
      },
      error: err => console.log("Failed to change password.")
    })
  }


  onSelectAvatar(avatar: number) {
    if (!this.user) return;

    this.avatarUrl = getAvatarUrl(avatar);       
    this.user.avatar = avatar;   
            
    this.updateAvatar(this.user.id, avatar);     
    this.showAvatarInput = false;                
  } 



  getAvatarUrl = getAvatarUrl;

}
