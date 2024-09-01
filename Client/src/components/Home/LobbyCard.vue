<template>
  <div v-if="room != null">
    <div :class="{'fade-out': isPrepareToStartPhase}">
      <h1 class="text-white text-4xl">Welcome to the lobby</h1>
      <p class="text-white">Game code: {{room.code}}</p>
      <p class="text-white">Players:</p>
      <ul class="text-white">
        <li v-for="player in room.players" :key="player.id">{{player.name}}</li>
      </ul>
      <p class="text-white">Host: {{room.host.name}}</p>
      <p class="text-white">Id: {{room.id}}</p>
      <p class="text-white">Status: {{room.currentGame.status}}</p>
      <button @click="startGame" class="btn border-none text-black btn-wide mt-3">Start game</button>
    </div>

    <span v-if="isPrepareToStartPhase" class="countdown-overlay">
      <CountdownComponent :end-time="countdownEndTime" />
    </span>

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
import {computed, ref, watch} from "vue";
import {GameStatus} from "@/Models/RoomModels";
import CountdownComponent from "@/components/Home/CountdownComponent.vue";

establishRoomConnection();

const roomStore = useRoomStore();
const { room } = storeToRefs(roomStore);

const isPrepareToStartPhase = computed(
    () => room.value?.currentGame?.status === GameStatus.Starting
)

const countdownEndTime = ref<Date | null>(null);

watch(isPrepareToStartPhase, (isPreparing) => {
  if (isPreparing) {

    countdownEndTime.value = room.value?.currentGame?.preparationEndTime
        ? new Date(room.value.currentGame.preparationEndTime)
        : null;// TODO: research null, undefined "?" operator
  } else {
    countdownEndTime.value = null;
  }
});

const startGame = () => {
  Api.postStartGame();
}
</script>
