import { AvatarType } from "./avatar";

export interface SessionUser {
  userId: number;
  username: string;
  avatar: AvatarType;
  avatarUrl?: string;
}