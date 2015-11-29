<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Consumentor.ShopGun.Domain.Company>" %>
<%@ Import Namespace="Consumentor.ShopGun.Domain" %>

<div class="editor-label">
    <%= Html.LabelFor(model => model.CompanyName) %>
</div>
<div class="editor-field">
    <%= Html.TextBoxFor(model => model.CompanyName) %>
    <%= Html.ValidationMessageFor(model => model.CompanyName) %>
</div>

<div class="editor-label">
    <%= Html.LabelFor(company => company.ParentId) %>
</div>
<div class="editor-field">
    <%= Html.DropDownListFor(company => company.ParentId, new SelectList(ViewData["Companies"] as IList<Company>, "Id", "CompanyName"), "Select parent company")%>
    <%= Html.ValidationMessageFor(company => company.ParentId) %>
</div>

<div class="editor-label">
    <%= Html.LabelFor(model => model.Address) %>
</div>
<div class="editor-field">
    <%= Html.TextBoxFor(model => model.Address) %>
    <%= Html.ValidationMessageFor(model => model.Address) %>
</div>

<div class="editor-label">
    <%= Html.LabelFor(model => model.PostCode) %>
</div>
<div class="editor-field">
    <%= Html.TextBoxFor(model => model.PostCode) %>
    <%= Html.ValidationMessageFor(model => model.PostCode) %>
</div>

<div class="editor-label">
    <%= Html.LabelFor(model => model.City) %>
</div>
<div class="editor-field">
    <%= Html.TextBoxFor(model => model.City) %>
    <%= Html.ValidationMessageFor(model => model.City) %>
</div>

<div class="editor-label">
    <%= Html.LabelFor(model => model.PhoneNumber) %>
</div>
<div class="editor-field">
    <%= Html.TextBoxFor(model => model.PhoneNumber) %>
    <%= Html.ValidationMessageFor(model => model.PhoneNumber) %>
</div>

<div class="editor-label">
    <%= Html.LabelFor(model => model.URLToHomePage) %>
</div>
<div class="editor-field">
    <%= Html.TextBoxFor(model => model.URLToHomePage) %>
    <%= Html.ValidationMessageFor(model => model.URLToHomePage) %>
</div>

<div class="editor-label">
    <%= Html.LabelFor(model => model.ContactEmailAddress) %>
</div>
<div class="editor-field">
    <%= Html.TextBoxFor(model => model.ContactEmailAddress) %>
    <%= Html.ValidationMessageFor(model => model.ContactEmailAddress) %>
</div>

<div class="editor-label">
    <%= Html.LabelFor(model => model.CountryId) %>
</div>
<div class="editor-field">
    <%= Html.DropDownListFor(company => company.CountryId, new SelectList(ViewData["Countries"] as IList<Country>, "Id", "CountryCode.Name"), "Select country of residence")%>
    <%= Html.ValidationMessageFor(company => company.CountryId) %>
</div>
