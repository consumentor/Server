<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Consumentor.ShopGun.Domain.Product>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Products</h2>
    
    <p>
        <%= Html.ActionLink("Create new product", "CreateProduct") %>
    </p>
    
    <% Html.RenderPartial("ProductGrid"); %>
    
    <%--
    <table>
        <tr>
            <th>
                GTIN
            </th>
            <th>
                ProductName
            </th> 
            <th>
                Brand
            </th>
            <th>
                Company
            </th>
             <th>
                Number advices
            </th>
            <th>
                Actions
            </th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td>
                <%= Html.Encode(item.GlobalTradeItemNumber) %>
            </td>
           <td>
                <%= Html.Encode(item.ProductName) %>
            </td>
            <td>
                <%= Html.Encode(item.Brand.BrandName) %>
            </td>
            <td>
                <%= Html.Encode(item.Brand.Owner != null ? item.Brand.Owner.CompanyName : "") %>
            </td>
            
           <td>
                <%= Html.Encode(item.ProductAdvices.Count) %>
            </td>
            <td>
                <%= Html.ActionLink("Edit", "EditProduct", new { id=item.Id }) %>
            </td>
        </tr>
    
    <% } %>

    </table>--%>
</asp:Content>