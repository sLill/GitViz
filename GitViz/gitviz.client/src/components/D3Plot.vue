<template>
  <div>
    <svg ref="chartRef"></svg>
  </div>
</template>

<script setup>
import { ref, onMounted, reactive } from 'vue';
import * as d3 from 'd3';

const chartRef = ref(null);
const data = reactive([12, 5, 6, 6, 9, 10]);

const createChart = () => {
  const svg = d3.select(chartRef.value)
    .attr('width', 400)
    .attr('height', 300);

  svg.selectAll('rect')
    .data(data)
    .join('rect')
    .attr('x', (d, i) => i * 70)
    .attr('y', (d) => 300 - 10 * d)
    .attr('width', 65)
    .attr('height', (d) => d * 10)
    .attr('fill', 'green');
};

onMounted(() => {
  createChart();
});

// If you need to expose methods to the parent component, you can define them here
const updateData = (newData) => {
  Object.assign(data, newData);
  createChart();
};

// Expose methods or data if needed
defineExpose({ updateData });
</script>