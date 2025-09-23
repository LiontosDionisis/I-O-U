import { ExpenseSplitDto } from "./expenseSplitDto";

export interface ExpenseDto {
  id: number;
  description: string;
  totalAmount: number;
  paidById: number;
  paidByUsername: string; // optional, helps display the name
  isEqualSplit: boolean;
  splits: ExpenseSplitDto[]; // each participant's owed amount
  createdAt: string; // optional
}