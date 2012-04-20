

function initLobbyChat(playerName) {


    $("document").ready(function () {
        var lobbyChatClient = $.connection.lobbyChat;


        function addPlayerToList(name) {
            $('#playerList').append("<div class='playerCheckbox' id=chatItem_" + name + "><input type='checkbox' name='player' value='" + name + "'/>" + name + "</div>");

            $('#chatItem_' + name).click(name, function (input) {
                lobbyChatClient.proposeGame(input.data);
                $('#proposal').append('<button id=confirm>Play game with ' + input.data + '?</button>');
                $('#confirm').click(input.data, function (input) {
                    lobbyChatClient.confirm(input.data, $('#SelectedDeck').val());
                });
            });
        }

        // Start the connection

        lobbyChatClient.addMessage = function (data) {
            var chatlog = $("#lobbychat");
            chatlog.append("<div id='chatline'>" + data + "</div>");
            chatlog.scrollTop(chatlog[0].scrollHeight);
        };

        lobbyChatClient.updatePlayerlist = function (array) {
            for (var i = 0; i < array.length; i++) {
                if (playerName != array[i]) {
                    addPlayerToList(array[i]);
                }
            }
        };

        lobbyChatClient.addPlayer = function (data) {
            if (playerName != data && $('#chatItem_' + data).length == 0) {
                addPlayerToList(data);
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

        lobbyChatClient.updateProposals = function (proposals) {
            $('#proposalList').empty();
            for (var i = 0; i < proposals.length; i++) {
                $('#proposalList').append("<div class='proposalEntry' id='proposal" + i + "'> </div></br>");

                var ready = false;
                for (var j = 0; j < proposals[i].players.length; j++) {
                    if (proposals[i].players[j].name == playerName && proposals[i].players[j].ready == true) {
                        ready = true;
                    }
                }
                if (ready == false)
                    $('#proposal' + i).append("<button id='ready" + i + "'>Ready</button>");
                else
                    $('#proposal' + i).append("<button id='cancel" + i + "'>Cancel</button>");

                $('#proposal' + i).append("<span>" + proposals[i].game + "</span>");
                $('#proposal' + i).append("<span> with </span>");
                for (var j = 0; j < proposals[i].players.length; j++) {
                    if (proposals[i].players[j].name != playerName) {
                        $('#proposal' + i).append("<span>" + proposals[i].players[j].name + "</span>");
                        if (proposals[i].players[j].ready)
                            $('#proposal' + i).append("<span>(Ready)</span>");
                        else
                            $('#proposal' + i).append("<span>(Waiting)</span>");
                    }
                }                
                $('#ready' + i).click(proposals[i], function (prop) {
                    lobbyChatClient.confirmProposal(prop.data.ProposalID, playerName);
                });
                $('#cancel' + i).click(proposals[i], function (prop) {
                    lobbyChatClient.cancelProposal(prop.data.ProposalID, playerName);
                });


            }
        }

        $("#proposalButton").click(function () {

            var checkedBoxes = $("input:checked");
            var opponents = [];
            for (var i = 0; i < checkedBoxes.length; i++) {
                opponents.push(checkedBoxes[i].value);
            }
            if (opponents.length > 0) {
                // SEND OPPONENT LIST TO SERVER
                lobbyChatClient.newProposal(playerName, opponents, $('#SelectedGame').val());
            }
        });


        $("#chatform").submit(function () {
            if ($("#lobbychatinput").val() != "") {
                lobbyChatClient.broadcast($("#lobbychatinput").val());
                $("#lobbychatinput").val("");
            }
            $("#lobbychatinput").focus();
            return false;
        });

        var broadcast = $("#chat_broadcast");
        broadcast.click(function () {
            lobbyChatClient.broadcast($('#chat_text').val());
        });

        function exitFunction() {
            lobbyChatClient.leaveLobby();
        }

        $.connection.hub.start(function () {
            lobbyChatClient.enterLobby(playerName);
            if (window.onpagehide || window.onpagehide === null) {
                window.addEventListener('pagehide', exitFunction, false);
            } else {
                window.addEventListener('unload', exitFunction, false);
            }
        });


    });
};