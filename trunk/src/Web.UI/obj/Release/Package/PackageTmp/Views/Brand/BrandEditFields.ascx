<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Consumentor.ShopGun.Domain.Brand>" %>

<div class="editor-label">
    <%= Html.LabelFor(model => model.BrandName) %>
</div>
<div class="editor-field">
    <%= Html.TextBoxFor(model => model.BrandName) %>
    <%= Html.ValidationMessageFor(model => model.BrandName) %>
</div>

<div class="editor-label">
    <label for="OwnerId">Brand owner</label>
</div>
<div class="editor-field">
    <%= Html.DropDownListFor(x => x.CompanyId, ViewData["OwnerId"] as SelectList, "Select an owner")%>
</div>