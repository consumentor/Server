<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Consumentor.ShopGun.Domain.CertificationMark>" %>

 <div class="editor-label">
    <%= Html.LabelFor(model => model.CertificationName) %>
</div>
<div class="editor-field">
    <%= Html.TextBoxFor(model => model.CertificationName) %>
    <%= Html.ValidationMessageFor(model => model.CertificationName) %>
</div>

<div class="editor-label">
    <%= Html.LabelFor(model => model.Description) %>
</div>
<div class="editor-field">
    <%= Html.TextAreaFor(model => model.Description) %>
    <%= Html.ValidationMessageFor(model => model.Description) %>
</div>

<div class="editor-label">
    <%= Html.LabelFor(model => model.Url) %>
</div>
<div class="editor-field">
    <%= Html.TextBoxFor(model => model.Url) %>
    <%= Html.ValidationMessageFor(model => model.Url) %>
</div>