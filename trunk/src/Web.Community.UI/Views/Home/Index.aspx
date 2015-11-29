<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="cssContent" ContentPlaceHolderID="AdditionalCSS" runat="server">
    <link href="../../Content/jquery-ui-1.8.23.custom.css" rel="stylesheet" type="text/css" />
<style>
	.ui-autocomplete-category {
		font-weight: bold;
		padding: .2em .4em;
		margin: .8em 0 .2em;
		line-height: 1.5;
	}
	</style>
</asp:Content>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript" src="http://code.jquery.com/jquery-latest.min.js" charset="utf-8"></script>
<script src="../../Scripts/jquery-ui-1.8.23.custom.min.js" type="text/javascript"></script>

    <h2><%= Html.Encode(ViewData["Message"]) %></h2>
    <p>
        To learn more about ASP.NET MVC visit <a href="http://asp.net/mvc" title="ASP.NET MVC Website">http://asp.net/mvc</a>.
    </p>
    
    /Advice/search/moep/7311271352730
    
    <script type="text/javascript">

        $(function () {
            $("#search").autocomplete({
                delay: 50,
                source: function (request, response) {
                    $.getJSON("http://213.115.114.114:9381/beta/ShopgunApp/iteminfo/" + request.term + "?callback=?&numResults = 3", null, function(suggestions) {
                        response($.map(suggestions, function(items, category) {
                            var result = new Array();
                            $.each(items, function(index, item) {
                                var entry = new Object();
                                entry.category = category;
                                switch (category) {
                                case "Ingredients":
                                    entry.label = item.IngredientName;
                                    break;
                                case "Products":
                                    entry.label = item.ProductName + " (" + item.BrandName + ")";
                                    break;
                                case "Brands":
                                    entry.label = item.BrandName;
                                    entry.label += item.CompanyName != "" ? " (" + item.CompanyName + ")" : "";
                                    break;
                                case "Companies":
                                    entry.label = item.CompanyName;
                                    break;
                                case "Countries":
                                    entry.label = item.CountryName;
                                    break;
                                case "Concepts":
                                    entry.label = item.ConceptName;
                                    break;
                                }
                                result[index] = entry;
                            }
                            );
                            return result;
                        }));
                    });
                }
            }
            );
        });
    </script>

<div class="demo">
	<label for="search">Search: </label>
	<input id="search" />
</div><!-- End demo -->
</asp:Content>
