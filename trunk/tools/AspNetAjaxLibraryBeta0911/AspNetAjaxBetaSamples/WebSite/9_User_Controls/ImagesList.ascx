<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ImagesList.ascx.cs" Inherits="UC_ImagesList" %>

<asp:ScriptManagerProxy runat="server">
    <Scripts>
        <asp:ScriptReference Path="Scripts/ImagesList.js" />
    </Scripts>
</asp:ScriptManagerProxy>

<!--Master View-->
<div id="<%= ListID %>" class="imageslist sys-template" 
    sys:attach="dataview"
    dataview:autofetch="true"
    dataview:dataprovider="{{ Uc.imagesDataContext }}" 
    dataview:fetchoperation="GetImages"
    dataview:fetchparameters="{{ Uc.fetchParameters }}"
    dataview:selecteditemclass="selecteditem" 
    dataview:onrendering="{{ onRendering }}" 
    dataview:initialselectedindex="0" 
    dataview:oncommand="{{ onImagesCommand }}"
>
    <span sys:command="Select" class="listitem" sys:key="listItem" >
        <input type="button" sys:value="{binding Name}" />
        <img sys:src="{binding Uri}"/>
        <span>{binding Contributor}</span>
    </span>
</div>
