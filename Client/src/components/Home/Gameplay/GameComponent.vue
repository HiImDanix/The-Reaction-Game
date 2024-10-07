<template>
  <p v-if="currentPair" class="word-display" :style="{ color: rgbColor }" :aria-label="`Color word: ${currentPair.word.name}`">
    {{ currentPair.word.name }}
  </p>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted, watch } from 'vue';
import { useColorWordPairScheduler } from "./useColorTapWordPairScheduler";
import type { ColorTapRound, Game } from "@/Models/RoomModels";

const props = defineProps<{ currentGame: Game }>();

const { currentPair, startScheduling, stopScheduling } = useColorWordPairScheduler();

const rgbColor = computed(() => {
  const color = currentPair.value?.color;
  return color ? `rgb(${color.r}, ${color.g}, ${color.b})` : '';
});

const initializeRound = (round?: ColorTapRound) => {
  stopScheduling();
  round ? startScheduling(round) : console.warn('No current round found');
};

onMounted(() => initializeRound(props.currentGame.currentMiniGame?.currentRound as ColorTapRound));
onUnmounted(stopScheduling);

watch(
    () => props.currentGame.currentMiniGame?.currentRound as ColorTapRound,
    initializeRound
);
</script>

<style scoped>
.word-display {
  font-size: 1.5rem;
  font-weight: bold;
  transition: color 0.3s ease;
}
</style>
