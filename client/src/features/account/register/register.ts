import { Component, inject, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RegisterCreds, User } from '../../../types/user';
import { AccountService } from '../../../core/services/account-service';


@Component({
  selector: 'app-register',
  imports: [FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  // how to get data from parent component
  membersFromHome = input.required<User[]>();

  private accountService = inject(AccountService)
  cancelRegiser = output<boolean>()
  protected creds = {} as RegisterCreds; // create using RegisterCreds type but not initialize the value while creating

  register() {
    this.accountService.register(this.creds).subscribe({
      next: response => {
        console.log(response)
        this.cancel()
      },
      error: error => console.log(error)
    })
  }

  cancel() {
    this.cancelRegiser.emit(false)
  }
}
