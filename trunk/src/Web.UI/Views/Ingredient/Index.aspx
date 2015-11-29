<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Consumentor.ShopGun.Domain.Ingredient>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	All ingredients
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>All Ingredients</h2>

    <p>
        <%= Html.ActionLink("Create New", "CreateIngredient") %>
    </p>

    <table>
        <tr>
            <th>
                Ingredient Name
            </th>
            <th>
                Number advices
            </th>
            <th></th>
        </tr>

    <% foreach (var ingredient in ViewData.Model) { %>
    
        <tr>
            <td>
                <%= Html.ActionLink(ingredient.IngredientName, "Index", "IngredientAdvice", new { ingredientId = ingredient.Id }, null)%>
                <%--<%= Html.Encode(item.IngredientName) %>--%>
            </td>
            <td>
                <%= Html.Encode(ingredient.IngredientAdvices.Count)%>
            </td>
            <td>
                <%= Html.ActionLink("Edit", "EditIngredient", new { id = ingredient.Id })%>
                |
                <%= Html.ActionLink("Delete", "DeleteIngredient", new { id=ingredient.Id })%>
            </td>
        </tr>
    
    <% } %>

    </table>

</asp:Content>

