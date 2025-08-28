import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const redirectIfLoggedInGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  return auth.redirectIfLoggedIn();
};
