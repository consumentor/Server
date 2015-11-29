<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Consumentor.ShopGun.Domain" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Shopgun
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
<div id="content" class="clear-block">
    <div class="grid_12">    
        <h2 class="title">Welcome to Shopgun Information System!</h2>
    </div>
    <div id="intro" class="grid_12">
        <p>Click on <% = Html.ActionLink("Advices", "Index", "Advice") %> to see and change your advices.</p>
        <table>
            <tr>
                <th>
                    Search terms
                </th>
                <th>
                    Hits
                </th>
            </tr>

        <%
            var topSearchterms = ViewData["TopSearchterms"] as IList<Searchterm>;
            foreach (var item in topSearchterms) { %>
    
            <tr>
                <td>
                    <%= Html.Encode(item.Searchstring) %>
                </td>
                <td>
                    <%= Html.Encode(item.SearchStatistics.Count) %>
                </td>
            </tr>
    
        <% } %>

        </table>
    </div>
</div>

</asp:Content>
