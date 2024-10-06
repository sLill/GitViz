<script setup>
    import { ref } from 'vue';
    import D3RepoLineChangesOverTime from '/src/components/D3/D3RepoLineChangesOverTime.vue';
    import useEndpointService from './composables/services/useEndpointService.js';
    import useDialogService from './composables/services/useDialogService.js';
    import { useToast } from 'primevue/usetoast';

    const endpointService = useEndpointService();
    const dialogService = useDialogService();
    const toast = useToast();

    const viewMode = ref('init');
    const chartData = ref(null);
    
    const localRepoPath = ref(null);
    const branchName = ref(null);
    const startDate = ref();
    const endDate = ref();
    const fileExtensions = ref(null);

    // Methods
    const start = async () => {
        viewMode.value = 'loading';

        let uri = `/api/v1/repo/getLinesChangedByMonth?localRepoPath=${localRepoPath.value}`;
        
        if (branchName.value)
            uri += `&branchName=${branchName.value}`;

        if (startDate.value)
            uri += `&startDate=${startDate.value.toISOString()}`;

        if (endDate.value)
            uri += `&endDate=${endDate.value.toISOString()}`;

        if (fileExtensions.value) {
            fileExtensions.value.split(" ").forEach(extension => {
                uri += `&fileExtensions=${extension}`;
            });
        }

        const result = await endpointService.getData(uri);
        if (result)
            showChart(result.data.json);
        else {
            toast.add({ severity: 'error', summary: null, detail: 'Repo analysis failed', life: 3000 });
            viewMode.value = 'init';
        }
    };

    const showChart = (json) => {
        const resultJson = JSON.parse(json);
        chartData.value = resultJson;

        viewMode.value = 'stats';
    };

</script>

<template>
    <div class="wrapper">
        <div v-if="viewMode == 'init'" class="init-container"> 

            <div style="font-size: 20px;">Local Repo Path</div>
            <InputText style="width: 100%" v-model="localRepoPath"></InputText>

            <div style="font-size: 20px;">Branch Name (optional)</div>
            <InputText style="width: 100%" v-model="branchName"></InputText>

            <div style="font-size: 20px;">File Extensions (space delimited. default: all)</div>
            <InputText style="width: 100%" v-model="fileExtensions"></InputText>

            <div style="display: flex; gap: 10px">
                <DatePicker placeholder="Start Date (optional)" v-model="startDate" />
                <DatePicker placeholder="End Date (optional)" v-model="endDate" />
            </div>

            <Button style="width: 100px; align-self: center" @click="start">Start</Button>
        </div>

        <div v-if="viewMode == 'loading'" class="loading-container">
            <div style="font-size: 24px;">Loading..</div>
        </div>

        <div v-if="viewMode == 'stats'" class="stats-container">
            <D3RepoLineChangesOverTime :data="chartData" />
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
