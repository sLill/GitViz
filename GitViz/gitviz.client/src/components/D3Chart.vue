<script setup>
    import { ref, onMounted } from 'vue';
    import * as d3 from 'd3';

    const chartRef = ref(null);
    const data = ref([12, 5, 6, 6, 9, 10]);

    // Methods
    const createChart = () => {
      const svg = d3.select(chartRef.value)
        .attr('width', 400)
        .attr('height', 300);

      svg.selectAll('rect')
        .data(data.value)
        .join('rect')
        .attr('x', (d, i) => i * 70)
        .attr('y', (d) => 300 - 10 * d)
        .attr('width', 65)
        .attr('height', (d) => d * 10)
        .attr('fill', 'green');
    };

    const updateData = (newData) => {
        data.value = newData;
        createChart();
    };

    defineExpose({ updateData });

    onMounted(() => {
      createChart();
    });


</script>

<template>
  <div>
    <svg ref="chartRef"></svg>
  </div>
</template>