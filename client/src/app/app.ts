import { Component, inject, OnInit, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom } from 'rxjs';
import { Nav } from "../layout/nav/nav";
import { Home } from '../features/home/home';
import { AccountService } from '../core/services/account-service';
import { User } from '../types/user';

// The @Component decorator transforms the App class into an Angular component.
@Component({
  selector: 'app-root', // specifies what element selector in index.html. e.g. <app-root> 
  imports: [Nav, Home], // specifies what to import
  templateUrl: './app.html', // specifies what template to use
  styleUrl: './app.css' // specifies which styles to apply
})

export class App implements OnInit {
  private accountService = inject(AccountService);
  private http = inject(HttpClient);
  protected readonly title = signal('Dating App');
  protected members = signal<User[]>([])


  // should reference using this. for the property in class

  //if using subscribe, will need to unsubscribe to avoid memory leaks
  // ngOnInit(): void {
  //   this.http.get("https://localhost:5001/api/members").subscribe({
  //     next: response => this.members.set(response),
  //     error: error => console.log(error),
  //     complete: () => console.log("Completed the http request")
  //   });
  // }

  async ngOnInit() {
    this.members.set(await this.getMembers())
    this.setCurrentUser()
  }

  setCurrentUser() {
    const userString = localStorage.getItem("user")
    if (!userString) return
    const user = JSON.parse(userString)
    this.accountService.currentUser.set(user)
  }

  async getMembers() {
    try {
      return lastValueFrom(this.http.get<User[]>("https://localhost:5001/api/members"))
    } catch (error) {
      console.log(error)
      throw error
    }
  }

}
