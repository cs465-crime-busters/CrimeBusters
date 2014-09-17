<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="CrimeBusters.WebApp.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="Content/themes/illinoisTheme/jquery-ui-1.10.4.custom.min.css" rel="stylesheet" />
    <link href="Content/index.css" rel="stylesheet" />
    <link href="Content/bootstrap.css" rel="stylesheet" />
    <link href="Content/bootstrap.css.map" rel="stylesheet" />
    <title>Crime Buster</title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="main">
            <div id="dashboard">
            <header>
                <h3>Crime Buster</h3>
                <div id="rightMenu">
                    Welcome <asp:LoginName ID="loginName" runat="server" />! | 
                    <div id="user" class="dropdown">
                        <span class="dropdown-toggle" data-toggle="dropdown">More Options</span>
                        <ul class="dropdown-menu">
                            <li><a href="#" id="allAlerts" data-reporttype="all">All Alerts</a></li>
                            <li><a href="#" id="hiAlerts" data-reporttype="high">High Alerts</a></li>
                            <li><a href="#" id="loAlerts" data-reporttype="low">Low Alerts</a></li>
                            <li class="divider"></li>
                            <li><a href="#" id="showReports">All Reports</a></li>
                            <li class="divider"></li>
                            <li><a href="#" id="signOut">Sign Out</a></li>
                        </ul>
                    </div>
                </div>
            </header>
            </div>
            <div id="map"></div>
        </div>

        <div id="uploadedMediaWindow" style="display: none">

        </div>
        <div id="reportsDashboard" style="display: none"></div>
    </form>
    <script src="https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false"></script>
    <script src="Scripts/jquery-1.10.2.js"></script>
    <script src="Scripts/bootstrap.min.js"></script>
    <script src="Scripts/jquery-ui-1.10.4.custom.min.js"></script>
    <script src="Scripts/index.js"></script>
</body>
</html>
