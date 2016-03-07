<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="analyse.aspx.cs" Inherits="ImageAdminWeb.analyse" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="https://code.jquery.com/jquery-2.2.1.min.js"></script>
    <link href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/mustache.js/2.2.1/mustache.min.js"></script>
    <script src="Scripts/Chartjs/Chart.js"></script>
    <script src="Scripts/document.js"></script>
    <link type="text/css" rel="stylesheet" href="Content/site.css" />
</head>
<body>

        <div class="container-fluid">
        <h2>Confusion matrix results based on setting changes</h2>

        <div id="graphWrapper">
            <div class="row headRow">
                <!--Graph 1 header -->
                <div class="col-md-2">
                    <div id="chart0-legend" class="chart-legend"></div>
                </div>
                <div class="col-md-4" >
                    <h4 id="chart0-title" class="chartTitle"></h4>
                </div>

                 <!--Graph 2 header -->
                <div class="col-md-2">
                    <div id="chart1-legend" class="chart-legend"></div>
                </div>
                <div class="col-md-4" >
                    <h4 id="chart1-title" class="chartTitle"></h4>
                </div>
            </div>
            <div class="row">
                <!--Graph 1  -->
                <div  class="col-md-6">
                    <canvas id="chart0" width="400" height="400"></canvas>
                </div>
                 <!--Graph 2  -->
                <div  class="col-md-6">
                    <canvas id="chart1" width="400" height="400"></canvas>
                </div>
            </div>

            <div class="row headRow">
                <!--Graph 1 header -->
                <div class="col-md-2">
                    <div id="chart2-legend" class="chart-legend"></div>
                </div>
                <div class="col-md-4" >
                    <h4 id="chart2-title" class="chartTitle"></h4>
                </div>

                 <!--Graph 2 header -->
                <div class="col-md-2">
                    <div id="chart3-legend" class="chart-legend"></div>
                </div>
                <div class="col-md-4" >
                    <h4 id="chart3-title" class="chartTitle"></h4>
                </div>
            </div>
            <div class="row">
                <!--Graph 1  -->
                <div  class="col-md-6">
                    <canvas id="chart2" width="400" height="400"></canvas>
                </div>
                 <!--Graph 2  -->
                <div  class="col-md-6">
                    <canvas id="chart3" width="400" height="400"></canvas>
                </div>
            </div>


        </div><!-- Graph wrapper -->



    </div>


        
        
        
        
</body>
</html>
