<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<Movie>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Details</h2>

    <fieldset>
        <legend>Fields</legend>
        <p>
            Id:
            <%= Html.Encode(Model.Id) %>
        </p>
        <p>
            Title:
            <%= Html.Encode(Model.Title) %>
        </p>
        <p>
            Director:
            <%= Html.Encode(Model.Director) %>
        </p>
        <p>
            DateReleased:
            <%= Html.Encode(String.Format("{0:g}", Model.DateReleased)) %>
        </p>
    </fieldset>
    <p>
        <% if (UserViewSettings.EditAllowed) { %>
        <%=Html.ActionLink("Edit", "Edit", new { id=Model.Id }) %> |
        <% } %>
        <% if (UserViewSettings.DeleteAllowed) { %>
        <% using (Html.BeginForm("Delete", "Home", new { id=Model.Id }, FormMethod.Post, new { name = "deleteForm" })) { %>
            <a href="javascript:document.deleteForm.submit();">Delete</a> |
        <% } %>
        <% } %>
        <%=Html.ActionLink("Back to List", "Index") %>
    </p>

</asp:Content>

