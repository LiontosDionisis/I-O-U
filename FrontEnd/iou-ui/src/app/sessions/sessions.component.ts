import { Component, OnInit } from '@angular/core';
import { Session } from '../Models/session';
import { SessionServiceService } from '../services/session.service.service';
import { CommonModule, NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-sessions',
  standalone: true,
  imports: [NgFor, NgIf, CommonModule],
  templateUrl: './sessions.component.html',
  styleUrl: './sessions.component.css'
})
export class SessionsComponent implements OnInit {
  sessions: Session[] = [];
  loading = true;
  error: string | null = null;

  constructor(private sessionService: SessionServiceService){

  }

  ngOnInit(): void {
    this.sessionService.getSessions().subscribe({
      next:(data) => {
        this.sessions = data,
        this.loading = false;
      },
      error: (err) => {
        this.error = err.error.message
        this.loading = false;
      }
    })
  }
}
