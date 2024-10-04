<script setup>
    import { ref } from 'vue';
    import D3Chart from '/src/components/D3Chart.vue';
    import useEndpointService from './composables/services/useEndpointService.js';
    import useDialogService from './composables/services/useDialogService.js';
    import { useToast } from 'primevue/usetoast';

    const endpointService = useEndpointService();
    const dialogService = useDialogService();
    const toast = useToast();

    const viewMode = ref('init');
    const chartComponent = ref(null);
    const localRepoPath = ref(null);

    // Methods
    const start = async () => {
        viewMode.value = 'loading';

        var result = await endpointService.getData(`/api/v1/repo/getLinesChangedByMonth?uri=${localRepoPath.value}`);
        // var result = await endpointService.getData('/api/v1/repo/ping');
        if (result)
            showChart();
        else {
            toast.add({ severity: 'error', summary: null, detail: 'Repo analysis failed', life: 3000 });
            viewMode.value = 'init';
        }
    };

    const showChart = () => {
        viewMode.value = 'stats';
    };

</script>

<template>
    <div class="wrapper">
        <div v-if="viewMode == 'init'" class="init-container"> 
            <div style="font-size: 20px;">Local repo path</div>
            <InputText style="width: 100%" :value="localRepoPath"></InputText>
            <Button style="width: 100px; align-self: center" @click="start">Start</Button>
        </div>

        <div v-if="viewMode == 'loading'" class="loading-container">
            <div style="font-size: 24px;">Loading..</div>
        </div>

        <div v-if="viewMode == 'stats'" class="stats-container">
            <D3Chart ref="chartComponent" />
        </div>
    </div>

    <DynamicDialog />
    <Toast />
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
    /* align-items: center; */
    gap: 15px;
}

.loading-container {
    width: 50%; 
    display: flex; 
    flex-direction: column; 
    gap: 20px; 
    align-items: center;
}

.stats-container {
    width: 80%;
    height: 80%; 
    display: flex; 
    flex-direction: column; 
    gap: 20px; 
    align-items: center;
}
</style>
