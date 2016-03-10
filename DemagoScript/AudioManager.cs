using GTA;
using GTA.Native;
using IrrKlang;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DemagoScript
{
    class AudioManager
    {
        private ISoundEngine engine = new ISoundEngine();
        private Dictionary<string, ISound> playlist = new Dictionary<string, ISound>();
        private List<ISound> independantSongs = new List<ISound>();
        private ISound currentInterruptSound = null;
        private int currentSoundIndex = 0;
        private Random random = new Random();

        private static AudioManager instance;

        public static AudioManager Instance
        {
            get
            {
                if (AudioManager.instance == null)
                {
                    AudioManager.instance = new AudioManager();
                }
                return AudioManager.instance;
            }
        }

        ~AudioManager()
        {
            if (engine != null)
            { 
                engine.Dispose();
            }
        }

        public string MusicsLocation
        {
            get
            {
                return Environment.CurrentDirectory + @"\musics\";
            }
        }

        public string FullPrefix
        {
            get
            {
                return MusicsLocation + FilesSubFolder;
            }
        }

        public string FilesSubFolder { get; set; }

        public void update()
        {
            updatePlaylist();
            updateInterruptSounds();
            updateSubtitles();
        }

        private void updatePlaylist()
        {
            if (currentSoundIndex >= playlist.Count)
            {
                return;
            }

            if (playlist.ElementAt(currentSoundIndex).Value.Finished)
            {
                this.playNext();
            }
        }

        private void updateInterruptSounds()
        {
            if (currentInterruptSound == null && DemagoScript.isThereACurrentMission())
            {
                string fileFullPath = "";
                if (Function.Call<Boolean>(Hash.HAS_PED_BEEN_DAMAGED_BY_WEAPON, Game.Player.Character, 0, 2))
                {
                    int next = random.Next(10) + 1;
                    fileFullPath = FullPrefix + "balle" + next + ".wav";
                }
                else
                {
                    if (Function.Call<Boolean>(Hash.HAS_ENTITY_BEEN_DAMAGED_BY_ANY_VEHICLE, Game.Player.Character))
                    {
                        int next = random.Next(8) + 1;
                        fileFullPath = FullPrefix + "voiture" + next + ".wav";
                    }
                    else
                    {
                        if (Function.Call<Boolean>(Hash.HAS_ENTITY_BEEN_DAMAGED_BY_ANY_OBJECT, Game.Player.Character))
                        {
                            int next = random.Next(7) + 1;
                            fileFullPath = FullPrefix + "insulte" + next + ".wav";
                        }
                    }
                }

                if (fileFullPath != "")
                {
                    if (File.Exists(fileFullPath))
                    {
                        if (currentSoundIndex < playlist.Count)
                        {
                            if (!playlist.ElementAt(currentSoundIndex).Key.StartsWith("flic"))
                                playlist.ElementAt(currentSoundIndex).Value.Paused = true;
                        }

                        currentInterruptSound = engine.Play2D(fileFullPath);
                        if (currentInterruptSound != null)
                            currentInterruptSound.Paused = false;
                        else
                            Tools.log("just created currentInterruptSound is null");
                    }
                    else
                    {
                        Tools.log("interruption file doesn't exist"+ fileFullPath);
                    }
                }
            }
            
            if (currentInterruptSound != null && currentInterruptSound.Finished)
            {
                if (currentSoundIndex < playlist.Count)
                {
                    playlist.ElementAt(currentSoundIndex).Value.Paused = true;
                }
                currentInterruptSound.Dispose();
                currentInterruptSound = null;

                Function.Call(Hash.CLEAR_ENTITY_LAST_DAMAGE_ENTITY, Game.Player.Character);
                Function.Call(Hash.CLEAR_ENTITY_LAST_WEAPON_DAMAGE, Game.Player.Character);
            }
        }

        private void updateSubtitles()
        {
            string subtitle = "";

            if (currentSoundIndex < playlist.Count)
            {
                KeyValuePair<string, ISound> currentPair = playlist.ElementAt(currentSoundIndex);
                string fileName = Path.GetFileName(FullPrefix + currentPair.Key + ".wav");

                subtitle = Subtitles.getSubtitle(fileName, (int)currentPair.Value.PlayPosition);
            }

            if (subtitle != "")
            {
                GTA.UI.ShowSubtitle(subtitle);
            }
        }

        public void startPlaylist(string[] soundsNames)
        {
            stopAll();
            foreach (string soundName in soundsNames)
            {
                var fileFullPath = FullPrefix + soundName + ".wav";
                if (File.Exists(fileFullPath))
                {
                    try
                    {
                        playlist.Add(soundName, engine.Play2D(fileFullPath));
                        playlist[soundName].Paused = true;
                    }
                    catch (Exception ex)
                    {
                        Tools.log("Error loading " + fileFullPath + " : " + ex.Message);
                        playlist.Remove(soundName);
                    }
                }
            }
            playlist.First().Value.Paused = false;
        }

        public void startSound(string soundName)
        {
            startPlaylist(new string[] { soundName });
        }

        public void startIndependantSound(string soundName)
        {
            var fileFullPath = FullPrefix + soundName + ".wav";
            if (File.Exists(fileFullPath))
            {
                var independantSound = engine.Play2D(fileFullPath);
                independantSongs.Add(independantSound);
            }
        }

        public void playNext()
        {
            if (currentSoundIndex < playlist.Count)
            {
                ISound currentSound = playlist.ElementAt(currentSoundIndex).Value;
                currentSound.Paused = true;
                currentSound.Dispose();
            }

            currentSoundIndex++;
            if (currentSoundIndex < playlist.Count)
            {
                ISound currentSound = playlist.ElementAt(currentSoundIndex).Value;
                currentSound.PlayPosition = 0;
                currentSound.Paused = false;
            }
        }

        /// <summary>
        /// Set audio pause from a boolean
        ///     pause = true, play = false
        /// </summary>
        /// <param name="state"></param>
        private void setAudioPause( bool state )
        {
            Tools.log( "setAudioPause:" + this.currentSoundIndex + " / " + this.playlist.Count() );

            if ( this.currentSoundIndex < this.playlist.Count ) {
                ISound currentSound = playlist.ElementAt( currentSoundIndex ).Value;
                currentSound.Paused = state;
            }            
        }

        /// <summary>
        /// Pause all sounds
        /// </summary>
        public void pauseAll()
        {
            this.setAudioPause( true );
        }

        /// <summary>
        /// Play all sounds
        /// </summary>
        public void playAll()
        {
            this.setAudioPause( false );
        }

        /// <summary>
        /// Stop all sounds
        /// </summary>
        public void stopAll()
        {
            #region Stop playlist
            foreach ( KeyValuePair<string, ISound> pair in playlist ) {
                pair.Value.Paused = true;
                pair.Value.Dispose();
            }
            playlist.Clear();
            currentSoundIndex = 0;
            #endregion

            #region Stop independant songs
            foreach ( ISound sound in independantSongs ) {
                sound.Paused = true;
                sound.Dispose();
            }
            independantSongs.Clear();
            #endregion

            this.clearSubtitles();
        }

        public void clearSubtitles()
        {
            GTA.UI.ShowSubtitle( "" );
        }

        public int getLength(string soundName)
        {
            int length = 0;

            if (playlist.ContainsKey(soundName))
            {
                length = (int)playlist[soundName].PlayLength;
            }
            else if (File.Exists(FullPrefix + soundName + ".wav"))
            {
                ISound sound = engine.Play2D(FullPrefix + soundName + ".wav");
                length = (int)sound.PlayLength;
                sound.Paused = true;
                sound.Dispose();
            }
            else
            {
                Tools.log("Try to get length on undefined music file : "+ FullPrefix + soundName + ".wav");
            }

            return length;
        }
    }
}
