using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemagoScript
{
    class Speed : Mission
    {
        private bool trapStarted = false;
        private bool notifSent = false;

        private bool activatedExplosion = true;
        private bool activatedSpeedCounter = true;
        private bool activedRemoveTraffic = true;
        private bool activedRemoveWantedSearch = true;
        private bool activatedEscort = true;

        private Vehicle bus = null; 
        private List<Ped> passengers = new List<Ped>(); 
        private Ped policeMan1 = null;
        private Ped policeMan2 = null;
        private Vehicle policeCar1 = null;
        private Vehicle policeCar2 = null;
        private Vehicle lastVehicle = null; 
        private float bombTriggerSpeedInKmh = 40.0f;
        private float escortDistance = 30.0f;

        private bool policeMan1IsWaiting = true;
        private bool policeMan1IsRushing = true;
        private bool policeMan2IsWaiting = true;
        private bool policeMan2IsRushing = true;

        public static Vector3 startPosition { get; } = new Vector3(50f, -1200f, 40f);
        public static Vector3 aeroportPosition { get; } = new Vector3(-1685, -2930, 13f);
        public static List<Vector3> checkPoints { get; } = new List<Vector3> { new Vector3(-452f, -1403f, 33f), new Vector3(-946f, -2755, 13f), aeroportPosition };

        private UIResText speedCounterText = null;
        private UIResText speedCounterShadow = null;

        public Speed()
        {
            this.name = "Mission Speed";
        }

        protected override void populateDestructibleElements()
        {
            base.populateDestructibleElements();
            
            if (activatedEscort)
            {
                escortDistance = 30f;
                initializeEscort(startPosition);
            }

            if (activatedSpeedCounter)
            {
                speedCounterText = new UIResText("", new Point(330, Game.ScreenResolution.Height - 70), 0.7f, Color.WhiteSmoke, GTA.Font.ChaletComprimeCologne, UIResText.Alignment.Left);
                speedCounterShadow = new UIResText("", new Point(332, Game.ScreenResolution.Height - 68), 0.7f, Color.Black, GTA.Font.ChaletComprimeCologne, UIResText.Alignment.Left);
            }

            bus = World.CreateVehicle(VehicleHash.Bus, Tools.GetSafeRoadPos(startPosition));
            bus.Rotation = new Vector3(0, 0, 90);
            Game.Player.Character.SetIntoVehicle(bus, VehicleSeat.Driver);

            int passengersNumber = (int)Math.Round((decimal)bus.PassengerSeats / 2);
            for (int i = 0; i < passengersNumber; i++)
            {
                Ped passenger = World.CreatePed(PedHash.Business01AFY, Game.Player.Character.Position);
                if (passenger != null && passenger.Exists())
                {
                    passenger.DrivingStyle = DrivingStyle.AvoidTrafficExtremely;
                    passenger.SetIntoVehicle(bus, VehicleSeat.Any);
                    passengers.Add(passenger);
                }
                else
                {
                    Tools.log("Passenger not created");
                }
            }

            foreach (Vector3 checkpoint in checkPoints)
            {
                GoToPositionInVehicle goToFirstCheckpointObjective = new GoToPositionInVehicle(checkpoint);
                goToFirstCheckpointObjective.OnStarted += (sender) =>
                {
                    goToFirstCheckpointObjective.setVehicle(bus);
                };
                addObjective(goToFirstCheckpointObjective);
            }
        }

        protected override void depopulateDestructibleElements(bool removePhysicalElements = false)
        {
            base.depopulateDestructibleElements(removePhysicalElements);

            if (bus != null && bus.Exists())
            {
                bus.MarkAsNoLongerNeeded();
                if (removePhysicalElements)
                {
                    bus.Delete();
                    bus = null;
                }
            }

            foreach (Ped passenger in passengers)
            {
                if (passenger != null && passenger.Exists())
                {
                    passenger.MarkAsNoLongerNeeded();
                    if (removePhysicalElements)
                        passenger.Delete();
                }
            }
            if (removePhysicalElements)
                passengers.Clear();

            if (policeMan1 != null && policeMan1.Exists())
            {
                if (policeMan1.CurrentVehicle != null && policeMan1.CurrentVehicle.Exists())
                {
                    policeMan1.CurrentVehicle.MarkAsNoLongerNeeded();
                    if (removePhysicalElements)
                        policeMan1.CurrentVehicle.Delete();
                }
                policeMan1.MarkAsNoLongerNeeded();
                if (removePhysicalElements)
                {
                    policeMan1.Delete();
                    policeMan1 = null;
                }
            }

            if (policeMan2 != null && policeMan2.Exists())
            {
                if (policeMan2.CurrentVehicle != null && policeMan2.CurrentVehicle.Exists())
                {
                    policeMan2.CurrentVehicle.MarkAsNoLongerNeeded();
                    if (removePhysicalElements)
                        policeMan2.CurrentVehicle.Delete();
                }
                policeMan2.MarkAsNoLongerNeeded();
                if (removePhysicalElements)
                {
                    policeMan2.Delete();
                    policeMan2 = null;
                }
            }
        }
        
        public void initializeEscort(Vector3 startPosition)
        {
            if (policeCar1 != null && policeCar1.Exists())
            {
                policeCar1.Delete();
                policeCar1 = null;
            }
            if (policeCar2 != null && policeCar2.Exists())
            {
                policeCar2.Delete();
                policeCar2 = null;
            }
            if (policeMan1 != null && policeMan1.Exists())
            {
                policeMan1.Delete();
                policeMan1 = null;
            }
            if (policeMan2 != null && policeMan2.Exists())
            {
                policeMan2.Delete();
                policeMan2 = null;
            }

            Vector3 policePos1 = Tools.GetSafeRoadPos(startPosition - new Vector3(escortDistance, 0, 0));
            policeCar1 = World.CreateVehicle(VehicleHash.Police, policePos1);
            if (policeCar1 != null)
            {
                policeCar1.SirenActive = true;
                policeCar1.EngineRunning = true;

                policeMan1 = World.CreatePed(PedHash.Cop01SMY, policePos1);
                if (policeMan1 != null)
                {
                    policeMan1.SetIntoVehicle(policeCar1, VehicleSeat.Driver);

                    policeMan1.Task.ClearAll();
                    policeMan1.AlwaysKeepTask = true;
                }
                else
                {
                    Tools.log("impossible de créer policeMan1");
                }
            }
            else
            {
                Tools.log("impossible de créer policeCar1");
            }

            Vector3 policePos2 = Tools.GetSafeRoadPos(startPosition + new Vector3(escortDistance, 0, 0));

            policeCar2 = World.CreateVehicle(VehicleHash.Police, policePos2);
            if (policeCar2 != null)
            {
                policeCar2.SirenActive = true;
                policeCar2.EngineRunning = true;

                policeMan2 = World.CreatePed(PedHash.Cop01SMY, policePos2);
                if (policeMan2 != null)
                {
                    policeMan2.SetIntoVehicle(policeCar2, VehicleSeat.Driver);

                    policeMan2.Task.ClearAll();
                    policeMan2.AlwaysKeepTask = true;
                }
                else
                {
                    Tools.log("impossible de créer policeMan2");
                }
            }
            else
            {
                Tools.log("impossible de créer policeCar2");
            }
        }
        
        public override bool update()
        {
            if (!base.update())
            {
                return false;
            }

            if (activatedSpeedCounter)
            {
                showSpeedCounter();
            }

            if (activedRemoveTraffic)
            {
                removeTraffic();
            }
            
            if (activedRemoveWantedSearch)
            {
                removeWantedSearch();
            }

            if (activatedExplosion)
            {
                checkExplosion();
            }

            if (activatedEscort)
            {
                escort();
            }

            return true;
        }

        private void checkExplosion()
        {
            if (Game.Player.Character.IsInVehicle())
            {
                Vehicle vehicle = Game.Player.Character.CurrentVehicle;
                lastVehicle = vehicle;

                if (!trapStarted)
                {
                    if (Tools.getSpeedInKmh(lastVehicle.Speed) > bombTriggerSpeedInKmh)
                    {
                        trapStarted = true;
                        GTA.UI.Notify("Le piége s'est enclenché");
                    }
                    else if (!notifSent)
                    {
                        notifSent = true;
                        GTA.UI.Notify("Une bombe a été placée sur le bus");
                    }
                }
            }


            if (trapStarted && lastVehicle != null && lastVehicle.Exists() && Tools.getSpeedInKmh(lastVehicle.Speed) < bombTriggerSpeedInKmh)
            {
                explodeTrapedVehicle();
            }
        }

        private void escort()
        {
            /*
            TO IMPROVE : MAKE CLASSES TO DO THIS
            */
            if (policeMan1 != null)
            {
                if (!policeMan1IsRushing && policeMan1.Position.DistanceTo(Game.Player.Character.Position) > escortDistance * 5)
                {
                    policeMan1.Task.ClearAll();
                    policeMan1.Task.DriveTo(policeCar1, aeroportPosition, 7.0f, 1.0f);
                    policeMan1IsRushing = true;
                }
                else if (!policeMan1IsWaiting && policeMan1.Position.DistanceTo(Game.Player.Character.Position) < escortDistance * 5 && policeMan1.Position.DistanceTo(Game.Player.Character.Position) > escortDistance * 2.5)
                {
                    policeMan1.Task.ClearAll();
                    policeMan1.Task.DriveTo(policeCar1, aeroportPosition, 7.0f, 1.0f);
                    policeMan1IsWaiting = true;
                }
                else if (policeMan1IsWaiting && policeMan1.Position.DistanceTo(Game.Player.Character.Position) < escortDistance * 1.8)
                {
                    policeMan1.Task.DriveTo(policeCar1, aeroportPosition, 7.0f, 70.0f);
                    policeMan1IsWaiting = false;
                }
            }

            if (policeMan2 != null)
            {
                if (!policeMan2IsRushing && policeMan1.Position.DistanceTo(Game.Player.Character.Position) > escortDistance * 5)
                {
                    policeMan2.Task.ClearAll();
                    policeMan2.Task.DriveTo(policeCar2, Game.Player.Character.Position, 7.0f, 1.0f);
                    policeMan2IsRushing = true;
                }
                else if (!policeMan2IsWaiting && policeMan2.Position.DistanceTo(Game.Player.Character.Position) < escortDistance)
                {
                    policeMan2.Task.ClearAll();
                    policeMan2.Task.DriveTo(policeCar2, aeroportPosition, 7.0f, 1.0f);
                    policeMan2IsWaiting = true;
                }
                if (policeMan2IsWaiting && policeMan2.Position.DistanceTo(Game.Player.Character.Position) > escortDistance * 2)
                {
                    policeMan2.Task.DriveTo(policeCar2, Game.Player.Character.Position, 7.0f, 70.0f);
                    policeMan2IsWaiting = false;
                }
            }
        }

        private void removeTraffic()
        {
            Vehicle[] vehiclesAround = World.GetNearbyVehicles(Game.Player.Character, 1000.0f).Where(v => v != policeCar1 && v != policeCar2 && v != bus).ToArray();
            for (int i = 0; i < vehiclesAround.Length; i++)
            {
                vehiclesAround[i].Delete();
            }
        }

        private void removeWantedSearch()
        {
            if (Game.Player.WantedLevel > 0)
            {
                Game.Player.WantedLevel = 0;
            }
        }

        private void showSpeedCounter()
        {
            if (activatedSpeedCounter && speedCounterText != null)
            {
                if (Game.Player.Character.IsInVehicle())
                {
                    if (trapStarted)
                    {
                        if (Tools.getSpeedInKmh(lastVehicle.Speed) < bombTriggerSpeedInKmh + 5)
                        {
                            speedCounterText.Color = Color.Red;
                        }
                        else if (Tools.getSpeedInKmh(lastVehicle.Speed) < bombTriggerSpeedInKmh + 10)
                        {
                            speedCounterText.Color = Color.Orange;
                        }
                        else
                        {
                            speedCounterText.Color = Color.Green;
                        }
                    }
                    else
                    {
                        speedCounterText.Color = Color.WhiteSmoke;
                    }

                    speedCounterShadow.Caption = Tools.getVehicleSpeedInKmh(Game.Player.Character.CurrentVehicle);
                    speedCounterShadow.Draw();

                    speedCounterText.Caption = Tools.getVehicleSpeedInKmh(Game.Player.Character.CurrentVehicle);
                    speedCounterText.Draw();

                }
            }
        }

        private void explodeTrapedVehicle()
        {
            lastVehicle.Explode();
            lastVehicle = null;
            trapStarted = false;
        }
       
        public override void fillMenu(ref UIMenu menu)
        {
            var explosionsItem = new UIMenuCheckboxItem("Activer les explosions", activatedExplosion, "Activer les explosions quand le vehicule passe en dessous des 40km/h ?");
            menu.AddItem(explosionsItem);

            var escortItem = new UIMenuCheckboxItem("Avec escorte", activatedEscort, "Avoir deux véhicules de police qui nous escorte sur la route");
            menu.AddItem(escortItem);

            var speedCounterItem = new UIMenuCheckboxItem("Activer le compteur de vitesse", activatedSpeedCounter);
            menu.AddItem(speedCounterItem);

            var removeTrafficItem = new UIMenuCheckboxItem("Supprimer le traffic routier", activedRemoveTraffic);
            menu.AddItem(removeTrafficItem);

            var removeWantedSearchItem = new UIMenuCheckboxItem("Empêcher les étoiles de police", activedRemoveWantedSearch);
            menu.AddItem(removeWantedSearchItem);

            menu.OnCheckboxChange += (sender, item, checked_) =>
            {
                if (item == explosionsItem)
                {
                    activatedExplosion = checked_;
                }
                if (item == speedCounterItem)
                {
                    activatedSpeedCounter = checked_;
                }
                if (item == escortItem)
                {
                    activatedEscort = checked_;
                }
                if (item == removeTrafficItem)
                {
                    activedRemoveTraffic = checked_;
                }
                if (item == removeWantedSearchItem)
                {
                    activedRemoveWantedSearch = checked_;
                }
                
            };
        }
    }
}
