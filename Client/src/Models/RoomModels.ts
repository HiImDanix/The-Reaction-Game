export interface RoomCreated {
    room: Room;
    you: Player;
    sessionToken: string;
}

export interface RoomJoined {
    room: Room;
    you: Player;
    sessionToken: string;
}

export interface Room {
    id: string;
    code: string;
    status: RoomStatus;
    players: Player[];
    host: Player;
    game: Game;
    currentGame: Game;
    createdAt: string;
}

export interface Game {
    id: string;
}

export interface Player {
    id: string;
    name: string;
}

export enum RoomStatus {
    Lobby,
    Starting,
    InProgress,
    Finished,
}
