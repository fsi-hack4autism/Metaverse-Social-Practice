using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.Text.RegularExpressions;
using TMPro;
using System;

[RequireComponent(typeof(VideoPlayer))]
public class Captions : MonoBehaviour
{
    public VideoPlayer video => GetComponent<VideoPlayer>();
    public TextAsset captionsFile;
    public string captionsData;
    public VTT vtt;
    [SerializeField] TMP_Text _captionText;

    private void Awake()
    {
        video.sendFrameReadyEvents = true;
        video.frameReady += Video_frameReady;
    }

    private void Update()
    {
        //Debug.Log(video.time);
    }

    private void Start()
    {
        if (captionsFile != null)
        {
            SetCaptionsFile(captionsFile);
        }
        else if (captionsData != null)
        {
            SetCaptionsFromVTT(captionsData);
        }
    }

    public void SetCaptionsFile(TextAsset captionsFile)
    {
        this.captionsFile = captionsFile;
        this.vtt = new VTT(captionsFile.text);
    }

    public void SetCaptionsFromVTT(string vttString)
    {
        this.captionsFile = null;
        this.vtt = new VTT(vttString);
    }

    private void Video_frameReady(VideoPlayer source, long frameIdx)
    {
        // get current time in video and appropriate cue
        float time = (float)source.time;
        Cue cue = vtt.cues.Find(c => c.start <= time && c.end >= time);
        if (cue == null) return;

        // get appropriate caption
        string caption = cue.caption;
        if (cue.speaker.Equals(""))
        {
            _captionText.text = caption;
        }
        else
        {
            _captionText.text = cue.speaker + ": " + caption;
        }
    }

    [System.Serializable]
    public class VTT
    {
        public string header;
        public List<Cue> cues;

        public VTT(string vttString)
        {
            this.header = "";
            this.cues = new List<Cue>();
            Regex cueRegex = new Regex(@"(^\S.*\n)?(([0-9][0-9]:)?[0-9][0-9]:[0-9][0-9].[0-9][0-9][0-9] --> ([0-9][0-9]:)?[0-9][0-9]:[0-9][0-9].[0-9][0-9][0-9].*$)[\s\S]*?$\s$", RegexOptions.Multiline);
            MatchCollection cueMatches = cueRegex.Matches(vttString);
            foreach (Match cueMatch in cueMatches)
            {
                // Debug.Log($"[VTT] cue: {cueMatch.Value}");
                this.cues.Add(new Cue(cueMatch.Value));
            }
        }

        public string ToVTTString()
        {
            string vttString = this.header + "\n\n";
            foreach (Cue cue in this.cues)
            {
                vttString += cue.ToVTTString() + "\n\n";
            }
            // Debug.Log(vttString);
            return vttString;
        }
    }
    [System.Serializable]
    public class Cue
    {

        public string note;
        public string chapter;
        public float start;
        public float end;
        public string speaker;
        public string caption;

