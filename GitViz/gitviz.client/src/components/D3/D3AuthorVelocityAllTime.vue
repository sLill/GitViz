<template>
    <div ref="chartContainer" class="chart-container"></div>
  </template>
  
  <script setup>
  import { ref, onMounted, onUnmounted, watch } from 'vue';
  import * as d3 from 'd3';
  
  const props = defineProps({
    data: {
      type: Object,
      required: true
    }
  });
  
  const chartContainer = ref(null);
  let svg = null;
  
  const drawChart = () => {
    if (!chartContainer.value) return;
  
    const containerWidth = chartContainer.value.clientWidth;
    const containerHeight = chartContainer.value.clientHeight;
  
    const margin = { top: 40, right: 20, bottom: 120, left: 60 };
    const width = containerWidth - margin.left - margin.right;
    const height = containerHeight - margin.top - margin.bottom;
  
    // Clear previous SVG
    d3.select(chartContainer.value).selectAll('*').remove();
  
    // Create SVG with a subtle gradient background
    svg = d3.select(chartContainer.value)
      .append('svg')
      .attr('width', containerWidth)
      .attr('height', containerHeight)
      .append('g')
      .attr('transform', `translate(${margin.left},${margin.top})`);
  
    // Add gradient background
    const defs = svg.append('defs');
    const gradient = defs.append('linearGradient')
      .attr('id', 'bg-gradient')
      .attr('x1', '0%')
      .attr('y1', '0%')
      .attr('x2', '0%')
      .attr('y2', '100%');
    gradient.append('stop')
      .attr('offset', '0%')
      .attr('style', 'stop-color:#f8f9fa;stop-opacity:1');
    gradient.append('stop')
      .attr('offset', '100%')
      .attr('style', 'stop-color:#e9ecef;stop-opacity:1');
  
    svg.append('rect')
      .attr('width', width + margin.left + margin.right)
      .attr('height', height + margin.top + margin.bottom)
      .attr('transform', `translate(${-margin.left},${-margin.top})`)
      .style('fill', 'transparent');
  
    // Process data
    const processedData = Object.entries(props.data).flatMap(([name, items]) =>
      Object.entries(items).map(([item, value]) => ({ name, item, value }))
    );
  
    // Set up scales
    const x0 = d3.scaleBand()
      .domain(processedData.map(d => d.name))
      .rangeRound([0, width])
      .paddingInner(0.1);
  
    const x1 = d3.scaleBand()
      .domain(Object.keys(props.data[Object.keys(props.data)[0]]))
      .rangeRound([0, x0.bandwidth()])
      .padding(0.05);
  
    const y = d3.scaleLinear()
      .domain([0, d3.max(processedData, d => d.value)])
      .nice()
      .rangeRound([height, 0]);
  
    const color = d3.scaleOrdinal()
      .domain(Object.keys(props.data[Object.keys(props.data)[0]]))
      .range(['#35a721', '#a72121']);
  
    // Draw bars
    svg.append('g')
      .selectAll('g')
      .data(Object.entries(props.data))
      .join('g')
        .attr('transform', d => `translate(${x0(d[0])},0)`)
      .selectAll('rect')
      .data(d => Object.entries(d[1]).map(([key, value]) => ({ key, value })))
      .join('rect')
        .attr('x', d => x1(d.key))
        .attr('y', d => y(d.value))
        .attr('width', x1.bandwidth())
        .attr('height', d => height - y(d.value))
        .attr('fill', d => color(d.key))
        .attr('rx', 4)
        .attr('ry', 4)
      .on('mouseover', function(event, d) {
        d3.select(this).attr('opacity', 0.8);
        svg.append('text')
          .attr('class', 'value-label')
          .attr('x', x0(d.name) + x1(d.key) + x1.bandwidth() / 2)
          .attr('y', y(d.value) - 5)
          .attr('text-anchor', 'middle')
          .text(d.value);
      })
      .on('mouseout', function() {
        d3.select(this).attr('opacity', 1);
        svg.selectAll('.value-label').remove();
      });
  
    // Add x-axis with vertical labels
    svg.append('g')
      .attr('transform', `translate(0,${height})`)
      .call(d3.axisBottom(x0))
      .selectAll('text')
        .style('text-anchor', 'end')
        .attr('dx', '-.8em')
        .attr('dy', '.15em')
        .attr('transform', 'rotate(-65)')
        .style('font-size', '12px')
        .style('fill', '#495057');
  
    // Add y-axis
    svg.append('g')
      .call(d3.axisLeft(y).ticks(null, 's'))
      .style('font-size', '12px')
      .call(g => g.select('.domain').remove())
      .call(g => g.selectAll('.tick line').clone()
          .attr('x2', width)
          .attr('stroke-opacity', 0.1))
      .append('text')
        .attr('x', -margin.left + 10)
        .attr('y', -20)
        .attr('fill', '#495057')
        .attr('font-weight', 'bold')
        .attr('text-anchor', 'start')
        .text('Lines of Code');
  
    // Add legend
    const legend = svg.append('g')
      .attr('font-family', 'sans-serif')
      .attr('font-size', 10)
      .attr('text-anchor', 'end')
      .selectAll('g')
      .data(Object.keys(props.data[Object.keys(props.data)[0]]).slice().reverse())
      .join('g')
        .attr('transform', (d, i) => `translate(0,${i * 20})`);
  
    legend.append('rect')
      .attr('x', width - 19)
      .attr('width', 19)
      .attr('height', 19)
      .attr('fill', color)
      .attr('rx', 4)
      .attr('ry', 4);
  
    legend.append('text')
      .attr('x', width - 24)
      .attr('y', 9.5)
      .attr('dy', '0.32em')
      .attr('fill', 'white')
      .text(d => d == 'Item1' ? 'Additions' : 'Deletions');
  };
  
  const handleResize = () => {
    drawChart();
  };
  
  onMounted(() => {
    drawChart();
    window.addEventListener('resize', handleResize);
  });
  
  onUnmounted(() => {
    window.removeEventListener('resize', handleResize);
  });
  
  watch(() => props.data, drawChart, { deep: true });
  </script>
  
  <style scoped>
  .chart-container {
    width: 100%;
    height: 100%;
    font-family: Arial, sans-serif;
  }
  </style>