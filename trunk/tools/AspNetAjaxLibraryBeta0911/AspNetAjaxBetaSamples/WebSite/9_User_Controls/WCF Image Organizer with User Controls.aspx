<%@ Page Title="Image Organizer using User Controls" Language="C#" MasterPageFile="~/9_User_Controls/MasterPage.master" AutoEventWireup="true"
    CodeFile="WCF Image Organizer with User Controls.aspx.cs" Inherits="ImageOrganizerWithUserControls" %>

<%@ Register TagPrefix="uc" TagName="ImagesData" Src="~/9_User_Controls/ImagesData.ascx" %>
<%@ Register TagPrefix="uc" TagName="ImagesToolbar" Src="~/9_User_Controls/ImagesToolbar.ascx" %>
<%@ Register TagPrefix="uc" TagName="ImagesList" Src="~/9_User_Controls/ImagesList.ascx" %>
<%@ Register TagPrefix="uc" TagName="ImageDetail" Src="~/9_User_Controls/ImageDetail.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="title">Image Organizer</div>

    <uc:ImagesData runat="server" />
    <uc:ImagesToolbar runat="server" />
    <uc:ImagesList ListID="imagesListView" runat="server" />
    <uc:ImageDetail MasterID="imagesListView" runat="server" />
</asp:Content>
