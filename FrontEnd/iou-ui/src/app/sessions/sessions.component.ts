import { Component, OnInit } from '@angular/core';
import { Session } from '../Models/session';
import { SessionServiceService } from '../services/session.service.service';
import { CommonModule, NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FriendshipDTO } from '../Models/friendshipDto';
import { UserService } from '../services/user.service.service';


@Component({
  selector: 'app-sessions',
  standalone: true,
  imports: [NgFor, NgIf, CommonModule, FormsModule],
  templateUrl: './sessions.component.html',
  styleUrl: './sessions.component.css'
})
export class SessionsComponent implements OnInit {
  sessions: Session[] = [];
  friends : FriendshipDTO[] = [];
  loading = true;
  error: string | null = null;
  showCreateForm = false;
  newSessionName = '';
  friendMenuOpen: number | null = null;

  

  constructor(private sessionService: SessionServiceService, private userService: UserService){

  }

  

  
  ngOnInit(): void {
    this.loadSessions();
    this.loadFriends();
  }

  loadFriends() {
    this.userService.getFriends().subscribe(f => {
      this.friends = f;
    });
  }

  toggleFriendMenu(sessionId: number) {
    this.friendMenuOpen = this.friendMenuOpen === sessionId ? null : sessionId;
  }
  
  addFriendToSession(sessionId: number, friendId: number) {
  this.userService.addFriendToSession(sessionId, friendId).subscribe({
    next: (res: any) => {
      console.log(res.message);
      this.friendMenuOpen = null; 
      this.loadSessions();        
    },
    error: (err) => {
      console.error('Failed to add friend', err);
      alert(err.error?.message || 'Something went wrong');
    }
  });
}


  loadSessions(): void {
    this.loading = true;
    this.sessionService.getSessions().subscribe({
      next: (data) => {
        this.sessions = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = err.error.message;
        this.loading = false;
      }
    });
  }

  toggleCreateSession(): void {
    this.showCreateForm = !this.showCreateForm;
    this.newSessionName = '';
  }

  createSession(): void {
    if (!this.newSessionName.trim()) return;
    

    this.sessionService.createSession(this.newSessionName).subscribe({
      next: () => {
        this.toggleCreateSession();
        this.loadSessions();
      },
      error: (err) => {
        this.error = "failed to create session";
      }
    });
  }

  removeParticipant(sessionId: number, userId: number) {
  this.userService.removeUserFromSession(sessionId, userId).subscribe({
    next: (res: any) => {
      console.log(res.message);
      // Update participants locally
      const session = this.sessions.find(s => s.id === sessionId); // <- use 'id'
      if (session) {
        session.participants = session.participants.filter(u => u.userId !== userId);
      }
    },
    error: (err) => {
      console.error('Failed to remove participant', err);
      alert(err.error || 'Something went wrong');
    }
  });
}




}
