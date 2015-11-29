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
        <%=Html.ResourceLink("movies", "Edit", new { id=Model.Id }, ActionType.GetUpdateForm)%> |
        <% using (Html.BeginResourceForm("movies", new { id=Model.Id }, new { name = "deleteForm" }, ActionType.Delete)) { %>
            <a href="javascript:document.deleteForm.submit();">Delete</a> |
            <%= Html.HttpMethodOverride(ActionType.Delete)%>
        <% } %>
        <%=Html.ResourceLink("movies", "Back to List", new { }, ActionType.Index)%>
    </p>

</asp:Content>

