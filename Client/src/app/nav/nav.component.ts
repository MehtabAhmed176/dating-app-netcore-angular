import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../_models/user';
import { Route, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {}

  // inject Account services
  constructor(public accountService: AccountService, private router: Router, private toastr: ToastrService) { }

  ngOnInit(): void {

  }

  login() {
    this.accountService.login(this.model).subscribe({
      next: resposne => {
        console.log('login resposne', resposne)
        this.router.navigateByUrl('/members')
      },
      error: err => {
        console.error('error in account request', err)
        this.toastr.error(err.error);
      }

    })
    console.log("model is", this.model)

  }

  logout() {
    this.accountService.logout()
    this.router.navigateByUrl('/')
  }
}
