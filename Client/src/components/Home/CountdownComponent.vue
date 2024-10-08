<template>
  <span class="countdown font-mono text-6xl text-white">{{ remainingSeconds }}</span>
</template>

<script setup lang="ts">
import { useCountdown } from './useCountdown';
import {computed} from "vue";

const props = defineProps<{
  endTime: Date | null;
}>();

const { remainingSeconds, startCountdown } = useCountdown({
  endTime: props.endTime
});

startCountdown();
</script>
