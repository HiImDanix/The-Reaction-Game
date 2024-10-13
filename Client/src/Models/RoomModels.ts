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

export interface ScoreboardLine {
    player: Player;
    score: number;
}

export enum MiniGameType {
    ColorTap = 'ColorTap',
}

export interface MiniGameRound {
    startTime: string;
    endTime: string;
    scoreboard?: ScoreboardLine[];
}

export interface ColorTapRound extends MiniGameRound {
    colorWordPairs: ColorWordPair[];
}

export interface ColorWordPair {
    color: Color;
    word: Color;
    displayTime: string;
}

export interface Color {
    name: string;
    r: number;
    g: number;
    b: number;
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
