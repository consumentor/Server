<%@ Control Language="C#" Inherits="ViewUserControl<Movie>" %>
{"DateReleased":"\/Date(<%= Model.DateReleased.ToJsonString()%>)\/","Director":"<%= Model.Director%>","Id":<%= Model.Id%>,"Title":"<%= Model.Title%>"}
