function initWaitingArea(playerName, waitingRoomId) {
    $("document").ready(function () {
        var playConnection = $.connection.waitingArea;

        playConnection.goToTable = function (data) {            
            window.location.href = "../Table/Play/"+data;
        };

        $.connection.hub.start(function () {
            playConnection.enterWaitingArea(playerName + waitingRoomId);
        });
    });

};



