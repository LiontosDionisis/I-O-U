import { Component, inject, signal } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { UserRegisterDTO } from '../../Models/user-register.dto';
import { NgIf } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [NgIf, HttpClientModule], 
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  
  email = signal('');
  username = signal('');
  password = signal('');
  loading = signal(false);
  errorMessage = signal('');

  updateEmail(value: string) { this.email.set(value); this.errorMessage.set(''); }
  updateUsername(value: string) { this.username.set(value); this.errorMessage.set(''); }
  updatePassword(value: string) { this.password.set(value); this.errorMessage.set(''); }

  onSubmit(event: Event) {
  event.preventDefault(); 

  // Reset any previous state
  this.errorMessage.set('');
  this.loading.set(true);

  const userDto: UserRegisterDTO = {
    email: this.email(),
    username: this.username(),
    password: this.password()
  };

  this.authService.register(userDto).subscribe({
    next: () => {
      this.router.navigate(['/login']);
      this.email.set('');
      this.username.set('');
      this.password.set('');
    },
    error: (err) => {
      this.loading.set(true);

      if (err.error?.message) {
        this.errorMessage.set(err.error.message);
      } else {
        this.errorMessage.set('Registration failed. Please try again.');
      }
    },
    complete: () => {
      this.loading.set(false);
    }
  });
}

}
