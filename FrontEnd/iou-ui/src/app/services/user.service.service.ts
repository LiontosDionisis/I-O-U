import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { FriendshipDTO } from '../Models/friendshipDto';
import { UserNotification } from '../Models/notification';
import { SessionNotification } from '../Models/notificationSession';
import { CurrentUserDTO } from '../Models/current-user.dto';
import { UpdateUserDto } from '../Models/updateUser.dto';
import { UpdateUsernameDto } from '../Models/UpdateUsernameDto';
import { UpdateEmailDto } from '../Models/updateEmailDto';
import { UpdateAvatarDto } from '../Models/UpdateAvatarDto';

export interface UserDTO {
  id: number;
  username: string;
  email: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private http = inject(HttpClient);
  private auth = inject(AuthService);
  private apiUrl = 'http://localhost:5062/api/user';
  // private apiUrl = 'http://192.168.1.94:5062/api/user'
  

  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      Authorization: `Bearer ${localStorage.getItem('token') || ''}`
    });
  }

  searchUsers(username: string): Observable<UserDTO[]> {
    return this.http.get<UserDTO[]>(`${this.apiUrl}/byUsername/${username}`, {
      headers: this.getHeaders()
    });
  }

  sendFriendRequest(friendId: number): Observable<any> {
    return this.http.post(`http://localhost:5062/api/friendship/${friendId}`, {}, {
      headers: this.getHeaders()
    });
  }

  getFriends(): Observable<FriendshipDTO[]> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    }) 
    return this.http.get<FriendshipDTO[]>(`http://localhost:5062/api/friendship/friends`, {
      headers: this.getHeaders()
    })
  }

  addFriendToSession(sessionId: number, friendId: number): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.post(`http://localhost:5062/api/sessions/${sessionId}/add/${friendId}`, {}, { headers });
  }

  removeUserFromSession(sessionId: number, friendId: number): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.delete(`http://localhost:5062/api/sessions/${sessionId}/remove/${friendId}`, { headers });
  }

  getNotifications(): Observable<UserNotification[]> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.get<UserNotification[]>(`http://localhost:5062/api/notifications/all`, {headers});
  }

  getSessionNotifications(): Observable<SessionNotification[]> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.get<SessionNotification[]>(`http://localhost:5062/api/notifications/s/all`, {headers});
  }

  acceptFriendRequest(friendshipId: number) {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.put(`http://localhost:5062/api/friendship/accept/${friendshipId}`, {}, {headers});
  }
  
  denyFriendRequest(friendshipId: number) {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.delete(`http://localhost:5062/api/friendship/deny/${friendshipId}`, {headers})
  }

  deleteNotification(notificationId: number) {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
    return this.http.delete(`http://localhost:5062/api/notifications/delete/${notificationId}`, {headers});
  }


  deleteSessionNotification(notificationId: number) {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
    return this.http.delete(`http://localhost:5062/api/notifications/s/${notificationId}`, {headers});
  }

  getCurrentUser(): Observable<CurrentUserDTO>{
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.get<CurrentUserDTO>(`http://localhost:5062/api/user/me`, {headers});
  }

  updateUser(userId: number, userDto: UpdateUserDto) {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.put(`http://localhost:5062/api/user/${userId}`, userDto,  {headers});
  }

  updateUsername(userId: number, dto: UpdateUsernameDto) {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.patch(`http://localhost:5062/api/user/update-username/${userId}`, dto, {headers});
  }

  updateEmail(userId: number, dto: UpdateEmailDto) {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.patch(`http://localhost:5062/api/user/update-email/${userId}`, dto, {headers});
  }

  updateAvatar(userId: number, dto: UpdateAvatarDto) {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.patch(`http://localhost:5062/api/user/update-avatar/${userId}`, dto, {headers});
  }
}
