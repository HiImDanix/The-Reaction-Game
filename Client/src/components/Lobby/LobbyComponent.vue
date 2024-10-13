<template>
  <div v-if="room != null" class="mb-24">
    <div :class="{'fade-out': isPrepareToStartPhase}" v-if="!isInstructionsPhase && !isGameplayPhase && !isRoundScoreboardPhase">
      <LobbyDetailsCard
          :roomCode="room.code"
          :players="room.players"
          :host="room.host"
          :roomId="room.id"
          :currentGameStatus="room.currentGame?.status ?? GameStatus.Lobby"
          @startGame="startGame"
      />
    </div>

    <span v-if="isPrepareToStartPhase" class="countdown-overlay">
      <CountdownComponent :end-time="prepareToStartEndTime" />
    </span>

    <InstructionsCard
        v-if="isInstructionsPhase"
        :title="room.currentGame.currentMiniGame!.name"
        :description="room.currentGame.currentMiniGame!.instructions"
    />

    <GameComponent v-if="isGameplayPhase" :game="room.currentGame" />

    <div v-if="isRoundScoreboardPhase">
      <ScoreboardComponent :scoreboard="room.currentGame.currentMiniGame?.currentRound?.scoreboard" />
    </div>


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
import {computed, onMounted, ref, watch} from "vue";
import {GameStatus} from "@/Models/RoomModels";
import CountdownComponent from "@/components/Lobby/CountdownComponent.vue";
import LobbyDetailsCard from "@/components/Lobby/LobbyDetailsCard.vue";
import InstructionsCard from "@/components/Lobby/GameInstructionsCard.vue";
import GameComponent from "@/components/Lobby/Gameplay/GameComponent.vue";
import ScoreboardComponent from "@/components/Lobby/Gameplay/ScoreboardComponent.vue";

establishRoomConnection();

const roomStore = useRoomStore();
const { room } = storeToRefs(roomStore);

const isPrepareToStartPhase = computed(
    () => room.value?.currentGame?.status === GameStatus.Starting
)

const prepareToStartEndTime = ref<Date | null>(null);

watch(isPrepareToStartPhase, (isPreparing) => {
  if (isPreparing) {
    prepareToStartEndTime.value = room.value?.currentGame?.preparationEndTime
        ? new Date(room.value.currentGame.preparationEndTime)
        : null;
  } else {
    prepareToStartEndTime.value = null;
  }
});

const isInstructionsPhase = computed(() => {
  const currentGame = room.value?.currentGame;
  const currentMiniGame = currentGame?.currentMiniGame;

  if (!currentGame || !currentMiniGame) return false;

  const now = new Date();
  const instructionsStartTime = new Date(currentMiniGame.instructionsStartTime);
  const instructionsEndTime = new Date(currentMiniGame.instructionsEndTime);

  return (
      currentGame.status === GameStatus.InProgress &&
      instructionsStartTime && instructionsEndTime &&
      now >= instructionsStartTime && now < instructionsEndTime
  );
});

const isGameplayPhase = computed(() => {
  const currentGame = room.value?.currentGame;
  const currentMiniGame = currentGame?.currentMiniGame;
  const currentRound = currentMiniGame?.currentRound;

  if (!currentGame || !currentMiniGame || !currentRound) return false;

  const now = new Date();
  const roundStartTime = new Date(currentRound.startTime);
  const roundEndTime = new Date(currentRound.endTime);

  return (
      currentGame.status === GameStatus.InProgress &&
      roundStartTime && roundEndTime &&
      now >= roundStartTime && now < roundEndTime
  );
});

const isRoundScoreboardPhase = computed(() => {
  const currentGame = room.value?.currentGame;
  const currentMiniGame = currentGame?.currentMiniGame;
  const currentRound = currentMiniGame?.currentRound;

  if (!currentGame || !currentMiniGame || !currentRound) return false;

  const now = new Date();
  const roundEndTime = new Date(currentRound.endTime);

  return (
      currentGame.status === GameStatus.InProgress &&
      roundEndTime &&
      now >= roundEndTime
  );
});

const startGame = () => {
  Api.postStartGame();
}
</script>

<style scoped>
.fade-out {
  opacity: 0.3;
  pointer-events: none;
  transition: opacity 0.5s ease-in-out;
}
.countdown-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: rgba(0, 0, 0, 0.7);
  z-index: 1000;
}
</style>
