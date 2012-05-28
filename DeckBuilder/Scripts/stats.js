var primaryConditionCount = 0
var secondaryConditionCount = 0
var versionID = 0


// Load the Visualization API and the piechart package.
google.load('visualization', '1.0', { 'packages': ['corechart'] });

// Set a callback to run when the Google Visualization API is loaded.
//google.setOnLoadCallback(drawChart);   

// Callback that creates and populates a data table,
// instantiates the pie chart, passes in the data and
// draws it.
function drawChart(axisData, dataSets, rawData, graphType) {

    // Create the data table.
    var data = new google.visualization.DataTable();
    data.addColumn('string', 'Player Index'); //Independent axis name
    for (var i = 0; i < dataSets.length; i++) {
        data.addColumn('number', dataSets[i]); //Dependendt axis name
    }

    var rows = [];
    for (var i = 0; i < axisData.length; i++) {
        result = [axisData[i]]
        for (var j = 0; j < dataSets.length; j++) {
            result.push(rawData[j][i]);
        }
        rows.push(result);
    }


    data.addRows(rows);

    // Set chart options
    var options = { 'title': 'How Much Pizza I Ate Last Night',
        'width': 800,
        'height': 600
    };
    if (graphType == "PercentOverItems" || graphType == "PercentOverGames") {
        options.vAxis = { 'minValue': 0, 'maxValue': 100 }
    }

    // Instantiate and draw our chart, passing in some options.
    var chart = new google.visualization.ColumnChart(document.getElementById('chartdiv'));
    chart.draw(data, options);
}


