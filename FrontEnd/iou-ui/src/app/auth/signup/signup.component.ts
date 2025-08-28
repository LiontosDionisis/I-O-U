import { Component, inject, signal, computed } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { UserRegisterDTO } from '../../Models/user-register.dto';
import { NgIf } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [NgIf, HttpClientModule], 
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent {
  private authService = inject(AuthService);

  // Form signals
  email = signal('');
  username = signal('');
  password = signal('');
  loading = signal(false);
  errorMessage = signal('');
  successMessage = signal('');

  // Form validation
  isFormValid = computed(() => {
    return this.email().trim() !== '' &&
           this.username().trim() !== '' &&
           this.password().trim() !== '' &&
           !this.loading();
  });

  // Input handlers
  updateEmail(event: Event) { this.email.set((event.target as HTMLInputElement).value); this.clearMessages(); }
  updateUsername(event: Event) { this.username.set((event.target as HTMLInputElement).value); this.clearMessages(); }
  updatePassword(event: Event) { this.password.set((event.target as HTMLInputElement).value); this.clearMessages(); }
  private clearMessages() { this.errorMessage.set(''); this.successMessage.set(''); }

  // Submit handler
  onSubmit(event: Event) {
    event.preventDefault();
    if (!this.isFormValid()) return;

    this.loading.set(true);
    this.clearMessages();

    const userDto: UserRegisterDTO = {
      email: this.email(),
      username: this.username(),
      password: this.password()
    };

    this.authService.register(userDto).subscribe({
      next: () => {
        this.successMessage.set('Registration successful!');
        this.email.set('');
        this.username.set('');
        this.password.set('');
      },
      error: (err) => {
        this.errorMessage.set(err.error?.message || 'Registration failed. Please try again.');
      },
      complete: () => this.loading.set(false)
    });
  }
}
