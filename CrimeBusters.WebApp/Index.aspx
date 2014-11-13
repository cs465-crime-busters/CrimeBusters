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
                    <h3>International Crime Busters</h3>
                    <div id="rightMenu">
                        Welcome
                        <asp:LoginName ID="loginName" runat="server" />
                        ! |  <a href="#" id="signOut">Sign Out</a>
                    </div>
                </header>
            </div>
            <div id="map"></div>
            <div id="lowerSection">
                <div id="mainSearch" title="Click on a crime report to zoom to it on the map">
                    <h6 class="roundCorners">Events & Search</h6>
                    <div id="searchPanel">
                        <ul>
                            <li><a id="emergenciesTab" href="#emergencies">EMERGENCIES</a></li>
                            <li><a id="crimesTab" href="#crimes">CRIMES</a></li>          
                            <li><a id="searchTab" href="#search">SEARCH</a></li>
                        </ul>
                        <div id="emergencies">
                            <ul></ul>
                        </div>
                        <div id="crimes">
                            <ul></ul>
                        </div>  
                        <div id="search">
                            <asp:TextBox ID="fromDate" runat="server" placeholder="From Date" CssClass="roundCorners"></asp:TextBox>
                            <%-- <input id="fromDate" type="text" placeholder="From Date" class="roundCorners" /> --%>
                            <input id="altFieldFrom" type="hidden" />
                            <asp:TextBox ID="toDate" runat="server" placeholder="To Date" CssClass="roundCorners"></asp:TextBox>
                            <%-- <input id="toDate" type="text" placeholder="To Date" class="roundCorners" /> --%>
                            <input id="altFieldTo" type="hidden" />
                            <div id="reportTypeSelection">
                                <asp:RadioButtonList ID="CrimeTypeList" runat="server" RepeatDirection="Horizontal" CssClass="radio-inline">
                                    <asp:ListItem Selected="True" Value="1">Emergency</asp:ListItem>
                                    <asp:ListItem Value="2">Crime</asp:ListItem>
                                </asp:RadioButtonList>
                            
  <%--                              <input id="highPriorityRadio" type="radio" name="reportType" value="1" checked="checked" /><label for="highPriorityRadio">Emergency</label>
                                <input id="lowPriorityRadio" type="radio" name="reportType" value="2" /><label for="lowPriorityRadio">Crime</label>--%>
                            
                            </div>
                            <asp:Button ID="searchByDateButton" runat="server" Text="Search" /><br />
                            <div id="searchResultList" class="roundCorners"><ul></ul></div>
                            
                            <input id="ShowOnMapButton" type="button" value="Show on Map" hidden="hidden" onclick="$.showOnMap()"/>
                            <asp:Label ID="ResultLabel" runat="server"></asp:Label>
                            
                            <!--<input type="button" value="Search" /> -->
                        </div>
                    </div>
                </div>
                <div id="mainPreview">
                    <h6 class="roundCorners">Preview</h6>
                    <div id="previewPanel">
      <%--                  <img src="/Content/uploads/testImage.JPG" alt="Uploaded Image" height="170" width="460" />--%>
                        
<%--                        <video width='460' height='170' controls>
                            <source src="/Content/uploads/TestVideo.mp4" type='video/mp4' />
                            <source src="/Content/uploads/TestVideo.mp4" type='video/ogg' />
                            <source src="/Content/uploads/TestVideo.mp4" type='video/webm' /> 
                        Your browser does not support the video tag.</video>--%>

<%--                        <audio controls>
                            <source src="/Content/uploads/testAudio.ogg" type="audio/mp3" />
                            <source src="/Content/uploads/testAudio.ogg" type="audio/ogg" />
                            <source src="/Content/uploads/testAudio.ogg"  type="audio/mpeg" />
                            Your browser does not support the audio element.
                        </audio>--%>
                    </div>
                </div>
            </div>
        </div>

        <div id="uploadedMediaWindow" style="display: none">
        </div>
        <div id="reportsDashboard" style="display: none"></div>
        <div id="editProfilePanel" style="display: none">
            <input id="firstName" />
            <input id="lastName" />
            <input id="phoneNumber" />
        </div>

    </form>
    <script src="https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false"></script>
    <script src="Scripts/jquery-1.10.2.js"></script>
    <script src="Scripts/bootstrap.min.js"></script>
    <script src="Scripts/jquery-ui-1.10.4.custom.min.js"></script>
    <script src="Scripts/index.js"></script>
</body>
</html>
