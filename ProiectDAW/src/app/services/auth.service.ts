import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http:HttpClient) { }

  login(username: string, password: string) {
    return this.http.post<any>(`${environment.baseUrl}/Account/Login`, { username, password })
  }

  register(username: string, emailAddress: string, password: string) {
    return this.http.post<any>(`${environment.baseUrl}/Account/Register`, { username, emailAddress, password })
  }
  
  logout() {
    // remove user from local storage to log user out
    localStorage.removeItem('Token');
  }
}
