<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<Consumentor.ShopGun.Domain.Ingredient>" %>

<%@ Import Namespace="Consumentor.ShopGun.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Edit ingredient
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function addAlternativeName(id, element) {
            $.ajax(
        {
            type: "POST",
            url: "/Ingredient/AddAlternativeIngredientName/",
            data: { ingredientId: id, alternativeName: $("#" +element).val() },
            success: function (added) {
                if (added) {
                    $("#AlternativeNames").append('<option>' + $("#"+element).val() + '</option>');
                    $("#" +element).val("");
                } else {
                    alert("An ingredient or alternative name with the choosen name already exists!");
                }
            }
        });
    };

    function removeAlternativeNames(id, elements) {

        var alternativeNamesToRemove = $("#" + elements).val();
        console.log(alternativeNamesToRemove);
        $.ajax(
        {
            type: "POST",
            url: "/Ingredient/RemoveAlternativeIngredientName/",
            data: { ingredientId: id, alternativeNamesToRemove: alternativeNamesToRemove },
            traditional: true,
            success: function (removed) {
                if (removed) {
                    $("#"+elements +" option:selected").remove();
                } else {
                    alert("One or more alternative names could not be removed");
                }
            }
        });
    };
    </script>
    <h2>
        Edit ingredient</h2>
    <% using (Html.BeginForm())
       {%>
    <fieldset>
        <legend>Fields</legend>
        <table>
            <tr>
                <td>
                    <div class="editor-label">
                        <%= Html.LabelFor(model => model.IngredientName)%>
                    </div>
                    <div class="editor-field">
                        <%= Html.TextBoxFor(model => model.IngredientName) %>
                        <%= Html.ValidationMessageFor(model => model.IngredientName)%>
                    </div>
                    <div class="editor-label">
                        <%= Html.LabelFor(model => model.ParentId) %>
                    </div>
                    <div class="editor-field">
                        <%= Html.DropDownListFor(ingredient => ingredient.ParentId, ViewData["Ingredients"] as SelectList, "Select parent ingredient")%>
                    </div>
                    <div class="editor-field">
                        <input type="submit" value="Save" />
                    </div>
                </td>
                <td>
                    <div class="editor-label">
                        <%= Html.LabelFor(model => ViewData.Model.AlternativeIngredientNames)%>
                        Changes take effect directly - it's not necessary to click save.
                    </div>
                    <div class="editor-field">
                        <%= Html.TextBox("newAlternativeName") %>
                        <button type="button" onclick='addAlternativeName(<%= Model.Id %>, "newAlternativeName")'>
                            Add</button>
                            <br/>
                        <%= Html.ListBox("AlternativeNames")%>
                        <button type="button" onclick='removeAlternativeNames(<%= Model.Id %>, "AlternativeNames")'>
                            Remove selected</button>
                    </div>
                </td>
            </tr>
        </table>
    </fieldset>
    <% } %>
    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>
</asp:Content>
