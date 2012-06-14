$("document").ready(function () {

    $('.RankedMatchButton').click(name, function (input) {

        //window.location.href = "../MatchRequest/Add?gameId=" + this.id + "&numPlayers=2";
    });

    function UpdateLaunchStatus(launchInNewWindow) {
        if (launchInNewWindow == "checked") {            
            $('.RankedMatchButton').attr("target", "_blank");
        }
        else {           
            $('.RankedMatchButton').removeAttr("target");
        }

    };

    $('#newTabBox').change(function (input) {
        UpdateLaunchStatus($(this).attr('checked'));
    });

});