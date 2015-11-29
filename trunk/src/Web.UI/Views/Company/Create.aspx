<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Consumentor.ShopGun.Domain.Company>" %>
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
                <%= Html.LabelFor(model => model.CompanyName) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.CompanyName) %>
                <%= Html.ValidationMessageFor(model => model.CompanyName) %>
            </div>
            
            <div class="editor-label">
                <%= Html.LabelFor(company => company.ParentId) %>
            </div>
            <div class="editor-field">
                <%= Html.DropDownListFor(company => company.ParentId, new SelectList(ViewData["Companies"] as IList<Company>, "Id", "CompanyName"), "Select parent company")%>
                <%= Html.ValidationMessageFor(company => company.ParentId) %>
            </div>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.Address) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.Address) %>
                <%= Html.ValidationMessageFor(model => model.Address) %>
            </div>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.PostCode) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.PostCode) %>
                <%= Html.ValidationMessageFor(model => model.PostCode) %>
            </div>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.City) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.City) %>
                <%= Html.ValidationMessageFor(model => model.City) %>
            </div>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.PhoneNumber) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.PhoneNumber) %>
                <%= Html.ValidationMessageFor(model => model.PhoneNumber) %>
            </div>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.URLToHomePage) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.URLToHomePage) %>
                <%= Html.ValidationMessageFor(model => model.URLToHomePage) %>
            </div>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.ContactEmailAddress) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.ContactEmailAddress) %>
                <%= Html.ValidationMessageFor(model => model.ContactEmailAddress) %>
            </div>
            
            <%--<div class="editor-label">
                <%= Html.LabelFor(model => model.CountryId) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.CountryId) %>
                <%= Html.ValidationMessageFor(model => model.CountryId) %>
            </div>--%>
            
            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

