<%@ Control Language="C#" %>

<link href="../../Content/css/ui.jqgrid.css" rel="stylesheet" type="text/css" />

<script src="../../Content/js/grid.locale-en.js" type="text/javascript"></script>
<script src="../../Content/js/jquery.jqGrid.min.js" type="text/javascript"></script>

<script type="text/javascript">
    var modelName = '<%= ViewData["AdvicesModelName"] %>';
    var modelTitleSuffix = '<%= ViewData["AdvicesModelTitleSuffix"] %>';
    
    var timeoutHnd;
    
    function AdvicesFormatter(cellvalue) {
        signals = [];
        for (var i in cellvalue) {
            signals.push(cellvalue[i].Semaphore.ColorName);
        }

        result = '';
        for (var i in signals) {
            result = result + semaphore(signals[i]);
        }
        return result;
    }

    jQuery(document).ready(function fillList() {
        //console.log('AdviceTable för '+modelName+' laddad');
        jQuery("#list-"+modelName).jqGrid(
            {
                url: '/' + modelName + 'Advice/' + modelName + 'Advices',
                datatype: 'json',
                mtype: 'POST',
                colNames: ['Id', modelName, 'Signals', 'Actions'],
                //colNames: ['Ingredient'],
                colModel: [
                    { name: 'Id', index: 'Id', hidden: true },
                    { name: modelName + modelTitleSuffix, index: modelName + modelTitleSuffix, width: 500, align: 'left' },
                    { name: modelName + 'Advices', index: modelName + 'Advices', width: 100, align: 'left', formatter: AdvicesFormatter },
                    { name: "Actions", index: "Actions", width: 244, formatter: subjectActionsFormatter }
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
                imgpath: "../../Content/images",
                subGrid: true,
                subGridRowExpanded: function(subgrid_id, row_id) {
                    // we pass two parameters
                    // subgrid_id is a id of the div tag created within a table
                    // the row_id is the id of the row
                    // If we want to pass additional parameters to the url we can use
                    // the method getRowData(row_id) - which returns associative array in type name-value
                    // here we can easy construct the following
                    var subgrid_table_id;
                    subgrid_table_id = subgrid_id + "_t";
                    jQuery("#" + subgrid_id).html("<table id='" + subgrid_table_id + "' class='scroll'></table>");
                    jQuery("#" + subgrid_table_id).jqGrid(
                        {
                            url: "/"+modelName +"Advice/GetAdvicesFor" + modelName + "/" + row_id,
                            datatype: "json",
                            colNames: ['Label', 'Introduction', 'Advice', 'Signal', 'Actions'],
                            colModel: [
                                    { name: "Label", index: "Label", width: 100 },
                                    { name: "Introduction", index: "Introduction", width: 140 },
                                    { name: "Advice", index: "Advice", width: 200, align: "left" },
                                    { name: "Semaphore.ColorName", index: "Semaphore.ColorName", formatter: semaphoreFormatter, width: 100 },
                                    { name: "Actions", index: "Actions", formatter: adviceActionsFormatter, width: 200 }
                                    ],
                            jsonReader: {
                                repeatitems: false,
                                id: "0"
                            },
                            rowNum: -1,
                            width: '800',
                            height: 'auto',
                            sortname: 'Label',
                            sortorder: "asc",
                            imgpath: "../../Content/images"
                        }
                    );
                }
            }
        );
    });

        function adviceActionsFormatter(cellvalue, options, rowObject) {
            id = rowObject.Id;
            //console.log(rowObject);
            var actions = "<a href='/" + modelName + "Advice/Edit/" + id + "'>Edit</a> ";
            actions += "<button onclick='deleteAdvice(" + id + ")'>Delete</button>";
            actions += rowObject.Published ?
                "<button onclick='unpublish("+id +")'>Unpublish</button> " :
                "<button onclick='publish(" +id +")'>Publish</button> ";
            return actions;
        }

        function subjectActionsFormatter(cellvalue, options, rowObject) {
            id = rowObject.Id;
            //console.log(rowObject);
            var actions = "<a href='/"+modelName+"Advice/Create/" + id + "'>Add advice</a> ";
            return actions;
        }

        function semaphoreFormatter(cellvalue, options, rowObject) {
            return semaphore(cellvalue);
        }

    function semaphore(colorName) {
        return "<img class='semaphore-small' src='/Content/images/" + colorName.toLowerCase() + ".png' alt='" + colorName + "' title='" + colorName + "' />";
    }

    function publish(id) {
        $.ajax(
        {
            type: "POST",
            url: '/' + modelName + "Advice/Publish/" + id,
            complete: refresh
        });
    }; 

    function unpublish(id) {
        $.ajax(
        {
            type: "POST",
            url: '/' + modelName + "Advice/Unpublish/" + id,
            complete: refresh
        });
    };

    function deleteAdvice(id) {
        if(confirm("Delete advice?"))
        {
            $.ajax(
            {
                type: "POST",
                url: '/' + modelName + "Advice/Delete/" + id,
                complete: refresh
            });
        }
    };

    function refresh() {
        $("div.ui-tabs-panel").not(".ui-tabs-hide").load("/Advice/" + modelName + "Advices");
    };

    function doSearch(ev) {
        var elem = ev.target||ev.srcElement; 
        if (timeoutHnd)
            clearTimeout(timeoutHnd)
        timeoutHnd = setTimeout(gridReload, 500)
    }

    function gridReload() {
        var search_mask = jQuery("#search_mask_"+modelName).val();
        jQuery("#list-<%= ViewData["AdvicesModelName"] %>").jqGrid('setGridParam', {
            url: '/' + modelName + 'Advice/' + modelName + 'Advices?searchMask='+search_mask,
            datatype: 'json',
            mtype: 'POST',
            page: 1
        }).trigger("reloadGrid");
    }
    
</script>
<div id="advice_panel_container">
    <div id="create_advice">    
        <%= Html.ActionLink("Create New", "Create", ViewData["AdvicesModelName"]+"Advice") %>
    </div>
    <div id="search_advice">
        <label for="search">Search</label>
        <input id="search_mask_<%= ViewData["AdvicesModelName"] %>" class="search_input" type="text" name="search_mask_<%= ViewData["AdvicesModelName"] %>" onkeydown="doSearch(arguments[0]||event)"/>        
    </div>
</div>
<div class="clear"></div>
<table id="list-<%= ViewData["AdvicesModelName"] %>" class="grid_12 scroll" cellpadding="0" cellspacing="0"></table>
<div id="pager"></div>

