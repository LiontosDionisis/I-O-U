export enum AvatarType {
    Cat = 1,
    Panda = 2,
    Dog = 3,
    Penguin = 4
}

export function getAvatarUrl(avatar: AvatarType): string {
    switch (avatar) {
        case AvatarType.Cat:
            return 'assets/avatars/avatarCat.png';
        case AvatarType.Dog:
            return 'assets/avatars/avatarDog.png';
        case AvatarType.Panda:
            return 'assets/avatars/avatarPanda.png';
        case AvatarType.Penguin:
            return 'assets/avatars/penguAvatar.png';
        default:
            return 'assets/avatars/avatarPenguin.png'; 
    }
}
