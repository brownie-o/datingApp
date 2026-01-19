import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AccountService } from '../services/account-service';

// An interceptor that adds a JWT token to the Authorization header of outgoing HTTP requests if the user is logged in.
export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const accountService = inject(AccountService)

  const user = accountService.currentUser()

  if(user){
    // req is immutable, so we need to clone it to modify
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${user.token}`
      }
    })
  }

  return next(req);
};
