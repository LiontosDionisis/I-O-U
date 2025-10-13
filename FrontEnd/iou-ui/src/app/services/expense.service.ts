import { Injectable } from '@angular/core';
import { AddExpenseDto } from '../Models/AddExpenseDto';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ExpenseDto } from '../Models/expenseDto';

@Injectable({
  providedIn: 'root'
})
export class ExpenseService {

  constructor(private http: HttpClient) { }

  private apiUrl = 'http://localhost:5062/api/expenses'
  //private apiUrl = 'http://192.168.1.94:5062/api/expenses'
  

  createExpense(dto: AddExpenseDto): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
    
    return this.http.post(`${this.apiUrl}`, dto, {headers});
  }

  getExpensesBySession(sessionId: number): Observable<ExpenseDto[]> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.get<ExpenseDto[]>(`${this.apiUrl}/session/${sessionId}`, {headers})
  }

  getSessionBalances(sessionId: number): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.get(`${this.apiUrl}/session/${sessionId}/balances`, {headers});
  }

  settleSplit(expenseId: number): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
    
    return this.http.put(`${this.apiUrl}/${expenseId}/settle`, {}, {headers});
  }
  
}
