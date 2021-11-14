import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { User } from '../model/user';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {}
  currentUser$: Observable<User> | undefined;

  constructor(private accountServices: AccountService, private router: Router, private toastrService: ToastrService) { }

  ngOnInit(): void {
    this.currentUser$ = this.accountServices.currentUser$;
  }

  login() {
    this.accountServices.login(this.model)
      .subscribe(() => {
        this.router.navigateByUrl('/members');
        this.toastrService.success('Logged in!');
      });
  }

  logout() {
    this.accountServices.logout();
    this.router.navigateByUrl('/');
    this.toastrService.warning('Logged out!');
    window.location.reload();
  }
}
