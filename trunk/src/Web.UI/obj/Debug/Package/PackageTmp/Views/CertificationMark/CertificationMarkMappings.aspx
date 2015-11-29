<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Consumentor.ShopGun.Domain.OpvCertificationMarkMapping>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	CertificationMarkMappings
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>CertificationMarkMappings</h2>
    
     <p>
        <%= Html.ActionLink("Add mapping", "AddOpvCertificationMarkMapping") %>
    </p>


    <table>
        <tr>
            <th>
                CertificationMark Name
            </th>
            <th>
                Certifier
            </th>
            <th>
                OPV's MarkId
            </th>
            <th>
                OPV's MarkName
            </th>
            <th></th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td>
                <%= Html.Encode(item.CertificationMark.CertificationName) %>
            </td>
            <td>
                <%= Html.Encode(item.CertificationMark.Certifier.MentorName) %>
            </td>
            <td>
                <%= Html.Encode(item.ProviderCertificationId) %>
            </td>
            <td>
                <%= Html.Encode(item.ProviderCertificationName) %>
            </td>
             <td>
                <%= Html.ActionLink("Delete", "DeleteCertificationMarkMapping", new { id=item.Id })%>
            </td>
        </tr>
    
    <% } %>

    </table>

    <p>
        <%= Html.ActionLink("Create New", "Create") %>
    </p>

</asp:Content>

