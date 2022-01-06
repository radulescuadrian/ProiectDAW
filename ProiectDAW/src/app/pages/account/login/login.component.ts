import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthService } from 'src/app/services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  formGroup: FormGroup;
  error = '';

  constructor(
    private router: Router,
    private authService: AuthService,
    private toastr: ToastrService) { }

  ngOnInit(): void {
    if(localStorage.getItem('Token')){
      this.router.navigate(['/']);
    }

    this.formGroup = new FormGroup({
      username: new FormControl('', [Validators.required]),
      password: new FormControl('', [Validators.required]),
    });
  }

  // convenience getter for easy access to form fields
  get formData() { return this.formGroup.controls; }

  login() {
    // stop here if form is invalid
    if (this.formGroup.invalid) {
      return;
    }
    
    this.authService.login(this.formData['username'].value, this.formData['password'].value)
      .subscribe((response:any) => {
        if(response && response.token) {
          this.toastr.clear()
          this.toastr.success("Login successfull");
          localStorage.setItem('Token', JSON.stringify(response.token));
          
          let tokenData = response.token.split('.')[1]
          let decodedTokenData = JSON.parse(window.atob(tokenData))
          localStorage.setItem('Role', decodedTokenData["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]);

          this.router.navigate(['/']);
        }
      },
      error => {
        this.error = error.error;
      });
  }

  register() {
    this.router.navigate(['/register']);
  }
}
