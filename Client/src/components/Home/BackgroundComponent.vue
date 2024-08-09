<template>
  <div class="fixed inset-0 w-full h-full bg-cover bg-center bg-no-repeat -z-10" :style="{ backgroundImage: `url(${bgImage})`}" >
    <div class="absolute inset-0 bg-black opacity-70"></div>
  </div>
</template>
<script setup lang="ts">
import bgImage from "@/assets/images/bg.jpg";
</script>
