import { Component, OnInit } from '@angular/core';
import { Member } from '../model/member';
import { Pagination } from '../model/pagination';
import { MemberService } from '../services/member.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {

  members!: Partial<Member[]>;
  predicate = 'liked';
  pageNumber = 1;
  pageSize = 6;
  pagination!: Pagination;

  constructor(private services: MemberService) { }

  ngOnInit(): void {
    this.loadLikes();
  }
  loadLikes() {
    this.services.getLikes(this.predicate, this.pageNumber, this.pageSize)
      .subscribe((res) => {
        this.members = res.result;
        this.pagination = res.pagination;
      })
  }

  pageChanged(event: any) {
    this.pageNumber = event.page;
    this.loadLikes();
  }


}
