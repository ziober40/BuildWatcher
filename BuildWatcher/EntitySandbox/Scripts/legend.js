if (!d3.chart) d3.chart = {};

d3.chart.legend = function () {
    var g;
    var data;
    var width = 800;
    var height = 30;
    var dispatch = d3.dispatch(chart, "legend");


    function chart(container) {
        g = container;
        update();
    }

    chart.update = update;

    function update() {
        $(".element").empty();

        //var cValue = function (d) {
        //    return d.BuildDefinitionName;
        //}
        //    , color = d3.scale.category10();

        //data.forEach(function (d) {
        //    color(cValue(d));
        //});

        var rects = g.selectAll(".legend")
        .data(updatebuildschart.graphColor.domain());

        d3.chart.legend.rectsScale = rects;

        rects.enter()
        .append("g");

        rects.transition()
        .attr({
            x: width,
            width: 0,
            height: 0,
            "class":"element"
        });

        // draw legend text
        rects.append("text")
            .attr("x", -80)
            .attr("y", function (d, i) {
                return i * 15.5+ 10;
            })
            .style("text-anchor", "end")
            .text(function (d, i) {
                return d;
            });

        rects.append("rect")
            .attr("x", -70)
            .attr("y", function (d, i) {
                return i * 15.5;
            })
            .attr("width", 12)
            .attr("height", 12)
            .style("fill", function (d) {
                return updatebuildschart.graphColor(d);
            });

    }

    chart.data = function (value) {
        if (!arguments.length) return data;
        data = value;
        return chart;
    }

    return d3.rebind(chart, dispatch, "on");
}