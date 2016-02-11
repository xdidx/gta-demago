using System;
using System.Collections.Generic;
using IrrKlang;
using System.IO;
using System.Text.RegularExpressions;

namespace DemagoScript
{
    static class Subtitles
    {
        private static Dictionary<string, Dictionary<int, string>> subtitles = null;

        private static void updateSubtitles()
        {
            subtitles = new Dictionary<string, Dictionary<int, string>>();

            string currentSongName = "";
            Dictionary<int, string> currentSongSubtitles = null;
            string[] lines = null;

            try
            {
                 lines = File.ReadAllLines(@"Music\joe-subtitles.txt");
            }
            catch (Exception e)
            {
                Tools.log(e.Message);
            }

            if (lines != null)
            {
                for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
                {
                    string currentLine = lines[lineIndex];
                    if (currentLine == "")
                    {
                        if (currentSongName != "" && currentSongSubtitles != null)
                        {
                            subtitles.Add(currentSongName, currentSongSubtitles);
                            lineIndex++;
                            currentLine = lines[lineIndex];
                        }
                        currentSongName = "";
                        currentSongSubtitles = null;
                    }

                    if (currentSongName == "" && currentLine.Length >= 4 && currentLine.Substring(currentLine.Length - 4) == ".wav")
                    {
                        currentSongName = currentLine;
                        currentSongSubtitles = new Dictionary<int, string>();
                        lineIndex++;
                        currentLine = lines[lineIndex];
                    }

                    if (currentSongName != "" && currentSongSubtitles != null)
                    {
                        Regex regex = new Regex("^([0-9]{2}):([0-9]{2})$");
                        Match matches = regex.Match(currentLine);
                        if (matches != null && matches.Success && matches.Groups.Count == 3)
                        {
                            int minutes = Convert.ToInt32(matches.Groups[1].ToString()),
                                seconds = Convert.ToInt32(matches.Groups[2].ToString());

                            int totalSeconds = (minutes * 60) + seconds;

                            lineIndex++;
                            currentSongSubtitles.Add(totalSeconds, lines[lineIndex]);
                        }
                        else if (currentLine != "")
                        {
                            currentSongSubtitles.Add(0, currentLine);
                        }
                    }
                }

                if (currentSongName != "" && currentSongSubtitles != null)
                {
                    subtitles.Add(currentSongName, currentSongSubtitles);
                }
            }
        }

        public static string getSubtitle(string songName, int songTime)
        {
            string subtitleToShow = "";

            if (subtitles == null)
            {
                Tools.log("null subtitles");
                updateSubtitles();
            }

            if (subtitles != null && subtitles.ContainsKey(songName))
            {
                Dictionary<int, string> songSubtitles = subtitles[songName];
                int maximumSeconds = -1;
                foreach (KeyValuePair<int, string> subtitleAssociation in songSubtitles)
                {
                    if (subtitleAssociation.Key > maximumSeconds && subtitleAssociation.Key <= songTime / 1000)
                    {
                        subtitleToShow = subtitleAssociation.Value;
                        maximumSeconds = subtitleAssociation.Key;
                    }
                }
            }
            else
            {
                Tools.log("Son sans sous titre : "+songName);
            }

            return subtitleToShow;
        }
    }
}
