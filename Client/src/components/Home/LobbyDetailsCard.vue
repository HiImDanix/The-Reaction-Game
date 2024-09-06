<template>
  <div>
    <h1 class="text-white text-4xl">Welcome to the lobby</h1>
    <p class="text-white">Game code: {{roomCode}}</p>
    <p class="text-white">Players:</p>
    <ul class="text-white">
      <li v-for="player in players" :key="player.id">{{player.name}}</li>
    </ul>
    <p class="text-white">Host: {{host.name}}</p>
    <p class="text-white">Id: {{roomId}}</p>
    <p class="text-white">Status: {{currentGameStatus}}</p>
    <button @click="startGame" class="btn border-none text-black btn-wide mt-3">Start game</button>
  </div>
</template>

<script setup lang="ts">

import type {Player} from "@/Models/RoomModels";
import {GameStatus} from "@/Models/RoomModels";

const props = defineProps<{
  roomCode: string,
  players: Player[],
  host: Player,
  roomId: string,
  currentGameStatus: GameStatus
}>();

const emit = defineEmits({
  startGame: () => true
});
const startGame = () => {
  emit('startGame');
}
</script>
