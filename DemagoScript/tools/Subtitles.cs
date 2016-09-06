using System;
using System.Collections.Generic;
using IrrKlang;
using System.IO;
using System.Text.RegularExpressions;

namespace DemagoScript
{
    static class Subtitles
    {
        /// <summary>
        /// dictionnaire ayant pour clé le nom de la mission
        /// et pour valeur un dictionnaire ayant pour clé le temps 
        /// auquel on doit afficher le sous-titre qui est en valeur
        /// TODO:translate en
        /// </summary>
        private static Dictionary<string, Dictionary<int, string>> subtitles = null;
        public static string SubtitlesPath { get; set; }

        private static void getSubtitlesFromFile(string subtitlesPath = "")
        {
            if (Subtitles.SubtitlesPath == "")
            {
                Tools.log("Subtitles path is empty");
                return;
            }
                

            subtitles = new Dictionary<string, Dictionary<int, string>>();

            string currentSongName = "";
            Dictionary<int, string> currentSongSubtitles = null;
            string[] lines = null;

            try
            {
                lines = File.ReadAllLines(subtitlesPath);
            }
            catch (Exception e)
            {
                Tools.log("Fichier de sous titre non valide " + e.Message);
                return;
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
                        currentSongName = currentLine.ToLower();
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
                            if (currentSongSubtitles.ContainsKey(totalSeconds))
                            {
                                Tools.log("Ligne déjà ajoutée : " + totalSeconds + " " + lines[lineIndex]);
                            }
                            else
                            { 
                                currentSongSubtitles.Add(totalSeconds, lines[lineIndex]);
                            }
                        }
                        else if (currentLine != "")
                        {
                            if (currentSongSubtitles.ContainsKey(0))
                            {
                                Tools.log("Ligne déjà ajoutée : 0 " + lines[lineIndex]);
                            }
                            else
                            {
                                currentSongSubtitles.Add(0, currentLine);
                            }
                        }
                    }
                }

                if (currentSongName != "" && currentSongSubtitles != null)
                {
                    subtitles.Add(currentSongName, currentSongSubtitles);
                }
            }
        }
        private static string without = "";
        public static string getSubtitle(string songName, int songTime)
        {
            string subtitleToShow = "";

            songName = songName.ToLower();

            if (subtitles == null)
            {
                Subtitles.getSubtitlesFromFile(Subtitles.SubtitlesPath);
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
            else if(without != songName)
            {
                without = songName;
                Tools.log("Son sans sous titre : "+songName);
            }

            return subtitleToShow;
        }
    }
}
