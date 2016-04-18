using DemagoScript.GUI;
using DemagoScript.GUI.popup;
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
    enum DemagoModel
    {
        Joe,
        Fouras,
        Gastrow
    }

    class ModelManager
    {
        private static ModelManager instance;

        public static ModelManager Instance
        {
            get
            {
                if (ModelManager.instance == null)
                {
                    ModelManager.instance = new ModelManager();
                }
                return ModelManager.instance;
            }
        }

        public void update()
        {
            if (Game.Player.IsDead || Function.Call<bool>(Hash.IS_PLAYER_BEING_ARRESTED, Game.Player, false))
            {
                DemagoScript.pauseCurrentMission();
                AudioManager.Instance.setAudioPause(true);
                this.resetPlayerModel();
            }
        }

        public void setDemagoModel(DemagoModel newModel)
        {
            if (newModel == DemagoModel.Joe)
            {
                if ((uint)Game.Player.Character.Model.Hash == (uint)PedHash.Acult01AMO)
                    return;

                Model joeModel = new Model(PedHash.Acult01AMO);
                joeModel.Request(500);
                if (joeModel.IsInCdImage && joeModel.IsValid)
                {
                    while (!joeModel.IsLoaded)
                        Script.Wait(0);

                    Function.Call(Hash.SET_PLAYER_MODEL, Game.Player.Handle, joeModel.Hash);
                    Function.Call(Hash.SET_PED_DEFAULT_COMPONENT_VARIATION, Game.Player.Character.Handle);

                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 0, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 1, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 2, 1, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 3, 1, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 4, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 5, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 6, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 7, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 8, 2, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 9, 1, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 10, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 11, 0, 0, 2);

                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 0, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 1, 0, 0, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 2, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 3, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 4, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 5, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 6, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 7, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 8, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 9, -1, -1, 2);
                }
            }

            if (newModel == DemagoModel.Fouras)
            {
                if ((uint)Game.Player.Character.Model.Hash == (uint)PedHash.PriestCutscene)
                    return;

                Model fourasModel = new Model(PedHash.PriestCutscene);
                fourasModel.Request(500);
                if (fourasModel.IsInCdImage && fourasModel.IsValid)
                {
                    while (!fourasModel.IsLoaded)
                        Script.Wait(0);

                    Function.Call(Hash.SET_PLAYER_MODEL, Game.Player.Handle, fourasModel.Hash);
                    Function.Call(Hash.SET_PED_DEFAULT_COMPONENT_VARIATION, Game.Player.Character.Handle);

                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 0, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 1, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 2, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 3, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 4, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 5, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 6, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 7, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 8, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 9, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 10, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 11, 0, 0, 2);

                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 0, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 1, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 2, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 3, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 4, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 5, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 6, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 7, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 8, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 9, -1, -1, 2);
                }
            }

            if (newModel == DemagoModel.Gastrow)
            {
                if ((uint)Game.Player.Character.Model.Hash == (uint)PedHash.Migrant01SFY)
                    return;

                Model gastrowModel = new Model(PedHash.Migrant01SFY);
                gastrowModel.Request(500);
                if (gastrowModel.IsInCdImage && gastrowModel.IsValid)
                {
                    while (!gastrowModel.IsLoaded)
                        Script.Wait(0);

                    Function.Call(Hash.SET_PLAYER_MODEL, Game.Player.Handle, gastrowModel.Hash);
                    Function.Call(Hash.SET_PED_DEFAULT_COMPONENT_VARIATION, Game.Player.Character.Handle);

                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 0, 1, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 1, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 2, 1, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 3, 1, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 4, 1, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 5, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 6, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 7, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 8, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 9, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 10, 0, 0, 2);
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character.Handle, 11, 0, 0, 2);

                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 0, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 1, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 2, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 3, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 4, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 5, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 6, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 7, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 8, -1, -1, 2);
                    Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character.Handle, 9, -1, -1, 2);
                }
            }
        }

        /// <summary>
        /// Reset old player model
        /// </summary>
        public void resetPlayerModel()
        {
            bool playerWasDead = false;
            bool playerWasArrested = false;
            Ped player = Game.Player.Character;
            if ((PedHash)player.Model.Hash != DemagoScript.savedPlayerModelHash)
            {
                Vehicle currentVehicle = null;
                if (player.IsInVehicle())
                {
                    currentVehicle = player.CurrentVehicle;
                }

                if (player.IsDead || Function.Call<bool>(Hash.IS_PLAYER_BEING_ARRESTED, Game.Player, false))
                {
                    #region Create remplacement ped
                    Ped replacementPed = Function.Call<Ped>(Hash.CLONE_PED, Game.Player.Character, Function.Call<int>(Hash.GET_ENTITY_HEADING, Function.Call<int>(Hash.PLAYER_PED_ID)), false, true);

                    if (player.IsDead)
                    {
                        playerWasDead = true;
                        replacementPed.Kill();
                    }
                    else
                    {
                        playerWasArrested = true;
                        replacementPed.Task.HandsUp(10000);
                    }

                    replacementPed.MarkAsNoLongerNeeded();
                    #endregion
                }

                #region Change player model with saved PedHash
                var characterModel = new Model(DemagoScript.savedPlayerModelHash);
                characterModel.Request(500);

                if (characterModel.IsInCdImage && characterModel.IsValid)
                {
                    while (!characterModel.IsLoaded) Script.Wait(100);

                    Function.Call(Hash.SET_PLAYER_MODEL, Game.Player, characterModel.Hash);
                    Function.Call(Hash.SET_PED_DEFAULT_COMPONENT_VARIATION, Game.Player.Character.Handle);
                }

                characterModel.MarkAsNoLongerNeeded();
                #endregion

                player = Game.Player.Character;
                if (currentVehicle != null)
                {
                    player.SetIntoVehicle(currentVehicle, VehicleSeat.Driver);
                }

                if (playerWasDead || playerWasArrested)
                {
                    #region Hide real player and wait for game recovery
                    player.IsVisible = false;
                    player.IsInvincible = true;
                    player.Task.StandStill(-1);

                    //Tant qu'on charge le jeu, on fait attendre le script GTA Démago
                    while (!Function.Call<bool>(Hash.IS_PLAYER_PLAYING, Game.Player))
                    {
                        Script.Wait(100);
                    }

                    player.IsVisible = true;
                    player.IsInvincible = false;
                    #endregion

                    while (!Function.Call<bool>(Hash.IS_PLAYER_CONTROL_ON, Game.Player))
                    {
                        Script.Wait(100);
                    }

                    #region Show death or arrested popups
                    var title = (playerWasDead) ? "Vous êtes mort" : "Vous vous êtes fait arrêter";
                    var subtitle = "Voulez-vous revenir au dernier checkpoint ?";

                    ConfirmationPopup checkpointPopup = new ConfirmationPopup(title, subtitle);
                    checkpointPopup.OnPopupAccept += () =>
                    {

                        DemagoScript.loadLastCheckpointOnCurrentMission();
                    };
                    checkpointPopup.OnPopupRefuse += () =>
                    {
                        DemagoScript.stopCurrentMission(true);
                    };
                    checkpointPopup.show();
                    #endregion
                }
            }
        }
    }
}
