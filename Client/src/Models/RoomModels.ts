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
    players: Player[];
    host: Player;
    currentGame: Game | null;
    pastGames: Game[];
    createdAt: string;
}

export interface Game {
    id: string;
    status: GameStatus;
    preparationStartTime: Date;
    preparationEndTime: Date;
}

export function parseGameDates(game: Game): Game {
    game.preparationStartTime = new Date(game.preparationStartTime);
    game.preparationEndTime = new Date(game.preparationEndTime);
    return game;
}

export enum GameStatus {
    Lobby,
    Starting,
    InProgress,
    Finished,
}

export interface Player {
    id: string;
    name: string;
}

export interface PlayerJoinedMsg {
    player: Player;
}
