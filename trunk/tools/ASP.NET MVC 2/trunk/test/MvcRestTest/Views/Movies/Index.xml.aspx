<?xml version="1.0" encoding="<%= ((ContentType)ViewData["responseFormat"]).CharSet %>"?>
<% Response.ContentType = ViewData["responseFormat"].ToString(); %>
<%@ Page Title="" Language="C#" Inherits="ViewPage<IEnumerable<Movie>>" %>
<ArrayOfMovie xmlns:i="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://schemas.datacontract.org/2004/07/MovieApp.Models">
<!-- this is an XML comment (Aspx), it shows up in the body -->
<% /* this is a code comment, it is swallowed during compilation */ %>
<% foreach (var item in Model) { %>
    <% /* the title attribute on the Movie element is ignored by the DataContractSerializer */ %>
    <Movie Title="<%= item.Title %>">
        <DateReleased><%= XmlConvert.ToString(item.DateReleased, XmlDateTimeSerializationMode.RoundtripKind) %></DateReleased>
        <Director><%= item.Director %></Director>
        <Id><%= item.Id %></Id>
        <Title><%= item.Title %></Title>
    </Movie>
<% } %>
</ArrayOfMovie>
