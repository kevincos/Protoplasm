$("document").ready(function () {
    $('.ParentHeader').click(name, function (input) {
        if ($(this).parent().hasClass("collapsed")) {
            $(this).parent().children('ul').show();
            $(this).parent().removeClass("collapsed");
        }
        else {
            $(this).parent().children('ul').hide();
            $(this).parent().addClass("collapsed");
        }
    });

    $('.ParentHeader').each(function (index) {
        var childCount = $(this).parent().children('ul').children().length;
        if (childCount > 6 || childCount == 0) {
            $(this).parent().children('ul').hide();
            $(this).parent().addClass("collapsed");
        }
    });

});