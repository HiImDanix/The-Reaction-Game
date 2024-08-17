<template>
    <div class="space-y-6 max-w-3xl">
      <h1 class="text-5xl font-bold text-primary font-mono">
        Challenge Your Friends To A Reaction Battle!
      </h1>
      <p class="text-gray-200 text-lg font-mono font-">
        Perfect for bar nights, breaking the ice, or settling who does the dishes.
      </p>
      <div class="space-y-3">
        <button class="btn btn-secondary w-full max-w-xs" @click="createGame">
          New Room
        </button>
        <p class="text-gray-200 text-lg font-mono">
          Been invited?
        </p>
        <form @submit.prevent="joinGame" class="flex justify-center">
          <div class="">
            <input
                type="text"
                class="input input-bordered text-black flex-grow mr-2"
                v-model="roomCode"
                placeholder="Enter Game Code"
            />
            <button class="btn btn-primary flex-grow" type="submit">Join Room</button>
          </div>
        </form>
      </div>
    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import {Api} from "@/Api/Api";

const roomCode = ref('');

const emit = defineEmits(['createGame', 'roomCodeIsJoinable']);

const createGame = () => {
  emit('createGame');
}

const joinGame = () => {
  if (!roomCode.value) {
    alert('Please enter a game code');
    return;
  }
  Api.getIsRoomJoinable(roomCode.value).then((isJoinable) => {
    if (isJoinable) {
      emit('roomCodeIsJoinable', roomCode.value);
    } else {
      alert('Game not found');
    }
  });
}
</script>

