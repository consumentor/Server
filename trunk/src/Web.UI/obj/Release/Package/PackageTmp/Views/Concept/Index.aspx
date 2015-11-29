<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Consumentor.ShopGun.Domain.Concept>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	All ingredients
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>All Concepts</h2>

    <table>
        <tr>
            <th>
                Concept Term
            </th>
            <th>
                Last Updated
            </th>
            <th></th>
        </tr>

    <% foreach (var concept in ViewData.Model) { %>
    
        <tr>
            <td>
                <%= Html.ActionLink(concept.ConceptTerm, "Index", "ConceptAdvice", new { conceptId = concept.Id }, null)%>
                <%--<%= Html.Encode(item.IngredientName) %>--%>
            </td>
            <td>
                <%= Html.Encode(String.Format("{0:g}", concept.LastUpdated))%>
            </td>
            <td>
                <%= Html.ActionLink("Edit", "Edit", new { id = concept.Id })%>
                <%--<%= Html.ActionLink("Details", "Details", new { id=item.Id })%>--%>
            </td>
        </tr>
    
    <% } %>

    </table>

    <p>
        <%= Html.ActionLink("Create New", "Create") %>
    </p>

</asp:Content>

