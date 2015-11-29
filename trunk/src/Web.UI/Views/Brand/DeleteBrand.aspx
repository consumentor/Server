<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<Consumentor.ShopGun.Domain.Brand>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	DeleteBrand
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>DeleteBrand</h2>
    
    <% using (Html.BeginForm())
       {%>
    <fieldset>
        <legend>Fields</legend>You chose to delete the brand <%= Model.BrandName%>. 
        
        <% if (Model.BrandAdvices.Count > 0)
           { %>
        There are advices associated with the brand you chose to delete.
        <% } %>
        <br />
        You can specify a new brand that should be associated with references from the deleted company.
        
        (Not implemetned yet ->)Alternatively you can choose to delete references that depend on this brand (e.g. advices).
        
        <div class="editor-label">
            <label for="SubstitutingBrandId">
                Substituting brand</label>
        </div>
        <div class="editor-field">
            <%= Html.DropDownList("SubstitutingBrandId", "Choose substituting brand")%>
        </div>
        
        <p>
            <input type="submit" value="Delete" />
        </p>
    </fieldset>
    <% } %>

</asp:Content>
