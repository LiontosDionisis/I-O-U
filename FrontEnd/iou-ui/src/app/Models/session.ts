import { SessionUser } from "./sessionUser";

export interface Session {
  id: number;
  name: string;
  createdAt: string;
  participants: SessionUser[];
}

