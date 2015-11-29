<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Consumentor.ShopGun.Domain.Mentor>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Index</h2>

    <p>
        <%=Html.ActionLink("Edit", "Edit") %> |
    </p>

    <fieldset>
        <legend>Fields</legend>
        
        <div class="display-label">My organisation's name</div>
        <div class="display-field"><%= Html.Encode(Model.MentorName) %></div>
        
        <div class="display-label">Description</div>
        <div class="display-field"><%= Html.Encode(Model.Description) %></div>
        
        <div class="display-label">Homepage</div>
        <div class="display-field"><%= Html.Encode(Model.Url) %></div>
        
    </fieldset>
</asp:Content>

