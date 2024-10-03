<script setup>
    import { ref } from 'vue';
    import D3Chart from '/src/components/D3Chart.vue';

    const viewMode = ref('init');
    const chartComponent = ref(null);
    const localRepoPath = ref(null);

    // Methods
    const updateChartData = () => {

        viewMode.value = 'loading';

        // const newData = [8, 3, 7, 2, 5, 11];
        // chartComponent.value.updateData(newData);
    };
</script>

<template>
    <div class="wrapper">
       
        <div v-if="viewMode == 'init'" class="init-container"> 
            <div style="font-size: 24px;">Local repo path</div>
            <InputText style="width: 100%" :value="localRepoPath"></InputText>
            <Button style="width: 100px" @click="updateChartData">GO</Button>
        </div>

        <div v-if="viewMode == 'loading'" class="loading-container">
            <div style="font-size: 24px;">Loading..</div>
        </div>

        <div v-if="viewMode == 'stats'" class="stats-container">
            
        </div>

        <!-- <div>
            <D3Chart ref="chartComponent" />
            <Button @click="updateChartData">Update Chart</Button>
        </div> -->
    </div>
</template>

<style scoped>
/* * { border: 2px red solid; } */

.wrapper {
    display: flex;
    align-items: center;
    justify-content: center;
    flex-wrap: wrap;

    width: 100%;
    height: 100%;
}

.init-container {
    width: 50%; 
    display: flex; 
    flex-direction: column; 
    gap: 20px; 
    align-items: center;
}

.loading-container {
    width: 50%; 
    display: flex; 
    flex-direction: column; 
    gap: 20px; 
    align-items: center;
}

.stats-container {
    width: 50%; 
    display: flex; 
    flex-direction: column; 
    gap: 20px; 
    align-items: center;
}
</style>
