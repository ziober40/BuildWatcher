var updatebuildschart = updatebuildschart || {};

updatebuildschart = {
    submitHours: function(hours) {
        updatebuildschart.ispullallteams = false;
        updatebuildschart.rewrite(updatebuildschart.filterByHours(updatebuildschart.data, hours));
    }
    ,filterByHours: function (dataToBeFiltered, hours){
        var filteredData = [];

        dataToBeFiltered.forEach(function (obj) {
            var dateToCheck = (new Date()) - (new Date(0).setHours(hours));

            if(obj.StartTime.getTime() > dateToCheck){
                filteredData.push(obj);
            }
        });

        return filteredData;
    }
    ,rewrite: function (data) {
        // update title

        console.log("filtered", data);

        updatebuildschart.scatter.data(data);
        updatebuildschart.scatter.update();

        updatebuildschart.legend.data(data);
        updatebuildschart.legend.update();
    }
    ,initializeChart: function (data) {
        var svgtest = d3.select("svg");
        if (!svgtest.empty()) {
            $("svg").remove();
        }
        $('#builds').append('<svg></svg>');

        updatebuildschart.cValue = function (d) {
            return d.TeamProject;
        }

        updatebuildschart.graphColor = d3.scale.category10();
        
        var display = d3.select("#display");
        var svg = d3.select("svg");

        //title
        svg.append("text")
            .attr("x", "500")
            .attr("y", "0")
            .attr("id", "titleChart")
            .attr("text-anchor", "middle")
            .style("font-size", "16px");

        //scatter plot

        updatebuildschart.sgroup = svg.append("g")
            .attr("transform", "translate(50, 0)");
        updatebuildschart.scatter = d3.chart.scatter();
        updatebuildschart.scatter.data(data);
        updatebuildschart.scatter(updatebuildschart.sgroup);

        //legend
        updatebuildschart.lgroup = svg.append("g")
            .attr("transform", "translate(1170,0)");
        updatebuildschart.legend = d3.chart.legend();
        updatebuildschart.legend.data(data);
        updatebuildschart.legend(updatebuildschart.lgroup);

        //brush
        updatebuildschart.bgroup = svg.append("g")
            .attr("transform", "translate(100, 430)");
        updatebuildschart.brush = d3.chart.brush();
        updatebuildschart.brush
            .data(data)
            .width(700);
        updatebuildschart.brush(updatebuildschart.bgroup);
        updatebuildschart.brush.on("filter", updatebuildschart.rewrite);

        updatebuildschart.scatter.on("hover", function (hovered) {
            updatebuildschart.brush.highlight(hovered);
        });

        if (updatebuildschart.ispullallteams) {
            $('#teamProject').empty();
            $.each(d3.chart.legend.rectsScale[0], function (ind, obj) {
                $('#teamProject').append(function () {
                    return $('<li ><a role="menuitem" href="#">' + obj.textContent + '</a></li>').click(updatebuildschart.teamProjectHandler);
                });
            });
            $('#teamProject').append('<li><a role="menuitem" href="#" id="allteams">All Teams</a></li>');
        }
       
        $('#allteams').unbind().click(function () {
            updatebuildschart.setTheLabelTitleOfDropDownList('dropdownMenu1','All Teams');
            updatebuildschart.pullAllTeams();
        });

        $('#hoursfromnow').unbind().keypress(function (e) {
            if (e.which == '13') {
                updatebuildschart.submitHours(this.value);
            }
        });

        $('#statusDropDownStatus').unbind().click(function (e) {
            updatebuildschart.showStatusOnly = true;
            updatebuildschart.SetHighlighting(this.innerText);
            updatebuildschart.rewrite(updatebuildschart.data);
        });

        $('#statusDropDownTeam').unbind().click(function (e) {
            updatebuildschart.showStatusOnly = false;
            updatebuildschart.SetHighlighting(this.innerText);
            updatebuildschart.rewrite(updatebuildschart.data);
        });

    }
    , SetHighlighting: function (name) {
        updatebuildschart.setTheLabelTitleOfDropDownList('statusDropDown', name);
    }
    , setTheLabelTitleOfDropDownList: function (dropdownid, title) {
        $('#' + dropdownid).html(title + ' <span class="caret"></span>');
    }
    , teamProjectHandler: function (e) {
        updatebuildschart.setTheLabelTitleOfDropDownList('dropdownMenu1', e.target.innerText);
        updatebuildschart.pullTeam(e.target.innerText);
    }
    ,pullAllTeamsFromServer: function () {
        updatebuildschart.ispullallteams = true;
        
        $.ajax({
            async: true, 
            url: updatebuildschart.pulllastbuildschartcontroller,
            data: { amount: 2000 },
            type: 'POST',
            success: function (data) {
                data = updatebuildschart.changeDateFormat(data);
                updatebuildschart.rawData = data;
                updatebuildschart.data = [];
                updatebuildschart.rawData.forEach(function (d) {
                    updatebuildschart.data.push(jQuery.extend(true, {}, d));
                });
                
                updatebuildschart.initializeChart(updatebuildschart.data);
                $('#titleChart').html("All projects");
            }
        });
    }
    , showStatusOnly: function () {

    }
    , pullAllTeams: function () {
        updatebuildschart.ispullallteams = true;
        updatebuildschart.data = [];
        updatebuildschart.rawData.forEach(function (d) {
            updatebuildschart.data.push(jQuery.extend(true, {}, d));
        });
        updatebuildschart.initializeChart(updatebuildschart.data);
        $('#titleChart').html("All projects");
    }
    , pullTeam: function (team) {
        updatebuildschart.ispullallteams = false;
        updatebuildschart.data = [];
        updatebuildschart.rawData.forEach(function (d) {
            var clonedObject = jQuery.extend(true, {}, d);
            if (clonedObject.TeamProject == team) {
                clonedObject.TeamProject = d.BuildDefinitionName;
                updatebuildschart.data.push(clonedObject);
            }
        });
        updatebuildschart.initializeChart(updatebuildschart.data);
        $('#titleChart').html(team);
    }
    , pullTeamFromServer: function (team) {
            updatebuildschart.ispullallteams = false;
            $.ajax({
                async: true,
                url: updatebuildschart.pullteamcontroller,
                data: { amount: 1000, team: team },
                type: 'POST',
                success: function (data) {
                    data = updatebuildschart.changeDateFormat(data);
                    updatebuildschart.data = data;
                    updatebuildschart.initializeChart(updatebuildschart.data);
                    // set the title of chart
                    $('#titleChart').html(team);
                }
            });
    }
    , changeDateFormat: function (data) {
        $.each(data, function (i, d) {
            d.StartTime = new Date(parseInt(String(d.StartTime).substr(6)));
            d.FinishTime = new Date(parseInt(String(d.FinishTime).substr(6)));
            //d.BuildStartTime = new Date(d.BuildStartTime);
        });

        return data;
    }
    , initialize: function () {
        updatebuildschart.showStatusOnly = false;
        updatebuildschart.showByTeam = true;
        updatebuildschart.pullAllTeamsFromServer();
        
    }
}