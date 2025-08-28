import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

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
}
