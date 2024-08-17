<template>
  <div v-if="room != null">
    <h1 class="text-white text-4xl">Welcome to the lobby</h1>
    <p class="text-white">Game code: {{room.code}}</p>
    <p class="text-white">Players:</p>
    <ul class="text-white">
      <li v-for="player in room.players" :key="player.id">{{player.name}}</li>
    </ul>
    <p class="text-white">Host: {{room.host.name}}</p>
    <p class="text-white">Id: {{room.id}}</p>
    <p class="text-white">Status: {{room.status}}</p>
    <button @click="startGame" class="btn border-none text-black btn-wide mt-3">Start game</button>
  </div>
  <div v-else>
    <div class="loading loading-spinner loading-lg text-white"></div>
  </div>
</template>

<script setup lang="ts">
import {establishRoomConnection} from "@/stores/establishRoomConnection";
import {useRoomStore} from "@/stores/RoomStore";
import {storeToRefs} from "pinia";
import {Api} from "@/Api/Api";

establishRoomConnection();

const roomStore = useRoomStore();
const { room } = storeToRefs(roomStore);

const startGame = () => {
  Api.postStartGame();
}

</script>
