<script setup>
import { ref, onMounted, onUnmounted, watch } from 'vue';
import * as d3 from 'd3';

const props = defineProps({
  data: {
    type: Object,
    required: true
  }
});

const chartRef = ref(null);
const width = ref(0);
const height = ref(0);
const svg = ref(null);
const xScale = ref(null);
const yScale = ref(null);
const xAxis = ref(null);
const yAxis = ref(null);
const chartContent = ref(null);

const updateDimensions = () => {
  if (chartRef.value) {
    width.value = chartRef.value.clientWidth;
    height.value = chartRef.value.clientHeight;
  }
};

const drawChart = () => {
  if (!chartRef.value) return;

  const margin = { top: 20, right: 80, bottom: 30, left: 50 };
  const innerWidth = width.value - margin.left - margin.right;
  const innerHeight = height.value - margin.top - margin.bottom;

  d3.select(chartRef.value).selectAll('*').remove();

  svg.value = d3.select(chartRef.value)
    .append('svg')
    .attr('width', '100%')
    .attr('height', '100%')
    .attr('viewBox', `0 0 ${width.value} ${height.value}`);

  // Add a rect to capture zoom events on the entire chart area
  svg.value.append('rect')
    .attr('width', width.value)
    .attr('height', height.value)
    .style('fill', 'none')
    .style('pointer-events', 'all');

  const g = svg.value.append('g')
    .attr('transform', `translate(${margin.left},${margin.top})`);

  const parseDate = d3.timeParse('%Y-%m-%dT%H:%M:%S');
  const data = Object.entries(props.data).map(([date, values]) => {
    return {
      date: parseDate(date),
      ...Object.fromEntries(Object.entries(values).map(([name, { Item1 }]) => [name, Item1]))
    };
  });

  const persons = Array.from(new Set(data.flatMap(d => Object.keys(d)).filter(key => key !== 'date')));

  xScale.value = d3.scaleTime().range([0, innerWidth]);
  yScale.value = d3.scaleLinear().range([innerHeight, 0]);
  const color = d3.scaleOrdinal(d3.schemeCategory10);

  xScale.value.domain(d3.extent(data, d => d.date));
  yScale.value.domain([
    d3.min(data, d => d3.min(Object.values(d).filter(v => typeof v === 'number'))),
    d3.max(data, d => d3.max(Object.values(d).filter(v => typeof v === 'number')))
  ]);

  const clipPath = g.append('defs').append('clipPath')
    .attr('id', 'clip')
    .append('rect')
    .attr('width', innerWidth)
    .attr('height', innerHeight);

  chartContent.value = g.append('g')
    .attr('clip-path', 'url(#clip)');

  xAxis.value = g.append('g')
    .attr('class', 'x axis')
    .attr('transform', `translate(0,${innerHeight})`)
    .call(d3.axisBottom(xScale.value));

  yAxis.value = g.append('g')
    .attr('class', 'y axis')
    .call(d3.axisLeft(yScale.value));

  yAxis.value.append('text')
    .attr('transform', 'rotate(-90)')
    .attr('y', 6)
    .attr('dy', '.71em')
    .style('text-anchor', 'end')
    .text('Item1 Value');

  const line = d3.line()
    .curve(d3.curveMonotoneX)
    .x(d => xScale.value(d.date))
    .y(d => yScale.value(d.value));

  persons.forEach(person => {
    const personData = data.map(d => ({
      date: d.date,
      value: d[person] || 0
    })).filter(d => d.value !== 0);

    if (personData.length > 0) {
      chartContent.value.append('path')
        .datum(personData)
        .attr('class', 'line')
        .attr('d', line)
        .style('stroke', color(person))
        .style('fill', 'none')
        .style('stroke-width', 2)
        .attr('data-person', person);
    }
  });

  const legendWidth = 150;
  const legendItemHeight = 20;
  const legendHeight = persons.length * legendItemHeight;

  const legend = g.append('g')
    .attr('class', 'legend')
    .attr('transform', `translate(${innerWidth - legendWidth}, ${innerHeight / 2 - legendHeight / 2})`);

  legend.selectAll('.legend-item')
    .data(persons)
    .enter().append('g')
    .attr('class', 'legend-item')
    .attr('transform', (d, i) => `translate(0, ${i * legendItemHeight})`)
    .attr('data-person', d => d)
    .each(function(d) {
      d3.select(this).append('rect')
        .attr('width', 18)
        .attr('height', 18)
        .style('fill', color(d));
      
      d3.select(this).append('text')
        .attr('x', 24)
        .attr('y', 9)
        .attr('dy', '.35em')
        .style('text-anchor', 'start')
        .style('fill', 'white')
        .style('opacity', '0.5')
        .text(d);
    });

  // Add hover effects
  g.selectAll('.line')
    .on('mouseover', function() {
      const person = d3.select(this).attr('data-person');
      g.selectAll('.line').style('opacity', 0.1);
      d3.select(this).style('opacity', 1).style('stroke-width', 4);
      legend.selectAll('.legend-item').style('opacity', 0.1);
      legend.select(`.legend-item[data-person="${person}"]`).style('opacity', 1);
    })
    .on('mouseout', function() {
      g.selectAll('.line').style('opacity', 1).style('stroke-width', 2);
      legend.selectAll('.legend-item').style('opacity', 1);
    });

  // Add hover effects to legend items
  legend.selectAll('.legend-item')
    .on('mouseover', function() {
      const person = d3.select(this).attr('data-person');
      g.selectAll('.line').style('opacity', 0.1);
      g.select(`.line[data-person="${person}"]`).style('opacity', 1).style('stroke-width', 4);
      legend.selectAll('.legend-item').style('opacity', 0.1);
      d3.select(this).style('opacity', 1);
    })
    .on('mouseout', function() {
      g.selectAll('.line').style('opacity', 1).style('stroke-width', 2);
      legend.selectAll('.legend-item').style('opacity', 1);
    });

  // Add zoom behavior
  const zoom = d3.zoom()
    .scaleExtent([0.1, 10])
    .extent([[0, 0], [width.value, height.value]])
    .on('zoom', zoomed);

  svg.value.call(zoom);

  function zoomed(event) {
    const newXScale = event.transform.rescaleX(xScale.value);
    const newYScale = event.transform.rescaleY(yScale.value);
    
    xAxis.value.call(d3.axisBottom(newXScale));
    yAxis.value.call(d3.axisLeft(newYScale));
    
    chartContent.value.selectAll('.line')
      .attr('d', d3.line()
        .curve(d3.curveMonotoneX)
        .x(d => newXScale(d.date))
        .y(d => newYScale(d.value))
      );
  }
};

onMounted(() => {
  updateDimensions();
  window.addEventListener('resize', updateDimensions);
  drawChart();
});

onUnmounted(() => {
  window.removeEventListener('resize', updateDimensions);
});

watch([width, height], drawChart);
watch(() => props.data, drawChart, { deep: true });
</script>

<template>
  <div ref="chartRef" style="width: 100%; height: 100%;"></div>
</template>

<style scoped>
.line {
  fill: none;
  stroke-width: 2px;
}
</style>