$("document").ready(function () {

    $('#ItemName').show()
    $('#TargetIndex').hide()
    $('#TargetDictKey').hide()
    $('#TargetKeyContainer').hide()
    $('#PrimaryConditions').show()
    $('#SecondaryConditions').show()

    $('#GraphDropdown').change(function () {
        var graphType = $('#GraphDropdown option:selected').text();

        if (graphType == "PercentOverGames") {
            $('#ItemName').hide()
            $('#TargetKeyContainer').hide()
            $('#PrimaryConditions').show()
            $('#SecondaryConditions').show()
        }
        if (graphType == "PercentOverItems") {
            $('#ItemName').show()
            $('#TargetKeyContainer').hide()
            $('#PrimaryConditions').show()
            $('#SecondaryConditions').show()
        }
        if (graphType == "CountOverGames") {
            $('#ItemName').hide()
            $('#TargetKeyContainer').hide()
            $('#PrimaryConditions').show()
            $('#SecondaryConditions').hide()
        }
        if (graphType == "CountOverItems") {
            $('#ItemName').show()
            $('#TargetKeyContainer').hide()
            $('#PrimaryConditions').show()
            $('#SecondaryConditions').hide()
        }
        if (graphType == "AverageOverGames") {
            $('#ItemName').hide()
            $('#TargetKeyContainer').show()
            $('#PrimaryConditions').show()
            $('#SecondaryConditions').hide()
        }
        if (graphType == "AverageOverItems") {
            $('#ItemName').show()
            $('#TargetKeyContainer').show()
            $('#PrimaryConditions').show()
            $('#SecondaryConditions').hide()
        }
        if (graphType == "SumOverGames") {
            $('#ItemName').hide()
            $('#TargetKeyContainer').show()
            $('#PrimaryConditions').show()
            $('#SecondaryConditions').hide()
        }
        if (graphType == "SumOverItems") {
            $('#ItemName').show()
            $('#TargetKeyContainer').show()
            $('#PrimaryConditions').show()
            $('#SecondaryConditions').hide()
        }

    });

    $('#TargetDropdown').change(function () {
        var targetType = $('#TargetDropdown option:selected').text();
        if (targetType == "Key-Value") {
            $('#TargetIndex').hide()
            $('#TargetDictKey').hide()
        }
        if (targetType == "Indexed-Key") {
            $('#TargetIndex').show()
            $('#TargetDictKey').hide()
        }
        if (targetType == "Dict-Key") {
            $('#TargetIndex').hide()
            $('#TargetDictKey').show()
        }
    });

    $('#AddPrimaryCondition').click(function () {

        $('#PrimaryConditions').append("<div class=Condition id=cond" + primaryConditionCount + "></div>");
        var newCondition = $('#PrimaryConditions').find('#cond' + primaryConditionCount);
        newCondition.append("<br /><select id=ConditionTypeDropdown><option value=1>Key-Value</option><option value=2>Indexed-Value</option><option value=0>Match-All</option><option value=3>Dict-Value</option><option value=4>Key-Range</option><option value=5>Indexed-Range</option><option value=6>Dict-Range</option></select>");
        newCondition.append("<span id=Key>Key<input id=KeyInput type=text /></span>");
        newCondition.append("<span id=Value>Value<input id=ValueInput type=text /></span>");
        newCondition.append("<span id=IndexKey>IndexKey<input id=IndexInput type=text /></span>");
        newCondition.append("<span id=DictKey>DictKey<input id=DictKeyInput type=text /></span>");
        newCondition.append("<span id=Min>Min<input id =MinInput type=text /></span>");
        newCondition.append("<span id=Max>Max<input id = MaxInput type=text /></span>");
        newCondition.find("#IndexKey").hide();
        newCondition.find("#DictKey").hide();
        newCondition.find("#Min").hide();
        newCondition.find("#Max").hide();
        newCondition.append("<button id=Delete>Delete</button>");
        newCondition.find("#Delete").click(function () {
            newCondition.remove();
        });
        newCondition.find("#ConditionTypeDropdown").change(function () {
            var conditionType = newCondition.find('#ConditionTypeDropdown option:selected').text();

            if (conditionType == "Key-Value") {
                newCondition.find("#Key").show();
                newCondition.find("#Value").show();
                newCondition.find("#IndexKey").hide();
                newCondition.find("#DictKey").hide();
                newCondition.find("#Min").hide();
                newCondition.find("#Max").hide();
            }
            if (conditionType == "Indexed-Value") {
                newCondition.find("#Key").show();
                newCondition.find("#Value").show();
                newCondition.find("#IndexKey").show();
                newCondition.find("#DictKey").hide();
                newCondition.find("#Min").hide();
                newCondition.find("#Max").hide();
            }
            if (conditionType == "Dict-Value") {
                newCondition.find("#Key").show();
                newCondition.find("#Value").show();
                newCondition.find("#IndexKey").hide();
                newCondition.find("#DictKey").show();
                newCondition.find("#Min").hide();
                newCondition.find("#Max").hide();
            }
            if (conditionType == "Key-Range") {
                newCondition.find("#Key").show();
                newCondition.find("#Value").hide();
                newCondition.find("#IndexKey").hide();
                newCondition.find("#DictKey").hide();
                newCondition.find("#Min").show();
                newCondition.find("#Max").show();
            }
            if (conditionType == "Indexed-Range") {
                newCondition.find("#Key").show();
                newCondition.find("#Value").hide();
                newCondition.find("#IndexKey").show();
                newCondition.find("#DictKey").hide();
                newCondition.find("#Min").show();
                newCondition.find("#Max").show();
            }
            if (conditionType == "Dict-Range") {
                newCondition.find("#Key").show();
                newCondition.find("#Value").hide();
                newCondition.find("#IndexKey").hide();
                newCondition.find("#DictKey").show();
                newCondition.find("#Min").show();
                newCondition.find("#Max").show();
            }
            if (conditionType == "Match-All") {
                newCondition.find("#Key").show();
                newCondition.find("#Key").hide();
                newCondition.find("#Value").hide();
                newCondition.find("#IndexKey").hide();
                newCondition.find("#DictKey").hide();
                newCondition.find("#Min").hide();
                newCondition.find("#Max").hide();
            }

        });
        primaryConditionCount++;

    });

    $('#AddSecondaryCondition').click(function () {

        $('#SecondaryConditions').append("<div class=Condition id=cond" + secondaryConditionCount + "></div>");
        var newCondition = $('#SecondaryConditions').find('#cond' + secondaryConditionCount);
        newCondition.append("<br /><select id=ConditionTypeDropdown><option value=1>Key-Value</option><option value=2>Indexed-Value</option><option value=0>Match-All</option><option value=3>Dict-Value</option><option value=4>Key-Range</option><option value=5>Indexed-Range</option><option value=6>Dict-Range</option></select>");
        newCondition.append("<span id=Key>Key<input id=KeyInput type=text /></span>");
        newCondition.append("<span id=Value>Value<input id=ValueInput type=text /></span>");
        newCondition.append("<span id=IndexKey>IndexKey<input id=IndexInput type=text /></span>");
        newCondition.append("<span id=DictKey>DictKey<input id=DictKeyInput type=text /></span>");
        newCondition.append("<span id=Min>Min<input id =MinInput type=text /></span>");
        newCondition.append("<span id=Max>Max<input id = MaxInput type=text /></span>");
        newCondition.find("#IndexKey").hide();
        newCondition.find("#DictKey").hide();
        newCondition.find("#Min").hide();
        newCondition.find("#Max").hide();
        newCondition.append("<button id=Delete>Delete</button>");
        newCondition.find("#Delete").click(function () {
            newCondition.remove();
        });
        newCondition.find("#ConditionTypeDropdown").change(function () {
            var conditionType = newCondition.find('#ConditionTypeDropdown option:selected').text();

            if (conditionType == "Key-Value") {
                newCondition.find("#Key").show();
                newCondition.find("#Value").show();
                newCondition.find("#IndexKey").hide();
                newCondition.find("#DictKey").hide();
                newCondition.find("#Min").hide();
                newCondition.find("#Max").hide();
            }
            if (conditionType == "Indexed-Value") {
                newCondition.find("#Key").show();
                newCondition.find("#Value").show();
                newCondition.find("#IndexKey").show();
                newCondition.find("#DictKey").hide();
                newCondition.find("#Min").hide();
                newCondition.find("#Max").hide();
            }
            if (conditionType == "Dict-Value") {
                newCondition.find("#Key").show();
                newCondition.find("#Value").show();
                newCondition.find("#IndexKey").hide();
                newCondition.find("#DictKey").show();
                newCondition.find("#Min").hide();
                newCondition.find("#Max").hide();
            }
            if (conditionType == "Key-Range") {
                newCondition.find("#Key").show();
                newCondition.find("#Value").hide();
                newCondition.find("#IndexKey").hide();
                newCondition.find("#DictKey").hide();
                newCondition.find("#Min").show();
                newCondition.find("#Max").show();
            }
            if (conditionType == "Indexed-Range") {
                newCondition.find("#Key").show();
                newCondition.find("#Value").hide();
                newCondition.find("#IndexKey").show();
                newCondition.find("#DictKey").hide();
                newCondition.find("#Min").show();
                newCondition.find("#Max").show();
            }
            if (conditionType == "Dict-Range") {
                newCondition.find("#Key").show();
                newCondition.find("#Value").hide();
                newCondition.find("#IndexKey").hide();
                newCondition.find("#DictKey").show();
                newCondition.find("#Min").show();
                newCondition.find("#Max").show();
            }
            if (conditionType == "Match-All") {
                newCondition.find("#Key").show();
                newCondition.find("#Key").hide();
                newCondition.find("#Value").hide();
                newCondition.find("#IndexKey").hide();
                newCondition.find("#DictKey").hide();
                newCondition.find("#Min").hide();
                newCondition.find("#Max").hide();
            }

        });
        secondaryConditionCount++;

    });

    $('#GraphButton').click(function () {
        /*Graph g = new Graph(new List<string> { "0", "1" });
        g.AddDataSet("WinRate");
        Condition winCondition = new Condition("result", "Win");
        Condition indexCondition = new Condition("index", "[x]");
        g.PercentOverItems("player", new List<Condition> { indexCondition }, new List<Condition> { winCondition });
        g.GenerateData(version.StatLog);*/
        var graph = {};
        graph.itemName = $('#ItemNameInput').val();
        graph.target = {};
        graph.target.key = $('#TargetKeyInput').val();
        graph.target.index = $('#TargetIndexInput').val();
        graph.target.dictKey = $('#TargetDictKeyInput').val();
        graph.target.type = $('#TargetDropdown').val();
        graph.type = $('#GraphDropdown').val();
        graph.inputDataSets = $('#DataSetInput').val();
        graph.inputIndependentAxis = $('#IndependentAxisInput').val();

        graph.dataSets = $('#DataSetInput').val().split(',');
        if ($('#IndependentAxisInput').val().indexOf('..') == -1)
            graph.independentAxis = $('#IndependentAxisInput').val().split(',');
        else {
            var min = $('#IndependentAxisInput').val().split('..')[0]
            var max = $('#IndependentAxisInput').val().split('..')[1]
            graph.independentAxis = []
            for (var i = parseInt(min); i <= parseInt(max); i++) {
                graph.independentAxis.push(i.toString())
            }
        }


        graph.primaryConditionList = [];
        var primaryConditions = $('#PrimaryConditions').find('.Condition');
        for (var i = 0; i < primaryConditions.length; i++) {
            var condition = {}
            condition.key = primaryConditions.eq(i).find("#KeyInput").val();
            condition.value = primaryConditions.eq(i).find("#ValueInput").val();
            condition.index = primaryConditions.eq(i).find("#IndexInput").val();
            condition.dictKey = primaryConditions.eq(i).find("#DictKeyInput").val();
            condition.min = primaryConditions.eq(i).find("#MinInput").val();
            condition.max = primaryConditions.eq(i).find("#MaxInput").val();
            condition.type = primaryConditions.eq(i).find("#ConditionTypeDropdown").val();
            graph.primaryConditionList.push(condition);
        }
        graph.secondaryConditionList = [];
        var secondaryConditions = $('#SecondaryConditions').find('.Condition');
        for (var i = 0; i < secondaryConditions.length; i++) {
            var condition = {}
            condition.key = secondaryConditions.eq(i).find("#KeyInput").val();
            condition.value = secondaryConditions.eq(i).find("#ValueInput").val();
            condition.index = secondaryConditions.eq(i).find("#IndexInput").val();
            condition.dictKey = secondaryConditions.eq(i).find("#DictKeyInput").val();
            condition.min = secondaryConditions.eq(i).find("#MinInput").val();
            condition.max = secondaryConditions.eq(i).find("#MaxInput").val();
            condition.type = secondaryConditions.eq(i).find("#ConditionTypeDropdown").val();
            graph.secondaryConditionList.push(condition);
        }

        var graphUrl = '../Graph/' + versionID;
        $.ajax({
            url: graphUrl,
            type: 'POST',
            dataType: 'json',
            data: JSON.stringify(graph),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                //$.jqplot('chartdiv', [[[1, 2], [3, 5.12], [5, 13.1], [7, 33.6], [9, 85.9], [11, 219.9]]]);
                //$.jqplot('chartdiv', [50,83]);                
                /*$.jqplot('chartdiv', data, {
                seriesDefaults: {
                renderer: $.jqplot.BarRenderer,
                rendererOptions: { fillToZero: true }
                }
                });*/
                drawChart(graph.independentAxis, graph.dataSets, data, $('#GraphDropdown option:selected').text());
            }
        });
    });
});