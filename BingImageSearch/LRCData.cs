﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Windows.ApplicationModel;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.UI.Xaml;
using Windows.UI.Core;
using System.Diagnostics;

namespace BingImageSearch
{
    public delegate void NextLinePlayedEventHandler(LyricInfo inf);
    public sealed class LRCData
    {
        private Regex _timeRegex;
        private Regex _artistRegex;
        private Regex _titleRegex;
        private Regex _albumRegex;
        private IList<LyricInfo> _lines;
        private bool _playing = false;
        private CoreDispatcher _dispatcher;

        private const string REGEX_PATTERN = @"^\[(\d\d):(\d\d)\.(\d\d)\](.*)";
        private const string ARTIST_PATTERN = @"\[ar:(.*)\]";
        private const string TITILE_PATTERN = @"\[ti:(.*)\]";
        private const string ALBUM_PATTERN = @"\[al:(.*)\]";

        public LRCData()
        {
            _timeRegex = new Regex(REGEX_PATTERN);
            _artistRegex = new Regex(ARTIST_PATTERN);
            _albumRegex = new Regex(ALBUM_PATTERN);
            _titleRegex = new Regex(TITILE_PATTERN);
            _lines = new List<LyricInfo>();
        }

        public void play()
        {
            play(0);
        }

        public void play(int startTimeMillis)
        {
            var window = Windows.UI.Core.CoreWindow.GetForCurrentThread();
            _dispatcher = window.Dispatcher;
            _playing = true;

            int startPointIndex = -1;
            if (startTimeMillis <= 0)
            {
                startPointIndex = 0;
            }
            else
            {
                for (int i = 0; i < _lines.Count; i++)
                {
                    LyricInfo inf = _lines[i];
                    if (inf.ShowTimeMillis > startTimeMillis)
                    {
                        startPointIndex = i - 1;
                        break;
                    }
                }
            }

            if (startPointIndex == -1)
                return;

            waitAndCallEvent(startPointIndex,startTimeMillis);
        }

        /*public void playFromPoint(int millisFromStart)
        {
            int startPointIndex = -1;
            for (int i=0; i < _lines.Count; i++)
            {
                LyricInfo inf = _lines[i];
                if (inf.ShowTimeMillis > millisFromStart)
                {
                    startPointIndex = i - 1;
                    break;
                }
            }
        }*/

        private void waitAndCallEvent(int index,int startTime)
        {
            int prevShowTime = startTime;
            if (index > 0)
                prevShowTime = Math.Max(_lines[index - 1].ShowTimeMillis,startTime);
            
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, Math.Max(0,_lines[index].ShowTimeMillis -prevShowTime-100));
            
            Task.Delay(ts).ContinueWith((i) =>
            {
                if (!_playing)
                {
                    return;
                }
                _dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    onNextLinePlayed(_lines[index]);
                });
                
                    
                //onNextLinePlayed(_lines[index]);
                
                waitAndCallEvent(index+1,startTime);
            });
        }

        private void Stop()
        {
            _playing = false;
        }

        public String Artist
        {
            get;
            set;
        }

        public String Title
        {
            get;
            set;
        }

        public String Album
        {
            get;
            set;
        }

        public IList<LyricInfo> Lines
        {
            get
            {
                return _lines;
            }
        }

        //public event EventHandler<LyricsEventArgs> onNextLinePlayed;
        public event NextLinePlayedEventHandler onNextLinePlayed;
        

        private void ParseLine(String line)
        {

            Match m = _artistRegex.Match(line);
            if (m.Success)
            {
                Artist = m.Groups[1].Value;
                return;
            }

            m = _albumRegex.Match(line);
            if (m.Success)
            {
                Album = m.Groups[1].Value;
                return;
            }

            m = _titleRegex.Match(line);
            if (m.Success)
            {
                Title = m.Groups[1].Value;
                return;
            }

    
            m = _timeRegex.Match(line);
            if (m.Success)
            {
     
                try
                {
                    int time = int.Parse(m.Groups[1].Value)*1000*60 + int.Parse(m.Groups[2].Value)*1000+int.Parse(m.Groups[3].Value);
                    String text = m.Groups[4].Value;
                    if (!String.IsNullOrEmpty(text))
                    {
                        _lines.Add(new LyricInfo(_lines.Count) { ShowTimeMillis = time, Text = text });
                    }
                } 
                catch(Exception e)
                {
                    // ..
                    Debug.WriteLine(e.ToString());
                }
            }
        }

        private void FillFromLRCFile(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                ParseLine(line);
            }
        }

        public static IAsyncOperation<LRCData> PraseLRCFile(String filePath)
        {
            return ParseLRCFileInternal(filePath).AsAsyncOperation<LRCData>();
        }

        /*public static string prepareBingQuery(String line)
        {
            String goodChars = "[^a-zA-z0-9]";
            Regex r = new Regex(goodChars);
            string replaced = r.Replace(line, "");
        }*/

        private static async Task<LRCData> ParseLRCFileInternal(string Path)
        {
            LRCData data = new LRCData();
            //var folder = Package.Current.InstalledLocation;
            var musicFolder = Windows.Storage.KnownFolders.MusicLibrary;
            var file = await musicFolder.GetFileAsync(Path);
            var stream = await System.IO.WindowsRuntimeStorageExtensions.OpenStreamForReadAsync(file);
            data.FillFromLRCFile(stream);

            System.Threading.ManualResetEvent e = new System.Threading.ManualResetEvent(false);
            List<Task<LyricInfo>> tasks = new List<Task<LyricInfo>>();
            List<List<LyricInfo>> l = splitToLists(data.Lines, 1);
            
            foreach (LyricInfo inf in data.Lines)
            {
                Task<LyricInfo> t = fillLyrics(inf);
                tasks.Add(t);
            }
            await Task.WhenAll(tasks.ToArray());
      
            return data;
        }

        private static async Task<LyricInfo> fillLyrics(LyricInfo inf)
        {
            JsonObject jsObj = await Bing.ImageSearchJsonAsyncInternal(inf.Text,10);

            JsonArray arr = JsonObject.Parse(jsObj["d"].Stringify())["results"].GetArray();
            foreach (JsonValue val in arr)
            {
                JsonObject thumbnail = JsonObject.Parse(JsonObject.Parse(val.Stringify())["Thumbnail"].Stringify());
                String link = thumbnail["MediaUrl"].Stringify().Trim('\"');
                inf.ImageLinks.Add(link);
            }

            return inf;
        }

        private static List<List<LyricInfo>> splitToLists(IList<LyricInfo> lst, int n)
        {
            int size = lst.Count / n;

            List<List<LyricInfo>> l = new List<List<LyricInfo>>();
            for (int i = 0; i < lst.Count; i++)
            {
                int index = i/n;
                
                if (l.Count < (index+1))
                {
                    l.Add(new List<LyricInfo>());
                }
                l[index].Add(lst[i]);
            }
            return l;
        }

    }

    public sealed class LyricsEventArgs
    {
        public LyricsEventArgs(LyricInfo inf)
        {
            LyricInfo = inf;
        }

        public LyricInfo LyricInfo
        {
            get;
            private set;

        }
    }

    public sealed class LyricInfo
    {
        public int Id { get; private set; }
        public String Text { get; set; }
        public int ShowTimeMillis { get; set; }
        public IList<string> ImageLinks { get; private set; }

        public LyricInfo(int id)
        {
            Id = id;
            ImageLinks = new List<String>();
        }
    }
}
