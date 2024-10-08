<template>
  <div>
    <h1 class="text-white text-4xl">{{title}}</h1>
    <p class="text-white mt-2">{{description}}</p>
  </div>
</template>

<script setup lang="ts">
const props = defineProps< {
  title: string,
  description: string
}>();
</script>
