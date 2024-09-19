import {ref} from 'vue'
import type {Ref} from "vue";
import { defineStore } from 'pinia'
import type {Game, MiniGame, Player, Room} from "@/Models/RoomModels";

export const useRoomStore = defineStore('room', () => {
    const room: Ref<Room | null> = ref(null);

    function setRoom(newRoom: Room): void {
        room.value = newRoom;
    }

    function clearRoom(): void {
        room.value = null;
    }

    function updateCurrentGame(game: Game): void {
        if (!room.value) return;
        room.value.currentGame = game;
    }

    function updateCurrentMiniGame(game: MiniGame): void {
        if (!room.value) return;
        room.value.currentGame.currentMiniGame = game;
    }

    function addPlayer(player: Player): void {
        if (!room.value) return;
        if (room.value.players.some(p => p.id === player.id)) return;
        room.value.players.push(player);
    }

    return {room, setRoom, clearRoom, addPlayer, updateCurrentGame, updateCurrentMiniGame}
})
