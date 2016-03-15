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
        <div id="settingGraphWrapper" class="container-fluid">

            <h2>Confusion matrix results based on setting changes</h2>

        </div><!--Container fluid-->

         <div id="lagGraphWrapper" class="container-fluid">

            <h2>Processing v received time lag testing</h2>

        </div><!--Container fluid-->
        
	<script type="text/template" id="headRow">        
		<div id={{rowId}} class="row headRow">
		</div>
	</script>
		
	<script type="text/template" id="headRowItem">        
		<div class="col-md-2">
			<div id={{legendId}} class="chart-legend"></div>
		</div>
		<div class="col-md-4">
			<h3 id={{titleId}} class="chartTitle"></h3>
		</div>
	</script>
	
	<script type="text/template" id="mainRow">        
		<div id={{rowId}} class="row">
		</div>
	</script>
	
	<script type="text/template" id="mainRowItem">        
		<div  class="col-md-6">
			<canvas id={{titleId}} width="400" height="400"></canvas>
		</div>
	</script>
	
        
        
    </body>
</html>
