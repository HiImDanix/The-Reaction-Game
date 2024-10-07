import {ref} from 'vue';
import type { Ref } from 'vue';
import type {ColorTapRound, ColorWordPair} from "@/Models/RoomModels";

export function useColorWordPairScheduler() {
    const currentPair: Ref<ColorWordPair | null> = ref(null);
    let timeoutIds: number[] = []; // Changed from NodeJS.Timeout[] to number[]

    /**
     * Schedules changes for color word pairs
     * @param {ColorTapRound} round - The current game round
     */
    const scheduleChanges = (round: ColorTapRound): void => {
        stopScheduling();

        const now = Date.now();

        round.colorWordPairs.forEach((pair) => {
            const displayTime = new Date(pair.displayTime).getTime();
            const delay = displayTime - now;

            if (delay > 0) {
                const timeoutId = setTimeout(() => {
                    currentPair.value = pair;
                }, delay);
                timeoutIds.push(timeoutId);
            } else if (delay <= 0 && (!currentPair.value || displayTime > new Date(currentPair.value.displayTime).getTime())) {
                currentPair.value = pair;
            }
        });
    };

    /**
     * Starts scheduling for a new round
     * @param {ColorTapRound} round - The current game round
     */
    const startScheduling = (round: ColorTapRound): void => {
        try {
            scheduleChanges(round);
        } catch (error) {
            console.error('Error while scheduling changes', error);
        }
    };

    /**
     * Stops all current schedules
     */
    const stopScheduling = (): void => {
        timeoutIds.forEach(clearTimeout);
        timeoutIds = [];
    };

    return {
        currentPair,
        startScheduling,
        stopScheduling
    };
}
