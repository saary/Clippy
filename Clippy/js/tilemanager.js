(function ($) {
    var manager = {
        insertTile: function (img) {
            img.duration = Math.random();
            var item = $("#tileTemplate").render(img);
            $('.stripe ul').append(item);
        },
        insertPanel: function (images) {
            images.forEach(function (img) { img.duration = Math.random(); });
            var tileKind = images.length;
            var theme = ["theme1", "theme2", "theme3"][Math.floor(Math.random() * 3)];
            
            var panel = $("#panelTemplate" + tileKind).render({ tiles: images, theme: theme });
            $('.stripe ul').append(panel);
        },
        load: function (filename) {
            BingImageSearch.LRCData.praseLRCFile().then(
                function (lrcdata) {

                },
                function (err) {

                });

        }
    };

    function createPanelForLine(lyricInfo) {
        var imagesPerTile = Math.floor((Math.random() * 4)) + 1;
        var tiles = results.splice(0, imagesPerTile);
        manager.insertPanel(tiles);

    }

    WinJS.Namespace.define('TileManager', manager);
}).call(this, jQuery);