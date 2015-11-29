<% Response.ContentType = ViewData["responseFormat"].ToString(); %>
<%@ Page Title="" Language="C#" Inherits="ViewPage<IEnumerable<Movie>>" %>
[
<% bool first = true;
    foreach (var item in Model) {
        if (!first) { %>,<% }
        first = false; %>
<% Html.RenderPartial("JsonMovie", item); %>
<% } %>
] 