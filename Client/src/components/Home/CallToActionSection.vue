<template>
  <div class="min-h-screen flex flex-col" data-theme="emerald">
    <div class="fixed inset-0 w-full h-full bg-cover bg-center bg-no-repeat -z-10" :style="{ backgroundImage: `url(${bgImage})`}" >
      <div class="absolute inset-0 bg-black opacity-60"></div>
    </div>
    <div class="flex-grow flex flex-col items-center justify-center relative z-10 text-center mb-24">
      <div class="space-y-6 max-w-3xl">
        <h1 class="text-5xl font-bold text-primary font-mono">
          Challenge Your Friends To A Reaction Battle!
        </h1>
        <p class="text-gray-200 text-lg font-mono">
          Perfect for bar nights, breaking the ice, or settling who does the dishes.
        </p>
        <div class="space-y-2">
          <form @submit.prevent="joinGame" class="flex justify-center space-x-2">
            <input
                type="text"
                class="input input-bordered text-black flex-grow max-w-[200px]"
                v-model="gameCode"
                placeholder="Enter Game Code"
            />
            <button class="btn btn-primary" type="submit">Join Game</button>
          </form>
          <button class="btn btn-secondary w-full max-w-xs" @click="createGame">
            Create Game
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import bgImage from '../../assets/images/bg.jpg'

const gameCode = ref('');

const createGame = () => {
  alert('Creating game...');
}

const joinGame = () => {
  if (!gameCode.value) {
    alert('Please enter a game code');
    return;
  }
  alert('Joining game with code: ' + gameCode.value);
}
</script>

