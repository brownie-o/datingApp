import { Component, input, Self } from '@angular/core';
import { ControlValueAccessor, Form, FormControl, NgControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  imports: [ReactiveFormsModule],
  templateUrl: './text-input.html',
  styleUrl: './text-input.css',
})

// ControlValueAccessor: tells angular that this component is a form control
export class TextInput implements ControlValueAccessor {
  label = input<string>('')
  type = input<string>('text')
  maxDate = input<string>('')

  // @Self(): dependency injection modifier that tells angular to look for the dependency on the component itself
  // this control is unique to this TextInput
  constructor(@Self() public ngControl: NgControl) {
    // our TextInput is a part of ngControl, and we are assigning the TextInput to the ngControl.valueAccessor 
    this.ngControl.valueAccessor = this;

  }

  writeValue(obj: any): void {

  }

  registerOnChange(fn: any): void {

  }

  registerOnTouched(fn: any): void {

  }

  get control(): FormControl{
    return this.ngControl.control as FormControl
  }

}
