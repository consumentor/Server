<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<Consumentor.ShopGun.Domain.CategoryInfo>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    DeleteCategoryInfo
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        DeleteCategoryInfo</h2>
    <% using (Html.BeginForm())
       {%>
    <fieldset>
        <legend>Fields</legend>Delete this CategoryInfo?
        <br />
        <br />
        <div>
        <%= Html.Encode(Model.CategoryName) %>
        </div>
        <br />
        <div>
        <%= Html.Encode(Model.InfoText) %>
        </div>
        <br />
        <p>
            <input type="submit" value="Delete" />
        </p>
    </fieldset>
    <% } %>
</asp:Content>
