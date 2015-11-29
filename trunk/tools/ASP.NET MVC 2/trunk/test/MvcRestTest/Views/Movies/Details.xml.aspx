<?xml version="1.0" encoding="<%= ((ContentType)ViewData["responseFormat"]).CharSet %>"?>
<% Response.ContentType = ViewData["responseFormat"].ToString(); %>
<%@ Page Title="" Language="C#" Inherits="ViewPage<Movie>" %>
<!-- this is an XML comment (Aspx), it shows up in the body -->
<% /* this is a code comment, it is swallowed during compilation */ %>
<Movie Title="<%= Model.Title %>"><% /* the title attribute on the Movie element is ignored by the DataContractSerializer */ %>
    <DateReleased><%= XmlConvert.ToString(Model.DateReleased, XmlDateTimeSerializationMode.RoundtripKind)%></DateReleased>
    <Director><%= Model.Director %></Director>
    <Id><%= Model.Id%></Id>
    <Title><%= Model.Title%></Title>
</Movie>
