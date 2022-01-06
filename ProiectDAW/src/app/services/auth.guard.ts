import { Injectable } from '@angular/core';
import { Router, ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(
    private router: Router,
    private toastr: ToastrService) { }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot) {

    //If token data exist, user may login to application
    if (localStorage.getItem('Token') && localStorage.getItem('Role')) {
      var role = localStorage.getItem('Role');
      if(route.data['role'] && role != route.data['role']) {
        this.toastr.clear()
        this.toastr.info("Unauthorized");
        return false;
      }

      return true;
    }

    // otherwise redirect to login page with the return url
    this.toastr.clear()
    this.toastr.info("Login required");
    this.router.navigate(['/login']);
    return false;
  }
}