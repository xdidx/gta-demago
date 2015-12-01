using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemagoScript
{
    class DemagoMenu
    {
        private MenuPool menuPool;
        private UIMenu mainMenu;
        private Vector3 teleportationPosition = Joe.joeHomePosition;
        private Vehicle lastSpawnedVehicle = null;
        private bool zeroGravity = false;
        private bool seePlayer = false;
        private bool seeVehicle = false;
        private bool godPlayer = false;
        private bool godVehicle = false;
        private Model oldModel = null;

        public delegate void MenuAction();
        
        public DemagoMenu(List<Mission> missions)
        {
            menuPool = new MenuPool();
            mainMenu = new UIMenu("GTA Demago", "~b~Configuration du mod");
            menuPool.Add(mainMenu);

            //Missions            
            var missionsMenu = menuPool.AddSubMenu(mainMenu, "Missions");
            foreach (Mission mission in missions)
            {
                var missionMenu = menuPool.AddSubMenu(missionsMenu, mission.getName());
                mission.fillMenu(ref missionMenu);
            }

            var teleportToTaxiItem = new UIMenuItem("Se téléporter à la mission taxi");
            missionsMenu.AddItem(teleportToTaxiItem);
            missionsMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == teleportToTaxiItem)
                {
                    Function.Call(Hash.SET_NEW_WAYPOINT, -197f, 4213f);
                    Game.Player.Character.Position = PlacesPositions.TaxiMission;
                }
            };


            //Modèles
            var joeModelItem = new UIMenuItem("Joe l'anticonformiste");
            var fourasModelItem = new UIMenuItem("Père Fourras", "La fonctionnalité est en cours de développement");
            var gastrowModelItem = new UIMenuItem("Gastrow Nomie", "La fonctionnalité est en cours de développement");
            var resetModelItem = new UIMenuItem("Récupérer le modèle de base");

            
            var modelMenu = menuPool.AddSubMenu(mainMenu, "Modèles");
            modelMenu.AddItem(joeModelItem);
            modelMenu.AddItem(fourasModelItem);
            modelMenu.AddItem(gastrowModelItem);
            modelMenu.AddItem(resetModelItem);

            modelMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == joeModelItem)
                {
                    if (oldModel != null)
                    {
                        Function.Call(Hash.SET_PLAYER_MODEL, Game.Player.Handle, oldModel.Hash);
                    }
                    else
                    {
                        oldModel = Game.Player.Character.Model;
                    }

                    Model joeModel = new Model(PedHash.Acult01AMO);
                    joeModel.Request(500);
                    if (joeModel.IsInCdImage && joeModel.IsValid)
                    {
                        while (!joeModel.IsLoaded)
                            Script.Wait(0);

                        Function.Call(Hash.SET_PLAYER_MODEL, Game.Player.Handle, joeModel.Hash);
                        Function.Call(Hash.SET_PED_DEFAULT_COMPONENT_VARIATION, Game.Player.Character.Handle);

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
                if (item == fourasModelItem)
                {
                    if (oldModel != null)
                        Function.Call(Hash.SET_PLAYER_MODEL, Game.Player.Handle, oldModel.Hash);
                    oldModel = Game.Player.Character.Model;
                    //TODO : transformation en Fourras
                }
                if (item == gastrowModelItem)
                {
                    if (oldModel != null)
                        Function.Call(Hash.SET_PLAYER_MODEL, Game.Player.Handle, oldModel.Hash);
                    oldModel = Game.Player.Character.Model;
                    //TODO : transformation en Gastrow
                }
                if (item == resetModelItem)
                {
                    if (oldModel != null)
                    {
                        Function.Call(Hash.SET_PLAYER_MODEL, Game.Player.Handle, oldModel.Hash);
                        oldModel = null;
                    }
                    else
                    {
                        UI.Notify("Vous possédez déjà un modèle de base !");
                    }
                }
            };

            //Vehicules
            var destroyCarsItem = new UIMenuItem("Supprimer les véhicules proches");
            var spawnCarItem = new UIMenuItem("Faire apparaitre un véhicule aléatoire");
            var spawnNiceCarItem = new UIMenuItem("Faire apparaitre un véhicule rapide");
            var healPlayerItem = new UIMenuItem("Se soigner et réparer le véhicule");
            var bikeJoe = new UIMenuItem("Le vélo de Joe", "La fonctionnalité est en cours de développement");
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
                if (item == bikeJoe)
                {
                    //TODO : Spawn du velo de Joe
                }
                if (item == carFouras)
                {
                    //TODO : Spawn de la voiture de Fouras
                }
            };

            //Teleportation
            var teleportItem = new UIMenuItem("Se téléporter");
            var teleportMarkerItem = new UIMenuItem("Se téléporter au marqueur");
            var roadTeleportItem = new UIMenuItem("Se téléporter sur la route");
            var safeTeleportItem = new UIMenuItem("Se téléporter sur le sol", "La fonctionnalité est en cours de développement");
            var xItem = new UIMenuEditableNumericItem("X", teleportationPosition.X, -8000, 8000, 1);
            var yItem = new UIMenuEditableNumericItem("Y", teleportationPosition.Y, -8000, 8000, 1);
            var zItem = new UIMenuEditableNumericItem("Z", teleportationPosition.Z, -8000, 8000, 1);

            var teleportMenu = menuPool.AddSubMenu(mainMenu, "Teleportation");
            teleportMenu.AddItem(teleportItem);
            teleportMenu.AddItem(teleportMarkerItem);
            teleportMenu.AddItem(roadTeleportItem);
            teleportMenu.AddItem(safeTeleportItem);
            teleportMenu.AddItem(xItem);
            teleportMenu.AddItem(yItem);
            teleportMenu.AddItem(zItem);

            teleportMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == roadTeleportItem)
                {
                    Tools.TeleportPlayer(Tools.GetSafeRoadPos(teleportationPosition));
                }
                if (item == safeTeleportItem)
                {
                    /*Tools.GetGroundedPosition() HAVE TO BE UPDATE*/
                    Tools.TeleportPlayer(Tools.GetGroundedPosition(teleportationPosition));
                }
                if (item == teleportItem)
                {
                    Tools.TeleportPlayer(teleportationPosition);
                }
                if (item == teleportMarkerItem)
                {
                    Tools.TeleportPlayerToWaypoint();
                }
            };
            teleportMenu.OnValueChange += (sender, item, value) =>
            {
                if (item == xItem)
                {
                    teleportationPosition.X = value;
                }
                if (item == yItem)
                {
                    teleportationPosition.Y = value;
                }
                if (item == zItem)
                {
                    teleportationPosition.Z = value;
                }
            };
        
            //Outils
            var seePlayerActiveItem = new UIMenuCheckboxItem("Personnage invisible", seePlayer, "Si la case est cochée, votre personnage est invisible");
            var godPlayerActiveItem = new UIMenuCheckboxItem("Personnage invincible", godPlayer, "Si la case est cochée, votre personnage est invincible");
            var seeVehicleActiveItem = new UIMenuCheckboxItem("Vehicle invisible", seeVehicle, "Si la case est cochée, votre véhicule est invisible");
            var godVehicleActiveItem = new UIMenuCheckboxItem("Vehicle invincible", godVehicle, "Si la case est cochée, votre véhicule est invincible");
            var wantedUpItem = new UIMenuItem("Ajouter une étoile");
            var wantedDownItem = new UIMenuItem("Supprimer une étoile");
            var wantedLevelItem = new UIMenuItem("Supprimer toute les étoiles");
            var gravityActiveItem = new UIMenuCheckboxItem("Zéro gravité", zeroGravity, "Si la case est cochée, il n'y aura plus de gravité sur la map entière");
            var showPositionItem = new UIMenuItem("Afficher la position");
            var showRotationItem = new UIMenuItem("Afficher la rotation");

            var toolsMenu = menuPool.AddSubMenu(mainMenu, "Outils");
            toolsMenu.AddItem(wantedLevelItem);
            toolsMenu.AddItem(wantedDownItem);
            toolsMenu.AddItem(wantedUpItem);
            toolsMenu.AddItem(showPositionItem);
            toolsMenu.AddItem(showRotationItem);
            toolsMenu.AddItem(gravityActiveItem);
            toolsMenu.AddItem(seePlayerActiveItem);
            toolsMenu.AddItem(godPlayerActiveItem);
            toolsMenu.AddItem(seeVehicleActiveItem);
            toolsMenu.AddItem(godVehicleActiveItem);

            toolsMenu.OnCheckboxChange += (sender, item, checked_) =>
            {
                Ped player = Game.Player.Character;
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
                    seePlayer = !checked_;
                    player.IsVisible = seePlayer;
                }
                if (item == godPlayerActiveItem)
                {
                    godPlayer = checked_;
                    Game.Player.IsInvincible = godPlayer;
                }
                if (item == seeVehicleActiveItem)
                {
                    seeVehicle = checked_;
                    if (player.IsInVehicle())
                    {
                        player.CurrentVehicle.IsVisible = !seeVehicle;
                    }
                    else
                    {
                        if (Game.Player.LastVehicle != null)
                        {
                            Game.Player.LastVehicle.IsVisible = false;
                        }
                        seeVehicle = false;
                        UI.Notify("Impossible , vous êtes à pied !");
                    }
                }
                if (item == godVehicleActiveItem)
                {
                    godVehicle = checked_;
                    if (player.IsInVehicle())
                    {
                        player.CurrentVehicle.IsInvincible = godVehicle;
                        player.CurrentVehicle.CanTiresBurst = godVehicle;
                    }
                    else
                    {
                        if (Game.Player.LastVehicle != null)
                        {
                            Game.Player.LastVehicle.IsInvincible = false;
                            Game.Player.LastVehicle.CanTiresBurst = false;
                        }
                        godVehicle = false;
                        UI.Notify("Impossible , vous êtes à pied !");
                    }
                }
            };

        }
        
        public void process()
        {
            menuPool.ProcessMenus();
        }

        public void show()
        {
            mainMenu.Visible = true;
        }

        public void hide()
        {
            mainMenu.Visible = false;
        }

        public void toggleDisplay()
        {
            mainMenu.Visible = !mainMenu.Visible;
        }

    }
}
