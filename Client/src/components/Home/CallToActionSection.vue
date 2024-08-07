<template>
  <h1>Challenge your friends (or enemies) to a reaction game battle tat tests your reflexes, wits, and knowledge!</h1>
  <h3>Perfect for bar nights, breaking the ice, or settling who does the dishes.</h3>
  <form @submit.prevent="joinGame">
    <input
      type="text"
      class="input"
      v-model="gameCode"
      placeholder="Enter Game Code"
    />
    <button class="btn btn-primary" type="submit">Join Game</button>
  </form>
  <button class="btn btn-wide" @click="createGame">
    Create Game
  </button>
</template>

<script setup lang="ts">
import { ref } from 'vue'

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

