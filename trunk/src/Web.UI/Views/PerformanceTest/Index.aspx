<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>FileUpload</h2>
    
     <% using (Html.BeginForm("Index", "PerformanceTest", 
                    FormMethod.Post, new { enctype = "multipart/form-data" }))
        {%>
        <input name="uploadFile" type="file" />
        <input type="submit" value="Upload File" />
<%} 
    if (ViewData["ProdWithAdvices"] != null) {%>
    <table>
    <%
        foreach (var prodName in ViewData["ProdWithAdvices"] as IList<string>)
        { %>
            <tr>
                <td>
                    <%= prodName %>
                </td>
            </tr>
<%      } %>
        </table>
<%}%>

</asp:Content>
