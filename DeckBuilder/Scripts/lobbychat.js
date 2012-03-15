function initLobbyChat(playerName) {

    $("document").ready(function () {
        var lobbyChatClient = $.connection.lobbyChat;


        // Start the connection

        lobbyChatClient.addMessage = function (data) {
            $('#chat_messages').append('<li>' + data + '</li>');
        };

        lobbyChatClient.updatePlayerlist = function (array) {
            for (var i = 0; i < array.length; i++) {
                if (playerName != array[i]) {
                    $('#chat_playerList').append('<button id=chatItem_' + array[i] + '>' + array[i] + '</button>');
                    $('#chatItem_' + array[i]).click(array[i], function (input) {
                        lobbyChatClient.proposeGame(input.data);
                        $('#proposal').append('<button id=confirm>Play game with ' + input.data + '?</button>');
                        $('#confirm').click(input.data, function (input) {
                            lobbyChatClient.confirm(input.data, $('#SelectedDeck').val());
                        });
                    });
                }
            }
        };

        lobbyChatClient.addPlayer = function (data) {
            if (playerName != data && $('#chatItem_' + data).length == 0) {
                $('#chat_playerList').append('<button id=chatItem_' + data + '>' + data + '</button>');
                $('#chatItem_' + data).click(data, function (input) {
                    lobbyChatClient.proposeGame(input.data);
                    $('#proposal').append('<button id=confirm>Play game with ' + input.data + '?</button>');
                    $('#confirm').click(input.data, function (input) {
                        lobbyChatClient.confirm(input.data, $('#SelectedDeck').val());
                    });
                });
            }
        }

        lobbyChatClient.removePlayer = function (data) {
            if (playerName != data && $('#chatItem_' + data).length != 0) {
                $('#chatItem_' + data).remove();
            }
        }

        lobbyChatClient.beginGame = function (data) {

            window.location.href = "../Table/Play/" + data;
        }

        lobbyChatClient.proposalNotification = function (opponent) {
            $('#proposal').append('<button id=confirm>Play game with ' + opponent + '?</button>');
            $('#confirm').click(opponent, function (input) {
                lobbyChatClient.confirm(input.data, $('#SelectedDeck').val());
            });
        }


        var broadcast = $("#chat_broadcast");

        broadcast.click(function () {
            lobbyChatClient.broadcast($('#chat_text').val());
        });

        function exitFunction() {
            lobbyChatClient.leaveLobby();
        }

        $.connection.hub.start(function () {
            lobbyChatClient.enterLobby(playerName);
            //$(window).unload(function () {                
            //  lobbyChatClient.leaveLobby();
            //});
            //$(window).bind('beforeunload', function () {                
            //return 'Do you really want to leave?';
            //});
            if (window.onpagehide || window.onpagehide === null) {
                window.addEventListener('pagehide', exitFunction, false);
            } else {
                window.addEventListener('unload', exitFunction, false);
            }
        });


    });
};