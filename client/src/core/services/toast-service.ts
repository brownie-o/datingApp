import { Injectable } from '@angular/core';

// for not to install 3rd party packages, we are creating our own toast service

@Injectable({
  providedIn: 'root',
})
export class ToastService {

  constructor() {
    this.createToastConatiner()
  }

  // services are singleton, initialized while app starts, and can be reused
  private createToastConatiner() {
    if (!document.getElementById('toast-container')) {
      const container = document.createElement('div');
      container.id = 'toast-container';
      container.className = 'toast toast-bottom toast-end'
      document.body.appendChild(container)
    }
  }

  private createToastElement(message: string, alertClass: string, duration = 5000) {
    const toastContainer = document.getElementById("toast-container");
    if (!toastContainer) return;

    const toast = document.createElement('div')
    toast.classList.add('alert', alertClass, 'shadow-lg')
    toast.innerHTML = `
    <span>${message}</span>
    <button class="ml-4 btn btn-sm btn-ghost">x</button>
    `

    toast.querySelector('button')?.addEventListener("click", () => {
      toastContainer.removeChild(toast)
    })

    toastContainer.append(toast)
    setTimeout(() => {
      if (toastContainer.contains(toast)) {
        toastContainer.removeChild(toast)
      }
    }, duration)
  }

  success(message: string, duration?: number) {
    this.createToastElement(message, 'alert-success', duration)
  }

  error(message: string, duration?: number) {
    this.createToastElement(message, 'alert-error', duration)
  }

  warning(message: string, duration?: number) {
    this.createToastElement(message, 'alert-warning', duration)
  }

  info(message: string, duration?: number) {
    this.createToastElement(message, 'alert-info', duration)
  }
}
