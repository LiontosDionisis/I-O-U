import { AvatarType } from "./avatar";

export interface CurrentUserDTO {
    id: number,
    username: string,
    email: string,
    avatar: AvatarType
}