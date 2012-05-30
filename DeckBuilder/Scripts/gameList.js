$("document").ready(function () {

    $('button.RankedMatchButton').click(name, function (input) {
        var searchId = "#" + this.value;
        var list = $(searchId);        
        window.location.href = "../MatchRequest/Add?gameId=" + this.value + "&numPlayers=" + list.val();
    });

});