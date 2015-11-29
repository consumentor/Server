<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Admin.master" Inherits="System.Web.Mvc.ViewPage<Consumentor.ShopGun.Domain.Mentor>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	EditAdvisor
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>EditAdvisor</h2>

    <% using (Html.BeginForm()) {%>

        <fieldset>
            <legend>Fields</legend>
            
                <%= Html.HiddenFor(model => model.Id) %>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.MentorName) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.MentorName) %>
                <%= Html.ValidationMessageFor(model => model.MentorName) %>
            </div>
            
            <%= Html.HiddenFor(model => model.Description) %>
            <%= Html.HiddenFor(model => model.Url) %>
            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

