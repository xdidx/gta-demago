﻿using System;
using System.Collections.Generic;
using IrrKlang;
using System.IO;

namespace DemagoScript
{
    class Music
    {
        private ISoundEngine engine = new ISoundEngine();
        private Dictionary<string, ISound> musicTable = new Dictionary<string, ISound>();
        private static string musicLocation = Environment.CurrentDirectory + @"\Music\";

        public Music(List<string[]> liste)
        {
            Tools.log("---------- Start music loading ----------");
            foreach (string[] keyValue in liste)
            {
                try
                {
                    Tools.log("Loading " + musicLocation + keyValue[1]);
                    musicTable.Add(keyValue[0], engine.Play2D(musicLocation + keyValue[1]));
                    pauseMusic(keyValue[0]);
                }
                catch (Exception ex)
                {
                    Tools.log("Error loading " + musicLocation + keyValue[1] + " : " + ex.Message);
                    musicTable.Remove(keyValue[0]);
                }
            }
            Tools.log("---------- End music loading ----------");
        }

        public Boolean isPlaying(string key)
        {
            if (key == null) {
                return false;
            }
            if ( keyExistInMusicTable(key) ) {
                return !musicTable[key].Finished;
            }
            return false;
        }

        public void playMusic( string key )
        {
            /*if ( this.isPlaying( key ) ) {
                Tools.log( "Music::playMusic - key: " + key + " already playing" );
                return;
            }*/

            if ( keyExistInMusicTable(key) ) {
                Tools.log( "Music::playMusic - key: " + key + " musictable: " + musicTable );
                musicTable[key].Paused = false;
            }
        }

        public void pauseMusic( string key )
        {
            /*if ( !this.isPlaying( key ) ) {
                Tools.log( "Music::pauseMusic - key: " + key + " is not playing" );
                return;
            }*/

            if ( keyExistInMusicTable(key) ) {
                Tools.log( "Music::pauseMusic - key: " + key + " paused" );
                musicTable[key].Paused = true;
            }
        }

        public void restart( string key )
        {
            if ( keyExistInMusicTable(key) ) {
                musicTable[key].PlayPosition = 0;
            }
        }

        private bool keyExistInMusicTable( string key )
        {
            return ( key != null && musicTable != null && musicTable.ContainsKey(key) );
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
