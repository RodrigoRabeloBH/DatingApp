import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/model/member';
import { Pagination } from 'src/app/model/pagination';
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
  pageNumber = 1;
  pageSize = 12;

  constructor(private service: MemberService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.getMembers();
  }

  getMembers() {
    this.service.getMembers(this.pageNumber, this.pageSize)
      .subscribe((res: any) => {
        this.members = res.result;
        this.pagination = res.pagination
      }, (error) => {
        this.toastr.error(error.message);
      });
  }

  pageChanged(event: any) {
    this.pageNumber = event.page;
    this.getMembers();
  }
}
