import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
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
    private authService: AuthService) { }

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
          if(response == 'User created successfully')
            this.router.navigate(['/login']);
        },
        error => {
          console.log(JSON.stringify(error.error));
          
          this.error = error.error;
        });
  }

  login() {
    this.router.navigate(['/login']);
  }
}