        public Cue(string cueString)
        {

            Regex noteRegex = new Regex(
                @"^NOTE[\s\S]*?($\n$)",
                RegexOptions.Multiline);
            Regex chapterRegex = new Regex(
                @"(^\S.*\n)(?=([0-9][0-9]:)?[0-9][0-9]:[0-9][0-9].[0-9][0-9][0-9] --> ([0-9][0-9]:)?[0-9][0-9]:[0-9][0-9].[0-9][0-9][0-9].*$)",
                RegexOptions.Multiline);
            Regex timeRegex = new Regex(
                @"([0-9][0-9]:)?[0-9][0-9]:[0-9][0-9].[0-9][0-9][0-9]",
                RegexOptions.Multiline);
            Regex timeFrameRegex = new Regex(
                @"([0-9][0-9]:)?[0-9][0-9]:[0-9][0-9].[0-9][0-9][0-9] --> ([0-9][0-9]:)?[0-9][0-9]:[0-9][0-9].[0-9][0-9][0-9].*$",
                RegexOptions.Multiline);
            // caption regex includes timeFrameRegex
            Regex captionRegex = new Regex(
                @"(([0-9][0-9]:)?[0-9][0-9]:[0-9][0-9].[0-9][0-9][0-9] --> ([0-9][0-9]:)?[0-9][0-9]:[0-9][0-9].[0-9][0-9][0-9].*$)[\s\S]*?$\s$",
                RegexOptions.Multiline);

            // get note
            MatchCollection noteMatch = noteRegex.Matches(cueString);
            if (noteMatch.Count > 0)
            {
                this.note = noteMatch[0].Value;
                // Debug.Log($"[Cue] note: {noteMatch[0].Value}");
            }

            // get chapter
            MatchCollection chapterMatch = chapterRegex.Matches(cueString);
            if (chapterMatch.Count > 0)
            {
                this.chapter = chapterMatch[0].Value;
                // Debug.Log($"[Cue] chapter: {chapterMatch[0].Value}");
            }

            // get start and end times
            Match timeMatch = timeFrameRegex.Match(cueString);
            if (timeMatch.Success)
            {
                var times = ParseTimes(timeMatch.Value);
                this.start = times.Item1;
                this.end = times.Item2;
                // Debug.Log($"[Cue] times {start} to {end}");
            }
            else
            {
                throw new System.FormatException($"Couldn't find a time in Cue string:\n{cueString}");
            }

            // get captions
            Match captionMatch = captionRegex.Match(cueString);
            if (captionMatch.Success)
            {
                this.caption = captionMatch.Value;

                // Debug.Log($"[Cue] caption: {captionMatch.Value}");
                // remove timeFrame line
                this.caption = string.Join("\n", this.caption.Split("\n"), 1, this.caption.Split("\n").Length - 1);
                this.caption = this.caption.Split("\n\n")[0];

                // check if speaker name is included <v Speaker Name>
                Match speakerMatch = Regex.Match(this.caption, @"(?<=<v\s).*(?=>)");
                this.speaker = "";
                if (speakerMatch.Success)
                {
                    this.caption = this.caption.Substring(this.caption.IndexOf('>') + 1); // extract message
                    this.speaker = speakerMatch.Value; // extract speaker name
                }
            }
            else
            {
                throw new System.FormatException($"[Cue] Couldn't find captions in Cue string:\n{cueString}");
            }
            // Debug.Log($"[Cue]\n{cueString}\n\nconverted to\n\n{this.ToVTTString()}");
        }

        /// <summary>
        /// Converts a VTT time string to seconds
        /// "hh:mm:ss.sss --> hh:mm:ss.sss" -> (seconds, seconds)
        /// </summary>
        /// <param name="timeLine"></param>
        /// <returns></returns>
        private (float, float) ParseTimes(string timeLine)
        {
            Regex timeRegex = new Regex(@"([0-9][0-9]:)?[0-9][0-9]:[0-9][0-9].[0-9][0-9][0-9]");
            // Debug.Log(timeLine);
            MatchCollection timeMatches = timeRegex.Matches(timeLine);
            if (timeMatches.Count != 2)
            {
                throw new System.Exception("Invalid time string: " + timeLine);
            }

            float start = ParseTimeString(timeMatches[0].Value);
            float end = ParseTimeString(timeMatches[1].Value);
            return (start, end);
        }

        /// <summary>
        /// Converts a VTT time string to seconds
        /// hh:mm:ss.sss -> seconds
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private float ParseTimeString(string time)
        {
            string[] timeChunks = time.Trim().Split(':');
            int hour;
            int minute; ;
            float second;

            if (timeChunks.Length == 3)
            {
                hour = int.Parse(timeChunks[0]) - 1;
                minute = int.Parse(timeChunks[1]);
                second = float.Parse(timeChunks[2]);
            }
            else if (timeChunks.Length == 2)
            {
                hour = 0;
                minute = int.Parse(timeChunks[0]);
                second = float.Parse(timeChunks[1]);
            }
            else
            {
                throw new System.Exception("Invalid time string: " + time);
            }

            // convert to seconds
            minute += hour * 60;
            second += minute * 60;
            return second;
        }

        /// <summary>
        /// Converts a time in seconds to a VTT time string
        /// seconds -> hh:mm:ss.sss
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private string TimeToString(float time)
        {
            int hour = 0;
            int minute = 0;
            float second = time;
            // conver to hh:mm:ss.sss
            while (second >= 60)
            {
                second -= 60;
                minute++;
            }
            while (minute >= 60)
            {
                minute -= 60;
                hour++;
            }
            return string.Format("{0:00}:{1:00}:{2:00.000}", hour, minute, second);
        }

        public string ToVTTString()
        {
            string vttString = this.chapter + "\n";
            vttString += this.TimeToString(this.start) + " --> " + this.TimeToString(this.end) + "\n";
            vttString += this.caption + "\n";
            vttString += "\n";
            return vttString;
        }
    }
}
