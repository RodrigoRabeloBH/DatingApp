import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Message } from '../model/message';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baseUrl = environment.url + 'messages';

  constructor(private http: HttpClient) { }

  getMessages(pageNumber: number, pageSize: number, container: string) {

    let params = getPaginationHeaders(pageNumber, pageSize);

    params = params.append('Container', container);

    return getPaginatedResult<Message[]>(this.baseUrl, params, this.http);
  }

  getMessageThread(pageNumber: number, pageSize: number, username: string) {

    let params = getPaginationHeaders(pageNumber, pageSize);

    params = params.append('RecipientUsername', username);

    return getPaginatedResult<Message[]>(this.baseUrl + '/thread', params, this.http);
  }

  sendMessage(recipientUsername: string, content: string) {
    return this.http.post<Message>(this.baseUrl, { recipientUsername, content });
  }

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + '/' + id);
  }
}
