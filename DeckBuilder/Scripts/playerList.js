$("document").ready(function () {

    $('button.ListPanelButton').click(name, function (input) {
        var searchId = "#" + this.value;
        var list = $(searchId);        
        window.location.href = "../Table/Challenge?opponentId=" + this.value + "&gameName=" + list.val();
    });

});