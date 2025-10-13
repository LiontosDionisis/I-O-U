import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs'; 

@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  private hubConnection!: signalR.HubConnection;

  private sessionCreatedSource = new BehaviorSubject<any>(null);
  sessionCreated$ = this.sessionCreatedSource.asObservable();

  private sessionDeletedSource = new BehaviorSubject<number | null>(null);
  sessionDeleted$ = this.sessionDeletedSource.asObservable();

  private expenseCreatedSource = new BehaviorSubject<any>(null);
  expenseCreated$ = this.expenseCreatedSource.asObservable();



  constructor() { }

  startConnection() {
    const token = localStorage.getItem('token') || '';
    if (!token) {
      console.error("No JWT token found. Cannot start SignalR Connection.");
    } 
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5062/hubs/sessions', {
        accessTokenFactory: () => token ? token : ''
      }).withAutomaticReconnect().build();

    this.hubConnection
      .start()
      .then(() => console.log("SignalR Connected"))
      .catch((err) => console.error("Error connecting SignalR", err));

    
    this.registerHandlers();
  }

  private registerHandlers() {
    // Sessions
    this.hubConnection.on("SessionCreated", (session: any) => {
      console.log("New session received:", session);
      this.sessionCreatedSource.next(session);
    });

    this.hubConnection.on("SessionDeleted", sessionId => {
      this.sessionDeletedSource.next(sessionId)
    });

    // Expenses
    this.hubConnection.on("ExpenseCreated", (expense: any) => {
      console.log("New expense created.", expense);
      this.expenseCreatedSource.next(expense);
    })
  }
}
