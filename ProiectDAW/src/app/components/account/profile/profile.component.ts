import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/shared/user';
import { AuthService } from './../../../shared/auth.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  currentUser: User;

  constructor(
    public authService: AuthService,
    private actRoute: ActivatedRoute
  ) {
    this.authService.getUserProfile().subscribe(res => {
      this.currentUser = res.msg;
    })
  }

  ngOnInit() { }
}
