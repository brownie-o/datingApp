import { Injectable } from '@angular/core';
import { ConfirmDialog } from '../../shared/confirm-dialog/confirm-dialog';

@Injectable({
  providedIn: 'root',
})
export class ConfirmDialogService {
  private dialogComponent?: ConfirmDialog
  
  // pass the confirm dialog component to the service so that it can call the open method on it
  register(component: ConfirmDialog){
    this.dialogComponent = component
  }

  confirm(message = 'Are you sure?'): Promise<boolean>{
    if(!this.dialogComponent){
      throw new Error("Confirm dialog component is not registered")
    }
    return this.dialogComponent.open(message)
  }
}
