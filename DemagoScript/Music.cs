using System;
using System.Collections.Generic;
using IrrKlang;
using System.IO;

namespace DemagoScript
{
    class Music
    {
        private ISoundEngine engine = new ISoundEngine();
        private Dictionary<string, ISound> musicTable = new Dictionary<string, ISound>();
        private static string musicLocation = Environment.CurrentDirectory + @"\musics\";
        private List<string> listePaused = new List<string>();

        public Music(List<string[]> liste)
        {
            foreach (string[] keyValue in liste)
            {
                musicInitialisation(keyValue);
            }
        }

        public Music(string[] associationBetweenMusicFileAndName)
        {
            musicInitialisation(associationBetweenMusicFileAndName);
        }

        public void musicInitialisation(string[] associationBetweenMusicFileAndName)
        {
            try
            {
                musicTable.Add(associationBetweenMusicFileAndName[0], engine.Play2D(musicLocation + associationBetweenMusicFileAndName[1]));
                musicTable[associationBetweenMusicFileAndName[0]].Paused = true;
            }
            catch (Exception ex)
            {
                Tools.log("Error loading " + musicLocation + associationBetweenMusicFileAndName[1] + " : " + ex.Message);
                musicTable.Remove(associationBetweenMusicFileAndName[0]);
            }
        }

        public bool isPlaying(string key)
        {
            if ( keyExistInMusicTable( key ) )
                return !musicTable[key].Finished && musicTable[key].PlayPosition != 0;

            return false;
        }

        public bool isFinished(string key)
        {
            if ( keyExistInMusicTable( key ) )
                return musicTable[key].Finished;

            return true;
        }


        public uint getPlayingPosition(string key)
        {
            if (keyExistInMusicTable(key))
                return musicTable[key].PlayPosition;

            return 0;
        }

        public void playMusic(string key)
        {
            if ( keyExistInMusicTable( key ) )
            {
                musicTable[key].Paused = false;
            }
        }

        public void pauseMusic( string key )
        {
            if ( keyExistInMusicTable( key ) ) {
                musicTable[key].Paused = true;
            }
        }

        public void restart(string key)
        {
            if (keyExistInMusicTable(key))
            {
                musicTable[key].PlayPosition = 0;
            }
        }

        public void stopMusic(string key)
        {
            if (keyExistInMusicTable(key))
            {
                musicTable[key].PlayPosition = 0;
                musicTable[key].Paused = true;
            }
        }

        public void stopAll()
        {
            foreach (String key in musicTable.Keys)
            {
                if (keyExistInMusicTable(key))
                {
                    musicTable[key].PlayPosition = 0;
                    musicTable[key].Paused = true;
                }
            }
        }

        private bool keyExistInMusicTable( string key )
        {
            return ( key != null && musicTable != null && musicTable.ContainsKey( key ) );
        }

        public int length(string key)
        {
            if (key != null && musicTable.ContainsKey(key))
            {
                return (int) musicTable[key].PlayLength;
            }
            return -1;
        }

        public void dispose()
        {
            engine.Dispose();
            musicTable.Clear();
        }

        public void setVolume( float volume )
        {
            engine.SoundVolume = volume;
        }

        public void stopBeforePause()
        {
            foreach(String key in musicTable.Keys)
            {
                if(!musicTable[key].Paused)
                {
                    listePaused.Add(key);
                    musicTable[key].Paused = true;
                }
            }
        }

        public void restartAfterPause()
        {
            foreach(String key in listePaused)
            {
                musicTable[key].Paused = false;
            }
            listePaused.Clear();
        }
    }
}
