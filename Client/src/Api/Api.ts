import axios from 'axios'
import type {RoomCreated, RoomJoined} from "@/Models/RoomModels";

const apiClient = axios.create({
    baseURL: 'http://localhost:5083', //TODO: put in Config
    headers: {
        'Content-Type': 'application/json'
    }
});

export async function getIsRoomJoinable(roomCode: string): Promise<boolean> {
    try {
        const response = await apiClient.get(`/rooms/by-code/${roomCode}/joinable`);
        return response.status === 200;
    } catch (error) {
        console.error(error);
        return false;
    }
}

export async function postCreateRoom(playerName: string): Promise<RoomCreated> {
    try {
        const r = await apiClient.post('/rooms', {playerName});
        return r.data;
    } catch (error) {
        console.error(error);
        throw error;
    }
}

export async function postJoinRoom(roomCode: string, playerName: string): Promise<RoomJoined> {
    try {
        const r = await apiClient.post(`/rooms/by-code/${roomCode}/players`, {playerName});
        return r.data;
    } catch (error) {
        console.error(error);
        throw error;
    }
}
