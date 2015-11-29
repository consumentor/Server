<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<Consumentor.ShopGun.Domain.ProductAdvice>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Create
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script src="../../Content/js/jquery-1.4.4.min.js" type="text/javascript"></script>
    <script src="../../Content/js/jquery-ui-1.8.7.min.js" type="text/javascript"></script>
    <script src="../../Content/js/functions.js" type="text/javascript"></script>
    <script type="text/javascript">

        jQuery(document).ready(function() {
            $("input#productsearch").autocomplete({
                source: function(request, response) {
                    $.ajax({
                        url: "/ProductAdvice/GetProducts", type: "POST", dataType: "json",
                        data: {
                            searchMask: request.term,
                            maxResults: 50
                        },
                        success: function(data) {
                            response($.map(data, function(item) {
                                var label = item.GlobalTradeItemNumber + ': ' + item.BrandName + ' - ' + item.ProductName;
                                if (item.CompanyName != "") {
                                    label = label + ' (' + item.CompanyName + ')';
                                }
                                return {
                                    label: label,
                                    value: item.ProductName,
                                    id: item.Id,
                                    gtin: item.GlobalTradeItemNumber,
                                    brand: item.BrandName,
                                    company: item.CompanyName
                                }
                            }))
                        }
                    })
                },
                minLength: 3,
                delay: 300,
                select: function(event, ui) {
                    $("#ProductsId").val(ui.item.id);
                    $("#gtin").text(ui.item.gtin);
                    $("#companyName").text(ui.item.company);
                    $("#brandName").text(ui.item.brand);
                    $("#productDataDiv").show("slow");
                    getProductInfo(ui.item.gtin);
                }
            });
        });

        function clearProductData() {
            $("#productDataDiv").hide("slow");
            $("#productImage").attr("src", "");
        }

        function getProductInfo(gtin) {
            $("#productImage").attr("src", "../../Content/images/indicator.gif");
            $.ajax({
                url: "/ProductAdvice/GetProductImageLink", type: "POST", dataType: "json",
                data: {
                    gtin: gtin
                },
                success: function(data) {
                    if (data != null) {
                        $("#productImage").attr("src", data);
                    }
                    else {
                        $("#productImage").attr("src", "");
                    }
                }
            })
        }
        
    </script>

    <h2>
        Create product advice</h2>
    <% using (Html.BeginForm())
       {%>
    <fieldset>
        <legend>Fields</legend>
        <div>
            <%= Html.Label("Product") %>
        </div>
        <div>
            <%= Html.TextBox("productsearch"
                , "Start typing a product name or GTIN..."
                            , new
                                  {
                                        @onfocus = "onFocusCheckEntry(this, 'Start typing a product name or GTIN...'); clearProductData();"
                                      , @onblur = "onBlurCheckEntry(this, 'Start typing a product name or GTIN...');"
                                      , @onkeypress = "return disableEnterKey(event);"
                                      , @class = "blurredEntry"
                                  }
            )%>
            <%= Html.HiddenFor(x => x.ProductsId) %>
        </div>
        <div id="productDataDiv" style="display: none;">
            <table id="productData">
                <tr>
                    <td>
                        <%= Html.Label("GTIN") %>
                    </td>
                    <td>
                        <label id="gtin">
                        </label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <%= Html.Label("Brand") %>
                    </td>
                    <td>
                        <label id="brandName">
                        </label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <%= Html.Label("Manufacturer: ") %>
                    </td>
                    <td>
                        <label id="companyName">
                        </label>
                    </td>
                </tr>
            </table>
            <img id="productImage" src="" alt="No image available"/>
        </div>
        <% Html.RenderPartial("CreateAdvice"); %>
        <p>
            <input type="submit" value="Create" />
        </p>
    </fieldset>
    <% } %>
    <div>
        <%=Html.ActionLink("Back to List", "Index", "Advice") %>
    </div>
</asp:Content>
