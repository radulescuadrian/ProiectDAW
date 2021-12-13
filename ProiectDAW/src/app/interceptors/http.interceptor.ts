import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError} from 'rxjs/operators'
import { AuthService } from '../services/auth.service';

@Injectable()
export class httpInterceptor implements HttpInterceptor {

  constructor(private authService: AuthService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // add authorization header to request

    //Get Token data from local storage
    let tokenInfo = JSON.parse(localStorage.getItem('Token') || '{}');
    
    if (tokenInfo && tokenInfo.token) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${tokenInfo.token}`
        }
      });
    }

    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        // console.log('error status => '+error.status);
        // console.log('error error => '+error.error)

        if (error.status === 401) {
          //if 401 response returned from api, logout from application & redirect to login page.
          //redirect to another page
          this.authService.logout();
        }

        return throwError(() => error)
      }));
  }
}