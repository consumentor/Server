<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<link href="../../Content/css/ui.jqgrid.css" rel="stylesheet" type="text/css" />
<link rel="stylesheet" href="<%=Url.Content("~/Content/css/jquery-ui-1.8.7.custom.css")%>"
    type="text/css" media="all" />

<script type='text/javascript' src="<%=Url.Content("~/Content/js/jquery-ui-1.8.7.min.js")%>"></script>

<script src="../../Content/js/grid.locale-en.js" type="text/javascript"></script>

<script src="../../Content/js/jquery.jqGrid.min.js" type="text/javascript"></script>

<script type="text/javascript">

    jQuery(document).ready(function fillList() {
        //console.log('AdviceTable för '+modelName+' laddad');
        jQuery("#productList").jqGrid(
            {
                url: '/Product/GetProducts',
                datatype: 'json',
                mtype: 'POST',
                colNames: ['Id', 'GTIN', 'ProductName', 'Brand', 'Actions'],
                //colNames: ['Ingredient'],
                colModel: [
                    { name: 'Id', index: 'Id', hidden: true },
                    { name: 'GlobalTradeItemNumber', index: 'GlobalTradeItemNumber', align: 'left' },
                    { name: 'ProductName', index: 'ProductName', align: 'left' },
                    { name: 'BrandName', index: 'BrandName', align: 'left' },
                    { name: "Actions", index: "Actions", formatter: subjectActionsFormatter }
                ],
                jsonReader: {
                    repeatitems: false,
                    id: "0"
                },
                pager: jQuery('#pager'),
                rowNum: 10,
                rowList: [5, 10, 20, 50],
                sortname: "Id",
                sortorder: "desc",
                viewrecords: true,
                width: '864',
                height: 'auto',
                imgpath: "../../Content/images"
            }
        );
    });

    function subjectActionsFormatter(cellvalue, options, rowObject) {
        id = rowObject.Id;
        //console.log(rowObject);
        var actions = "<a href='/Product/EditProduct/" + id + "'>Edit</a> ";
        return actions;
    }

    var timeoutHnd;
    var flAuto = false;

    function doSearch(ev) {
        // var elem = ev.target||ev.srcElement; 
        if (timeoutHnd)
            clearTimeout(timeoutHnd)
        timeoutHnd = setTimeout(gridReload, 500)
    }

    function gridReload() {
        var search_mask = jQuery("#search_mask").val();
        var gtin_mask = jQuery("#gtin_mask").val();
        jQuery("#productList").jqGrid('setGridParam', {
        url: '/Product/GetProducts?gtinMask=' +gtin_mask +'&searchMask=' +search_mask,
        datatype: 'json',
        mtype: 'POST', 
            page: 1 }).trigger("reloadGrid");
    }
    
</script>

<div class="h">
    Start typing a GTIN or product name:
</div>
<div>
    <br/>
    <input type="text" id="search_mask" onkeydown="doSearch(arguments[0]||event)" />
</div>

<table id="productList" class="grid_12 scroll" cellpadding="0" cellspacing="0">
</table>
<div id="pager">
</div>
