import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { ApiError } from '../../../types/error';

@Component({
  selector: 'app-server-error',
  imports: [],
  templateUrl: './server-error.html',
  styleUrl: './server-error.css',
})
export class ServerError {
  // error will be in the format of ApiError what we defined in types/error.ts or null, and we initialize it to null
  // protected error = signal<ApiError | null>(null)

  // since we are sure that error will be always there when we navigate to this component, we can just define it as ApiError type
  protected error: ApiError
  private router = inject(Router)
  protected showDetails = false


  constructor() {
    const navigation = this.router.currentNavigation();
    this.error = navigation?.extras?.state?.['error']
  }

  detailsToggle() {
    this.showDetails = !this.showDetails
  }
}

