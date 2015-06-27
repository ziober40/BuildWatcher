

var dependencyWheel = function () {
    var data = corelation,
        valueOfFirst = function (d) { return d.first; },
        valueOfSecond = function (d) { return d.second; },
        scaleFirst = d3.scale.category10(),
        scaleSecond = d3.scale.category10();

    data.forEach(function (d) {
        scaleFirst(valueOfFirst(d));
        scaleSecond(valueOfSecond(d));
    });
 }