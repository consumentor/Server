<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Consumentor.ShopGun.Domain.AdviceBase>" %>
<%@ Import Namespace="Consumentor.ShopGun.Domain" %>
<script type='text/javascript' src="<%=Url.Content("~/Content/js/jquery-ui-1.8.7.min.js")%>"></script>
<link rel="stylesheet" href="<%=Url.Content("~/Content/css/jquery-ui-1.8.7.custom.css")%>" type="text/css" media="all" />
<script src="<%=Url.Content("~/Content/js/jquery.combobox.js")%>" type="text/javascript"></script>

<script type="text/javascript" src="<%=Url.Content("~/Content/tinymce/jscripts/tiny_mce/tiny_mce.js")%>" ></script >
<script type="text/javascript" >
tinyMCE.init({
        mode : "textareas",
        theme : "advanced",   //(n.b. no trailing comma, this will be critical as you experiment later)
        encoding: "xml",
        //plugins : "paste",
        valid_elements : ""
            // add regular parapgraph-like text elements
            +"p,address,pre,blockquote,cite,"
            // add headings
            +"h1,h2,h3,h4,h5,h6,"
            // add lists
            +"dl,dt,dd,ul[type],ol[type],li,"
            // add bold italic and underlined (or do you whish to use <em> and <strong> instead?)
            +"strong/b,em/i,u,"
            +"a[!href|target|title|rel|rev|charset|tabindex|accesskey|type|name],span,b,o",
            // more to add? -> add a comma before the closing quotation marks!        
        paste_auto_cleanup_on_paste : true,
        paste_convert_headers_to_strong : false,
        //paste_strip_class_attributes : "all",
        // Theme options - button# indicated the row# only
        //  Theme options #1
        theme_advanced_buttons1: "bold,italic,formatselect,cut,copy,paste,pastetext,pasteword",
        theme_advanced_buttons2: "fontsizeselect,|,link,unlink,code",
        theme_advanced_buttons3: "justifyleft,justifycenter,justifyright,justifyfull,|,bullist,numlist",
        theme_advanced_buttons4: "",
        
        // Align and place toolbar
        theme_advanced_toolbar_location: "top",
        theme_advanced_toolbar_align: "left"
});
</script >
    <script type="text/javascript">

        // new code
        $(function() {
            $(".combobox").combobox();
        });
    </script>
    <div class="clear-block"></div>
    <div class="editor-label">
        <%= Html.LabelFor(advice => advice.Label) %>
    </div>
    <div class="editor-field">
        <%= Html.TextBoxFor(advice => advice.Label) %>
        <%= Html.ValidationMessageFor(advice => advice.Label) %>
    </div>
    <div class="clear-block"></div>

    <div class="editor-label">
        <%= Html.LabelFor(advice => advice.Introduction) %>
    </div>
    <div class="editor-field">
        <%= Html.TextBoxFor(advice => advice.Introduction) %>
        <%= Html.ValidationMessageFor(advice => advice.Introduction) %>
    </div>
    <div class="clear-block"></div>

    <div class="editor-label">
        <%= Html.LabelFor(advice => advice.Advice) %>
    </div>
    <div class="editor-field">
        <%= Html.TextAreaFor(((advice => advice.HtmlDecodedAdvice)))%>
        <%= Html.ValidationMessageFor(advice => advice.HtmlDecodedAdvice)%>
    </div>
   <div class="clear-block"></div>

    <div class="editor-label">
        <%= Html.LabelFor(advice => advice.Tag) %>
    </div>
    <div class="editor-field">
        <%= Html.DropDownListFor(advice => advice.TagId, ViewData["AdviceTags"] as SelectList, "Select a tag...") %>
        <%= Html.ValidationMessageFor(advice => advice.TagId) %>
    </div>
    <div class="clear-block"></div>


    <div class="editor-label">
        <%= Html.LabelFor(advice => advice.Semaphore) %>
        <%= Html.ValidationMessageFor(advice => advice.Semaphore) %>
    </div>

    <div class="editor-radio">
        <% foreach (var semaphore in ViewData["Semaphores"] as List<Semaphore>) { %>
            
            <label class="label_radio_<%=semaphore.ColorName.ToLower() %>" for="semaphore<%= semaphore.Id %>">
                <%= Html.RadioButtonFor(advice => advice.SemaphoreId, semaphore.Id, new Dictionary<string, object>{{"id", "semaphore"+semaphore.Id}})%>
            </label>
           
        <% } %>
    </div>
    <div class="clear-block"></div>

    <div class="editor-label">
        <%= Html.Label("Publish") %>
    </div>
    <div class="editor-field">
        <%= Html.CheckBoxFor(advice => advice.Published) %>
        <%= Html.ValidationMessageFor(advice => advice.Published) %>
    </div>
    <div class="clear-block"></div>
