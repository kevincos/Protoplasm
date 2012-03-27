function initPlayArea(playerName, tableId) {
    $("document").ready(function () {
        var playConnection = $.connection.gameList;

        playConnection.refreshGame = function (data) {
            window.location.reload();
        };

        playConnection.updateGameState = function (data) {
            gameState = data;            
            activePlayerId = gameState.playerContexts[gameState.activePlayerIndex].playerId;
            for (var i = 0; i < gameState.playerContexts.length; i++) {
                if (gameState.playerContexts[i].playerId == currentPlayerId) {
                    currentPlayerIndex = i;
                    playerHand = gameState.playerContexts[i].hand;
                }
            }
            refreshView = true;
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