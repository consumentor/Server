<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Admin.master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Consumentor.ShopGun.Domain.Mentor>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Index</h2>

    <p>
        <%= Html.ActionLink("Create New", "CreateAdvisor") %>
    </p>


    <table>
        <tr>
            <th>
                MentorName
            </th>
            <th>
                Url
            </th>
            <th></th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td>
                <%= Html.Encode(item.MentorName) %>
            </td>
            <td>
                <%= Html.Encode(item.Url) %>
            </td>
            <td>
                <%= Html.ActionLink("Edit", "EditAdvisor", new { id=item.Id }) %>
            </td>
        </tr>
    
    <% } %>

    </table>

   
</asp:Content>

