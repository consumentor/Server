<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Consumentor.ShopGun.Domain.User>" %>
<%@ Import Namespace="Consumentor.ShopGun.Domain" %>
<link href="../../Content/css/Site.css" rel="stylesheet" type="text/css" />
             <div class="editor-label">
                <%= Html.LabelFor(model => model.UserName) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.UserName) %>
                <%= Html.ValidationMessageFor(model => model.UserName) %>
            </div>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.FirstName) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.FirstName) %>
                <%= Html.ValidationMessageFor(model => model.FirstName) %>
            </div>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.LastName) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.LastName) %>
                <%= Html.ValidationMessageFor(model => model.LastName) %>
            </div>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.DisplayName) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.DisplayName) %>
                <%= Html.ValidationMessageFor(model => model.DisplayName) %>
            </div>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.Email) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.Email) %>
                <%= Html.ValidationMessageFor(model => model.Email) %>
            </div>

            <div class="editor-label">
                <label for="MentorId">Mentor</label>
            </div>
            <div class="editor-field">
                <%= Html.DropDownListFor(model => model.MentorId, ViewData["Mentor"]as SelectList, "Choose the user's organisation...") %>
            </div>
            
            <div class="">
    <%
        var userRoles = ViewData["UsersRoles"] as string[] ?? new string[]{};
        foreach (var role in ViewData["Role"] as IEnumerable<Role>) { %>
            <%= Html.CheckBox("Roles", userRoles.Contains(role.RoleName), new{id = role.RoleName, value = role.RoleName})%>
            <label for="<%= role.RoleName %>" title="<%= role.RoleDescription %>"><%= role.RoleName %></label>
        <br />
    <% } %>
</div>