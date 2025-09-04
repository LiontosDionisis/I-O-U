import { Component, inject, signal, ElementRef, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { NgIf, NgFor } from '@angular/common';
import { AuthService } from '../services/auth.service';
import { RouterLink, RouterOutlet } from '@angular/router';
import { UserService } from '../services/user.service.service';
import { Observable } from 'rxjs';
import { UserNotification } from '../Models/notification';
import { SessionNotification } from '../Models/notificationSession';

interface UserDTO {
  id: number;
  username: string;
  email: string;
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [NgIf, NgFor, RouterLink, RouterOutlet],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  private el = inject(ElementRef);

  searchQuery = signal('');
  users = signal<UserDTO[]>([]);
  loading = signal(false);
  errorMessage = signal('');
  successMessage = signal('');
  notifications: UserNotification[] = [];
  sessionNotifications: SessionNotification[] = [];
  loadingNotifications = false;
  errorNotifications: string | null = null;

  constructor(private userService: UserService) {}
  ngOnInit() {
    window.addEventListener('click', this.handleClickOutside.bind(this));
    this.loadNotifications();
    this.loadSessionNotifications();
  }

  handleClickOutside(event: Event) {
    const searchContainer = this.el.nativeElement.querySelector('.search-container');
    if (searchContainer && !searchContainer.contains(event.target)) {
      this.users.set([]);
      this.errorMessage.set('');
      this.successMessage.set('');
    }
  }

  updateQuery(value: string) {
    this.searchQuery.set(value);
    this.errorMessage.set('');
    this.successMessage.set('');

    if (value.trim() === '') {
      this.users.set([]);
      return;
    }

    this.searchUsers();
  }

  searchUsers() {
    const token = localStorage.getItem('token');
    if (!token) {
      this.errorMessage.set('You must be logged in to search users.');
      return;
    }

    this.loading.set(true);
    this.errorMessage.set('');
    this.users.set([]);

    this.http.get<UserDTO>(`http://localhost:5062/api/user/byUsername/${this.searchQuery()}`, {
      headers: new HttpHeaders({ Authorization: `Bearer ${token}` })
    }).subscribe({
      next: (res) => {
        const userArray = res ? [res] : [];
        const currentUsername = this.authService.getCurrentUsernameFromToken();
        const filtered = userArray.filter(u => u.username !== currentUsername);
        this.users.set(filtered);

        if (filtered.length === 0) this.errorMessage.set('No users found.');
      },
      error: (err) => {
        if (err.status === 404) {
          this.users.set([]);
          this.errorMessage.set('No users found.');
        } else {
          console.error(err);
          this.errorMessage.set('Failed to fetch users. Please try again.');
        }
      },
      complete: () => this.loading.set(false)
    });
  }

  isFriendRequest(message: string | undefined): boolean {
    return !!message && /.+ wants to be your friend/.test(message);
  }

  sendFriendRequest(userId: number) {
    const token = localStorage.getItem('token');
    if (!token) return;

    this.http.post(`http://localhost:5062/api/friendship/${userId}`, {}, {
      headers: new HttpHeaders({ Authorization: `Bearer ${token}` })
    }).subscribe({
      next: () => this.successMessage.set('Friend request sent!'),
      error: (err) => this.errorMessage.set(err.error?.message || 'Failed to send friend request.')
    });
  }

  logoutFunc(){
    this.authService.logout();
  }

   loadNotifications() {
    this.loadingNotifications = true;

    this.userService.getNotifications().subscribe({
      next: (data: UserNotification[]) => {
        this.notifications = data;
        this.loadingNotifications = false;
      },
      error: (err: any) => {
        this.errorNotifications = err.error?.message || "Failed to load notifications";
        this.loadingNotifications = false;
      }
    });
  }

  loadSessionNotifications() {
    this.userService.getSessionNotifications().subscribe({
      next: (data: SessionNotification[]) => {
        this.sessionNotifications = data;
        this.loadingNotifications = false;
      },
      error: (err: any) => {
        this.errorNotifications = err.error?.message || "Failed to load notifications";
        this.loadingNotifications = false;
      }
    });
  }

  acceptFriendRequest(friendshipId: number, notificationId: number) {
    this.userService.acceptFriendRequest(friendshipId).subscribe({
      next: () => {
        this.deleteNotification(notificationId);
        this.loadingNotifications = false;
      },
      error: (err: any) => {
        this.loadingNotifications = false;
      }
    })
  }
  denyFriendRequest(friendshipId: number, notificationId: number) {
    this.userService.denyFriendRequest(friendshipId).subscribe({
      next: () => {
        this.deleteNotification(notificationId);
        this.loadingNotifications = false;
      },
      error: (err: any) => {
        this.loadingNotifications = false;
      }
    });
  }

  deleteNotification(notificationId: number) {
    this.userService.deleteNotification(notificationId).subscribe({
      next: () => {
        this.loadingNotifications = false;
        this.loadNotifications();
      },
      error: (err: any) => {
        this.loadingNotifications = false;
      }
    });
  }
  
  deleteSessionNotification(notificationId: number) {
    this.userService.deleteSessionNotification(notificationId).subscribe({
      next: () => {
        this.loadingNotifications = false;
        this.loadSessionNotifications();
      },
      error: (err: any) => {
        this.loadingNotifications = false;
      }
    });
  }
}
