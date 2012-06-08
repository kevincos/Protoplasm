function initNotifications(playerName) {
        if (playerName != "") {
            initHub(playerName);
            $("#LogonHover").css("display", "none");
            $("#NotificationHover").css("display", "inline-block");
            $("#LogoffHover").css("display", "inline-block");
        }
        else {
            $("#LogonHover").css("display", "inline-block");
            $("#NotificationHover").css("display", "none");
            $("#LogoffHover").css("display", "none");
        }


        $("#NotificationHover").hover(
            function () {
                $("#NotificationDropdown").show();
            },
            function () {
                $("#NotificationDropdown").hide();
            }
        );

        $("#LogonHover").hover(
            function () {
                $("#LogonDropdown").show();
            },
            function () {
                $("#LogonDropdown").hide();
            }
        );

        $("#LogonHover").hover(
            function () {
                $("#LogonDropdown").show();
            },
            function () {
                $("#LogonDropdown").hide();
            }
        );

};


function initHub(playerName) {
    var notificationsConnection = $.connection.notificationsHub;

    $.connection.hub.start(function () {
        notificationsConnection.activateHub(playerName);
    });
}


