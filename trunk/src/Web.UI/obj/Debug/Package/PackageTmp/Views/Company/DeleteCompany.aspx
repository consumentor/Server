<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Consumentor.ShopGun.Domain.Company>" %>

<%@ Import Namespace="Consumentor.ShopGun.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    DeleteCompany
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        DeleteCompany</h2>
    <% using (Html.BeginForm())
       {%>
    <fieldset>
        <legend>Fields</legend>You chose to delete the company <%= Model.CompanyName %>. 
        The following child companies won't have this company as their parent company anymore if you delete <%= Model.CompanyName %>:
        <br />
        <br />
        <% foreach (var childCompany in ViewData["ChildCompanies"] as IList<Company>)
           { %>
        <%= childCompany.CompanyName %>
        <br />
        <% } %>
        <br />
        
        <% if (Model.CompanyAdvices.Count > 0)
           { %>
        There are also advices associated with the company you chose to delete.
        <% } %>
        <br />
        You can specify a new company that should be associated with references from the deleted company.
        
        (Not implemetned yet ->)Alternatively you can choose to delete references that depend on this company (e.g. advices).
        
        <div class="editor-label">
            <label for="SubstitutingCompany">
                Substituting company</label>
        </div>
        <div class="editor-field">
            <%= Html.DropDownList("SubstitutingCompany", "Choose substituting company")%>
        </div>
        
        <p>
            <input type="submit" value="Delete" />
        </p>
    </fieldset>
    <% } %>
    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>
</asp:Content>
