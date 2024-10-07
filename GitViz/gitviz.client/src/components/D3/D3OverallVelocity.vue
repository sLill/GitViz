<script setup>
import { ref, onMounted, defineExpose, onUnmounted } from 'vue';
import * as d3 from 'd3';

const props = defineProps({
  data: {
    type: Object,
    required: true
  }
});

const chartRef = ref(null);
const chartContainer = ref(null);
let resizeObserver;

// Methods
const setupResizeObserver = () => {
  resizeObserver = new ResizeObserver(entries => {
    for (let entry of entries) {
      drawChart();
    }
  });

  resizeObserver.observe(chartContainer.value);
};

const drawChart = () => {
  if (!chartContainer.value) 
  return;

  const containerRect = chartContainer.value.getBoundingClientRect();
  const width = containerRect.width;
  const height = containerRect.height;

  // Process the data
  const processedData = Object.entries(props.data).map(([date, values]) => ({
    date: new Date(date),
    item1: values.Item1,
    item2: values.Item2
  }));

  // Sort data by date
  processedData.sort((a, b) => a.date - b.date);

  // Set up chart dimensions
  const margin = { top: 40, right: 80, bottom: 60, left: 80 };
  const chartWidth = width - margin.left - margin.right;
  const chartHeight = height - margin.top - margin.bottom;

  // Clear any existing SVG
  d3.select(chartRef.value).selectAll("*").remove();

  // Create SVG
  const svg = d3.select(chartRef.value)
    .append("svg")
    .attr("width", width)
    .attr("height", height)
    .append("g")
    .attr("transform", `translate(${margin.left},${margin.top})`);

  // Add a subtle gradient background
  svg.append("rect")
    .attr("width", chartWidth)
    .attr("height", chartHeight)
    .attr("fill", "url(bg-gradient)");

  // Define gradient
  const gradient = svg.append("defs")
    .append("linearGradient")
    .attr("id", "bg-gradient")
    .attr("x1", "0%")
    .attr("y1", "0%")
    .attr("x2", "0%")
    .attr("y2", "100%");

  gradient.append("stop")
    .attr("offset", "0%")
    .attr("stop-color", "#f3f4f6");

  gradient.append("stop")
    .attr("offset", "100%")
    .attr("stop-color", "#ffffff");

  // Set up scales
  const x = d3.scaleTime()
    .domain(d3.extent(processedData, d => d.date))
    .range([0, chartWidth]);

  const y = d3.scaleLinear()
    .domain([0, d3.max(processedData, d => Math.max(d.item1, d.item2))])
    .range([chartHeight, 0]);

  // Create lines
  const line1 = d3.line()
    .x(d => x(d.date))
    .y(d => y(d.item1))
    .curve(d3.curveCatmullRom.alpha(0.5));

  // Add lines to chart with transition
  const path1 = svg.append("path")
    .datum(processedData)
    .attr("fill", "none")
    .attr("stroke", "#3b82f6")
    .attr("stroke-width", 3)
    .attr("d", line1);

  const totalLength1 = path1.node().getTotalLength();
  path1.attr("stroke-dasharray", totalLength1 + " " + totalLength1)
    .attr("stroke-dashoffset", totalLength1)
    .transition()
    .duration(2000)
    .ease(d3.easeLinear)
    .attr("stroke-dashoffset", 0);

  // Add axes
  svg.append("g")
    .attr("transform", `translate(0,${chartHeight})`)
    .call(d3.axisBottom(x))
    .attr("class", "axis")
    .selectAll("text")
    .attr("y", 10)
    .attr("x", 9)
    .attr("dy", ".35em")
    .attr("transform", "rotate(45)")
    .style("text-anchor", "start");

  svg.append("g")
    .call(d3.axisLeft(y))
    .attr("class", "axis");

  // Add y-axis label
  svg.append('text')
    .attr('class', 'y-axis-label')
    .attr('text-anchor', 'middle')
    .attr('transform', 'rotate(-90)')
    .attr('y', -65)
    .attr('x', -chartHeight / 2)
    .style("fill", "white")
    .style("font-weight", "bold")
    .text('Lines of Code');

  // Add data points
  svg.selectAll(".dot1")
    .data(processedData)
    .enter().append("circle")
    .attr("class", "dot1")
    .attr("cx", d => x(d.date))
    .attr("cy", d => y(d.item1))
    .attr("r", 5)
    .attr("fill", "#3b82f6")
    .on("mouseover", function(event, d) {
      d3.select(this).attr("r", 8);
      tooltip.transition()
        .duration(200)
        .style("opacity", .9);
      tooltip.html(`Date: ${d.date.toDateString()}<br/>Item1: ${d.item1}`)
        .style("left", (event.pageX) + "px")
        .style("top", (event.pageY - 28) + "px");
    })
    .on("mouseout", function(d) {
      d3.select(this).attr("r", 5);
      tooltip.transition()
        .duration(500)
        .style("opacity", 0);
    });

  // Add legend
  const legend = svg.append("g")
    .attr("font-family", "sans-serif")
    .attr("font-size", 10)
    .attr("text-anchor", "end")
    .selectAll("g")
    .data(["Lines Added"])
    .enter().append("g")
    .attr("transform", (d, i) => `translate(${chartWidth},${i * 20})`);

  legend.append("rect")
    .attr("x", 20)
    .attr("width", 19)
    .attr("height", 19)
    .attr("fill", (d, i) => i === 0 ? "#3b82f6" : "#ef4444");

  legend.append("text")
    .attr("x", 14)
    .attr("y", 9.5)
    .attr("dy", "0.32em")
    .style("fill", "white")
    .style("font-size", "16px")
    .text(d => d);

  // Add chart title
  // svg.append("text")
  //   .attr("x", chartWidth / 2)
  //   .attr("y", -margin.top / 2)
  //   .attr("text-anchor", "middle")
  //   .style("font-size", "16px")
  //   .style("font-weight", "bold")
  //   .style("fill", "white")
  //   .text("R50 Code Velocity");

  // Add tooltip
  const tooltip = d3.select("body").append("div")
    .attr("class", "tooltip")
    .style("opacity", 0);
};

onMounted(() => {
  drawChart();
  setupResizeObserver();
});

onUnmounted(() => {
  if (resizeObserver)
    resizeObserver.disconnect();
});

defineExpose({ drawChart });
</script>

<template>
  <div ref="chartContainer" style="width: 100%; height: 100%;">
    <div ref="chartRef"></div>
  </div>
</template>

<style scoped>
.axis line, .axis path {
  stroke: #ccc;
}
.grid line {
  stroke: #e5e7eb;
  stroke-opacity: 0.7;
  shape-rendering: crispEdges;
}
.grid path {
  stroke-width: 0;
}
.tooltip {
  position: absolute;
  text-align: center;
  padding: 8px;
  font: 12px sans-serif;
  background: #f3f4f6;
  border: 1px solid #ccc;
  border-radius: 4px;
  pointer-events: none;
}
</style>