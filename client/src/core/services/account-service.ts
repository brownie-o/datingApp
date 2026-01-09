import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { LoginCreds, RegisterCreds, User } from '../../types/user';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root', // provides to the application as a whole
  // will servive as a singleton, servive as long as the application is running
})

// this class can use dependency injection from angular and can be injected into other components/services/classes
export class AccountService {
  private http = inject(HttpClient);
  currentUser = signal<User | null>(null);

  baseUrl = 'https://localhost:5001/api/';

  register(creds: RegisterCreds) {
    return this.http.post<User>(this.baseUrl + 'account/register', creds).pipe(
      tap(user => {
        if (user) {
          this.setCurrentUser(user)
        }
      })
    )
  }

  login(creds: LoginCreds) {
    // this.http.post add <User> to specify the type we expect back from the api
    return this.http.post<User>(this.baseUrl + 'account/login', creds).pipe(
      // can use Rxjs operators to do something with the observable
      // tap allows to do something with what we get back from api without modifying the data
      tap(user => {
        if (user) {
          this.setCurrentUser(user)
        }
      })
    )
  }

  setCurrentUser(user: User) {
    localStorage.setItem("user", JSON.stringify(user))
    this.currentUser.set(user)
  }

  logout() {
    localStorage.removeItem("user")
    this.currentUser.set(null)
  }
}
