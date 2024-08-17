<template>
  <div class="flex flex-col items-center space-y-6 max-w-3xl">
    <h1 class="text-5xl text-primary font-mono font-bold">What shall we call you?</h1>
    <form class="flex flex-col space-y-2" @submit.prevent="submitName">
      <input type="text" v-model="name" class="input input-bordered" placeholder="Daniel" />
      <button class="btn btn-primary">Create Room</button>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import type {RoomCreated} from "@/Models/RoomModels";
import {Api} from "@/Api/Api";

const emit = defineEmits(['roomCreated']);

const name = ref('');

const submitName = async () => {
  if ((name.value.trim())) {
    await Api.postCreateRoom(name.value).then((r: RoomCreated) => {
      emit('roomCreated', r);
    }).catch((e) => {
      console.error(e);
      throw e;
    });
  }
}
</script>
