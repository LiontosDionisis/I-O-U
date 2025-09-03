import { FriendshipStatus } from "./friendshipStatus";

export interface FriendshipDTO {
  id: number;
  friendId: number;
  friendUsername: string;
  status: FriendshipStatus;
}

