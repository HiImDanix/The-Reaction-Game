<template>
  <div class="w-full p-8">
    <div class="bg-white rounded-lg p-6">
      <h1 class="text-center text-5xl font-bold text-black mb-6">Scoreboard</h1>
      <div class="overflow-x-auto">
        <!-- Apply table-fixed for full-width, evenly spaced columns -->
        <table class="table table-fixed w-full table-zebra">
          <thead>
          <tr class="text-2xl text-black">
            <th class="w-1/12">#</th>
            <th class="w-8/12">Player</th>
            <th class="w-3/12">Score</th>
          </tr>
          </thead>
          <tbody>
          <tr v-for="(line, index) in scoreboard" :key="line.player.name" class="hover text-2xl text-black">
            <td >{{ index + 1 }}</td>
            <td >{{ line.player.name }}</td>
            <td>{{ line.score }}</td>
          </tr>

          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { ScoreboardLine } from "@/Models/RoomModels";

// Define the props that take in the scoreboard array
const props = defineProps<{ scoreboard: ScoreboardLine[] }>();
</script>
