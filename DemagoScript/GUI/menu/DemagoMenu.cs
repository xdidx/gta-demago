﻿using DemagoScript.GUI;
using DemagoScript.GUI.elements;
using DemagoScript.GUI.popup;
using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemagoScript
{
    class DemagoMenu
    {
        private MenuPool menuPool;
        private UIMenu mainMenu;
        private Vector3 teleportationPosition = Joe.joeHomePosition;
        private Vehicle lastSpawnedVehicle = null;
        private Vehicle lastSpecialVehicle = null;
        private bool zeroGravity = false;
        private bool seePlayer = false;
        private bool seeVehicle = false;
        private bool godPlayer = false;
        private bool godVehicle = false;
        private Vehicle toChangeVehicle = null;

        private Dictionary<Keys, float> lastTimeKeysPressed = new Dictionary<Keys, float>();
        private Dictionary<GTA.Control, float> lastTimeControlPressed = new Dictionary<GTA.Control, float>();
        private int doublePressDelay = 300;

        private UIMenuCheckboxItem godVehicleActiveItem, seeVehicleActiveItem, godPlayerActiveItem, seePlayerActiveItem;

        public delegate void MenuAction();

        public delegate void KeysPressedEvent(Keys key);
        public delegate void KeysDoublePressedEvent(Keys key);

        public delegate void KeysDownEvent(Keys key);

        public delegate void ControlPressedEvent(GTA.Control control);
        public delegate void ControlDoublePressedEvent(GTA.Control control); 

        public delegate void ControlDownEvent(GTA.Control control); 

        /// <summary>
        /// Called when user press a GTA control
        /// </summary>
        public event ControlPressedEvent OnControlPressed;

        /// <summary>
        /// Called when user press a GTA control
        /// </summary>
        public event ControlDownEvent OnControlDown;

        /// <summary>
        /// Called when user press 2 time the same GTA control in a short time
        /// </summary>
        public event ControlDoublePressedEvent OnControlDoublePressed;

        /// <summary>
        /// Called when user press a key on the keyboard
        /// </summary>
        public event KeysPressedEvent OnKeysPressedEvent;

        /// <summary>
        /// Called when user press a key on the keyboard
        /// </summary>
        public event KeysDownEvent OnKeysDownEvent;

        /// <summary>
        /// Called when user pressed 2 time the same key in a short time
        /// </summary>
        public event KeysDoublePressedEvent OnKeysDoublePressedEvent;

        public DemagoMenu(List<Mission> missions = null)
        {
            if (missions == null)
            {
                missions = new List<Mission>();
            }

            #region Controls gestion
            this.OnKeysPressedEvent += (Keys key) => {
                if (key == Keys.F5)
                {
                    this.toggleDisplay();
                }
            };
            this.OnControlDoublePressed += (GTA.Control control) => {
                if (control == GTA.Control.SelectWeapon)
                {
                    this.toggleDisplay();
                }
            };
            #endregion

            #region Menu creation
            menuPool = new MenuPool();
            mainMenu = new UIMenu("GTA Demago", "~b~Configuration du mod");
            menuPool.Add(mainMenu);

            //Missions
            var missionsMenu = menuPool.AddSubMenu(mainMenu, "Missions");
            foreach (Mission mission in missions)
            {
                var missionMenu = menuPool.AddSubMenu(missionsMenu, mission.getName());
                var startItem = mission.addStartItem(ref missionMenu);

                missionMenu.OnItemSelect += (sender, item, index) =>
                {
                    if (item == startItem)
                    {
                        DemagoScript.stopCurrentMission(true);
                        mission.start();
                    }
                };
            }

            var teleportToTaxiItem = new UIMenuItem("Se téléporter à la mission taxi");
            var stopCurrentMissionItem = new UIMenuItem("Stopper la mission");

            missionsMenu.AddItem(teleportToTaxiItem);
            missionsMenu.AddItem(stopCurrentMissionItem);

            missionsMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == stopCurrentMissionItem)
                {
                    DemagoScript.stopCurrentMission(true);
                }

                if (item == teleportToTaxiItem)
                {
                    Function.Call(Hash.SET_NEW_WAYPOINT, -197f, 4213f);
                    Game.Player.Character.Position = PlacesPositions.TaxiMission;
                }
            };


            //Modèles
            var joeModelItem = new UIMenuItem("Joe l'anticonformiste");
            var fourasModelItem = new UIMenuItem("Père Fouras", "La fonctionnalité est en cours de développement");
            var gastrowModelItem = new UIMenuItem("Gastrow Nomie", "La fonctionnalité est en cours de développement");
            var dissociateModelItem = new UIMenuItem("Dissocier", "Dissocier son corps avec le personnage le plus proche");
            var resetModelItem = new UIMenuItem("Récupérer le modèle de base");


            var modelMenu = menuPool.AddSubMenu(mainMenu, "Modèles");
            modelMenu.AddItem(joeModelItem);
            modelMenu.AddItem(fourasModelItem);
            modelMenu.AddItem(gastrowModelItem);
            modelMenu.AddItem(dissociateModelItem);
            modelMenu.AddItem(resetModelItem);

            modelMenu.OnItemSelect += (sender, item, index) =>
            {
                if ((PedHash)Game.Player.Character.Model.Hash == PedHash.Michael || (PedHash)Game.Player.Character.Model.Hash == PedHash.Franklin || (PedHash)Game.Player.Character.Model.Hash == PedHash.Trevor)
                {
                    DemagoScript.savedPlayerModelHash = (PedHash)Game.Player.Character.Model.Hash;
                }

                if (item == resetModelItem && DemagoScript.savedPlayerModelHash == (PedHash)Game.Player.Character.Model.Hash)
                {
                    UI.Notify("Vous possédez déjà le modèle de base !");
                }

                if (DemagoScript.savedPlayerModelHash != (PedHash)Game.Player.Character.Model.Hash)
                {
                    //Reset to old model
                    Function.Call(Hash.SET_PLAYER_MODEL, Game.Player.Handle, (int)DemagoScript.savedPlayerModelHash);
                }



                if (item == dissociateModelItem)
                {
                    Ped closestPed = Tools.GetClosestPedAroundPlayer();
                    if (closestPed != null && closestPed.Exists())
                    {
                        Tools.setModel(closestPed.Model);

                        if (closestPed.Position != Vector3.Zero)
                        {
                            Vector3 oldPlayerPosition = Game.Player.Character.Position;
                            Game.Player.Character.Position = closestPed.Position;
                            closestPed.Position = oldPlayerPosition;
                        }
                        else
                        {
                            Tools.log("Closest ped position == 0 ...");
                        }
                    }
                    else
                    {
                        GTA.UI.Notify("Pas de personnage proche...");
                    }
                }

                if (item == joeModelItem)
                {
                    ModelManager.Instance.setDemagoModel(DemagoModel.Joe);
                }

                if (item == fourasModelItem)
                {
                    ModelManager.Instance.setDemagoModel(DemagoModel.Fouras);
                }

                if (item == gastrowModelItem)
                {
                    ModelManager.Instance.setDemagoModel(DemagoModel.Gastrow);
                }
            };

            //Vehicules
            var destroyCarsItem = new UIMenuItem("Supprimer les véhicules proches");
            var spawnCarItem = new UIMenuItem("Faire apparaitre un véhicule aléatoire");
            var spawnNiceCarItem = new UIMenuItem("Faire apparaitre un véhicule rapide");
            var healPlayerItem = new UIMenuItem("Se soigner et réparer le véhicule");
            var bikeJoe = new UIMenuItem("Le vélo de Joe");
            var carFouras = new UIMenuItem("La voiture du père Fouras", "La fonctionnalité est en cours de développement");

            var vehiclesMenu = menuPool.AddSubMenu(mainMenu, "Véhicules");
            vehiclesMenu.AddItem(healPlayerItem);
            vehiclesMenu.AddItem(spawnCarItem);
            vehiclesMenu.AddItem(spawnNiceCarItem);
            vehiclesMenu.AddItem(destroyCarsItem);
            vehiclesMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == destroyCarsItem)
                {
                    int counter = 0;
                    foreach (Vehicle vehicle in World.GetNearbyVehicles(Game.Player.Character, 10))
                    {
                        if (vehicle != Game.Player.Character.CurrentVehicle)
                        {
                            counter++;
                            vehicle.Delete();
                        }
                    }
                    GTA.UI.Notify(counter + " véhicule(s) supprimé(s) !");
                }
                if (item == spawnCarItem)
                {
                    Array vehicleValues = Enum.GetValues(typeof(VehicleHash));
                    Random random = new Random();
                    VehicleHash randomValue = (VehicleHash)vehicleValues.GetValue(random.Next(vehicleValues.Length));
                    if (lastSpawnedVehicle != null && lastSpawnedVehicle != Game.Player.Character.CurrentVehicle)
                    {
                        lastSpawnedVehicle.Delete();
                    }
                    lastSpawnedVehicle = World.CreateVehicle(randomValue, Tools.GetSafeRoadPos(Game.Player.Character.Position));
                }
                if (item == spawnNiceCarItem)
                {
                    if (lastSpawnedVehicle != null && lastSpawnedVehicle != Game.Player.Character.CurrentVehicle)
                    {
                        lastSpawnedVehicle.Delete();
                    }
                    lastSpawnedVehicle = World.CreateVehicle(VehicleHash.T20, Tools.GetSafeRoadPos(Game.Player.Character.Position));
                }
                if (item == healPlayerItem)
                {
                    Game.Player.Character.Health = Game.Player.Character.MaxHealth;
                    Game.Player.Character.Armor = 100;
                    if (Game.Player.Character.IsInVehicle())
                    {
                        Game.Player.Character.CurrentVehicle.Repair();
                    }
                }
            };

            var specialVehiclesMenu = menuPool.AddSubMenu(vehiclesMenu, "Véhicules spéciaux");
            specialVehiclesMenu.AddItem(bikeJoe);
            specialVehiclesMenu.AddItem(carFouras);
            specialVehiclesMenu.OnItemSelect += (sender, item, index) =>
            {
                if (lastSpecialVehicle != null && lastSpecialVehicle.Exists())
                {
                    lastSpecialVehicle.Delete();
                    lastSpecialVehicle = null;
                }

                if (item == bikeJoe)
                {
                    Vehicle bike = null;
                    do
                    {
                        bike = World.CreateVehicle(VehicleHash.TriBike, Game.Player.Character.Position.Around(2));
                    } while (bike == null || !bike.Exists());

                    lastSpecialVehicle = bike;
                }
                if (item == carFouras)
                {
                    //TODO : Spawn de la voiture de Fouras
                }
            };

            //Outils
            seePlayerActiveItem = new UIMenuCheckboxItem("Personnage invisible", seePlayer, "Si la case est cochée, votre personnage est invisible");
            godPlayerActiveItem = new UIMenuCheckboxItem("Personnage invincible", godPlayer, "Si la case est cochée, votre personnage est invincible");
            seeVehicleActiveItem = new UIMenuCheckboxItem("Vehicle invisible", seeVehicle, "Si la case est cochée, votre véhicule est invisible");
            godVehicleActiveItem = new UIMenuCheckboxItem("Vehicle invincible", godVehicle, "Si la case est cochée, votre véhicule est invincible");
            var teleportMarkerItem = new UIMenuItem("Se téléporter au marqueur");
            var wantedUpItem = new UIMenuItem("Ajouter une étoile");
            var wantedDownItem = new UIMenuItem("Supprimer une étoile");
            var wantedLevelItem = new UIMenuItem("Supprimer toutes les étoiles");
            var addMoney = new UIMenuItem("Ajouter 50.000$");
            var removeMoney = new UIMenuItem("Enlever 50.000$");
            var gravityActiveItem = new UIMenuCheckboxItem("Zéro gravité", zeroGravity, "Si la case est cochée, il n'y aura plus de gravité sur la map entière");
            var showPositionItem = new UIMenuItem("Afficher la position");
            var showRotationItem = new UIMenuItem("Afficher la rotation");
            
            var toolsMenu = menuPool.AddSubMenu(mainMenu, "Outils");
            toolsMenu.AddItem(wantedLevelItem);
            toolsMenu.AddItem(wantedDownItem);
            toolsMenu.AddItem(wantedUpItem);
            toolsMenu.AddItem(teleportMarkerItem);
            toolsMenu.AddItem(addMoney);
            toolsMenu.AddItem(removeMoney);
            toolsMenu.AddItem(gravityActiveItem);
            toolsMenu.AddItem(seePlayerActiveItem);
            toolsMenu.AddItem(godPlayerActiveItem);
            toolsMenu.AddItem(seeVehicleActiveItem);
            toolsMenu.AddItem(godVehicleActiveItem);
            toolsMenu.AddItem(showPositionItem);
            toolsMenu.AddItem(showRotationItem);

            toolsMenu.OnItemSelect += (sender, item, checked_) =>
            {
                if (item == teleportMarkerItem)
                {
                    Tools.TeleportPlayerToWaypoint();
                }

                if (item == showPositionItem)
                {
                    GTA.UI.Notify("player X : " + Game.Player.Character.Position.X + " / Y : " + Game.Player.Character.Position.Y + " / Z : " + Game.Player.Character.Position.Z);
                }
                if (item == showRotationItem)
                {
                    GTA.UI.Notify("rot X : " + Game.Player.Character.Rotation.X + " / Y : " + Game.Player.Character.Rotation.Y + " / Z : " + Game.Player.Character.Rotation.Z);
                }
                if (item == wantedDownItem)
                {
                    if (Game.Player.WantedLevel > 0)
                        Game.Player.WantedLevel--;
                }
                if (item == wantedLevelItem)
                {
                    Game.Player.WantedLevel = 0;
                }
                if (item == wantedUpItem)
                {
                    if (Game.Player.WantedLevel < 5)
                        Game.Player.WantedLevel++;
                }
                if (item == addMoney)
                {
                    Game.Player.Money += 50000;
                }
                if (item == removeMoney)
                {
                    if (Game.Player.Money > 50000)
                        Game.Player.Money -= 50000;
                    else
                        Game.Player.Money = 0;
                }
            };


            toolsMenu.OnCheckboxChange += (sender, item, checked_) =>
            { 
                Ped player = Game.Player.Character;
                if (item == gravityActiveItem)
                {
                    zeroGravity = checked_;
                    if (zeroGravity)
                    {
                        Function.Call(Hash.SET_GRAVITY_LEVEL, 3);
                    }
                    else
                    {
                        Function.Call(Hash.SET_GRAVITY_LEVEL, 0);
                    }
                }
                if (item == seePlayerActiveItem)
                {
                    seePlayer = checked_;
                    player.IsVisible = !seePlayer;
                }
                if (item == godPlayerActiveItem)
                {
                    godPlayer = checked_;
                    Game.Player.IsInvincible = godPlayer;
                }

                if (item == seeVehicleActiveItem)
                {
                    if (player.IsInVehicle())
                    {
                        seeVehicle = checked_;
                        toChangeVehicle.IsVisible = !seeVehicle;
                        player.IsVisible = !seePlayer;
                    }
                    else
                    {
                        seeVehicleActiveItem.Checked = false;
                        UI.Notify("Impossible , vous êtes à pied !");
                    }
                }
                if (item == godVehicleActiveItem)
                {
                    if (player.IsInVehicle())
                    {
                        godVehicle = checked_;
                        toChangeVehicle.IsInvincible = godVehicle;
                        toChangeVehicle.CanTiresBurst = godVehicle;
                    }
                    else
                    {
                        godVehicleActiveItem.Checked = false;
                        UI.Notify("Impossible , vous êtes à pied !");
                    }
                }
            };
            #endregion
        }

        public void process()
        {
            controlsGestion();

            menuPool.ProcessMenus();

            Ped player = Game.Player.Character;
            if (playerModelIsValid())
            {
                if (player.IsInVehicle() && toChangeVehicle == null)
                {
                    toChangeVehicle = player.CurrentVehicle;

                    godVehicle = toChangeVehicle.IsInvincible;
                    godVehicleActiveItem.Checked = toChangeVehicle.IsInvincible;
                    seeVehicle = toChangeVehicle.IsVisible;
                    seeVehicleActiveItem.Checked = !toChangeVehicle.IsVisible;
                }

                if (!player.IsInVehicle() && toChangeVehicle != null)
                {
                    if (godVehicle)
                    {
                        godVehicle = false;
                        godVehicleActiveItem.Checked = false;
                        toChangeVehicle.IsInvincible = false;
                        toChangeVehicle.CanTiresBurst = false;
                    }

                    if (seeVehicle)
                    {
                        seeVehicle = false;
                        seeVehicleActiveItem.Checked = false;
                        toChangeVehicle.IsVisible = true;
                        player.IsVisible = !seePlayer;
                    }

                    toChangeVehicle = null;
                }
            }
        }

        private bool playerModelIsValid()
        {
            Ped player = Game.Player.Character;
            return ((PedHash)player.Model.Hash == DemagoScript.savedPlayerModelHash || (!player.IsDead && !Function.Call<bool>(Hash.IS_PLAYER_BEING_ARRESTED, Game.Player, false)));
        }
        
        public void controlsGestion()
        {
            foreach (GTA.Control control in Enum.GetValues(typeof(GTA.Control)))
            {
                if (Game.IsControlJustPressed(2, control))
                {
                    OnControlDown?.Invoke(control);
                }

                if (Game.IsControlJustReleased(2, control))
                {
                    if (((IDictionary)lastTimeControlPressed).Contains(control) && (DemagoScript.getScriptTime() - lastTimeControlPressed[control]) < doublePressDelay)
                    {
                        lastTimeControlPressed[control] = 0;
                        OnControlDoublePressed?.Invoke(control);
                    }
                    else
                    {
                        OnControlPressed?.Invoke(control);
                    }

                    if (((IDictionary)lastTimeControlPressed).Contains(control))
                    {
                        lastTimeControlPressed[control] = DemagoScript.getScriptTime();
                    }
                    else
                    {
                        lastTimeControlPressed.Add(control, DemagoScript.getScriptTime());
                    }
                }
            }
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            menuPool.ResetKey(NativeUI.UIMenu.MenuControls.Up);
            menuPool.ResetKey(NativeUI.UIMenu.MenuControls.Down);
            menuPool.ResetKey(NativeUI.UIMenu.MenuControls.Left);
            menuPool.ResetKey(NativeUI.UIMenu.MenuControls.Right);
            menuPool.ResetKey(NativeUI.UIMenu.MenuControls.Select);
            menuPool.ResetKey(NativeUI.UIMenu.MenuControls.Back);

            menuPool.SetKey(NativeUI.UIMenu.MenuControls.Up, GTA.Control.PhoneUp);
            menuPool.SetKey(NativeUI.UIMenu.MenuControls.Down, GTA.Control.PhoneDown);
            menuPool.SetKey(NativeUI.UIMenu.MenuControls.Left, GTA.Control.PhoneLeft);
            menuPool.SetKey(NativeUI.UIMenu.MenuControls.Right, GTA.Control.PhoneRight);
            menuPool.SetKey(NativeUI.UIMenu.MenuControls.Select, GTA.Control.PhoneSelect);
            menuPool.SetKey(NativeUI.UIMenu.MenuControls.Back, GTA.Control.PhoneCancel);

            menuPool.SetKey(NativeUI.UIMenu.MenuControls.Up, Keys.NumPad8);
            menuPool.SetKey(NativeUI.UIMenu.MenuControls.Down, Keys.NumPad2);
            menuPool.SetKey(NativeUI.UIMenu.MenuControls.Left, Keys.NumPad4);
            menuPool.SetKey(NativeUI.UIMenu.MenuControls.Right, Keys.NumPad6);
            menuPool.SetKey(NativeUI.UIMenu.MenuControls.Select, Keys.NumPad5);
            menuPool.SetKey(NativeUI.UIMenu.MenuControls.Back, Keys.NumPad0);
            
            menuPool.ProcessKey(e.KeyCode);

            OnKeysDownEvent?.Invoke(e.KeyCode);
        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            Keys key = e.KeyCode;
            if (((IDictionary)lastTimeKeysPressed).Contains(key) && (DemagoScript.getScriptTime() - lastTimeKeysPressed[key]) < doublePressDelay)
            {
                lastTimeKeysPressed[key] = 0;
                OnKeysDoublePressedEvent?.Invoke(key);
            }
            else
            {
                OnKeysPressedEvent?.Invoke(key);
            }

            if (((IDictionary)lastTimeKeysPressed).Contains(key))
            {
                lastTimeKeysPressed[key] = DemagoScript.getScriptTime();
            }
            else
            {
                lastTimeKeysPressed.Add(key, DemagoScript.getScriptTime());
            }
        }

        public void toggleDisplay()
        {
            if (menuPool.isVisible())
            {
                menuPool.hideAll();
            }
            else
            {
                mainMenu.Visible = true;
            }
        }
        
        public void hide()
        {
            menuPool.hideAll();
            mainMenu.Visible = false;
        }

        public void show()
        {
            mainMenu.Visible = true;
        }

        public MenuPool getMenuPool()
        {
            return menuPool;
        }
    }
}
