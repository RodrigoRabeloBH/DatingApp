import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Message } from 'src/app/model/message';
import { MessageService } from 'src/app/services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm: NgForm;
  @Input() messages: Message[] = [];
  @Input() username!: string;
  messageContent: string;

  constructor(private service: MessageService, private toastr: ToastrService) { }

  ngOnInit(): void {

  }

  sendMessage() {
    this.service.sendMessage(this.username, this.messageContent)
      .subscribe((res) => {  
        this.messages.push(res);
        this.messageForm.reset();
      });
  }
}
