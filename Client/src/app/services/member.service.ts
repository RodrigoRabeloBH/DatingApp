import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../model/member';
import { PaginatedResult } from '../model/pagination';
import { Photo } from '../model/photo';


@Injectable({
  providedIn: 'root'
})
export class MemberService {
  baseUrl = environment.url + "users/"
  paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();


  constructor(private http: HttpClient) { }

  getMembers(page?: number, itemsPerPage?: number) {

    let params = new HttpParams();

    if (page !== null && itemsPerPage !== null) {
      params = params.append('pageNumber', page?.toString()!);
      params = params.append('pageSize', itemsPerPage?.toString()!);
    }

    return this.http.get<Member[]>(this.baseUrl + 'all', { observe: 'response', params })
      .pipe(map(response => {
        this.paginatedResult.result = response.body;
        if (response.headers.get('Pagination') !== null) {
          this.paginatedResult.pagination = JSON.parse(response.headers.get('Pagination')!);
        }
        return this.paginatedResult;
      }));
  }

  getMemberByName(name: string): Observable<Member> {
    return this.http.get<Member>(this.baseUrl + name);
  }

  updateMember(member: any) {
    return this.http.put(this.baseUrl, member);
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'set-main-photo/' + photoId, {});
  }

  removePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'remove-photo/' + photoId);
  }
}
