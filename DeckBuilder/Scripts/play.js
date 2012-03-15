function initPlayArea(playerName, tableId) {
    $("document").ready(function () {
        var playConnection = $.connection.gameList;

        playConnection.refreshGame = function (data) {
            window.location.reload();
        };

        $("#playMove").click(function () {
            var dropdowndebug = $('#MoveList');
            var valdebug = dropdowndebug.val();
            playConnection.submitMove(tableId, playerName, $('#MoveList').val());
        });


        $.connection.hub.start(function () {
            playConnection.enterGame(playerName);
        });
    });
};