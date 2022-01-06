import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  
  formGroup: FormGroup;
  error = ''

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
      emailAddress: new FormControl('', [Validators.required]),
      password: new FormControl('', [Validators.required]),
    });
  }

  get formData() { return this.formGroup.controls; }

  register() {
    if (this.formGroup.invalid) {
      return;
    }
    
    this.authService.register(this.formData['username'].value, this.formData['emailAddress'].value, this.formData['password'].value)
      .subscribe((response:any) => {
          if(response) {
            this.toastr.clear()
            this.toastr.success("User registered successfully");
            this.router.navigate(['/login']);
          }
        },
        error => {
          this.error = error.error;
        });
  }

  login() {
    this.router.navigate(['/login']);
  }
}
