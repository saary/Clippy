(function ($) {
    var manager = {
        insertTile: function (img) {
            img.duration = Math.random();
            var item = $("#tileTemplate").render(img);
            $('.stripe ul').append(item);
        },
        insertPanel: function (images) {
            var durations = [];
            images.forEach(function (img) { durations.push(Math.random()); });
            var tileKind = images.length;
            var theme = ["theme1", "theme2", "theme3"][Math.floor(Math.random() * 3)];
            
            var panel = $("#panelTemplate" + tileKind).render({ tiles: images, durations: durations, theme: theme });
            $('.stripe ul').append(panel);
        },
        load: function (filename) {
            BingImageSearch.LRCData.praseLRCFile(filename).then(
                function (lrcdata) {
                    Debug.writeln(lrc.Lines.length);
                },
                function (err) {

                });
        }
    };

    function randomPermutation(arr) {
        
        for (var i = (arr.length - 1) ; i >= 0; --i) {
            var randPos = Math.floor(i * Math.random());
            var tmpStore = arr[i];
            arr[i] = arr[randPos];
            arr[randPos] = tmpStore;
        }
        return arr;
    }

    function lineToPanels(lyricInfo) {
        var count = Math.floor(lyricInfo.Text.length / 20);

        var images = lyricInfo.ImageLinks.AsArray();
        randomPermutation(images);
        images.splice(count);
        addPanels(images);
    }

    function addPanels(id, images) {
        while (images.length > 0) {
            var imagesPerTile = Math.floor((Math.random() * 4)) + 1;
            var tiles = images.splice(0, imagesPerTile);
            TileManager.insertPanel(tiles);
        }
    }

    WinJS.Namespace.define('TileManager', manager);
}).call(this, jQuery);