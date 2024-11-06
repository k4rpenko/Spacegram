// FindPeopleComponent
import { Component, inject, ViewChild } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { FindUserData } from '../../../data/HTTP/GetPosts/User/FindUserData.service';
import { User } from '../../../data/interface/Users/User.interface';
import { CommonModule } from '@angular/common';
import { CookieService } from 'ngx-cookie-service';
import { WebSocketService } from '../../../data/HTTP/WEB/WebSocket.service';
import { MessageComponent } from '../../../pages/message/message.component';
import { ChatModel } from '../../../data/interface/Chats/ChatModel';

@Component({
  selector: 'app-find-people',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './find-people.component.html',
  styleUrls: ['./find-people.component.scss']
})
export class FindPeopleComponent {
  profileService = inject(FindUserData);
  User: User[] = [];
  inputNick: string = ''; 
  id!: string;

  constructor(public dialog: MatDialog, public dialogRef: MatDialogRef<FindPeopleComponent>, private cookieService: CookieService, private WS: WebSocketService) {  
    this.id = this.cookieService.get('authToken')
  }

  onClose(): void {
    this.dialogRef.close();
  }

  CreatChat(Id: string) {
    const AddUsersIdChat: Array<string> = [this.id, Id];
    const ChatModel: ChatModel = {
      AddUsersIdChat: AddUsersIdChat
    }
    this.WS.CreatChat(ChatModel);
    this.dialogRef.close(this.inputNick);
}

  onInputChange(event: Event): void {
    const nick = (event.target as HTMLInputElement).value;
    this.inputNick = nick;
    this.profileService.FindUserData(nick).subscribe({
      next: (response) => {
        this.User = response.user;
      },
      error: (err) => {
        console.error(err);
      }
    });
  }
}