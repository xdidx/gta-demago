using System;
using System.Collections.Generic;
using IrrKlang;
using System.Threading;

namespace DemagoScript
{
    class Music
    {
        private ISoundEngine engine = new ISoundEngine();
        private Dictionary<string, ISound> musicTable = new Dictionary<string, ISound>();
        private static string musicLocation = @"C:\Program Files\Rockstar Games\Grand Theft Auto V\Music\";

        public Music(List<string[]> liste)
        {
            Tools.log("---------- Start music loading ----------");
            foreach (string[] keyValue in liste)
            {
                try
                {
                    Tools.log("Loading " + musicLocation + keyValue[1]);
                    musicTable.Add(keyValue[0], engine.Play2D(musicLocation + keyValue[1]));
                }
                catch (Exception ex)
                {
                    Tools.log("Error loading " + musicLocation + keyValue[1] + " : " + ex.Message);
                }
                musicTable[keyValue[0]].Paused = true;
            }
            Tools.log("---------- End music loading ----------");
        }

        public Boolean isPlaying(string key)
        {
            if (key == null)
                return false;

            if (musicTable.ContainsKey(key))
                return !musicTable[key].Finished;

            return false;
        }

        public void playMusic(string key)
        {
            if (musicTable.ContainsKey(key))
            {
                    musicTable[key].Paused = false;
            }
        }

        public void pauseMusic(string key)
        {
            if (musicTable.ContainsKey(key))
            {
                musicTable[key].Paused = true;
            }
        }

        public void restart(string key)
        {
            if (musicTable.ContainsKey(key))
            {
                musicTable[key].PlayPosition = 0;
            }
        }

        public void dispose()
        {
            engine.Dispose();
            musicTable.Clear();
        }

        public void setVolume(float volume)
        {
            engine.SoundVolume = volume;
        }
    }
}
