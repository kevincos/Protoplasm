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

            $(".NotificationRemove").click(function () {
                var id = $(this).attr('id');
                var graphUrl = window.location.protocol + "//" + window.location.host + '/Notification/ActiveRemove/' + id;
                $.ajax({
                    url: graphUrl,
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    success: function (data) {
                        RefreshNotifications(data);
                    },
                    error: function (data) {
                        alert(data.responseText);
                    }
                });
            });

};


function initHub(playerName) {
    var notificationsConnection = $.connection.notificationsHub;

    $.connection.hub.start(function () {
        notificationsConnection.activateHub(playerName);
    });

    notificationsConnection.updateNotifications = function (data) {
        RefreshNotifications(data);
    };
}


function RefreshNotifications(playerId) {
    var graphUrl = window.location.protocol + "//" + window.location.host + '/Player/RecentNotifications/' + playerId;
    $.ajax({
        url: graphUrl,
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            $("#NotificationList").empty()
            for (var i = 0; i < data.length; i++) {
                $("#NotificationList").append("<div class=NotificationMessage id=n" + i + "></div>");

                var newMessage = $('#NotificationList').find('#n' + i);
                if (data[i].Read == true) {
                    newMessage.append("<a href="+data[i].Url+"> " + data[i].Message + "</a>");
                }
                else {
                    newMessage.append("<a class=UnreadMessage href=" + data[i].Url + "> " + data[i].Message + "</a>");
                }
                $("#NotificationList").append("<div class=NotificationRemove id=" + data[i].NotificationID + ">X</div><br/>");
            }

            $(".NotificationRemove").click(function () {
                var id = $(this).attr('id');
                var graphUrl = window.location.protocol + "//" + window.location.host + '/Notification/ActiveRemove/' + id;
                $.ajax({
                    url: graphUrl,
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    success: function (data) {
                        RefreshNotifications(data);
                    },
                    error: function (data) {
                        alert(data.responseText);
                    }
                });
            });
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
    var graphUrl = window.location.protocol + "//" + window.location.host + '/Player/UnreadNotificationsCount/' + playerId;
    $.ajax({
        url: graphUrl,
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            if (data == 1) {
                $("#AlertCount").text("1 new alert.");
            }
            else {
                $("#AlertCount").text(data + " new alerts.");
            }
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
}
