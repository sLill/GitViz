<script setup>
  import { ref, onMounted, reactive, onUnmounted } from 'vue';
  import * as d3 from 'd3';

  const chartRef = ref(null);
  const data = reactive([12, 5, 6, 6, 9, 10]);

  const margin = { top: 20, right: 20, bottom: 30, left: 40 };
  let width = 0;
  let height = 0;
  let svg, xScale, yScale;

  // Methods
  const updateDimensions = () => {
    const container = chartRef.value.parentElement;
    width = container.clientWidth - margin.left - margin.right;
    height = container.clientHeight - margin.top - margin.bottom;
  };

  const createChart = () => {
    updateDimensions();

    svg = d3.select(chartRef.value)
      .attr('viewBox', `0 0 ${width + margin.left + margin.right} ${height + margin.top + margin.bottom}`)
      .append('g')
      .attr('transform', `translate(${margin.left},${margin.top})`);

    xScale = d3.scaleBand()
      .range([0, width])
      .padding(0.1);

    yScale = d3.scaleLinear()
      .range([height, 0]);

    updateChart();
  };

  const updateChart = () => {
    xScale.domain(data.map((_, i) => i));
    yScale.domain([0, d3.max(data)]);

    svg.selectAll('*').remove(); // Clear previous content

    svg.selectAll('rect')
      .data(data)
      .join('rect')
      .attr('x', (d, i) => xScale(i))
      .attr('y', d => yScale(d))
      .attr('width', xScale.bandwidth())
      .attr('height', d => height - yScale(d))
      .attr('fill', 'green');

    // Add x-axis
    svg.append('g')
      .attr('transform', `translate(0,${height})`)
      .call(d3.axisBottom(xScale));

    // Add y-axis
    svg.append('g')
      .call(d3.axisLeft(yScale));
  };

  const resizeHandler = () => {
    updateDimensions();
    updateChart();
  };

  onMounted(() => {
    createChart();
    window.addEventListener('resize', resizeHandler);
  });

  onUnmounted(() => {
    window.removeEventListener('resize', resizeHandler);
  });

  // Method to update data and redraw chart
  const updateData = (newData) => {
    Object.assign(data, newData);
    updateChart();
  };

  defineExpose({ updateData });
</script>

<template>
  <div class="wrapper">
    <div class="container-component fill">
      <svg ref="chartRef" class="svg fill"></svg>
    </div>
  </div>
</template>

<style scoped>
svg {

}
</style>