<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateUserTemp.aspx.cs" Inherits="CrimeBusters.WebApp.CreateUserTemp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>Create User</h1>
            <asp:TextBox ID="UserNameTextBox" runat="server" placeholder="Username"></asp:TextBox>
            <br />
            <br />
            <asp:TextBox ID="PasswordTextBox" runat="server" placeholder="Password" TextMode="Password"></asp:TextBox>
            <br />
            <br />
            <asp:TextBox ID="EmailTextBox" runat="server" placeholder="Email"></asp:TextBox>
            <br />
            <br />
            <asp:TextBox ID="FirstNameTextBox" runat="server" placeholder="First Name"></asp:TextBox>
            <br />
            <br />
            <asp:TextBox ID="LastNameTextBox" runat="server" placeholder="Last Name"></asp:TextBox>
            <br />
            <br />
            <asp:TextBox ID="PhoneNumberTextBox" runat="server" placeholder="Phone Number"></asp:TextBox>
            <br />
            <br />
            <asp:RadioButtonList ID="RoleRadioButtonList" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem>Police</asp:ListItem>
                <asp:ListItem>User</asp:ListItem>
            </asp:RadioButtonList><br />
            <br />
            <asp:Button ID="CreateUserButton" runat="server" Text="Create User" OnClick="CreateUser_Click" />
            <asp:Label ID="ResultLabel" runat="server"></asp:Label>

        </div>
    </form>
</body>
</html>
