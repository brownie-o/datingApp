import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { LoginCreds, RegisterCreds, User } from '../../types/user';
import { tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { LikesService } from './likes-service';

@Injectable({
  providedIn: 'root', // provides to the application as a whole
  // will servive as a singleton, servive as long as the application is running
})

// this class can use dependency injection from angular and can be injected into other components/services/classes
export class AccountService {
  private http = inject(HttpClient);
  private likesService = inject(LikesService)
  currentUser = signal<User | null>(null);

  private baseUrl = environment.apiUrl;

  register(creds: RegisterCreds) {
    return this.http.post<User>(this.baseUrl + 'account/register', creds, { withCredentials: true }).pipe(
      tap(user => {
        if (user) {
          this.setCurrentUser(user)
          this.startTokenRefreshInterval()
        }
      })
    )
  }

  login(creds: LoginCreds) {
    // this.http.post add <User> to specify the type we expect back from the api
    return this.http.post<User>(this.baseUrl + 'account/login', creds, { withCredentials: true }).pipe(
      // can use Rxjs operators to do something with the observable
      // tap allows to do something with what we get back from api without modifying the data
      tap(user => {
        if (user) {
          this.setCurrentUser(user)
          this.startTokenRefreshInterval()
        }
      })
    )
  }

  refreshToken() {
    return this.http.post<User>(this.baseUrl + 'account/refresh-token', {}, { withCredentials: true })
  }

  startTokenRefreshInterval() {
    setInterval(() => {
      this.http.post<User>(this.baseUrl + 'account/refresh-token', {}, { withCredentials: true }).subscribe({
        next: user => {
          this.setCurrentUser(user)
        },
        error: () => {
          this.logout()
        }
      })
    }, 5 * 60 * 1000) // every 5 minutes
  }

  setCurrentUser(user: User) {
    user.roles = this.getRoleFromToken(user)

    this.currentUser.set(user)
    this.likesService.getLikeIds()
  }

  logout() {
    localStorage.removeItem("filters")
    this.likesService.clearLikeIds()
    this.currentUser.set(null)
  }

  private getRoleFromToken(user: User): string[] {
    const payload = user.token.split('.')[1]
    // atob is a built-in function to decode base64 string
    const decoded = atob(payload)
    const jsonPayload = JSON.parse(decoded)
    // always return an array
    return Array.isArray(jsonPayload.role) ? jsonPayload.role : [jsonPayload.role]
  }
}
