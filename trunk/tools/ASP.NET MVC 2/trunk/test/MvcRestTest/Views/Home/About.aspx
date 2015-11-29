<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	About
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>About</h2>
    
    <p>
        This example MVC application demonstrates how to provide a web service API on top of an MVC website.
    </p>
    
    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>
</asp:Content>
