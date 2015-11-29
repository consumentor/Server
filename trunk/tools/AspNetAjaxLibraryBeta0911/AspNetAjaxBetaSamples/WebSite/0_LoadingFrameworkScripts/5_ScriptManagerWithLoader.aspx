<%@ Page Language="C#" %>

<%@ Register Assembly="System.Web.Ajax" Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register Namespace="SDRServerControls" Assembly="SDRServerControls" TagPrefix="sdr" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<!--
This sample demonstrates how to register a custom client control with the client Script Loader, in
an aspx page using the Script Manager.

The custom control is registered in the Scripts/MyClientControls/RegisterMyClientControls.js file.
-->
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Registering Custom Scripts, with Script Manager</title>
    <link href="../styles/images.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:AjaxScriptManager ID="AjaxScriptManager1" runat="server" EnablePartialRendering="false">
            <Scripts>
                <asp:ScriptReference Name="Start.js" />
                <asp:ScriptReference Path="~/Scripts/MyClientControls/RegisterMyClientControls.js" />
            </Scripts>
        </asp:AjaxScriptManager>
    
        <script type="text/javascript">
            var gallery = [
                { Name: "Morro Rock", Uri: "../images/p58.jpg" },
                { Name: "Seagull reflections", Uri: "../images/p52.jpg" },
                { Name: "Pier", Uri: "../images/p59.jpg" },
                { Name: "Giraffe Zoom", Uri: "../images/p183.jpg" },
                { Name: "Oryx", Uri: "../images/p172.jpg" }
            ];
            
            //Update basePath for script loader since we are including Start.js from System.Web.Ajax.dll
            Sys.loader.basePath = "../Scripts/MicrosoftAjax";
            
            Sys.require([Sys.components.imageView]);
          
            Sys.onReady(function() {
                Sys.create.imageView(".images", { data: gallery })
            });
        </script>
        <div class="images"></div>
    </form>
</body>
</html>
