import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/model/member';
import { Photo } from 'src/app/model/photo';
import { User } from 'src/app/model/user';
import { AccountService } from 'src/app/services/account.service';
import { MemberService } from 'src/app/services/member.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {

  @Input() member: Member | any
  uploader: FileUploader | any;
  hasBaseDropZoneOver = false;
  baseUrl = environment.url;
  user: User | any;

  constructor(private accountService: AccountService, private service: MemberService, private toastr: ToastrService) {
    this.accountService.currentUser$.pipe(take(1))
      .subscribe(user => this.user = user);
  }

  ngOnInit(): void {
    this.initializeUploader();
  }
  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.user.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = (file: any) => {
      file.withCredentials = false;
    }

    this.uploader.onSuccessItem = (item: any, response: any, status: any, headers: any) => {
      if (response) {
        const photo: Photo = JSON.parse(response);
        this.member.photos.push(photo);
        console.log(response);
        if (photo.isMain) {
          this.user.photoUrl = photo.url;
          this.member.photoUrl = photo.url;
          this.accountService.setCurrentUser(this.user);
        }
      }
    }
  }

  setMainPhoto(photo: Photo) {
    this.service.setMainPhoto(photo.id)
      .subscribe(() => {
        this.user.photoUrl = photo.url;
        this.accountService.setCurrentUser(this.user);
        this.member.photoUrl = photo.url;
        this.toastr.success('Photo set at successfully');
        this.member.photos.forEach((p: Photo) => {
          if (p.isMain) p.isMain = false;
          if (p.id === photo.id) p.isMain = true;

        });
      }, (error) => {
        this.toastr.error(error.message);
      });
  }

  removePhoto(photo: Photo) {
    this.service.removePhoto(photo.id)
      .subscribe(() => {
        this.member.photos.pop(photo);
        this.toastr.success('Photo removed successfully');
      }, (error) => {
        this.toastr.error(error.message);
      });
  }
}
