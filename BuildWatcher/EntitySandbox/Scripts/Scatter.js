if (!d3.chart) d3.chart = {};

d3.chart.scatter = function () {
    var g;
    var data;
    var width = 850;
    var height = 400;
    var cx = 10;
    var numberBins = 5;
    var dispatch = d3.dispatch(chart, "hover");

    function chart(container) {
        g = container;
        g.append("g")
        .classed("xaxis", true);
        g.append("g")
        .classed("yaxis", true);

        update();
    }

    chart.update = update;

    function returnColourBasedOnStatus(status) {
        if (status === "Succeeded") {
            return "green";
        }
        else {
            return "red";
        }
    }

    function attachToolTip(statusCircles, div) {
        statusCircles.on("mouseover", function (d) {
            //d3.select(this).style("fill", '#E98300');
            console.log(d);
            div.transition()
                .duration(200)
                .style("opacity", .9);
            div.html("Build Name: <b>" + d.LabelName + "</b><br/>" + "Build Duration: <b>" + d.DurationInMinutes + "m </b><br/>Build StartTime: <b>" + d.StartTime + "</b>")
                .style("left", (d3.event.pageX) + "px")
                .style("top", (d3.event.pageY) + "px");
            dispatch.hover([d]);
        });
        statusCircles.on("mouseout", function (d) {
            // d3.select(this).style("fill", '#0079C2');
            div.transition()
            .duration(500)
            .style("opacity", 0);
            dispatch.hover([]);
        });
    }

    function update() {
        var maxStartTime = d3.max(data, function (d) { return d.StartTime; });
        var minStartTime = d3.min(data, function (d) { return d.StartTime; });
        var maxDuration = d3.max(data, function (d) { return d.DurationInMinutes; });
        var colorScale = d3.scale.category20();
        var startTimeScale = d3.time.scale()
            .domain([minStartTime, maxStartTime])
            .range([cx, width]);
        var blocksCoveredScale = d3.scale.linear()
        .domain(d3.extent(data, function (d) { return d.DurationInMinutes; }))
        .range([3, 15]);
        var yScale = d3.scale.linear()
          .domain([0, maxDuration])
          .range([height, cx]);
        var xAxis = d3.svg.axis()
       .scale(startTimeScale)
       .ticks(5)
       .tickFormat(d3.time.format("%x %H:%M"));
        var yAxis = d3.svg.axis()
        .scale(yScale)
        .ticks(3)
        .orient("left");
        var xg = g.select(".xaxis")
          .classed("axis", true)
          .attr("transform", "translate(" + [0, height] + ")")
          .transition()
          .call(xAxis);
        var yg = g.select(".yaxis")
          .classed("axis", true)
          .classed("yaxis", true)
          .attr(
          {
              transform: "translate(" + [cx - 5, 0] + ")"
          })
          .transition()
          .call(yAxis);

        var circles = g.selectAll("circle")
        .data(data, function (d) {
            return d.Uri;
        });

        circles.enter()
        .append("circle");

        circles
        .transition()
        .attr({
            cx: function (d, i) {
                return startTimeScale(d.StartTime);
            },
            cy: function (d, i) {
                return yScale(d.DurationInMinutes);
            },
            r: function (d) {
                return blocksCoveredScale(d.DurationInMinutes);
            },
            fill: function (d) {
                    return updatebuildschart.graphColor(updatebuildschart.cValue(d));
            }
        });

        var div = d3.select("body").append("div")
            .attr("class", "tooltip")
            .style("opacity", 0);

        if (updatebuildschart.showStatusOnly) {
            var statusCircles = g.selectAll("statusCircle")
            .data(data, function (d) {
                return d.Uri;
            });

            statusCircles.enter().append("circle");

            statusCircles
           .transition()
           .attr({
               cx: function (d, i) {
                   return startTimeScale(d.StartTime);
               },
               cy: function (d, i) {
                   return yScale(d.DurationInMinutes);
               },
               r: function (d) {
                   return blocksCoveredScale(d.DurationInMinutes);
               },
               fill: function (d) {
                   return returnColourBasedOnStatus(d.CompilationStatus);
               }
           });
           statusCircles.exit().remove();
           attachToolTip(statusCircles,div);
        }

        circles.exit().remove();
        attachToolTip(circles, div);

        //update histogram chart
        //var hist = d3.layout.histogram()
        //.value(function (d) { return d.BuildDuration; })
        //.range([0, d3.max(data, function (d) { return d.BuildDuration })])
        //.bins(numberBins);
        //var layout = hist(data);
        //for (var i = 0; i < layout.length; i++) {
        //    var bin = layout[i];
        //    g.selectAll("circle")
        //    .data(bin, function (d) { return d; })
        //    .style("fill", function () { return colorScale(i); });
        //}

    }

    chart.highlight = function (data) {
        var circles = g.selectAll("circle")
        .style("stroke", "");

        circles.data(data, function (d) { return d.Uri; })
        .style("stroke", "orange")
        .style("stroke-width", 3);
    }

    chart.data = function (value) {
        if (!arguments.length) return data;
        data = value;
        return chart;
    }
    chart.width = function (value) {
        if (!arguments.length) return width;
        width = value;
        return chart;
    }
    chart.height = function (value) {
        if (!arguments.length) return height;
        height = value;
        return chart;
    }

    return d3.rebind(chart, dispatch, "on");
}