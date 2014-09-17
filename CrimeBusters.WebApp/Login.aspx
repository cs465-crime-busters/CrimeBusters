<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="CrimeBusters.WebApp.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="Content/themes/illinoisTheme/jquery-ui-1.10.4.custom.min.css" rel="stylesheet" />
    <link href="Content/index.css" rel="stylesheet" />
    <title>Welcome to Crime Busters</title>
</head>
<body id="login">
    <form id="form1" runat="server">
        <div id="mainLogin">
            <img id="logo" alt="Illinois Logo" src="/Content/images/ilogo_horz_bold.gif" />
            <h2 id="crimebusterHeader">Crime Busters</h2>
            <div id="innerLogin">
                <h4 id="loginHeader">Please Log In</h4>
                <input id="userName" type="text" placeholder="username" class="required" />
                <input id="password" type="password" placeholder="password" class="required" />
                <input id="rememberMe" type="checkbox" /><label for="rememberMe">Remember me</label>
                <input id="loginButton" type="submit" value="Login" />
            </div>
        </div>
    </form>
    <script src="Scripts/jquery-1.10.2.js"></script>
    <script src="Scripts/jquery-ui-1.10.4.custom.min.js"></script>
    <script src="Scripts/login.js"></script>
</body>
</html>
