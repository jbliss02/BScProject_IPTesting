var data;

var template = "<h1>{{firstName}} {{lastName}}</h1>Blog: {{blogURL}}";

$(document).ready(function () {
    drawSettingData();
    drawLagData();
});


var options = {

    ///Boolean - Whether grid lines are shown across the chart
    scaleShowGridLines: true,

    //String - Colour of the grid lines
    scaleGridLineColor: "rgba(0,0,0,.05)",

    //Number - Width of the grid lines
    scaleGridLineWidth: 1,

    //Boolean - Whether to show horizontal lines (except X axis)
    scaleShowHorizontalLines: true,

    //Boolean - Whether to show vertical lines (except Y axis)
    scaleShowVerticalLines: true,

    //Boolean - Whether the line is curved between points
    bezierCurve: true,

    //Number - Tension of the bezier curve between points
    bezierCurveTension: 0.4,

    //Boolean - Whether to show a dot for each point
    pointDot: true,

    //Number - Radius of each point dot in pixels
    pointDotRadius: 4,

    //Number - Pixel width of point dot stroke
    pointDotStrokeWidth: 1,

    //Number - amount extra to add to the radius to cater for hit detection outside the drawn point
    pointHitDetectionRadius: 20,

    //Boolean - Whether to show a stroke for datasets
    datasetStroke: true,

    //Number - Pixel width of dataset stroke
    datasetStrokeWidth: 2,

    //Boolean - Whether to fill the dataset with a colour
    datasetFill: true,

    //String - A legend template
    legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].strokeColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>",

    responsive: true,

    // Boolean - whether to maintain the starting aspect ratio or not when responsive, if set to false, will take up entire container

    maintainAspectRatio: false,

};



function drawSettingData(){
    var that = this;

    $.ajax({
        url: 'http://localhost:9001/api/testdata',
        dataType: 'application/text',
        complete: function (data) {
            that.data = JSON.parse(data.responseText);
            that.doDrawSettingChart();
        },

    });
}

function doDrawSettingChart() {

    for (var i = 0; i < this.data.list.length; i++) {
        var chartArea = document.getElementById("chart" + i).getContext("2d");
        myLineChart = new Chart(chartArea).Line(data.list[i], options);
        document.getElementById('chart' + i +'-legend').innerHTML = myLineChart.generateLegend();
        $('#chart' + i + '-title').text(data.list[i].chartTitle);
    }
}


function drawLagData() {
    var that = this;

    $.ajax({
        url: 'http://localhost:9001/api/testlag',
        dataType: 'application/text',
        complete: function (data) {
            that.data = JSON.parse(data.responseText);
            that.doDrawLagChart();
        },

    });
}

function doDrawLagChart() {

    for (var i = 0; i < this.data.list.length; i++) {
        var chartArea = document.getElementById("chart" + i + "b").getContext("2d");
        myLineChart = new Chart(chartArea).Line(data.list[i], options);
        document.getElementById('chart' + i + 'b-legend').innerHTML = myLineChart.generateLegend();
        $('#chart' + i + '-title').text(data.list[i].chartTitle);
    }
}

