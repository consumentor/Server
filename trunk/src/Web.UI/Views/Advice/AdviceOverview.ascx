<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Consumentor.ShopGun.Domain" %>
<script type='text/javascript' src="<%=Url.Content("~/Content/js/jquery-ui-1.8.7.min.js")%>"></script>
<script type='text/javascript' src="<%=Url.Content("~/Content/js/jquery.cookie.js")%>"></script>
<link rel="stylesheet" href="<%=Url.Content("~/Content/css/jquery-ui-1.8.7.custom.css")%>" type="text/css" media="all" />

<script type="text/javascript">

    jQuery(document).ready(function() {
        $(function() {
            $("#mentor").change(function() {
                $.ajax({
                    type: "POST",
                    data: ({ mentorId: $("#mentor").val() }),
                    url: "Advice/SetMentor",
                    dataType: "json",
                    success: refresh,
                    error: function() {
                        alert("error");
                    }
                });
            })
        });

        $(function() {
        $("#advice_list_tabs").tabs();
        });
    });

</script>            
<div id="content" class="clear-block">
    <div id="content-inner" class="clear-block">
        <div class="grid_12">
            
                <h2 class="title">All Advices</h2>
            
            <div id="advice_selector">
                <b>Mentor</b>
                <% if (ViewData["mentors"] != null){%>
                    <%=Html.DropDownList("mentor", ViewData["mentors"] as SelectList)%>
                <% }%>
            </div>
            <div id="advice_list_tabs" class="grid_12">
                <ul>              
                    <li><a href="/Advice/IngredientAdvices">Ingredient</a></li>
                    <li><a href="/Advice/ConceptAdvices">Concept</a></li>
                    <li><a href="/Advice/CompanyAdvices">Company</a></li>
                    <li><a href="/Advice/BrandAdvices">Brand</a></li>
                    <li><a href="/Advice/ProductAdvices">Product</a></li>
                    <li><a href="/Advice/CountryAdvices">Country</a></li>
                </ul>
            </div>
        </div>
    </div>
</div>

