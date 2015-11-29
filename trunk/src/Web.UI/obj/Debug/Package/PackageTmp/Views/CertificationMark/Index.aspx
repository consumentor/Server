<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Consumentor.ShopGun.Domain.CertificationMark>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Index</h2>

    <p>
        <%= Html.ActionLink("Create New", "CreateCertificationMark") %>
    </p>

    <table>
        <tr>
            <th>
                CertificationName
            </th>
            <th></th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td>
                <%= Html.Encode(item.CertificationName) %>
            </td>
            <td>
                <%= Html.ActionLink("Edit", "EditCertificationMark", new { id=item.Id }) %> |
                <%= Html.ActionLink("Delete", "DeleteCertificationMark", new { id=item.Id }) %>
            </td>
        </tr>
    
    <% } %>

    </table>
</asp:Content>

