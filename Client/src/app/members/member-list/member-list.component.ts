import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/model/member';
import { Pagination } from 'src/app/model/pagination';
import { User } from 'src/app/model/user';
import { USerParams } from 'src/app/model/userParams';
import { MemberService } from 'src/app/services/member.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  members: Member[] = [];
  member: Member | undefined;
  pagination: Pagination | any;
  user: User | any;
  userParams!: USerParams;

  genderList = [
    { value: 'male', display: 'Males' },
    { value: 'female', display: 'Females' },
  ]


  constructor(private service: MemberService, private toastr: ToastrService) {
    this.userParams = this.service.getUserParams();
  }

  ngOnInit(): void {
    this.getMembers();
  }

  getMembers() {

    this.service.setUserParams(this.userParams);

    this.service.getMembers(this.userParams)
      .subscribe((res: any) => {
        this.members = res.result;
        this.pagination = res.pagination
      }, (error) => {
        this.toastr.error(error.message);
      });
  }

  pageChanged(event: any) {
    this.userParams.pageNumber = event.page;
    this.service.setUserParams(this.userParams);
    this.getMembers();
  }

  resetFilters() {
    this.userParams = this.service.resetUserParams();
    this.getMembers();
  }
}
