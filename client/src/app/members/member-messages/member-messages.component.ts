import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  Input,
  OnInit,
  ViewChild,
} from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  standalone: true,
  imports: [CommonModule, TimeagoModule, FormsModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MemberMessagesComponent implements OnInit {
  @Input() userName?: string;
  @ViewChild('messageForm') messageForm?: NgForm;
  messageContent = '';

  constructor(public messageService: MessageService) {}

  ngOnInit(): void {}

  sendMessage() {
    if (!this.userName) return;

    this.messageService
      .sendMessage(this.userName, this.messageContent)
      .then((_) => {
        this.messageForm?.reset();
      });
  }
}
