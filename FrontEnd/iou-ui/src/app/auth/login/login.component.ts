import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgIf } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { UserLoginDTO } from '../../Models/user-login.dto';
import { SignalrService } from '../../services/signalr.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [NgIf, HttpClientModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  private signalrService = inject(SignalrService);

  username = signal('');
  password = signal('');
  errorMessage = signal('');
  loading = signal(false);

  updateUsername(value: string) { this.username.set(value); this.errorMessage.set(''); }
  updatePassword(value: string) { this.password.set(value); this.errorMessage.set(''); }


  onSubmit(event: Event) {
  event.preventDefault();
  this.errorMessage.set('');
  this.loading.set(true);

  const userDto: UserLoginDTO = {
    username: this.username(),
    password: this.password()
  };

  this.authService.login(userDto).subscribe({
    next: (response) => {
      this.loading.set(false);
      localStorage.setItem('token', response.token);
      localStorage.setItem('username', response.Username);
      this.signalrService.startConnection();
      this.router.navigate(['/home']);
      this.username.set('');
      this.password.set('');
    },
    error: (err) => {
      this.loading.set(false); 
      if (err.error?.message) {
        this.errorMessage.set(err.error.message);
      } else {
        this.errorMessage.set("Login failed. Please try again later.");
      }
    }
  });
}

}
