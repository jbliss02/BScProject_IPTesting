
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

function drawSettingData() {
    var that = this;

    $.ajax({
        url: 'http://localhost:9001/api/testdata',
        dataType: 'application/text',
        complete: function (data) {

            var settingData = JSON.parse(data.responseText);
            that.doDrawChart("settingGraphWrapper", settingData);

        },

    });
}

function drawLagData() {
    var that = this;

    $.ajax({
        url: 'http://localhost:9001/api/testlag',
        dataType: 'application/text',
        complete: function (data) {
            var lagData = JSON.parse(data.responseText);
            that.doDrawChart("lagGraphWrapper", lagData);
        },

    });
}

function doDrawChart(mainAnchorString, chartData) {

    //change the row ids from i so they dont double up

    for (var i = 0; i < chartData.list.length; i += 2) {
        var mainAnchor = $('#' + mainAnchorString); //the main Div which everything hangs off			
        drawHeadRow(mainAnchor, i);//draw the header row, for the container-fluid		
        var secondColumn = i + 1 < chartData.list.length; //whether this section has two columns

        //create the title and legend div arrays
        var legendDivIds = returnLegendDivIds(i, secondColumn);
        var titleDivIds = returnTitleDivIds(i, secondColumn);
        drawHeaderRows(legendDivIds, titleDivIds, i)

        //the the main content rows
        drawMainRow(mainAnchor, i); //draw the mainRow, the one that contains the graph				
        var mainSections = returnMainSectionArray(i);// an array of main sections
        drawMainRowItems(mainSections, i); //draw the main item divs

        //draw the charts, and get an array of their references
        var charts = drawCharts(mainSections, chartData, i);

        //draw the titles and legends
        drawLegends(legendDivIds, charts);
        drawTitle(titleDivIds[0], chartData.list[i].chartTitle);
        if (secondColumn) { drawTitle(titleDivIds[1], chartData.list[i].chartTitle); }

    }//for each chart in the list

}

function returnMainSectionArray(i) {

    var mainSections = new Array(2);
    mainSections[0] = "chart" + i;
    mainSections[1] = "chart" + (i + 1).toString();
    return mainSections;
}

function returnLegendDivIds(i, secondColumn) {

    var legendDivIds = secondColumn ? new Array(2) : new Array(1);
    legendDivIds[0] = returnLegendDivId(i);
    if (secondColumn) { legendDivIds[1] = returnLegendDivId(i + 1); }
    return legendDivIds;
}

function returnLegendDivId(i) {
    return "chart" + i + "-legend";
}

function returnTitleDivIds(i, secondColumn) {

    var titleDivIds = secondColumn ? new Array(2) : new Array(1);
    titleDivIds[0] = returnTitleDivId(i);
    if (secondColumn) { titleDivIds[1] = returnTitleDivId(i + 1); }
    return titleDivIds;
}

function returnTitleDivId(i) {
    return "chart" + i + "-title";
}

function drawHeadRow(mainAnchor, rowId) {

    var obj = new Object();
    obj.rowId = 'row' + rowId;
    var headRow = $('#headRow').html();
    mainAnchor.append(Mustache.render(headRow, obj));
}

function drawHeaderRows(legendDivIds, titleDivIds, rowId) {

    for (var i = 0; i < legendDivIds.length; i++) { //assumes legendDivIds and titleDivIds always same length

        var rowItem = new Object();
        rowItem.legendId = legendDivIds[i];
        rowItem.titleId = titleDivIds[i];
        var rowItemTemplate = $('#headRowItem').html();
        $('#row' + rowId).append(Mustache.render(rowItemTemplate, rowItem));
    }

}

function drawMainRow(mainAnchor, rowId) {

    var obj = new Object();
    obj.rowId = "mainRow" + rowId;
    var headRow = $('#mainRow').html();
    mainAnchor.append(Mustache.render(headRow, obj));
}

function drawMainRowItems(mainSections, rowId) {

    for (var i = 0; i < mainSections.length; i++) {

        var rowItemDetail = new Object();
        rowItemDetail.titleId = mainSections[i];
        var rowItemDetailTemplate = $('#mainRowItem').html();
        $('#' + "mainRow" + rowId).append(Mustache.render(rowItemDetailTemplate, rowItemDetail));
    }

}//drawMainRowItem

function drawLegends(legendDivIds, charts) {

    for (var i = 0; i < charts.length; i++) { //assume charts and legendDivIds are same length		
        document.getElementById(legendDivIds[i]).innerHTML = charts[i].generateLegend();
    }

}

function drawTitle(divId, chartTitle) {
    $('#' + divId).text(chartTitle);
}

function drawCharts(mainSections, chartData, i) {

    //work out how many charts to draw
    var max = chartData.list.length < mainSections.length ? chartData.list.length : mainSections.length;

    //draws the charts, and returns references in an array	
    var charts = new Array(max);

    for (var n = 0; n < max; n++) {
        var chartArea = document.getElementById(mainSections[n]).getContext("2d");
        charts[n] = new Chart(chartArea).Line(chartData.list[i + n], options);
    }

    return charts;

}//drawCharts

