import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'dockerui';
  users: any;
  error: any;
  constructor(private http: HttpClient) {}
  ngOnInit(): void {
    this.http.get(environment.api + '/api/u/users').subscribe(x => {
      this.users = x;
    }, e => this.error = e.message);
  }
}
