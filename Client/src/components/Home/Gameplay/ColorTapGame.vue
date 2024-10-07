<template>
  <div class="absolute inset-0 bg-gradient-to-b from-gray-100 to-gray-300">
    <div class="h-full flex flex-col">
      <div class="flex-1 flex items-center justify-center">
        <p v-if="currentPair"
           class="text-5xl sm:text-7xl md:text-8xl font-bold px-4 drop-shadow-md"
           :style="{ color: rgbColor }"
           :aria-label="`Color word: ${currentPair.word.name}`">
          {{ currentPair.word.name }}
        </p>
      </div>
      <div class="flex-1 relative">
        <button ref="tapButton"
                class="w-full h-full text-4xl sm:text-6xl md:text-7xl font-bold bg-primary text-primary-content px-4">
          <span class="relative z-10">Tap when colors match</span>
        </button>
        <div ref="feedbackOverlay" class="absolute inset-0 bg-white opacity-0 pointer-events-none"></div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import {computed, onMounted, onUnmounted, watch, ref, shallowRef} from 'vue';
import { useColorWordPairScheduler } from "./useColorTapWordPairScheduler";
import type { ColorTapRound, Game } from "@/Models/RoomModels";

const props = defineProps<{ game: Game }>();
const emit = defineEmits(['matchAttempt']);

const tapButton = shallowRef<HTMLButtonElement | null>(null);
const feedbackOverlay = shallowRef<HTMLDivElement | null>(null);

const { currentPair, startScheduling, stopScheduling } = useColorWordPairScheduler();

const rgbColor = computed(() => {
  const color = currentPair.value?.color;
  return color ? `rgb(${color.r}, ${color.g}, ${color.b})` : '';
});

const initializeRound = (round?: ColorTapRound) => {
  stopScheduling();
  if (round) {
    startScheduling(round);
  } else {
    console.warn('No current round found');
  }
};

const checkMatch = () => {
  console.log('Match attempt');
  emit('matchAttempt');

  if (feedbackOverlay.value) {
    feedbackOverlay.value.style.opacity = '0.3';
    setTimeout(() => {
      if (feedbackOverlay.value) {
        feedbackOverlay.value.style.opacity = '0';
      }
    }, 150);
  }
};

onMounted(() => {
  if (props.game.currentMiniGame?.currentRound) {
    initializeRound(props.game.currentMiniGame.currentRound as ColorTapRound);
  }

  if (tapButton.value) {
    tapButton.value.addEventListener('mousedown', checkMatch, { passive: true });
    tapButton.value.addEventListener('touchstart', checkMatch, { passive: true });
  }
});

onUnmounted(() => {
  stopScheduling();
  if (tapButton.value) {
    tapButton.value.removeEventListener('mousedown', checkMatch);
    tapButton.value.removeEventListener('touchstart', checkMatch);
  }
});

watch(
    () => props.game.currentMiniGame?.currentRound as ColorTapRound,
    (newRound) => {
      if (newRound) {
        initializeRound(newRound);
      }
    }
);
</script>

<style scoped>
.text-shadow {
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.1);
}

button {
  cursor: pointer;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  border-top: 4px solid rgba(255, 255, 255, 0.2);
}

div[ref="feedbackOverlay"] {
  transition: opacity 150ms ease-out;
}
</style>
