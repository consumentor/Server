<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<Consumentor.Shopgun.Web.UI.Models.Account.LogInModel>" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Shopgun Information System
</asp:Content>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
<div id="content" class="clear-block">
    <div class="grid_12">
        <h2 class="title">Log in</h2>
    </div>
    <div id="login" class="grid_12">
        <p>
            Please enter your username and password. 
        </p>
        <%= Html.ValidationSummary(true, "Login was unsuccessful. Please correct the errors and try again.") %>

        <% using (Html.BeginForm()) { %>
            <div class="editor-label">
                <%= Html.LabelFor(m => m.UserName) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(m => m.UserName) %>
                <%= Html.ValidationMessageFor(m => m.UserName) %>
            </div>
            <div class="clear-block"></div>
            <div class="editor-label">
                <%= Html.LabelFor(m => m.Password) %>
            </div>
            <div class="editor-field">
                <%= Html.PasswordFor(m => m.Password) %>
                <%= Html.ValidationMessageFor(m => m.Password) %>
            </div>
            <div class="clear-block"></div>
            <div class="editor-label">&nbsp;</div>
            <div class="editor-field">
                <%= Html.CheckBoxFor(m => m.RememberMe) %>
                <%= Html.LabelFor(m => m.RememberMe) %>
            </div>
            <div class="clear-block"></div>
            <div id="login_button_container" class="editor-field">
                <input type="submit" value="Log On" />
            </div>
            <div class="clear-block"></div>
        <% } %>
    </div>
</div>
</asp:Content>
