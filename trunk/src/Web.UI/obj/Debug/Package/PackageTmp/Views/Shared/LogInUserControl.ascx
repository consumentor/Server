<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<ul>
<%
    if (Request.IsAuthenticated) {
%>
        <li><%= Html.ActionLink("Welcome " + Html.Encode(Page.User.Identity.Name), "ChangePassword", "Account") %></li>
        <li><%= Html.ActionLink("Log out", "LogOut", "Account") %><li>
<%
    }
    else {
%> 
        <li><%= Html.ActionLink("Log in", "LogIn", "Account") %></li>
<%
    }
%>
</ul>