var logCount = 0;

function initPlayArea(playerName, tableId, initialChatText) {
    $("document").ready(function () {
        var playConnection = $.connection.gameList;

        playConnection.refreshGame = function (data) {
            window.location.reload();
        };

        playConnection.updateGameState = function (data) {
            geomancer_setGameState(data);
        };

        playConnection.onslaught_updateGameState = function (data) {
            onslaught_setGameState(data);
        };

        playConnection.convoy_updateGameState = function (data) {
            convoy_setGameState(data);
        };

        playConnection.mechtonic_updateGameState = function (data) {
            mechtonic_setGameState(data);
        };

        playConnection.main_updateGameState = function (data) {
            main_setGameState(JSON.parse(data));
        };

        $("#chat").append("<div id='chatline'>" + initialChatText + "</div>");

        playConnection.update_chat = function (data) {
            var chatlog = $("#chat");
            chatlog.append("<div id='chatline'>" + data + "</div>");
            chatlog.scrollTop(chatlog[0].scrollHeight);
        };

        $("#chatform").submit(function () {
            if ($("#chatinput").val() != "") {
                playConnection.chat(tableId, $("#chatinput").val());
                $("#chatinput").val("");
            }
            $("#chatinput").focus();
            return false;
        });

        $.connection.hub.start(function () {
            playConnection.enterGame(playerName + tableId);
        });
    });

};

function updateLogs(logs) {
    for (var i = logCount; i < logs.length; i++) {
        var loglist = $("#log");
        loglist.append("<div id='chatline'>" + logs[i] + "</div>");
        loglist.scrollTop(loglist[0].scrollHeight);
    }
    logCount = logs.length;
};