﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<Consumentor.ShopGun.Domain.BrandAdvice>" %>
<%@ Import Namespace="Consumentor.ShopGun.Domain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Create</h2>

    <% using (Html.BeginForm()) {%>

        <fieldset>
            <legend>Fields</legend>
            
            <div class="editor-label">
                <%= Html.Label("Brand") %>
            </div>
            <div class="editor-field">
                <%= Html.DropDownListFor(brandAdvice => brandAdvice.BrandsId, ViewData["Brands"] as SelectList, "Choose a brand...", new { @class = "combobox" })%>
                <%= Html.ValidationMessageFor(brandAdvice => brandAdvice.BrandsId) %>
            </div>
            
            <% Html.RenderPartial("CreateAdvice"); %>
            
            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%=Html.ActionLink("Back to List", "Index", "Advice") %>
    </div>

</asp:Content>

