<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Consumentor.ShopGun.Domain.Company>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Companies</h2>
    
    <p>
        <%= Html.ActionLink("Create New", "CreateCompany") %>
    </p>

    <table>
        <tr>
            <th>
                CompanyName
            </th>
           <th>
                Parent company
            </th>
            <th>
                ContactEmailAddress
            </th>            
            <th>
                URLToHomePage
            </th>
            <th>
                Country
            </th>
            <th></th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td>
                <%= Html.Encode(item.CompanyName) %>
            </td>
            <td>
                <%= Html.Encode(item.Owner != null ? item.Owner.CompanyName : "N/A") %>
            </td>
           <td>
                <%= Html.Encode(item.ContactEmailAddress) %>
            </td>          
            <td>
                <a href="<%= Html.Encode(item.URLToHomePage) %>"><%= Html.Encode(item.URLToHomePage) %></a>
            </td>
            <td>
                <%= Html.Encode(item.Country != null ? item.Country.CountryCode.Name : "N/A") %>
            </td>
            <td>
                <%= Html.ActionLink("Edit", "EditCompany", new { id=item.Id }) %>
                <%= Html.ActionLink("Delete", "DeleteCompany", new { id=item.Id }) %>
            </td>
        </tr>
    
    <% } %>

    </table>
    
</asp:Content>

