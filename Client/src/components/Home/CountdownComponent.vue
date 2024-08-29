<template>
  <span class="countdown font-mono text-6xl text-white">{{ remainingSeconds }}</span>
</template>

<script setup lang="ts">
import { ref, onUnmounted, watch } from 'vue';

const props = defineProps<{
  endTime: Date | null;
}>();

const remainingSeconds = ref(0);
let countdownInterval: ReturnType<typeof setInterval> | null = null;

function calculateRemainingSeconds(endTime: Date | null): number {
  if (!endTime) return 1;
  const milliseconds = endTime.getTime() - Date.now();
  return Math.max(1, Math.ceil(milliseconds / 1000));
}

function updateCountdown() {
  remainingSeconds.value = calculateRemainingSeconds(props.endTime);
}

function startCountdown() {
  updateCountdown();
  if (!countdownInterval) {
    countdownInterval = setInterval(updateCountdown, 1000);
  }
}

watch(() => props.endTime, startCountdown, { immediate: true });

onUnmounted(() => {
  if (countdownInterval) clearInterval(countdownInterval);
});
</script>

<style>
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
