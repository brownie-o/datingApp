import { Component, inject } from '@angular/core';
import { Nav } from "../layout/nav/nav";
import { Router, RouterOutlet } from '@angular/router';

// The @Component decorator transforms the App class into an Angular component.
@Component({
  selector: 'app-root', // specifies what element selector in index.html. e.g. <app-root> 
  imports: [Nav, RouterOutlet], // specifies what to import
  templateUrl: './app.html', // specifies what template to use
  styleUrl: './app.css' // specifies which styles to apply
})

export class App {
  protected router = inject(Router)



  // should reference using this. for the property in class

  //if using subscribe, will need to unsubscribe to avoid memory leaks
  // ngOnInit(): void {
  //   this.http.get("https://localhost:5001/api/members").subscribe({
  //     next: response => this.members.set(response),
  //     error: error => console.log(error),
  //     complete: () => console.log("Completed the http request")
  //   });
  // }
}
