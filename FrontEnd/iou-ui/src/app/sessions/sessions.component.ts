import { Component, OnInit } from '@angular/core';
import { Session } from '../Models/session';
import { SessionServiceService } from '../services/session.service.service';
import { CommonModule, NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';


@Component({
  selector: 'app-sessions',
  standalone: true,
  imports: [NgFor, NgIf, CommonModule, FormsModule],
  templateUrl: './sessions.component.html',
  styleUrl: './sessions.component.css'
})
export class SessionsComponent implements OnInit {
  sessions: Session[] = [];
  loading = true;
  error: string | null = null;
  showCreateForm = false;
  newSessionName = '';

  

  constructor(private sessionService: SessionServiceService){

  }

  ngOnInit(): void {
    this.loadSessions();
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
}
