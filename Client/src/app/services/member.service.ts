import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../model/member';
import { User } from '../model/user';
import { USerParams } from '../model/userParams';
import { AccountService } from './account.service';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';


@Injectable({
  providedIn: 'root'
})
export class MemberService {
  baseUrl = environment.url + "users/";
  members: Member[] = [];
  memberCache = new Map();
  user: User | any;
  userParams!: USerParams;

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      this.userParams = new USerParams(user);
    });
  }

  getUserParams() {
    return this.userParams;
  }

  setUserParams(params: USerParams) {
    this.userParams = params;
  }

  resetUserParams() {
    this.userParams = new USerParams(this.user);
    return this.userParams;
  }

  getMembers(userParams: USerParams) {

    var response = this.memberCache.get(Object.values(userParams).join('-'));

    if (response) {
      return of(response);
    }

    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return getPaginatedResult<Member[]>(this.baseUrl + 'all', params, this.http)
      .pipe(map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      }))
  }


  getMemberByName(name: string): Observable<Member> {

    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((member: Member) => member.userName === name);

    if (member) {
      return of(member);
    }
    return this.http.get<Member>(this.baseUrl + name);
  }

  updateMember(member: any) {
    return this.http.put(this.baseUrl, member);
  }

  removeUser() {
    return this.http.delete(this.baseUrl);
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'set-main-photo/' + photoId, {});
  }

  removePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'remove-photo/' + photoId);
  }

  addLike(username: string) {
    return this.http.post(this.baseUrl.replace('users/', 'likes/') + username, {});
  }

  getLikes(predicate: string, pageNumber: number, pageSize: number) {

    let params = getPaginationHeaders(pageNumber, pageSize);

    params = params.append('predicate', predicate);

    return getPaginatedResult<Partial<Member[]>>(this.baseUrl.replace('users/', 'likes'), params, this.http);
  }
}
