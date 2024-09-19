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
    currentGame: Game;
    pastGames: Game[];
    createdAt: string;
}

export interface Game {
    id: string;
    status: GameStatus;
    preparationStartTime: string;
    preparationEndTime: string;
    currentMiniGame: MiniGame | null;
}

export interface MiniGame {
    id: string;
    name: string;
    instructions: string;
    instructionsStartTime: string;
    instructionsEndTime: string;
    type: MiniGameType;
    currentRoundNo: number;
    totalRoundsNo: number;
    currentRound: MiniGameRound;
}

export enum MiniGameType {
    ColorTap = 'ColorTap',
}

export interface MiniGameRound {
    startTime: string;
    endTime: string;
}

export interface ColorTapRound extends MiniGameRound {
    colorWordPairs: ColorWordPair[];
}

export interface ColorWordPair {
    color: string;
    word: string;
    displayTime: string;
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
