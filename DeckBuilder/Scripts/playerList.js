var playerList_page = 0;
var playerList_searchText = "";
var playerList_count = 0;
var playerList_maxPages = 0;
var playerList_currentPlayer = "";

function initPlayerList(playerName) {

    playerList_currentPlayer = playerName;

    $("document").ready(function () {

        $('.ChallengeButton').click(name, function (input) {
            var searchId = "#Dropdown" + this.id;
            var list = $(searchId);
            window.location.href = "../Table/Challenge?opponentId=" + this.id + "&versionId=" + list.val();
        });

        $('#SearchBox').keyup(function (input) {
            playerList_page = 0;
            playerList_searchText = $('#SearchBox').val();
            PopulatePlayerList();
        });

        $('#NextPage').click(function () {
            playerList_page += 1;
            if (playerList_page >= playerList_maxPages) {
                playerList_page = playerList_maxPages;
            }

            PopulatePlayerList();
        });

        $('#PrevPage').click(function () {
            playerList_page -= 1;
            if (playerList_page < 0) {
                playerList_page = 0;
            }
            else {
                PopulatePlayerList();
            }
        });

    });

    function PopulatePlayerList() {
        $.ajax({
            url: window.location.protocol + "//" + window.location.host + '/Player/Search?searchString=' + playerList_searchText + '&page=' + playerList_page,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                playerList_count = data.count;
                playerList_maxPages = Math.round(data.count / 10);
                $('#DisplayRange').text("Players " + (1 + playerList_page * 10) + "-" + Math.min((playerList_page + 1) * 10, playerList_count) + " of " + playerList_count);
                $('.ListPanel').each(function (index) {
                    if (index < data.players.length) {
                        $(this).find('.PanelLinkAnchor').css("display", "block");
                        $(this).find('.ListPanelInfo').css("display", "inline-block");
                        $(this).find('.ListPanelInvite').css("display", "inline-block");
                        $(this).find('.ListPanelImage').css("display", "inline-block");

                        var imageSrc = data.players[index].ImageUrl;
                        if (imageSrc == "" || imageSrc == null)
                            imageSrc = "/Content/Images/Site/questionmark.png";
                        $(this).find('.ListPanelImage').attr('src', imageSrc);
                        $(this).find('.ListPanelImage').css('display', 'inline-block');
                        $(this).find('.ListPanelHeader').text(data.players[index].Name);
                        $(this).find('.PanelLinkAnchor').attr('href', "Player/Details/" + data.players[index].PlayerId);
                        if (data.players[index].Name != playerList_currentPlayer) {
                            $(this).find('.ChallengeButton').show();
                            $(this).find('.ListPanelSelect').show();
                            $(this).find('.ChallengeButton').attr('id', data.players[index].PlayerId);
                            $(this).find('.ListPanelSelect').attr('id', "Dropdown" + data.players[index].PlayerId);
                        }
                        else {
                            $(this).find('.ChallengeButton').hide();
                            $(this).find('.ListPanelSelect').hide();
                        }
                    }
                    else {
                        $(this).find('.PanelLinkAnchor').css("display", "none");
                        $(this).find('.ListPanelInfo').css("display", "none");
                        $(this).find('.ListPanelInvite').css("display", "none");
                        $(this).find('.ListPanelImage').css("display", "none");
                        $(this).find('.ChallengeButton').removeAttr('id');
                        $(this).find('.ListPanelSelect').removeAttr('id');                        
                    }
                });
            }
        });
    }

    PopulatePlayerList();
}