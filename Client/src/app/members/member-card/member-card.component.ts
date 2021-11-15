import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/model/member';
import { MemberService } from 'src/app/services/member.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {

  @Input() member: Member | any

  constructor(private service: MemberService, private toastrs: ToastrService) { }

  ngOnInit(): void {

  }

  addLike(member: Member) {
    console.log(member);
    this.service.addLike(member.userName)
      .subscribe(() => {
        this.toastrs.success('You have liked ' + member.knownAs);
      })
  }
}
