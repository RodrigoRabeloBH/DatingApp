import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/model/member';
import { MemberService } from 'src/app/services/member.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {

  member: Member | undefined;
  galleryOptions: NgxGalleryOptions[] = [];
  galleryImages: NgxGalleryImage[] = [];

  constructor(private service: MemberService, private toastr: ToastrService, private route: ActivatedRoute) { }

  ngOnInit(): void {

    this.loadMember();

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


  loadMember() {
    this.service.getMemberByName(this.route.snapshot.paramMap.get('username')!)
      .subscribe((res) => {
        this.member = res;
        const imageUrls = [];
        for (const photo of res.photos) {
          imageUrls.push({
            small: photo?.url,
            medium: photo?.url,
            big: photo?.url,
          }
          )
        }
        this.galleryImages = imageUrls;
      }, (error) => {
        this.toastr.error(error.message);
      });
  }

  addLike(member: Member) {
    console.log(member);
    this.service.addLike(member.userName)
      .subscribe(() => {
        this.toastr.success('You have liked ' + member.knownAs);
      })
  }
}
