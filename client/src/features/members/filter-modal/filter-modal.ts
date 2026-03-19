import { Component, ElementRef, model, output, ViewChild } from '@angular/core';
import { MemberParams } from '../../../types/member';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-filter-modal',
  imports: [FormsModule],
  templateUrl: './filter-modal.html',
  styleUrl: './filter-modal.css',
})
export class FilterModal {
  // Using a dialog element to create a modal, which is supported in modern browsers. The modal can be opened and closed using the showModal() and close() methods of the dialog element.
  // modalRef!: ! overrides ts eslint since it will be available when the user clicks the btn to open the modal 
  @ViewChild('filterModal') modalRef!: ElementRef<HTMLDialogElement>
  // closeModal do not need a type since it will not emit any data
  closeModal = output()
  // emit a type of MemberParams
  submitData = output<MemberParams>();
  memberParams = model(new MemberParams())

  constructor() {
    const filters = localStorage.getItem('filters');
    if (filters) {
      this.memberParams.set(JSON.parse(filters));
    }
  }

  open() {
    // nativeElement.showModal(): js method working wiht HTMLDialogElement to open the modal
    this.modalRef.nativeElement.showModal();
  }

  close() {
    this.modalRef.nativeElement.close()
    this.closeModal.emit() // msg to parent that the modal is closed
  }

  submit() {
    this.submitData.emit(this.memberParams());
    this.close();
  }

  onMinAgeChange() {
    if (this.memberParams().minAge < 18) this.memberParams().minAge = 18
  }

  onMaxAgeChange() {
    if (this.memberParams().maxAge < this.memberParams().minAge) {
      this.memberParams().maxAge = this.memberParams().minAge
    }
  }
}
