<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<Consumentor.ShopGun.Domain.IngredientAdvice>" %>
<%@ Import Namespace="Consumentor.ShopGun.Domain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit Advice
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div id="content" class="clear-block">
    <div class="grid_12">
        <h2 class="title">Edit ingredient advice for <%= Html.Encode((ViewData["Ingredient"] as Ingredient).IngredientName) %></h2>
    </div>
    <div id="advice" class="grid_12">
    <% using (Html.BeginForm()) {%>
        <%= Html.HiddenFor(model => model.Id, new{value = Model.Id}) %>
            
        <% Html.RenderPartial("CreateAdvice"); %>
        <div class="editor-label">&nbsp;</div>
        <div id="button_container" class="editor-field">
            <div id="right_button">
                <%=Html.ActionLink("Cancel", "Index", "Advice")%>
            </div>
            <div>
                <input type="submit" value="Save" />
            </div>
        </div>
        <div class="clear-block"></div>
    <% } %>   
    </div>
</div>
</asp:Content>

