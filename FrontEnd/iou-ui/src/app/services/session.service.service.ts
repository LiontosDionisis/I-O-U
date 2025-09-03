import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Session } from '../Models/session';

@Injectable({
  providedIn: 'root'
})
export class SessionServiceService {
  private apiUrl = 'http://localhost:5062/api/sessions';
  constructor(private http: HttpClient) { }


  getSessions(): Observable<Session[]>{
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
    return this.http.get<Session[]>(`${this.apiUrl}/my-sessions`, {headers});
  }

  
  createSession(name: string) {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    })
    const nameData = {
      name: name
    }
    
    return this.http.post(`${this.apiUrl}`, nameData, {headers});
  }

}

