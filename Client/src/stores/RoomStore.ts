import {ref} from 'vue'
import type {Ref} from "vue";
import { defineStore } from 'pinia'
import type {Player, Room} from "@/Models/RoomModels";

export const useRoomStore = defineStore('room', () => {
    const room: Ref<Room> = ref(null);

    function setRoom(newRoom: Room): void {
        room.value = newRoom;
    }

    function addPlayer(player: Player): void {
        room.value.players.push(player);
    }

    return {room, setRoom, addPlayer}
})
