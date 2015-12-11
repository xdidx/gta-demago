using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemagoScript
{
    enum DemagoModel {
        Joe,
        Fourras,
        Gastrow
    }

    static class PlacesPositions
    {
        public static Vector3 GrooveStreet { get; } = new Vector3(110f, -1950f, 21.0f);
        public static Vector3 TaxiMission { get; } = new Vector3(-1495f, 4990f, 63f);
    }

    static class Tools
    {
        private static Vector3 lastPlayerPosition = Vector3.Zero;
        private static Timer clockTransitionTimer;

        //traveling variables
        private static List<Vector3> travelingPositions = new List<Vector3>();
        private static float travelingDuration = 0;
        private static int travelingIndex = 0;
        private static bool mergeWithPlayerCameraOnTravelingEnd = true;
        private static Camera travelingCamera;
        private static float remainingTimeForTraveling = 0;
        private static bool travelingHasTarget = false;
        private static Entity travelingTarget;
        
        public static void update()
        {
            lastPlayerPosition = Game.Player.Character.Position;

            //traveling update
            if (travelingIndex < travelingPositions.Count)
            {
                remainingTimeForTraveling -= (Game.LastFrameTime * 1000);

                Vector3 distanceToNextPosition = travelingPositions[travelingIndex] - travelingCamera.Position;
                float durationPerPosition = travelingDuration / (travelingPositions.Count - 1);
                float remainingTimeForCurrentTransition = remainingTimeForTraveling - (durationPerPosition * (travelingPositions.Count - travelingIndex - 1));                
                float remainingFrames = remainingTimeForCurrentTransition / (Game.LastFrameTime * 1000);

                if (remainingFrames <= 0)
                {
                    travelingIndex++;
                }
                else
                {
                    Vector3 travelingVector = distanceToNextPosition / remainingFrames;
                    travelingCamera.Position += travelingVector;
                }

                if (travelingHasTarget && (travelingTarget == null || !travelingTarget.Exists()))
                {
                    travelingCamera.PointAt(Game.Player.Character);
                }

                if (travelingPositions.Count == travelingIndex)
                {
                    Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 1, 0, 1, 1);
                }
            }
        }

        public static void traveling(List<Vector3> positions, float duration, Entity target = null, bool mergeWithPlayerCameraOnEnd = false)
        {
            if (positions.Count > 1)
            {
                travelingPositions = positions;
                travelingDuration = duration;
                remainingTimeForTraveling = travelingDuration;
                mergeWithPlayerCameraOnTravelingEnd = mergeWithPlayerCameraOnEnd;
                travelingTarget = target;
                travelingIndex = 0;

                if (mergeWithPlayerCameraOnTravelingEnd)
                {
                    positions.Add(GameplayCamera.Position);
                }

                travelingHasTarget = false;
                travelingCamera = World.CreateCamera(positions.First<Vector3>(), Vector3.Zero, GameplayCamera.FieldOfView);
                if(target != null)
                {
                    travelingCamera.PointAt(target);
                    travelingHasTarget = true;
                }
                Function.Call(Hash.RENDER_SCRIPT_CAMS, 1, 0, travelingCamera.Handle, 0, 0);
                World.RenderingCamera = travelingCamera;
            }
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
            float minDistance = 16000f;//mapLength
            Ped closestPed = null;

            Ped[] peds = World.GetAllPeds();
            for (int i = 0; i < peds.Length; i++)
            {
                if (peds[i] != Game.Player.Character)
                {
                    float pedDistanceToPlayer = peds[i].Position.DistanceTo(Game.Player.Character.Position);
                    if (pedDistanceToPlayer < minDistance)
                    {
                        closestPed = peds[i];
                        minDistance = pedDistanceToPlayer;
                        Tools.log("minDistance : " + minDistance);
                    }
                }
            }
            Tools.log("minDistance FINAL : " + minDistance);

            return closestPed;
        }

        public static void setDemagoModel(DemagoModel newModel)
        {
            if (newModel == DemagoModel.Joe)
            {
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

            if (newModel == DemagoModel.Fourras)
            {
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

        public static bool playerMoved()
        {
            return lastPlayerPosition != Game.Player.Character.Position;
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
                clockTransitionTimer.stop();
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
                clockTransitionTimer.OnTimerStop += (sender) =>
                {
                    if (sender == clockTransitionTimer)
                    {
                        clockTransitionTimer = null;
                    }
                };
            }
        }

        /*TO IMPROVE => POSSIBLE BUG : GROUND DON'T LOAD QUICKLY ENOUGH ?*/
        private static Dictionary<Vector2, Vector3> savedGroundedPositions = new Dictionary<Vector2, Vector3>();
        public static Vector3 GetGroundedPosition(Vector3 position)
        {            
            bool groundFound = false;
            Vector2 position2d = new Vector2(position.X, position.Y);
            if (savedGroundedPositions.ContainsKey(position2d))
            {
                log("Tools.GetGroundedPosition() => Ground FOUND IN CACHE : x : " + position.X + " / y : " + position.Y + " / z : " + position.Z);
                return savedGroundedPositions[position2d];
            }
            
            Ped heightTestPed = World.CreatePed(PedHash.Abigail, Game.Player.Character.Position);
            if (heightTestPed == null || !heightTestPed.Exists())
            {
                return position;
            }

            for (int pedGroundHeight = 0; pedGroundHeight > 0; pedGroundHeight += 50)
            {
                position.Z = pedGroundHeight;
                heightTestPed.Position = position;

                Script.Wait(100);

                unsafe
                {
                    //But now, I have another issue of course ^^ If player is far from point on map, 
                    OutputArgument outArg = new OutputArgument();
                    if (Function.Call<bool>(Hash.GET_GROUND_Z_FOR_3D_COORD, position.X, position.Y, pedGroundHeight, outArg))
                    {
                        float output = outArg.GetResult<float>();
                        position.Z = output;
                        log("Tools.GetGroundedPosition() => Ground FOUND ! : x : " + position.X + " / y : " + position.Y + " / z : " + position.Z);
                        GTA.UI.Notify("Ground FOUND ! : x: " + position.X + " / y : " + position.Y + " / z : " + position.Z);
                        groundFound = true;
                        position.Z += 4.0f;
                        savedGroundedPositions.Add(new Vector2(position.X, position.Y), position);
                        break;
                    }
                    else
                        log("Tools.GetGroundedPosition() => Ground : x : " + position.X + " / y : " + position.Y + " / z : " + position.Z);
                }
            }
            heightTestPed.Delete();

            if (!groundFound)
                log("Tools.GetGroundedPosition() => Ground NOT FOUND : x : " + position.X + " / y : " + position.Y + " / z : " + position.Z);
                //position.Z = 50;

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

                TeleportPlayer(waypointCoord, false);
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

        public static void log(string message, Vector3 position)
        {
            log(message + " - X : " + position.X + " / Y : " + position.Y + " - Z : " + position.Z);
        }

        public static void log(string message)
        {
            using (StreamWriter logStreamWriter = new StreamWriter("C:/Program Files/Rockstar Games/Grand Theft Auto V/scripts/test.log", true))
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
    }
}
