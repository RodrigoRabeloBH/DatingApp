import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-error',
  templateUrl: './error.component.html',
  styleUrls: ['./error.component.css']
})
export class ErrorComponent implements OnInit {
  baseUurl = environment.url;
  validationErrors: string[] = [];

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
  }

  getNotFound() {
    return this.http.get(this.baseUurl + 'buggy/not-found')
      .subscribe((res) => {
        console.log(res);
      }, error => {
        console.log(error);
      })
  }

  getServerError() {
    return this.http.get(this.baseUurl + 'buggy/server-error')
      .subscribe((res) => {
        console.log(res);
      }, error => {
        console.log(error);
      })
  }

  getBadRequest() {
    return this.http.get(this.baseUurl + 'buggy/bad-request')
      .subscribe((res) => {
        console.log(res);
      }, error => {
        console.log(error);
      })
  }

  getBadRequestValidation() {
    return this.http.post(this.baseUurl + 'account/register', {})
      .subscribe((res) => {
        console.log(res);
      }, (error) => {
        console.log(error);
        this.validationErrors = error
      })
  }

  getUnauthorized() {
    return this.http.get(this.baseUurl + 'buggy/auth')
      .subscribe((res) => {
        console.log(res);
      }, error => {
        console.log(error);
      })
  }
}
