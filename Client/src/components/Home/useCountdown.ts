// useCountdown.ts
import { ref, onUnmounted, watch, computed, unref } from 'vue';
import type { Ref } from 'vue';

interface CountdownOptions {
    endTime: Date | null | Ref<Date | null>;
    onComplete?: () => void;
    interval?: number;
}

export function useCountdown(options: CountdownOptions) {
    const { endTime, onComplete, interval = 1000 } = options;
    const remainingSeconds = ref(0);
    let countdownInterval: number | null = null;

    const isCompleted = computed(() => remainingSeconds.value === 0);

    function calculateRemainingSeconds(end: Date | null): number {
        if (!end) return 0;
        const milliseconds = end.getTime() - Date.now();
        return Math.max(0, Math.floor(milliseconds / 1000));
    }

    function updateCountdown() {
        const end = unref(endTime);
        remainingSeconds.value = calculateRemainingSeconds(end);

        if (isCompleted.value) {
            stopCountdown();
            onComplete?.();
        }
    }

    function startCountdown() {
        updateCountdown();
        if (!countdownInterval) {
            countdownInterval = window.setInterval(updateCountdown, interval);
        }
    }

    function stopCountdown() {
        if (countdownInterval) {
            clearInterval(countdownInterval);
            countdownInterval = null;
        }
    }

    function resetCountdown() {
        stopCountdown();
        startCountdown();
    }

    watch(() => unref(endTime), (newEndTime) => {
        if (newEndTime) {
            resetCountdown();
        } else {
            stopCountdown();
        }
    });

    onUnmounted(stopCountdown);

    return {
        remainingSeconds,
        isCompleted,
        startCountdown,
        stopCountdown,
        resetCountdown
    };
}
