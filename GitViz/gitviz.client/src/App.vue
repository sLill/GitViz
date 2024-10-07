<script setup>
    import { ref } from 'vue';
    import D3OverallVelocity from '/src/components/D3/D3OverallVelocity.vue';
    import D3AuthorVelocity from '/src/components/D3/D3AuthorVelocity.vue';
    import useEndpointService from './composables/services/useEndpointService.js';
    import useDialogService from './composables/services/useDialogService.js';
    import { useToast } from 'primevue/usetoast';

    const endpointService = useEndpointService();
    const dialogService = useDialogService();
    const toast = useToast();

    const viewMode = ref('init');
    const chartData = ref(null);
    
    const graphOptions = ref([null, 'Overall Velocity', 'Author Velocity']);
    const selectedGraph = ref(null);
    
    const localRepoPath = ref(null);
    const branchName = ref(null);
    const startDate = ref();
    const endDate = ref();
    const ignoreWhitespace = ref(true);
    const fileExtensions = ref(null);

    // Methods
    const start = async () => {
        viewMode.value = 'loading';

        const data = await getGraphData();
        if (data)
            showChart(data);
        else {
            toast.add({ severity: 'error', summary: null, detail: 'Repo analysis failed', life: 3000 });
            viewMode.value = 'init';
        }
    };

    const getGraphData = async () => {
        let data = null;

        switch (selectedGraph.value) {
            case 'Overall Velocity':
                data = await getOverallVelocity();
            break;
            case 'Author Velocity':
                data = await getAuthorVelocity();
            break;
        }

        return data;
    };

    const getOverallVelocity = async () => {
        let uri = `/api/v1/repo/getOverallVelocity?localRepoPath=${localRepoPath.value}&ignoreWhitespace=${ignoreWhitespace.value}`;
        
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
        return result ? result.data.json : null;
    };

    const getAuthorVelocity = async () => {
        let uri = `/api/v1/repo/getAuthorVelocity?localRepoPath=${localRepoPath.value}&ignoreWhitespace=${ignoreWhitespace.value}`;
        
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
        return result ? result.data.json : null;
    };

    const showChart = (json) => {
        const resultJson = JSON.parse(json);
        chartData.value = resultJson;

        viewMode.value = 'stats';
    };

</script>

<template>
    <div class="wrapper">
        <div class="visualization-container">
            <div style="font-size: 20px;">Visualization</div>
            <Dropdown style="width: 100%" :options="graphOptions" v-model="selectedGraph" placeholder="Select a Graph" v-on:change="viewMode = 'init'"></Dropdown>
        </div>

        <div v-if="viewMode == 'init'" class="init-container"> 

            <div style="font-size: 20px;">Local Repo Path</div>
            <InputText style="width: 100%" v-model="localRepoPath"></InputText>

            <div style="font-size: 20px;">Branch Name</div>
            <InputText style="width: 100%" v-model="branchName" placeholder="-- optional --"></InputText>

            <div style="font-size: 20px;">File Extensions (space delimited)</div>
            <InputText style="width: 100%" v-model="fileExtensions" placeholder="-- optional (default: all) --"></InputText>

            <div style="display: flex; gap: 10px">
                <div>
                    <div style="font-size: 20px; margin-bottom: 10px;">Start Date</div>
                    <DatePicker placeholder="-- optional --" v-model="startDate" />
                </div>

                <div>
                    <div style="font-size: 20px; margin-bottom: 10px;">End Date</div>
                    <DatePicker placeholder="-- optional --" v-model="endDate" />
                </div>
            </div>

            <div style="display: flex;">
                <Checkbox v-model="ignoreWhitespace" :binary="true" />
                <div style="margin: 0 0 0 10px;">Ignore Whitespace</div>
            </div> 

            <Button style="width: 100px; align-self: center" @click="start">Start</Button>
        </div>

        <div v-if="viewMode == 'loading'" class="loading-container">
            <div style="font-size: 24px;">Loading. This may take some time..</div>
        </div>

        <div v-if="viewMode == 'stats'" class="stats-container">
            <D3OverallVelocity v-if="selectedGraph == 'Overall Velocity'" :data="chartData" />
            <D3AuthorVelocity v-if="selectedGraph == 'Author Velocity'" :data="chartData" />
        </div>
    </div>

    <DynamicDialog />
    <Toast />
</template>

<style scoped>
/* * { border: 2px red solid; } */

.wrapper {
    display: flex;
    flex-direction: column;
    flex-wrap: wrap;
    gap: 15px;
    width: 100%;
    height: 100%;
}

.visualization-container {
    display: flex;
    width: 80%;
    
    flex-direction: column;
    align-items: start;
    justify-content: left;
    
    gap: 15px;
}

.init-container {
    width: 80%;

    display: flex; 
    flex-direction: column; 
    /* align-items: center; */
    gap: 15px;
}

.loading-container {
    width: 80%;

    display: flex; 
    flex-direction: column; 
    align-items: center;
}

.stats-container {
    width: 80%;
    height: 80%; 
    
    display: flex; 
    flex-direction: column; 
    align-items: center;
}
</style>
