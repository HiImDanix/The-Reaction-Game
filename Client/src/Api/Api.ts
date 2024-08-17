import axios from 'axios'
import type {Room, RoomCreated, RoomJoined} from "@/Models/RoomModels";
import {useUserStore} from "@/stores/UserStore";

const apiClient = axios.create({
    baseURL: 'http://localhost:5083', //TODO: put in Config
    headers: {
        'Content-Type': 'application/json'
    }
});

apiClient.interceptors.request.use(function (config) {
    const userStore = useUserStore();
    const token = userStore.getToken();
    if (token) {
        config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
}, function (error) {
    return Promise.reject(error);
})

export const Api = {
    async getIsRoomJoinable(roomCode: string): Promise<boolean> {
        try {
            const response = await apiClient.get(`/rooms/by-code/${roomCode}/joinable`);
            return response.status === 200;
        } catch (error) {
            console.error(error);
            return false;
        }
    },

    async postCreateRoom(playerName: string): Promise<RoomCreated> {
        try {
            const r = await apiClient.post('/rooms', {playerName});
            return r.data;
        } catch (error) {
            console.error(error);
            throw error;
        }
    },

    async postJoinRoom(roomCode: string, playerName: string): Promise<RoomJoined> {
        try {
            const r = await apiClient.post(`/rooms/by-code/${roomCode}/players`, {playerName});
            return r.data;
        } catch (error) {
            console.error(error);
            throw error;
        }
    },

    async getRoom(): Promise<Room> {
        try {
            const r = await apiClient.get('/rooms/me');
            return r.data;
        } catch (error) {
            console.error(error);
            throw error;
        }
    }
}


