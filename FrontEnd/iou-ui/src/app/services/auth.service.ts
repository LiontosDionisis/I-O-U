import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { UserRegisterDTO } from '../Models/user-register.dto';
import { HttpClientModule } from '@angular/common/http';
import { UserLoginDTO } from '../Models/user-login.dto';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5062/api/user';
  //private apiUrl = 'http://192.168.1.94:5062/api/user'
  private router = inject(Router)

  constructor() { }

  isLoggedIn() : boolean {
    return !!localStorage.getItem('token');
  }

  redirectIfLoggedIn(){
    if (this.isLoggedIn()) {
      this.router.navigate(['/home']);
      return false;
    }
    return true;
  }

  logout() {
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }

  register(user: UserRegisterDTO){
    // Send the POST Request
    return this.http.post<any>(`${this.apiUrl}/register`, user).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error("Error:", error);
        return throwError(() => error);
      })
    );
  }

  login(userLogin: UserLoginDTO){
    return this.http.post<any>(`${this.apiUrl}/login`, userLogin).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error("Error: ", error);
        return throwError(() => error);
      })
    )
  }


  getCurrentUsernameFromToken(): string | null {
    const token = localStorage.getItem('token');
    if (!token) return null;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload['unique_name'] || payload['name'] || null;
    } catch (e) {
      console.error('Failed to decode token', e);
      return null;
    }
  }

  
  
}
