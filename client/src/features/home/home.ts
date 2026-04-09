import { Component, inject, Input, signal } from '@angular/core';
import { Register } from "../account/register/register";
import { User } from '../../types/user';
import { AccountService } from '../../core/services/account-service';

@Component({
  selector: 'app-home',
  imports: [Register],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
  // how to pass data to child component
  @Input({required: true}) membersFromApp: User[] = [];

  protected accountService = inject(AccountService)
  protected registerMode = signal(false)

  showRegister(value: boolean) {
    this.registerMode.set(value)
  }
}
