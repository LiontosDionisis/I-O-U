import { Component } from '@angular/core';
import { UserDTO, UserService } from '../services/user.service.service';
import { AvatarType, getAvatarUrl } from '../Models/avatar';
import { CurrentUserDTO } from '../Models/current-user.dto';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [NgIf],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent {
  user?: CurrentUserDTO
  avatarUrl: string = '';
  avatars: AvatarType[] = [AvatarType.Cat, AvatarType.Dog, AvatarType.Panda, AvatarType.Penguin];

  constructor(private userService: UserService){}

  ngOnInit(): void {
    this.userService.getCurrentUser().subscribe(user => {
      this.user = user;
      this.avatarUrl = getAvatarUrl(user.avatar);
    });
  }


  getAvatarUrl = getAvatarUrl;

}
