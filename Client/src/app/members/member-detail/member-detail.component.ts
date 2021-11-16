import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/model/member';
import { Message } from 'src/app/model/message';
import { Pagination } from 'src/app/model/pagination';
import { MemberService } from 'src/app/services/member.service';
import { MessageService } from 'src/app/services/message.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {

  @ViewChild('memberTabs', { static: true }) memberTabs: TabsetComponent;

  member: Member;
  galleryOptions: NgxGalleryOptions[] = [];
  galleryImages: NgxGalleryImage[] = [];
  activeTab: TabDirective;
  messages: Message[] = [];
  pageNumber = 1;
  pageSize = 5;
  pagination: Pagination;

  constructor(private service: MemberService, private toastr: ToastrService,
    private route: ActivatedRoute, private messageServices: MessageService) { }

  ngOnInit(): void {

    this.route.data.subscribe(data => {
      this.member = data.member;

      const imageUrls = [];
      for (const photo of this.member.photos) {
        imageUrls.push({
          small: photo?.url,
          medium: photo?.url,
          big: photo?.url,
        }
        )
      }
      this.galleryImages = imageUrls;
    })

    this.route.queryParams.subscribe((params) => {
      params.tab ? this.selectTab(params.tab) : this.selectTab(0);
    })

    this.galleryOptions = [
      {
        width: '30rem',
        height: '40rem',
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ];
  }

  loadMessages() {
    this.messageServices.getMessageThread(this.pageNumber, this.pageSize, this.member.userName)
      .subscribe((res) => {
        this.messages = res.result;
        this.pagination = res.pagination;
      });
  }

  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }

  onTabActivated(dada: TabDirective) {
    this.activeTab = dada;
    if (this.activeTab.heading === 'Messages' && this.messages.length === 0) {
      this.loadMessages();
    }
  }

  addLike(member: Member) {
    console.log(member);
    this.service.addLike(member.userName)
      .subscribe(() => {
        this.toastr.success('You have liked ' + member.knownAs);
      })
  }

  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadMessages();
    }
  }
}
