<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Consumentor.ShopGun.Domain.CategoryInfo>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>CategoryInfo</h2>
    
    <p>
        <%= Html.ActionLink("Create New", "CreateCategoryInfo") %>
    </p>

    <table>
        <tr>
            <th>
                CategoryName
            </th>
            <th>
                InfoText
            </th>
            <th></th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td>
                <%= Html.Encode(item.CategoryName) %>
            </td>
            <td>
                <%= Html.Encode(item.InfoText) %>
            </td>
               <td>
                <%= Html.ActionLink("Edit", "EditCategoryInfo", new { id=item.Id }) %> |
                <%= Html.ActionLink("Delete", "DeleteCategoryInfo", new { id=item.Id })%>
            </td>
        </tr>
    
    <% } %>

    </table>

</asp:Content>

