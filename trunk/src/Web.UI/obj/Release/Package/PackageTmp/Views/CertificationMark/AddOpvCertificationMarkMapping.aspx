<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Consumentor.ShopGun.Domain.OpvCertificationMarkMapping>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	AddOpvCertificationMarkMapping
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>AddOpvCertificationMarkMapping</h2>

    <% using (Html.BeginForm()) {%>

        <fieldset>
            <legend>Fields</legend>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.CertificationMarkId) %>
            </div>
            <div class="editor-field">
                <%= Html.DropDownListFor(model => model.CertificationMarkId, ViewData["CertificationMarks"] as SelectList) %>
                <%= Html.ValidationMessageFor(model => model.CertificationMarkId) %>
            </div>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.ProviderCertificationId) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.ProviderCertificationId) %>
                <%= Html.ValidationMessageFor(model => model.ProviderCertificationId) %>
            </div>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.ProviderCertificationName) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.ProviderCertificationName) %>
                <%= Html.ValidationMessageFor(model => model.ProviderCertificationName) %>
            </div>
            
            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

