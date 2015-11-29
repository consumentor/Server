<% Response.ContentType = ViewData["responseFormat"].ToString(); %>
<%@ Page Title="" Language="C#" Inherits="ViewPage<Movie>" %>
[<% Html.RenderPartial("JsonMovie", Model); %>]
