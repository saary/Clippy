(function ($) {
    var manager = {
        insertTile: function (img) {
            img.duration = Math.random();
            var item = $("#tileTemplate").render(img);
            $('.stripe ul').append(item);
        },
        insertPanel: function (id, images) {
            var durations = [];
            images.forEach(function (img) { durations.push(Math.random()); });
            var tileKind = images.length;
            var theme = ["theme1", "theme2", "theme3"][Math.floor(Math.random() * 3)];
            
            var panel = $("#panelTemplate" + tileKind).render({ tiles: images, durations: durations, theme: theme, id:id });
            $('.stripe ul').append(panel);
        },
        load: function (filename, cb) {
            cb = cb || function () { };
            BingImageSearch.LRCData.praseLRCFile(filename).then(
                function (lrcdata) {
                    for (var i = 0, l = lrcdata.lines.size; i < l; i++) {
                        var lyricInfo = lrcdata.lines[i];
                        lineToPanels(lyricInfo);
                        cb(null);
                    }
                },
                function (err) {
                    cb(err);
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
        var count = Math.floor(lyricInfo.text.length / 10);

        var images = [];
        for (var i = 0, l = lyricInfo.imageLinks.size; i < l; i++) {
            images.push(lyricInfo.imageLinks[i]);
        }
        randomPermutation(images);
        images.splice(count);
        addPanels(lyricInfo.id, images);
    }

    function addPanels(id, images) {
        while (images.length > 0) {
            var imagesPerTile = Math.floor((Math.random() * 4)) + 1;
            var tiles = images.splice(0, imagesPerTile);
            TileManager.insertPanel(id, tiles);
        }
    }

    WinJS.Namespace.define('TileManager', manager);
}).call(this, jQuery);