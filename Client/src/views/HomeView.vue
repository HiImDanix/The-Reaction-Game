<template>
  <div class="min-h-screen flex flex-col" data-theme="emerald">
    <BackgroundComponent />
    <transition name="navbar-fade">
      <div class="navbar bg-black bg-opacity-60" v-if="currentView != View.CallToAction">
        <button @click="onGoBack" class="btn btn-circle bg-transparent border-none text-white hover:text-black">
          <i class="fas fa-arrow-left"></i>
        </button>
      </div>
    </transition>
    <div class="flex-grow flex flex-col items-center justify-center relative z-10 text-center mb-24">
      <Transition name="slide" >
        <CreateRoomChooseNameCard
            v-if="currentView === View.CreateGameChooseName"
            @roomCreated="onRoomCreated"
        />
        <JoinRoomChooseNameCard
            v-else-if="currentView === View.JoinGameChooseName"
            @roomJoined="onRoomJoined"
            :roomCode="roomCode"
        />
        <JoinOrCreateRoomCard
            v-else-if="currentView === View.CallToAction"
            @createGame="currentView = View.CreateGameChooseName"
            @roomCodeIsJoinable="onRoomCodeIsJoinable"
        />
        <LobbyCard
            v-else-if="currentView === View.Lobby"
        />
      </Transition>
    </div>
  </div>
</template>

<script setup lang="ts">
import JoinOrCreateRoomCard from '../components/Home/JoinOrCreateRoomCard.vue'
import {onMounted, ref, watch} from 'vue'
import BackgroundComponent from "@/components/Home/BackgroundComponent.vue";
import CreateRoomChooseNameCard from "@/components/Home/CreateRoomChooseNameCard.vue";
import JoinRoomChooseNameCard from "@/components/Home/JoinRoomChooseNameCard.vue";
import LobbyCard from "@/components/Home/LobbyComponent.vue";
import type {RoomCreated, RoomJoined} from "@/Models/RoomModels";
import {useRoomStore} from "@/stores/RoomStore";
import {useUserStore} from "@/stores/UserStore";
import {Api} from "@/Api/Api";
import {useSignalRStore} from "@/stores/SignalRStore";

const roomCode = ref('');

enum View {
  CallToAction,
  CreateGameChooseName,
  JoinGameChooseName,
  Lobby
}

const currentView = ref<View>(View.CallToAction);

const roomStore = useRoomStore();
const userStore = useUserStore();
const signalRStore = useSignalRStore();

onMounted(async () => {
  if (userStore.user) {
    try {
      const room = await Api.getRoom();
      roomStore.setRoom(room);
      currentView.value = View.Lobby;
    } catch (e) {
      // Do nothing
    }
  }
});

const onRoomCreated = (r: RoomCreated) => {
  userStore.setUser(r.you, r.sessionToken);
  currentView.value = View.Lobby;
}

const onRoomJoined = (r: RoomJoined) => {
  userStore.setUser(r.you, r.sessionToken);
  currentView.value = View.Lobby;
}

const onRoomCodeIsJoinable = (code: string) => {
  roomCode.value = code;
  currentView.value = View.JoinGameChooseName;
}

const onGoBack = async () => {
  if (currentView.value === View.Lobby) {
    if (confirm('Are you sure you want to leave the room?')) {
      userStore.logout();
      roomStore.clearRoom();
      await signalRStore.disconnect();
      signalRStore.clearSubscriptions();
      currentView.value = View.CallToAction;
    }

  } else {
    currentView.value = View.CallToAction;
  }
}

// // watch: if status is 3 and currentGame.currentMiniGame.instructionsStartTime and currentGame.currentMiniGame.instructionsEndTime is between now, then show instructions
// watch(() => roomStore.room?.currentGame?.status, (status) => {
//   if (status === GameStatus.InProgress) {
//     const currentGame = roomStore.room?.currentGame;
//     if (currentGame?.currentMiniGame?.instructionsStartTime && currentGame?.currentMiniGame.instructionsEndTime) {
//       const instructionsStartTime = new Date(currentGame.currentMiniGame.instructionsStartTime);
//       const instructionsEndTime = new Date(currentGame.currentMiniGame.instructionsEndTime);
//       const now = new Date();
//       if (now >= instructionsStartTime && now <= instructionsEndTime) {
//         currentView.value = View.Instructions;
//       }
//     }
//   } else if (status === GameStatus.Finished) {
//     currentView.value = View.Lobby;
//   }
// });
</script>

<style scoped>
.min-h-screen {
  overflow-x: hidden;
}

.slide-enter-active,
.slide-leave-active {
  transition: all 0.5s ease;
  position: absolute;
  width: 100%;
}

.slide-enter-from {
  transform: translateX(200%);
}

.slide-leave-to {
  transform: translateX(-200%);
}

.slide-enter-to,
.slide-leave-from {
  transform: translateX(0);
}

.slide-enter-active {
  transition-delay: 0s;
}

.slide-leave-active {
  transition-delay: 0s;
}

.navbar-fade-enter-active, .fade-leave-active {
  transition: opacity 0.5s;
}
.navbar-fade-enter, .fade-leave-to /* .fade-leave-active below version 2.1.8 */ {
  opacity: 0;
}
</style>
