<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Consumentor.ShopGun.Domain.Brand>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    BrandIndex
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        BrandIndex</h2>
    <p>
        <%= Html.ActionLink("Create New", "CreateBrand") %>
    </p>
    <table>
        <tr>
            <th>
                Brand name
            </th>
            <th>
                Brand owner
            </th>
            <th>
                Number advices
            </th>
            <th>
            </th>
        </tr>
        <% foreach (var item in Model)
           { %>
        <tr>
            <td>
                <%= Html.Encode(item.BrandName) %>
            </td>
            <td>
                <%= Html.Encode(item.Owner != null ? item.Owner.CompanyName : "N/A") %>
            </td>
            <td>
                <%= Html.Encode(item.BrandAdvices.Count) %>
            </td>
            <td>
                <%= Html.ActionLink("Edit", "EditBrand", new { id=item.Id }) %>
                |
                <%= Html.ActionLink("Delete", "DeleteBrand", new { id=item.Id })%>
            </td>
        </tr>
        <% } %>
    </table>
</asp:Content>
