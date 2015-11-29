<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<String>" %>
<%= Html.TextArea("", Model, new { @class = "html" })%>

<script src="../../../Content/htmlbox/htmlbox.min.js" type="text/javascript"></script>

<script type="text/javascript">
    jQuery(function($) {
        $('textarea.html').htmlbox({
    idir:"http://remiya.com/htmlbox/files/demo/4.0/images/",
toolbars:[
["bold", "italic", "underline", "fontsize", "link"]
]
}) });
</script>