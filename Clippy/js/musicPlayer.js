
var musicPlayer = {
    filesToPlay: {},

    mediaControl: Windows.Media.MediaControl,

    onPlay: function (e) {
        console.log("played " + musicPlayer.currentTime());
    },
    onPause: function (e) {
        console.log("paused " + musicPlayer.currentTime());
    },
    onEnded: function (e) {
        console.log("song ended");
    },
    onTimeupdate: function (e) {
        var lyricInfo = BingImageSearch.LRCData.getInfo(musicPlayer.currentTime());


        console.log("time update.. " + musicPlayer.currentTime());
    },

    bind: function () {
        //$('.play').click(this.pickMusicFilesToPlay);
        musicPlayer.audioPlayerJQuery = $(".audioPlayer");
        musicPlayer.audioPlayerHTML = musicPlayer.audioPlayerJQuery[0];
        musicPlayer.audioPlayerHTML.onplay = musicPlayer.onPlay;
        musicPlayer.audioPlayerHTML.onpause = musicPlayer.onPause;
        musicPlayer.audioPlayerHTML.onended = musicPlayer.onEnded;
        musicPlayer.audioPlayerHTML.ontimeupdate = musicPlayer.onTimeupdate;
    },

    setSourceFromFile: function (file) {
        musicPlayer.audioPlayerJQuery.removeClass("invisible");
        musicPlayer.audioPlayerHTML.src = URL.createObjectURL(file, { oneTimeOnly: true });
        musicPlayer.currentSongName = file.name;
    },

    setSourceFromFiles: function (files) {
        musicPlayer.setSourceFromFile(files[0]);
    },

    pickMusicFilesToPlay: function () {
        var fileOpenPicker = new Windows.Storage.Pickers.FileOpenPicker();
        fileOpenPicker.viewMode = Windows.Storage.Pickers.PickerViewMode.list;
        fileOpenPicker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.musicLibrary;
        // Users expect to have a filtered view of their folders depending on the scenario.
        // For example, when choosing a documents folder, restrict the filetypes to documents for your application.
        fileOpenPicker.fileTypeFilter.replaceAll([".mp3"]);

        // Currenty for single file.. change for multiple if needed.
        fileOpenPicker.pickSingleFileAsync().then(function (files) {

            //if (files.size > 0) {
            // Application now has read/write access to the picked file(s)
            //var outputString = "Picked files:\n";
            //for (var i = 0; i < files.size; i++) {
            //    outputString = outputString + files[i].name + "\n";
            //}
            //console.log(outputString, "sample", "status");

            // Operation was cancelled.
            if (!files) {
                return;
            }
            musicPlayer.filesToPlay = files;
            musicPlayer.setSourceFromFile(musicPlayer.filesToPlay);
            //} else {
            //    // The picker was dismissed with no selected file
            //    console.log("Operation cancelled.", "sample", "status");
            //}
        });
    },

    currentTime: function () {
        return musicPlayer.audioPlayerHTML.currentTime
    },

    play: function () {
        return musicPlayer.audioPlayerHTML.play();
    },

    pause: function () {
        return musicPlayer.audioPlayerHTML.pause();
    },
}

WinJS.Namespace.define('MusicPlayer', musicPlayer);
