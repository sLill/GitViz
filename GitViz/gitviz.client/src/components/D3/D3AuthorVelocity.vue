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

  const svg = d3.select(chartRef.value)
    .append('svg')
    .attr('width', '100%')
    .attr('height', '100%')
    .attr('viewBox', `0 0 ${width.value} ${height.value}`)
    .append('g')
    .attr('transform', `translate(${margin.left},${margin.top})`);

  const parseDate = d3.timeParse('%Y-%m-%dT%H:%M:%S');
  const data = Object.entries(props.data).map(([date, values]) => {
    return {
      date: parseDate(date),
      ...Object.fromEntries(Object.entries(values).map(([name, { Item1 }]) => [name, Item1]))
    };
  });

  const persons = Array.from(new Set(data.flatMap(d => Object.keys(d)).filter(key => key !== 'date')));

  const x = d3.scaleTime().range([0, innerWidth]);
  const y = d3.scaleLinear().range([innerHeight, 0]);
  const color = d3.scaleOrdinal(d3.schemeCategory10);

  const line = d3.line()
    .curve(d3.curveMonotoneX)
    .x(d => x(d.date))
    .y(d => y(d.value));

  x.domain(d3.extent(data, d => d.date));
  y.domain([
    d3.min(data, d => d3.min(Object.values(d).filter(v => typeof v === 'number'))),
    d3.max(data, d => d3.max(Object.values(d).filter(v => typeof v === 'number')))
  ]);

  svg.append('g')
    .attr('class', 'x axis')
    .attr('transform', `translate(0,${innerHeight})`)
    .call(d3.axisBottom(x));

  svg.append('g')
    .attr('class', 'y axis')
    .call(d3.axisLeft(y))
    .append('text')
    .attr('transform', 'rotate(-90)')
    .attr('y', 6)
    .attr('dy', '.71em')
    .style('text-anchor', 'end')
    .text('Item1 Value');

  persons.forEach(person => {
    const personData = data.map(d => ({
      date: d.date,
      value: d[person] || 0
    })).filter(d => d.value !== 0);

    if (personData.length > 0) {
      svg.append('path')
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

  const legend = svg.append('g')
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
        .text(d);
    });

  // Add hover effects
  svg.selectAll('.line')
    .on('mouseover', function() {
      const person = d3.select(this).attr('data-person');
      svg.selectAll('.line').style('opacity', 0.1);
      d3.select(this).style('opacity', 1).style('stroke-width', 4);
      legend.selectAll('.legend-item').style('display', 'none');
      legend.select(`.legend-item[data-person="${person}"]`).style('display', 'block');
    })
    .on('mouseout', function() {
      svg.selectAll('.line').style('opacity', 1).style('stroke-width', 2);
      legend.selectAll('.legend-item').style('display', 'block');
    });

  // Add hover effects to legend items
  legend.selectAll('.legend-item')
    .on('mouseover', function() {
      const person = d3.select(this).attr('data-person');
      svg.selectAll('.line').style('opacity', 0.1);
      svg.select(`.line[data-person="${person}"]`).style('opacity', 1).style('stroke-width', 4);
      legend.selectAll('.legend-item').style('display', 'none');
      d3.select(this).style('display', 'block');
    })
    .on('mouseout', function() {
      svg.selectAll('.line').style('opacity', 1).style('stroke-width', 2);
      legend.selectAll('.legend-item').style('display', 'block');
    });
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