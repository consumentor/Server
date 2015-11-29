﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<Consumentor.ShopGun.Domain.ConceptAdvice>" %>
<%@ Import Namespace="Consumentor.ShopGun.Domain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Edit concept advice for</h2>

    <div class="display-field">
    <h3><%= Html.Encode((ViewData["Concept"] as Concept).ConceptTerm) %></h3>
    </div>

    <% using (Html.BeginForm()) {%>

        <fieldset>
            <legend>Fields</legend>
            
            <%= Html.HiddenFor(model => model.Id, new{value = Model.Id}) %>
            
            <% Html.RenderPartial("CreateAdvice"); %>
            
            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

