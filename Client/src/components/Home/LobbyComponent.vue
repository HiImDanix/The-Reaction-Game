<template>
  <div v-if="room != null">
    <div :class="{'fade-out': isPrepareToStartPhase}" v-if="!isInstructionsPhase && !isGameplayPhase">
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

    <GameComponent v-if="isGameplayPhase" />


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
import LobbyDetailsCard from "@/components/Home/LobbyDetailsCard.vue";
import InstructionsCard from "@/components/Home/InstructionsCard.vue";
import GameComponent from "@/components/Home/Gameplay/GameComponent.vue";

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
        : null;// TODO: research null, undefined "?" operator
  } else {
    prepareToStartEndTime.value = null;
  }
});

const isInstructionsPhase = computed(
    () => {
      return room.value?.currentGame?.status === GameStatus.InProgress &&
          room.value?.currentGame?.currentMiniGame != null &&
          room.value?.currentGame?.currentMiniGame.instructionsStartTime != null &&
          room.value?.currentGame?.currentMiniGame.instructionsEndTime != null &&
          new Date() >= new Date(room.value.currentGame.currentMiniGame.instructionsStartTime) &&
          new Date() < new Date(room.value.currentGame.currentMiniGame.instructionsEndTime)
    }
)

const isGameplayPhase = computed(
    () => {
      return room.value?.currentGame?.status === GameStatus.InProgress &&
          room.value?.currentGame?.currentMiniGame != null &&
          room.value?.currentGame?.currentMiniGame.currentRound != null &&
          room.value?.currentGame?.currentMiniGame.currentRound.startTime != null &&
          room.value?.currentGame?.currentMiniGame.currentRound.endTime != null &&
          new Date() >= new Date(room.value.currentGame.currentMiniGame.currentRound.startTime) &&
          new Date() < new Date(room.value.currentGame.currentMiniGame.currentRound.endTime)
    }
)



const startGame = () => {
  Api.postStartGame();
}
</script>
