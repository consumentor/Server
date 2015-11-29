<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Consumentor.ShopGun.Domain.Product>" %>
<%@ Import Namespace="Consumentor.ShopGun.Domain" %>
<link href="../../Content/css/Site.css" rel="stylesheet" type="text/css" />

<div class="editor-label">
    <%= Html.LabelFor(model => model.GlobalTradeItemNumber) %>
</div>
<div class="editor-field">
    <%= Html.TextBoxFor(model => model.GlobalTradeItemNumber) %>
    <%= Html.ValidationMessageFor(model => model.GlobalTradeItemNumber) %>
</div>

<div class="editor-label">
    <%= Html.LabelFor(model => model.ProductName) %>
</div>
<div class="editor-field">
    <%= Html.TextBoxFor(model => model.ProductName) %>
    <%= Html.ValidationMessageFor(model => model.ProductName) %>
</div>

<div class="editor-label">
    <label for="BrandId">Brand</label>
</div>
<div class="editor-field">
    <%= Html.DropDownListFor(model => model.BrandId, ViewData["Brand"] as SelectList, "Select a brand...")%>
</div>

<div class="editor-label">
    <label for="OriginCountryId">Origin country</label>
</div>
<div class="editor-field">
    <%= Html.DropDownListFor(model => model.OriginCountryId, ViewData["OriginCountry"] as SelectList, "Select a country")%>
</div>

<div class="editor-label">
    <%= Html.LabelFor(model => model.Quantity) %>
</div>
<div class="editor-field">
    <%= Html.TextBoxFor(model => model.Quantity) %>
    <%= Html.ValidationMessageFor(model => model.Quantity) %>
</div>

<div class="editor-label">
    <%= Html.LabelFor(model => model.QuantityUnit) %>
</div>
<div class="editor-field">
    <%= Html.TextBoxFor(model => model.QuantityUnit) %>
    <%= Html.ValidationMessageFor(model => model.QuantityUnit) %>
</div>

<div class="editor-label">
    <%= Html.LabelFor(model => model.TableOfContent) %>
</div>
<div class="editor-field">
    <%= Html.TextAreaFor(model => model.TableOfContent, new{width="300px", height="400px"}) %>
    <%= Html.ValidationMessageFor(model => model.TableOfContent) %>
</div>

<%--<div class="editor-label">
    <%= Html.LabelFor(model => model.Labels) %>
</div>
<div class="editor-field">
    <%= Html.TextBoxFor(model => model.Labels) %>
    <%= Html.ValidationMessageFor(model => model.Labels) %>
</div>
--%>
<div class="editor-label">
    <%= Html.LabelFor(model => model.Description) %>
</div>
<div class="editor-field">
    <%= Html.TextBoxFor(model => model.Description) %>
    <%= Html.ValidationMessageFor(model => model.Description) %>
</div>

<div class="editor-label">
    <label for="CertificationMarks">Certification marks:</label>
</div>
<div class="">
    <%
        foreach (var certificationMark in ViewData["CertificationMarks"] as IEnumerable<CertificationMark>)
        {
            var certificationMarkId = certificationMark.Id; 
            var hasCurrentCertification = Model == null ? false : Model.CertificationMarks.Any(x => x.Id == certificationMarkId);%>
            <%= Html.CheckBox("CertificationMarks", hasCurrentCertification, new{id = certificationMark.CertificationName, value = certificationMark.Id})%>
            <label for="<%= certificationMark.CertificationName %>" title="<%= certificationMark.CertificationName %>"><%= certificationMark.CertificationName %></label>
        <br />
    <% } %>
</div>