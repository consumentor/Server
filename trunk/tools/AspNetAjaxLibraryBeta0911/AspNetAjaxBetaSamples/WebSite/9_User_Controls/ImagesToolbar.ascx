<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ImagesToolbar.ascx.cs" Inherits="UC_ImagesToolbar" %>
<asp:ScriptManagerProxy runat="server">
    <Scripts>
        <asp:ScriptReference Path="Scripts/ImagesToolbar.js" />
    </Scripts>
</asp:ScriptManagerProxy>

<!--Toolbar-->
<div class="sorttoolbar">
    Sort:
    <select onchange="onSort(this.value)">
        <option value="Name">Name</option>
        <option value="Contributor">Contributor</option>
        <option value="Description">Description</option>
    </select>
    <a class="editbutton" href="#" onclick="onInsert()">Add Image</a>
    <a class="editbutton" href="#" onclick="Uc.imagesDataContext.saveChanges()">Save Changes</a>
    <a class="editbutton" href="#" onclick="Uc.imagesList.fetchData()">Fetch Images</a>
</div>


