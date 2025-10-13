import { Component, OnInit } from '@angular/core';
import { Session } from '../Models/session';
import { SessionServiceService } from '../services/session.service.service';
import { CommonModule, NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FriendshipDTO } from '../Models/friendshipDto';
import { UserService } from '../services/user.service.service';
import { ExpenseService } from '../services/expense.service';
import { ExpenseSplitDto } from '../Models/expenseSplitDto';
import { AddExpenseDto } from '../Models/AddExpenseDto';
import { ExpenseDto } from '../Models/expenseDto';
import { RouterLink, RouterOutlet } from '@angular/router';
import { trigger, state, style, animate, transition } from '@angular/animations';
import { AvatarType, getAvatarUrl } from '../Models/avatar';
import { SessionUser } from '../Models/sessionUser';
import { SignalrService } from '../services/signalr.service';



@Component({
  selector: 'app-sessions',
  standalone: true,
  imports: [NgFor, NgIf, CommonModule, FormsModule, RouterLink, RouterOutlet],
  templateUrl: './sessions.component.html',
  styleUrl: './sessions.component.css',
  animations: [
    trigger('expandCollapse', [
      state('open', style({
        height: '*',
        opacity: 1,
        padding: '*',
        marginTop: '*'
      })),
      state('closed', style({
        height: '0px',
        opacity: 0,
        padding: '0px',
        marginTop: '0px'
      })),
      transition('closed => open', [
        animate('300ms ease-out')
      ]),
      transition('open => closed', [
        animate('300ms ease-in')
      ])
    ])
  ]
})
export class SessionsComponent implements OnInit {
  sessions: Session[] = [];
  friends : FriendshipDTO[] = [];
  loading = true;
  error: string | null = null;
  showCreateForm = false;
  newSessionName = '';
  friendMenuOpen: number | null = null;
  expenseFormOpen: number | null = null;
  newExpense = {
    description: "",
    totalAmount: 0,
    isEqualSplit: true,
    paidById: 0
  };
  customSplits: {[userId: number]: number} = {};
  expenses: { [sessionId: number]: ExpenseDto[] } = {};



  

  constructor(private sessionService: SessionServiceService, private userService: UserService, private expenseService: ExpenseService, private signalrService: SignalrService){

  }

  

  
  ngOnInit(): void {
    this.signalrService.startConnection();
    this.loadSessions();
    this.loadFriends();

    this.signalrService.sessionDeleted$.subscribe(sessionId => {
      if (!sessionId) return;

      this.sessions = this.sessions.filter(s => s.id !== sessionId);
      delete this.expenses[sessionId];
    });
    
    this.signalrService.expenseCreated$.subscribe(expense => {
      if (!expense) return;

      this.expenses = {
        ...this.expenses,
        [expense.sessionId]: [...(this.expenses[expense.sessionId] || []), expense]
      };
    });
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
        this.sessions.forEach(session => {
          session.participants.forEach((user: SessionUser) => {
            user.avatarUrl = getAvatarUrl(user.avatar);
          })
        })
        this.loading = false;
        this.sessions.forEach(session => {
          this.loadExpenses(session.id)
        });
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
      const session = this.sessions.find(s => s.id === sessionId); 
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


  toggleExpenseForm(sessionId: number){
    this.expenseFormOpen = this.expenseFormOpen === sessionId ? null : sessionId;
    this.newExpense = {
      description: '',
      totalAmount: 0,
      isEqualSplit: true,
      paidById: this.getCurrentUserId()
    };
    this.customSplits = {};
  }

  createExpense(sessionId: number){
    let splits: ExpenseSplitDto[] | undefined = undefined;

    if (!this.newExpense.isEqualSplit) {
      splits = Object.entries(this.customSplits).map(([userId, amount]) => ({
        userId: Number(userId),
        userUsername: '',
        amount: Number(amount),
        status: 0
      }));
    }

    const dto: AddExpenseDto = {
      sessionId,
      description: this.newExpense.description,
      totalAmount: this.newExpense.totalAmount,
      paidById: this.newExpense.paidById,
      isEqualSplit: this.newExpense.isEqualSplit,
      customSplits: splits
    };

    this.expenseService.createExpense(dto).subscribe({
      next: (res) => {
        console.log("Expense created", res);
        this.expenseFormOpen = null;
      },
      error: (err) => {
        console.log("Failed to create expense", err);
        alert(err.error?.message || "Something went wrong.");
      }
    })
  }


  private getCurrentUserId(): number {
    const token = localStorage.getItem('token');
    if (!token) throw new Error('Token not found');

    const payload = JSON.parse(atob(token.split('.')[1]));
    const userId = payload['nameid'] || payload['sub'];
    if (!userId) throw new Error('User ID not found in token');

    return Number(userId);
  }

  loadExpenses(sessionId: number) {
    this.expenseService.getExpensesBySession(sessionId).subscribe({
      next: (res) => {
        this.expenses[sessionId] = res;
      },
      error: (err) => {
        console.error("Failed to load expenses", err)
      }
    })
  }



  getUsername(session: Session, userId: number): string {
    const participant = session.participants.find(p => p.userId === userId);
    return participant ? participant.username : 'Unknown';
  }

  // Removes payer from expense list
  // getSplitsForDisplay(expense: ExpenseDto, session: Session): ExpenseSplitDto[] {
  //   if (!expense.splits) return [];

  //   return expense.splits.filter(s => s.userId !== expense.paidById);
  // }

  getSplitsForDisplay(expense: ExpenseDto, session: Session): ExpenseSplitDto[] {
  if (!expense.splits) return [];
  const currentUserId = this.getCurrentUserId();
  // show all splits owed to me if I am the payer
  if (expense.paidById === currentUserId) {
    return expense.splits.filter(s => s.userId !== currentUserId);
  }
  return expense.splits.filter(s => s.userId !== expense.paidById);
}
  


  deleteSession(sessionid: number) {
    if(confirm("Are you sure you want to delete this session?")){
      this.sessionService.deleteSession(sessionid).subscribe({
      next: (res) => {
        console.log("Session deleted successfully");
      },
      error: (err) => {
        console.error("Error deleting session", err.status, err.message);
      }
    })
    }
    
  }

  getUserAvatar(user: any): string {
    const avatarNumber = typeof user.avatar === 'number'
      ? user.avatar
      : AvatarType[user.avatar as keyof typeof AvatarType];
    return getAvatarUrl(avatarNumber);
  }
settleSplit(expenseId: number) {
  this.expenseService.settleSplit(expenseId).subscribe({
    next: (res) => {
      console.log("Expense settled.", res);

      // Find the expense in any session
      const sessionExpenses = Object.values(this.expenses).find(expenses =>
        expenses.some(e => e.id === expenseId)
      );

      if (!sessionExpenses) return;

      const expense = sessionExpenses.find(e => e.id === expenseId);
      if (!expense || !expense.splits) return;

      // Update the split for the user returned by the backend
      const splitIndex = expense.splits.findIndex(s => s.userId === res.userId);
      if (splitIndex !== -1) {
        // Replace the object to trigger Angular change detection
        expense.splits[splitIndex] = { ...expense.splits[splitIndex], status: res.status };

        // Reassign the array reference so *ngFor notices the change
        expense.splits = [...expense.splits];
      }
    },
    error: (err) => {
      console.log("Error while settling split.", err);
    }
  });
}




  getAvatarUrl = getAvatarUrl;
}
