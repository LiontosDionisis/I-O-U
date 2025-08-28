import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { UserRegisterDTO } from '../Models/user-register.dto';
import { HttpClientModule } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5062/api/user';

  constructor() { }

  register(user: UserRegisterDTO){
    // Send the POST Request
    return this.http.post<any>(`${this.apiUrl}/register`, user).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error("Server side error:", error);
        return throwError(() => new Error("Registration failed. Please check your data and try again"));
      })
    );
  }

  
}
