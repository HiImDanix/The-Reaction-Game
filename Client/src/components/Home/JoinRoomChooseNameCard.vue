<template>
  <div class="flex flex-col items-center space-y-6 max-w-3xl mb-24">
    <h1 class="text-5xl text-primary font-mono font-bold">What shall we call you?</h1>
    <form class="flex flex-col space-y-2" @submit.prevent="submitName">
      <input type="text" v-model="name" class="input input-bordered" placeholder="Daniel" />
      <button class="btn btn-primary">Join Room</button>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import {Api} from "@/Api/Api";
import type {RoomJoined} from "@/Models/RoomModels";

const props = defineProps({
  roomCode: {
    type: String,
  }
});

const emit = defineEmits(['roomJoined']);

const name = ref('');

const submitName = async () => {
  if ((name.value.trim())) {
    await Api.postJoinRoom(props.roomCode, name.value).then((r: RoomJoined) => {
      emit('roomJoined', r);
    }).catch((e) => {
      console.error(e);
      throw e;
    });
  }
}
</script>
