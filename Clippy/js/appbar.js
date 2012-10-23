(function ($) {
    "use strict";
    var page = WinJS.UI.Pages.define("default.html", {
        ready: function (element, options) {
            document.getElementById("cmdFilePicker").addEventListener("click", musicPlayer.pickMusicFilesToPlay, false);
          
            WinJS.log && WinJS.log("To show the bar, swipe up from " +
                "the bottom of the screen, right-click, or " +
                "press Windows Logo + z. To dismiss the bar, " +
                "tap in the application, swipe, right-click, " +
                "or press Windows Logo + z again.", "sample", "status");
        },
    });
}).call(this, jQuery);