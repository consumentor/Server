<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Consumentor.ShopGun.Domain.IngredientAdvice>" %>
<%@ Import Namespace="Consumentor.ShopGun.Domain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<style type='text/css'>

.display-label
{
	padding-top:5px
}

.display-field
{
	border-width:thin;
	border-style:solid;	
	width:450px;
}

</style>

    <h2>Details</h2>

    <fieldset>
        <legend>Fields</legend>
        
        <div class="display-label">Ingredient</div>
        <div class="display-field"><%= Html.Encode((ViewData["Ingredient"] as Ingredient).IngredientName) %></div>
        
        <div class="display-label">Label</div>
        <div class="display-field"><%= Html.Encode(Model.Label) %></div>
        
        <div class="display-label">Introduction</div>
        <div class="display-field"><%= Html.Encode(Model.Introduction) %></div>
        
        <div class="display-label">Advice</div>
        <div class="display-field"><%= Html.Encode(Model.Advice) %></div>
        
        <div class="display-label">Key words</div>
        <div class="display-field"><%= Html.Encode(Model.KeyWords) %></div>
        
        <div class="display-label">Signal</div>
        <div class="display-field"><%= Html.Encode(Model.Semaphore.ColorName) %></div>
        
        <div class="display-label">Published</div>
        <div class="display-field"><%= Html.Encode(Model.Published) %></div>
        
    </fieldset>
    <p>
        <%=Html.ActionLink("Edit", "Edit", new { id=Model.Id }) %> |
        <%=Html.ActionLink("Back to List", "Index") %>
    </p>

</asp:Content>

