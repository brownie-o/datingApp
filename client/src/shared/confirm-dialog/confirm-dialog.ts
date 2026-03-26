import { Component, ElementRef, inject, ViewChild } from '@angular/core';
import { ConfirmDialogService } from '../../core/services/confirm-dialog-service';

@Component({
  selector: 'app-confirm-dialog',
  imports: [],
  templateUrl: './confirm-dialog.html',
  styleUrl: './confirm-dialog.css',
})
export class ConfirmDialog {
  @ViewChild('dialogRef') dialogRef!: ElementRef<HTMLDialogElement> // reference to the dialog element in the template
  message = "Are you sure?"
  private resolver: ((result: boolean) => void) | null = null

  constructor() {
    inject(ConfirmDialogService).register(this) // this: dialog component instance, register it with the service so that it can call the open method on it
  }

  open(message: string): Promise<boolean> {
    this.message = message
    this.dialogRef.nativeElement.showModal()
    // setting the resolver to resolve, the resolver can be used later to confirm or cancel the dialog
    return new Promise(resolve => (this.resolver = resolve))
  }

  confirm() {
    this.dialogRef.nativeElement.close()
    this.resolver?.(true)
    this.resolver = null
  }

  cancel() {
    this.dialogRef.nativeElement.close()
    this.resolver?.(false)
    this.resolver = null
  }
}
