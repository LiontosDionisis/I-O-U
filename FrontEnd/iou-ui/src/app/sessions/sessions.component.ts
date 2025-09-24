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


@Component({
  selector: 'app-sessions',
  standalone: true,
  imports: [NgFor, NgIf, CommonModule, FormsModule, RouterLink, RouterOutlet],
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
  expenseFormOpen: number | null = null;
  newExpense = {
    description: "",
    totalAmount: 0,
    isEqualSplit: true,
    paidById: 0
  };
  customSplits: {[userId: number]: number} = {};
  expenses: { [sessionId: number]: ExpenseDto[] } = {};



  

  constructor(private sessionService: SessionServiceService, private userService: UserService, private expenseService: ExpenseService){

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
  getSplitsForDisplay(expense: ExpenseDto, session: Session): ExpenseSplitDto[] {
    if (!expense.splits) return [];

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


}
