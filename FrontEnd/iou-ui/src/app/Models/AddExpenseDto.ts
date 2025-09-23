import { ExpenseSplitDto } from "./expenseSplitDto";

export interface AddExpenseDto {
    sessionId: number;
    description: string;
    totalAmount: number;
    paidById: number;
    isEqualSplit: boolean;
    customSplits?: ExpenseSplitDto[];
}