var runningbuildstatus = runningbuildstatus || {};

runningbuildstatus = {
    $runningbuildswindow : $('#runningBuilds'),
    push: function (list) {
        var s = "";

        runningbuildstatus.$runningbuildswindow.empty();

        list.forEach(function (list) {
            s = s + e.BuildName + " \n ";
            s = s + e.BuildDuration + " \n ";
            s = s + e.AvarageBuildTime + " \n ";

            runningbuildstatus.$runningbuildswindow.append(runningbuildstatus.createBuildProgressString(e.BuildDuration, e.AvarageBuildTime, e.BuildName));
        });
    },
    createBuildProgressString : function(buildduration, averagebuildtime, buildname) {
       return "<div id=\"buildDetails\">" +
        buildname + " average build time:" + averagebuildtime + " build duration:" + buildduration +
        "</div>" +
        "<div class=\"progress\">" +
        "<div class=\"progress-bar\" id=\"progressBar\" role=\"progressbar\" aria-valuenow=\"90\" aria-valuemin=\"0\" aria-valuemax=\"100\" style=\"width: " + buildduration * 100 / averagebuildtime + "%;\">" +
        "<span class=\"sr-only\"></span>" +
        "</div>" +
        "</div>";
    }
}