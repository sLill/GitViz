<script setup>
    import { ref, computed, onMounted } from 'vue';
    import D3Plot from '/src/components/D3Plot.vue';

    const plotComponent = ref(null);

    // Methods
    const updateChartData = () => {
        const newData = [8, 3, 7, 2, 5, 11];
        plotComponent.value.updateData(newData);
    };
    onMounted(async () => {

    });
</script>

<template>
    <div class="wrapper">
       <D3Plot ref="plotComponent" />
        <button @click="updateChartData">Update Chart</button>
    </div>
</template>

<style scoped>
.wrapper {
    display: flex;
    align-items: center;
    justify-content: center;
    flex-wrap: wrap;

    width: 100%;
    height: 100%;
}
</style>
