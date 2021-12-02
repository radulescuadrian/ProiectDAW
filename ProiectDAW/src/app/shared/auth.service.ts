import { Injectable } from '@angular/core';
import { User } from './user';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})

export class AuthService {
    baseUrl: string = environment.baseUrl;
    headers = new HttpHeaders().set('Content-Type', 'application/json');
    currentUser = {};

    constructor(
      private http: HttpClient,
      public router: Router
    ) {
  }

  // Register
  register(user: User): Observable<any> {
    let url = `${this.baseUrl}/Account/Register`;
    return this.http.post(url, user)
      .pipe(
        catchError(this.handleError)
      )
  }

  // Login
  login(user: User) {
    return this.http.post<any>(`${this.baseUrl}/Account/Login`, user)
      .subscribe((res: any) => {
        localStorage.setItem('access_token', res.token)
        this.getUserProfile().subscribe((res) => {
          this.currentUser = res;
          this.router.navigate(['profile']);
        })
      })
  }

  getToken() {
    return localStorage.getItem('access_token');
  }

  get isLoggedIn(): boolean {
    let authToken = localStorage.getItem('access_token');
    return (authToken !== null) ? true : false;
  }

  logout() {
    let removeToken = localStorage.removeItem('access_token');
    if (removeToken == null) {
      this.router.navigate(['login']);
    }
  }

  // User profile
  getUserProfile() {
    let url = `${this.baseUrl}/Account/GetUserDetails`;
    return this.http.get(url, { headers: this.headers }).pipe(
      map((res: any) => {
        return res || {}
      }),
      catchError(this.handleError)
    )
  }

  // Error 
  handleError(error: HttpErrorResponse) {
    let msg = '';
    if (error.error instanceof ErrorEvent) {
      // client-side error
      msg = error.error.message;
    } else {
      // server-side error
      msg = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    return throwError(msg);
  }
}