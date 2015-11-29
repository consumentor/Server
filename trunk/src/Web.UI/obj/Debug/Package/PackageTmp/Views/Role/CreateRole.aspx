<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Consumentor.ShopGun.Domain.Role>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	CreateRole
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>CreateRole</h2>

    <% using (Html.BeginForm()) {%>

        <fieldset>
            <legend>Fields</legend>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.RoleName) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.RoleName) %>
                <%= Html.ValidationMessageFor(model => model.RoleName) %>
            </div>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.RoleDescription) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.RoleDescription) %>
                <%= Html.ValidationMessageFor(model => model.RoleDescription) %>
            </div>
            
            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

