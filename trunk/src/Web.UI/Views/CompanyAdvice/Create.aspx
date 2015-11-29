<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<Consumentor.ShopGun.Domain.CompanyAdvice>" %>
<%@ Import Namespace="Consumentor.ShopGun.Domain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create New Company Advice
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div id="content" class="clear-block">
    <div class="grid_12"> 
        <h2 class="title">Create New Company Advice</h2>
    </div>
    <div id="advice" class="grid_12">
    <% using (Html.BeginForm()) {%>
            <div class="editor-label">
                <%= Html.Label("Company") %>
            </div>
            <div class="editor-field">
                <%= Html.DropDownListFor(companyAdvice => companyAdvice.CompanysId, ViewData["Companies"] as SelectList, "Choose a company...", new { @class = "combobox" })%>
                <%= Html.ValidationMessageFor(companyAdvice => companyAdvice.CompanysId) %>
            </div>
            
            <% Html.RenderPartial("CreateAdvice"); %>
            <div class="editor-label">&nbsp;</div>
            <div id="button_container" class="editor-field">
                <div id="right_button">
                    <%=Html.ActionLink("Cancel", "Index", "Advice") %>
                </div>
                <div>
                    <input type="submit" value="Create" />
                </div>
            </div>
             <div class="clear-block"></div>

    <% } %>
    </div>
</div>
</asp:Content>

