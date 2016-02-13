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
        private int currentSoundIndex = 0;
        private string filesPrefix = MusicsLocation;
        private ISound currentInterruptSound = null;
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

        public static string MusicsLocation
        {
            get
            {
                return Environment.CurrentDirectory + @"\musics\";
            }
        }

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
            if (currentInterruptSound == null)
            {
                string fileFullPath = "";
                if (Function.Call<Boolean>(Hash.HAS_PED_BEEN_DAMAGED_BY_WEAPON, Game.Player.Character, 0, 2))
                {
                    int next = random.Next(10) + 1;
                    fileFullPath = filesPrefix + "balle" + next + ".wav";
                }
                else
                {
                    if (Function.Call<Boolean>(Hash.HAS_ENTITY_BEEN_DAMAGED_BY_ANY_VEHICLE, Game.Player.Character))
                    {
                        int next = random.Next(8) + 1;
                        fileFullPath = filesPrefix + "voiture" + next;
                    }
                    else
                    {
                        if (Function.Call<Boolean>(Hash.HAS_ENTITY_BEEN_DAMAGED_BY_ANY_OBJECT, Game.Player.Character))
                        {
                            int next = random.Next(7) + 1;
                            fileFullPath = filesPrefix + "insulte" + next;
                        }
                    }
                }

                if (fileFullPath != "")
                {
                    if (currentSoundIndex <= playlist.Count)
                    {
                        if (!playlist.ElementAt(currentSoundIndex).Key.StartsWith("flic"))
                            playlist.ElementAt(currentSoundIndex).Value.Paused = true;
                    }

                    currentInterruptSound = engine.Play2D(fileFullPath);
                    currentInterruptSound.Paused = false;
                }
            }
            
            if (currentInterruptSound != null && currentInterruptSound.Finished)
            {
                if (currentSoundIndex <= playlist.Count)
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

            if (currentSoundIndex <= playlist.Count)
            {
                KeyValuePair<string, ISound> currentPair = playlist.ElementAt(currentSoundIndex);
                string fileName = Path.GetFileName(this.filesPrefix + currentPair.Key + ".wav");
                subtitle = Subtitles.getSubtitle(fileName, (int)currentPair.Value.PlayPosition);
            }

            if (subtitle != "")
            {
                GTA.UI.ShowSubtitle(subtitle);
            }
        }

        public void startPlaylist(string[] soundsNames, string subfolder = "", string filesPrefix = "")
        {
            stopPlaylist();

            var directoryPath = MusicsLocation + subfolder;
            this.filesPrefix = directoryPath + filesPrefix;
            if (Directory.Exists(directoryPath))
            {
                foreach (string soundName in soundsNames)
                {
                    var fileFullPath = Path.Combine(directoryPath, filesPrefix + soundName + ".wav");
                    if (File.Exists(fileFullPath))
                    {
                        Tools.log("music : " + soundName + " / " +fileFullPath);
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
            }
        }

        public void startSound(string soundName, string subfolder = "", string filesPrefix = "")
        {
            startPlaylist(new string[] { soundName }, subfolder, filesPrefix);
        }

        public void startIndependantSound(string soundName, string subfolder = "", string filesPrefix = "")
        {
            var directoryPath = MusicsLocation + subfolder;
            this.filesPrefix = directoryPath + filesPrefix;
            if (Directory.Exists(directoryPath))
            {
                var fileFullPath = Path.Combine(directoryPath, filesPrefix + soundName + ".wav");
                if (File.Exists(fileFullPath))
                {
                    var independantSound = engine.Play2D(fileFullPath);
                    independantSongs.Add(independantSound);
                }
            }
        }

        public void playNext()
        {
            currentSoundIndex++;
            if (currentSoundIndex < playlist.Count)
            {
                ISound currentSound = playlist.ElementAt(currentSoundIndex).Value;
                currentSound.PlayPosition = 0;
                currentSound.Paused = false;
            }
        }

        public void restartPlaylist()
        {
            currentSoundIndex = 0;
        }

        public void stopPlaylist()
        {
            foreach (KeyValuePair<string, ISound> pair in playlist)
            {
                pair.Value.Dispose();
            }
            playlist.Clear();
        }

        public void stopAll()
        {
            stopPlaylist();
            stopIndependantSounds();
        }

        public void stopIndependantSounds()
        {
            foreach (ISound sound in independantSongs)
            {
                sound.Dispose();
            }
            independantSongs.Clear();
        }

        public int getLength(string soundName)
        {
            if (playlist.ContainsKey(soundName))
            {
                return (int)playlist[soundName].PlayLength;
            }
            else
            {
                return 0;
            }
        }
    }
}
