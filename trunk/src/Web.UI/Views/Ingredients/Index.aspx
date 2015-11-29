<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Consumentor.ShopGun.Domain.Ingredient>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	ListIngredients
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>All Ingredients</h2>

    <table>
        <tr>
            <th>
                Id
            </th>
            <th>
                IngredientName
            </th>
            <th>
                LastUpdated
            </th>
            <th></th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td>
                <%= Html.Encode(item.Id) %>
            </td>
            <td>
                <%= Html.Encode(item.IngredientName) %>
            </td>
            <td>
                <%= Html.Encode(String.Format("{0:g}", item.LastUpdated)) %>
            </td>
            <td>
                <%= Html.ActionLink("Edit", "Edit", new { id=item.Id }) %>
                <%--<%= Html.ActionLink("Details", "Details", new { id=item.Id })%>--%>
            </td>
        </tr>
    
    <% } %>

    </table>

    <p>
        <%= Html.ActionLink("Create New", "Create") %>
    </p>

</asp:Content>

