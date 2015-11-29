<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Consumentor.ShopGun.Domain.Product>>" %>

    <table>
        <tr>
            <th></th>
           
            <th>
                Label
            </th>
            <th>
                Introduction
            </th>
            <th>
                Advice
            </th>
            <th>
                KeyWords
            </th>
            <th>
                SemaphoreId
            </th>
            <th>
                Published
            </th>
            <th>
                PublishDate
            </th>
            <th>
                UnpublishDate
            </th>
        </tr>

    <%-- <% foreach (var item in Model) { %>
     
        <tr>
            <td>
                <%= Html.Encode(item.ProductName) %>
            </td>
            <td></td> <td></td> <td></td> <td></td> <td></td> <td></td>
        </tr>    
          
        <% foreach (var advice in item.ProductAdvices) { %>
            <tr>
                <td></td>
                <td>
                    <%= Html.Encode(advice.Label) %>
                </td>
                <td>
                    <%= Html.Encode(advice.Introduction) %>
                </td>
                <td>
                    <%= Html.Encode(advice.Advice) %>
                </td>
                <td>
                    <%= Html.Encode(advice.KeyWords) %>
                </td>
                 <td>
                    <div class="<%= Html.Encode(advice.Semaphore.ColorName) %>"></div>
                </td>
                 <td>
                    <%= advice.Published ? "Yes" : "No"%>
                    <%= advice.Published ? Html.ActionLink("Unpublish", "Unpublish", new { id = advice.Id }) : Html.ActionLink("Publish", "Publish", new { id = advice.Id })%>
                </td>
                 <td>
                    <%= Html.ActionLink("Edit", "Edit", new { id = advice.Id })%> |
                    <%= Html.ActionLink("Details", "Details", new { id = advice.Id })%> |
                    <%= Html.ActionLink("Delete", "Delete", new { id = advice.Id })%>
                </td>
            </tr>
        <% } %>   
    <% } %>--%>

    </table>

    <p>
        <%= Html.ActionLink("Create New", "Create", "ProductAdvice") %>
    </p>


