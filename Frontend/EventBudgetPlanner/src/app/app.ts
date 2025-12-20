import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { environment } from './environments/environment';
import { NavbarComponent } from './shared/components/navbar/navbar';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet , NavbarComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('EventBudgetPlanner');
  constructor(){
    console.log('API Base URL :',environment.apiBaseUrl);
    console.log('Production :',environment.production);
  }
}
