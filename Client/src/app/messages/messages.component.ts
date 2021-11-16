import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Message } from '../model/message';
import { Pagination } from '../model/pagination';
import { MessageService } from '../services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {

  messages: Message[] = [];
  pagination!: Pagination;
  container = 'Unread';
  pageNumber = 1;
  pageSize = 5;
  loading = false;

  constructor(private service: MessageService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.loadMessages();
  }

  deleteMessage(id: number) {
    this.service.deleteMessage(id)
      .subscribe(() => {
        this.toastr.success('Message deleted!');
      })

  }

  loadMessages() {
    this.loading = true;
    this.service.getMessages(this.pageNumber, this.pageSize, this.container)
      .subscribe((res) => {
        this.messages = res.result;
        this.pagination = res.pagination;
        this.loading = false;
      });
  }

  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadMessages();
    }
  }
}
