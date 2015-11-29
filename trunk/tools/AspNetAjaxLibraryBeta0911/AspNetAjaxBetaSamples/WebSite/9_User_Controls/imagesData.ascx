<%@ Control Language="C#" AutoEventWireup="true" CodeFile="imagesData.ascx.cs" Inherits="UC_imagesData" %>

<asp:ScriptManagerProxy runat="server">
    <Scripts>
        <asp:ScriptReference Path="Scripts/ImagesData.js" />
    </Scripts>
<%--    <Services>
        <asp:ServiceReference Path="~/Services/<%=Service %>.svc" />
    </Services> --%>
</asp:ScriptManagerProxy>
