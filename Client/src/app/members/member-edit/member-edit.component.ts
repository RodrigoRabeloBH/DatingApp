import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/model/member';
import { AccountService } from 'src/app/services/account.service';
import { MemberService } from 'src/app/services/member.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {

  @ViewChild('editForm') editForm: NgForm | any;

  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  member: Member | undefined;
  user: any;

  constructor(private service: MemberService, private accountService: AccountService, private toastr: ToastrService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user)
  }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    this.service.getMemberByName(this.user.username)
      .subscribe((res) => {
        this.member = res;
      }, (error) => {
        this.toastr.error(error.message);
      });
  }

  updateMember() {
    this.service.updateMember(this.member).subscribe((res) => {
      this.toastr.success('Profile updated successfully')
      this.editForm.reset(this.member);
    }, (error) => {
      this.toastr.error(error.message);
    });
  }
}
