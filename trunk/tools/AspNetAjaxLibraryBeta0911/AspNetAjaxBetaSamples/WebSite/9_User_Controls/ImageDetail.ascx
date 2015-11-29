<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ImageDetail.ascx.cs" Inherits="UC_ImageDetail" %>

<!--Detail View-->
<!--Live binding to Master View-->
<div class="sys-template"
    sys:attach="dataview"
    dataview:data="{binding selectedData, source=$<%= MasterID %>}" 
>
    <div class="detailbox edit">
        <a href="#" sys:command="Delete" sys:commandtarget="#<%= MasterID %>" class="leftbutton" >Delete Image</a>
        <div class="detailtitle">{binding Name}</div>
        <div class="fieldblock">
            <label for="name">Name: </label>
            <input id="name" type="text" sys:value="{binding Name}"/>
        </div>
        <div class="fieldblock">
            <label for="description">Description: </label>
            <input id="description" type="text" sys:value="{binding Description}"/>
        </div>
        <div class="fieldblock">
            <label for="uri">Uri: </label>
            <input id="uri" type="text" sys:value="{binding Uri}"/>
        </div>
        <div class="fieldblock">
            <label for="contributor">Contributor: </label>
            <input id="contributor" type="text" sys:value="{binding Contributor}"/>
        </div>
    </div>
    <img class="detailimage" sys:alt="{binding Name}" sys:src="{binding Uri}" />
