<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Consumentor.ShopGun.Domain.User>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">


    <p>
        <%= Html.ActionLink("Create New", "CreateUser") %>
    </p>


    <h2>Index</h2>

    <table>
        <tr>
             <th>
                UserName
            </th>
            <th>
                FirstName
            </th>
            <th>
                LastName
            </th>
            <th>
                DisplayName
            </th>
            <th>
                Email
            </th>
            <th>
                Mentor
            </th>
            <th></th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
           
            <td>
                <%= Html.Encode(item.UserName) %>
            </td>
            <td>
                <%= Html.Encode(item.FirstName) %>
            </td>
            <td>
                <%= Html.Encode(item.LastName) %>
            </td>
            <td>
                <%= Html.Encode(item.DisplayName) %>
            </td>
            <td>
                <%= Html.Encode(item.Email) %>
            </td>
            <td>
                <%
                var mentorName = item.Mentor == null ? "n/a" : item.Mentor.MentorName; %>
                <%= Html.Encode(mentorName) %>
            </td>
             <td>
                <%= Html.ActionLink("Edit", "EditUser", new { id=item.Id }) %> |
                <%= Html.ActionLink("Delete", "DeleteUser", new { id=item.Id })%>
            </td>
        </tr>
    
    <% } %>

    </table>

</asp:Content>

