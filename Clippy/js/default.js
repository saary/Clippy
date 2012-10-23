// For an introduction to the Fixed Layout template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232508
(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {

                MusicPlayer.bind();
                
                MusicPlayer.pickMusicFilesToPlay();
                TileManager.load('- Viva La Vida-lrc-base.com.lrc', function (err) {
                    if (err) {
                        Debug.writeln('Failed to load clip');
                        return;
                    }

                    var x = -300;
                    setInterval(function () {
                        $('.stripe ul').css({
                            'left': x.toString() + 'px',
                        });
                        x -= 300;
                    }, 3000);

                });
                //BingImageSearch.Bing.imageSearchAsync("viva la vida", 30).then(
                //    function (result) {
                //        var json = JSON.parse(result);
                //        var results = json.d.results;

                //        while (results.length > 0) {
                //            var imagesPerTile = Math.floor((Math.random() * 4)) + 1;
                //            var tiles = results.splice(0, imagesPerTile);
                //            TileManager.insertPanel(tiles);
                //        }

                        
                //        var x = -300;
                //        setInterval(function () {
                //            $('.stripe ul').css({
                //                'left': x.toString() + 'px',
                //            });
                //            x -= 300;
                //        }, 3000);
                        
                //    },
                //    function error(err) {
                //        Debug.writeln('ERROR - ' + err);
                //    });
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }
            args.setPromise(WinJS.UI.processAll());
        }
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };

    app.start();
})();
