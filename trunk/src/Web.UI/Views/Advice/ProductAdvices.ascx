<%@ Control Language="C#" %>

<link href="../../Content/css/ui.jqgrid.css" rel="stylesheet" type="text/css" />

<script src="../../Content/js/grid.locale-en.js" type="text/javascript"></script>
<script src="../../Content/js/jquery.jqGrid.min.js" type="text/javascript"></script>

<script type="text/javascript">

    function productAdvicesFormatter(cellvalue) {
        semaphores = {};
        for (var i in cellvalue) {
            semaphores[cellvalue[i].Semaphore.ColorName] = true;
        }

        result = '';
        for (var i in semaphores) {
            result = result + semaphore(i);
        }
        return result;
    }

    function productAdvices(cellvalue) {
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
        jQuery("#list").jqGrid(
            {
                url: '/productAdvice/productAdvices',
                datatype: 'json',
                mtype: 'POST',
                colNames: ['Id', 'product', 'Signals', 'Actions'],
                //colNames: ['product'],
                colModel: [
                    { name: 'Id', index: 'Id', hidden: true },
                    { name: 'ProductName', index: 'ProductName', align: 'left' },
                    { name: 'ProductAdvices', index: 'ProductAdvices', align: 'left', formatter: productAdvices },
                    { name: "Actions", index: "Actions", formatter: productActionsFormatter }
                ],
                jsonReader: {
                    repeatitems: false,
                    id: "0"
                },
                pager: jQuery('#pager'),
                //rowNum: 10,
                //rowList: [5, 10, 20, 50],
                sortname: 'Id',
                sortorder: "desc",
                viewrecords: true,
                width: '864',
                height: 'auto',
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
                            url: "/Advice/AdvicesForProduct/" + row_id,
                            datatype: "json",
                            colNames: ['Label', 'Introduction', 'Advice', 'Signal', 'Actions'],
                            colModel: [
                                    { name: "Label", index: "Label", width: 80 },
                                    { name: "Introduction", index: "Introduction", width: 130 },
                                    { name: "Advice", index: "Advice", width: 80, align: "right" },
                                    { name: "Semaphore.ColorName", index: "Semaphore.ColorName", formatter: semaphoreFormatter },
                                    { name: "Actions", index: "Actions", formatter: adviceActionsFormatter }
                                    ],
                            jsonReader: {
                                repeatitems: false,
                                id: "0"
                            },
                            //rowNum: 5,
                            sortname: 'Label',
                            sortorder: "asc",
                            imgpath: '../../Content/images'
                        }
                    );
                }
            }
        );
        });

        function adviceActionsFormatter(cellvalue, options, rowObject) {
            id = rowObject.Id;
            //console.log(rowObject);
            var actions = "<a href='/ProductAdvice/Edit/" + id + "'>Edit</a> ";
            actions += rowObject.Published ?
                "<a onclick='unpublish("+id +")'>Unpublish</a> " :
                "<a onclick='publish(" +id +")'>Publish</a> ";
            actions += "<a onclick='deleteAdvice(" +id +")'>Delete</a>";
            return actions;
        }

        function productActionsFormatter(cellvalue, options, rowObject) {
            id = rowObject.Id;
            //console.log(rowObject);
            var actions = "<a href='/ProductAdvice/Create/" + id + "'>Add advice</a> ";
            return actions;
        }

        function semaphoreFormatter(cellvalue, options, rowObject) {
            return semaphore(cellvalue);
        }

    function semaphore(colorName) {
        return "<img class='semaphore-small' src='../../Content/images/" + colorName.toLowerCase() + ".png' alt='" + colorName + "' title='" + colorName + "' />";
    }

    function publish(id) {
        $.ajax(
        {
            type: "POST",
            url: "/Advice/Publish/" + id,
            complete: refresh
        });
    }; 

    function unpublish(id) {
        $.ajax(
        {
            type: "POST",
            url: "/Advice/Unpublish/" + id,
            complete: refresh
        });
    };

    function deleteAdvice(id) {
        $.ajax(
        {
            type: "POST",
            url: "/Advice/Delete/" + id,
            complete: refresh
        });
    };

    function refresh() {
        $("#ui-tabs-1").load("/Advice/productAdvices");
    };
    
</script>

<table id="list" class="grid_12 scroll" cellpadding="0" cellspacing="0"></table>
<div id="pager"></div>

<p>
    <%= Html.ActionLink("Create New", "Create", "ProductAdvice") %>
</p>

