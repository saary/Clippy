(function ($) {
    var manager = {
        insertTile: function (img) {
            var item = $("#tileTemplate").render(img);
            $('.strip').append(item);
        }
    };

    WinJS.Namespace.define('TileManager', manager);
}).call(this, jQuery);