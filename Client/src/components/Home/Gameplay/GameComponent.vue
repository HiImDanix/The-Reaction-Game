<template>
  <ColorTapGame v-if="game.currentMiniGame?.type == MiniGameType.ColorTap" :game="props.game" />
</template>

<script setup lang="ts">
import ColorTapGame from "@/components/Home/Gameplay/ColorTapGame.vue";
import type {Game} from "@/Models/RoomModels";
import {MiniGameType} from "@/Models/RoomModels";

const props = defineProps<{ game: Game }>();
</script>
