<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Consumentor.ShopGun.Domain.Ingredient>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	DeleteIngredient
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>DeleteIngredient</h2>
    
    <% using (Html.BeginForm())
       {%>
    <fieldset>
        <legend>Fields</legend>
        
        You chose to delete the ingredient <%= Model.IngredientName%>. 
        <br />
        <% if (Model.IngredientAdvices.Count > 0)
           { %>
        There are advices associated with the ingredient you chose to delete.
        <% } %>
        <br />
        You can specify a new ingredient that should be associated with references from the deleted ingredient.
        <br />
        (Not implemetned yet ->)Alternatively you can choose to delete references that depend on this ingredient (e.g. advices).
        
        <div class="editor-label">
            <label for="SubstitutingIngredientId">
                Substituting ingredient</label>
        </div>
        <div class="editor-field">
            <%= Html.DropDownList("SubstitutingIngredientId", "Choose substituting ingredient")%>
        </div>
        
        <p>
            <input type="submit" value="Delete" />
        </p>
    </fieldset>
    <% } %>

</asp:Content>
