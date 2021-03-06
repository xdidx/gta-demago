﻿using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DemagoScript
{
    static class PlacesPositions
    {
        public static Vector3 GrooveStreet { get; } = new Vector3(110f, -1950f, 21.0f);
        public static Vector3 TaxiMission { get; } = new Vector3(-1495f, 4990f, 63f);
    }

    static class Tools
    {
        private static string pathToLogFile = Environment.CurrentDirectory + "/scripts/DemagoScript.log";

        private static Vector3 lastPlayerPosition = Vector3.Zero;
        private static Timer clockTransitionTimer;
        
        public static void update()
        {
            lastPlayerPosition = Game.Player.Character.Position;
        }
                
        public static void setModel(Model newModel)
        {
            if (newModel.IsInCdImage && newModel.IsValid)
            {
                Function.Call(Hash.SET_PLAYER_MODEL, Game.Player.Handle, newModel.Hash);
            }
        }

        public static Ped GetClosestPedAroundPlayer()
        {
            return GetClosestPedAroundPed(Game.Player.Character);
        }

        public static Ped GetClosestPedAroundPed(Ped ped)
        {
            float minDistance = 16000f;//mapLength
            Ped closestPed = null;

            Ped[] peds = World.GetAllPeds();
            for (int i = 0; i < peds.Length; i++)
            {
                if (peds[i] != ped && ped.IsAlive && !ped.IsDead && ped.IsHuman)
                {
                    float pedDistanceToPlayer = peds[i].Position.DistanceTo(ped.Position);
                    if (pedDistanceToPlayer < minDistance)
                    {
                        closestPed = peds[i];
                        minDistance = pedDistanceToPlayer;
                    }
                }
            }

            return closestPed;
        }

        public static string getVehicleSpeedInKmh(Vehicle vehicle)
        {
            return getSpeedInKmh(vehicle.Speed).ToString() + "Km/h";
        }

        public static float getSpeedInKmh(float speed)
        {
            return (float)Math.Round(speed * 3);
        }

        public static void setClockTime(int newHour, float transitionDuration = 0, bool onlyForward = false)
        {
            int currentHour = Function.Call<int>(Hash.GET_CLOCK_HOURS);

            newHour = newHour % 24;

            float hoursToAdd = (newHour > currentHour) ? (newHour - currentHour) : ((newHour + 24) - currentHour);
            if (!onlyForward && hoursToAdd > 12)
            {
                hoursToAdd = 24 - hoursToAdd;
            }

            if (clockTransitionTimer != null)
            {
                Tools.log("clockTransitionTimer interrupt");
                clockTransitionTimer.interrupt();
            }

            if (transitionDuration <= 0)
            {
                Function.Call(Hash.SET_CLOCK_TIME, newHour, 0, 0);
            }
            else
            {
                clockTransitionTimer = new Timer(transitionDuration);
                clockTransitionTimer.OnTimerUpdate += (elapsedMilliseconds, elapsedPourcent) =>
                {
                    float floatingHour = (currentHour + (hoursToAdd * elapsedPourcent)) % 24;
                    int hour = (int)Math.Floor(floatingHour),
                        minute = (int)Math.Floor(((floatingHour - hour) * 60) % 60);

                    Function.Call(Hash.SET_CLOCK_TIME, hour, minute, 0);
                };
                clockTransitionTimer.OnTimerInterrupt += (sender, elapsedMilliseconds) =>
                {
                    clockTransitionTimer = null;
                };
            }
        }

        /*TO IMPROVE => POSSIBLE BUG : GROUND DON'T LOAD QUICKLY ENOUGH ?*/
        private static Dictionary<Vector2, Vector3> savedGroundedPositions = new Dictionary<Vector2, Vector3>();
        public static Vector3 GetGroundedPosition(Vector3 position)
        {
            float height;
            Vector2 position2d = new Vector2(position.X, position.Y);

            if (savedGroundedPositions.ContainsKey(position2d))
            {
                return savedGroundedPositions[position2d];
            }

            for (int i = 5000; i >= 0; i -= 500)
            {
                unsafe
                {
                    if (Function.Call<bool>(Hash.GET_GROUND_Z_FOR_3D_COORD, position.X, position.Y, (float)i, &height))
                    {
                        if (height != 0)
                        {
                            position.Z = height + 4.0f;
                            savedGroundedPositions.Add(new Vector2(position.X, position.Y), position);
                            break;
                        }
                    }
                    Script.Wait(100);
                }
            }

            return position;
        }

        public static void TeleportPlayer(Vector3 position, bool withVehicle = true)
        {
            Entity entityToTeleport = Game.Player.Character;
            if (withVehicle && Game.Player.Character.IsInVehicle())
                entityToTeleport = Game.Player.Character.CurrentVehicle;

            entityToTeleport.Position = position;
            entityToTeleport.Velocity = Vector3.Zero;
            entityToTeleport.Rotation = Vector3.Zero;
        }

        public static void TeleportPlayerToWaypoint()
        {
            if (Function.Call<bool>(Hash.IS_WAYPOINT_ACTIVE))
            {
                int waypointID = Function.Call<int>(Hash.GET_FIRST_BLIP_INFO_ID, Function.Call<int>(Hash._GET_BLIP_INFO_ID_ITERATOR));
                Vector3 waypointCoord = Function.Call<Vector3>(Hash.GET_BLIP_COORDS, waypointID);
                waypointCoord = GetGroundedPosition(waypointCoord);

                TeleportPlayer(waypointCoord);
            }
            else
                GTA.UI.Notify("Marqueur inexistant !");
        }

        public static void GiveParachuteToPlayer()
        {
            Function.Call(Hash.GIVE_WEAPON_TO_PED, Game.Player.Character, Function.Call<int>(Hash.GET_HASH_KEY, "gadget_parachute"), 1, 0, 0);
        }

        public static string getTextFromSeconds(float seconds)
        {
            return getTextFromTimespan(new TimeSpan(0, 0, (int)seconds));
        }

        public static string getTextFromMilliSeconds(float seconds)
        {
            return getTextFromTimespan(new TimeSpan(0, 0, (int)(seconds / 1000)));
        }        

        public static string getTextFromTimespan(TimeSpan time)
        {
            var totalTime = "";
            if (time.Minutes > 0)
            {
                totalTime += " " + time.Minutes + " minute";
                if (time.Minutes > 1)
                {
                    totalTime += "s";
                }
            }
            if (time.Seconds > 0)
            {
                if (totalTime != "")
                {
                    totalTime += " et ";
                }

                totalTime += time.Seconds + " seconde";

                if (time.Seconds > 1)
                {
                    totalTime += "s";
                }
            }
            return totalTime;
        }

        public static Vector3 GetSafeRoadPos(Vector3 originalPos)
        {
            OutputArgument outArg = new OutputArgument();
            Function.Call<int>(Hash.GET_CLOSEST_VEHICLE_NODE, originalPos.X, originalPos.Y, originalPos.Z, outArg, 1, 1077936128, 0);
            Vector3 output = outArg.GetResult<Vector3>();
            if (output != Vector3.Zero)
            {
                output.Z += 3;
                return output;
            }
            return originalPos;
        }

        public static void log(string message, Entity entity)
        {
            if (entity == null)
            {
                log(message + " entity is null");
            }
            else if (!entity.Exists())
            {
                log(message + " entity is not null but doesn't exist");
            }
            else
            {
                log(message + " entity exist : " + entity.GetType().Name);
            }
        }

        public static void log(string message, Model model)
        {
            if (model == null)
            {
                log(message + " entity is null");
            }
            else if (!model.IsValid)
            {
                log(message + " entity is not null but isn't valid");
            }
            else
            {
                log(message + " entity exist : " + model.GetType().Name);
            }
        }

        public static void log(string message, Vector3 position)
        {
            log(message + " - X : " + position.X + " / Y : " + position.Y + " - Z : " + position.Z);
        }

        public static void log(string message)
        {
            using (StreamWriter logStreamWriter = new StreamWriter(Tools.pathToLogFile, true))
            {
                try
                {
                    logStreamWriter.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + message);
                }
                finally
                {
                    logStreamWriter.Close();
                }
            }
        }

        public static void trace(string message, string function = "", string function_class = "")
        {
            using ( StreamWriter logStreamWriter = new StreamWriter( Tools.pathToLogFile, true ) ) {
                try {
                    if (function_class != "") {
                        function_class += "::";
                    }
                    logStreamWriter.WriteLine( "[" + DateTime.Now.ToString( "HH:mm:ss" ) + "]" + function_class + function + " - " + message );
                } finally {
                    logStreamWriter.Close();
                }
            }
        }
    }
}
