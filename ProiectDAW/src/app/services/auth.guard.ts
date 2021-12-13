import { Injectable } from '@angular/core';
import { Router, ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private router: Router) { }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot) {

    //If token data exist, user may login to application
    if (localStorage.getItem('Token')) {
      return true;
    }

    // otherwise redirect to login page with the return url
    this.router.navigate(['/login']);
    return false;
  }
}