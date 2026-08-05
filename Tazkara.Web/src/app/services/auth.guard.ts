import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from './auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn) {
    const expectedRoles = route.data['roles'] as Array<string>;
    if (!expectedRoles || expectedRoles.includes(authService.currentUserValue?.role || '')) {
      return true;
    }
    // Role unauthorized
    router.navigate(['/']);
    return false;
  }

  // Not logged in
  router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
  return false;
};
export const noAuthGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn) {
    router.navigate(['/']);
    return false;
  }
  return true;
};
