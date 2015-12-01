﻿//Copyright 2015 Guadmaz
//Do not redistribute this project without my permission.
//Contact me at rockeurp@gmail.com

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;

class Vigilante : Script
{
    //private UIText _debug;

    private bool _onMission;
    private int _level = 1;
    private int _kills;
    private bool _fighting;

    private bool _spotted;

    private bool _tmpWorkaround; //OH GOD SOMEONE SHOOTME

    private readonly List<Ped> _criminals = new List<Ped>();
    private readonly List<Blip> _criminalBlips = new List<Blip>();
    private Vehicle _hostVehicle;

    private readonly Random _rndGet = new Random();

    private readonly WeaponHash[] _weaponList = { WeaponHash.Pistol, WeaponHash.CombatPistol, WeaponHash.APPistol, WeaponHash.BullpupShotgun, WeaponHash.SawnOffShotgun, WeaponHash.MicroSMG, WeaponHash.SMG, WeaponHash.AssaultRifle, WeaponHash.CarbineRifle };

    private int _criminalGroup;

    private UIText _headsup;
    private readonly UIRectangle _headsupRectangle;

    private readonly Model[] _vehicleList = { new Model(VehicleHash.Oracle),
        new Model(VehicleHash.Buffalo),
        new Model(VehicleHash.Exemplar),
        new Model(VehicleHash.Sultan),
        new Model(VehicleHash.Tailgater)
    };

    private int _seconds;
    private int _tick;

    public Vigilante()
    {
        KeyDown += OnKeyDown;
        Tick += OnTick;

        //this._debug = new UIText("debug goes here", new Point(10, 10), 0.5f, Color.White, 0, false);

        //CriminalGroup = World.AddRelationShipGroup("CRIMINALS_MOD"); //Wont work
        //OutputArgument outArg = new OutputArgument();
        //Function.Call(Hash.ADD_RELATIONSHIP_GROUP, "CRIMINALS_MOD", outArg);
        //CriminalGroup = outArg.GetResult<int>();

        _headsup = new UIText("Level: ~b~" + _level, new Point(2, 325), 0.7f, Color.WhiteSmoke, 1, false);
        _headsupRectangle = new UIRectangle(new Point(0, 320), new Size(200, 110), Color.FromArgb(100, 0, 0, 0));

        //World.SetRelationshipBetweenGroups(Relationship.Hate, CriminalGroup, PlayerGroup);
    }

    void OnTick(object sender, EventArgs e)
    {
        //this._debug.Text = ;
        //this._debug.Draw();
        BigMessage.OnTick();
        Ped player = Game.Player.Character;

        if (_onMission)
        {
            Game.Player.WantedLevel = 0;
            if (_tick >= 60)
            {
                if (!_spotted)
                {
                    _seconds--;
                    _tick = 0;
                }
            }
            else
            {
                _tick++;
            }
            //FUTURE
            _headsup.Caption = "Level: ~b~" + _level;
            if (!_spotted)
                _headsup.Caption += "~w~\nStart Chase: ~b~" + ParseTime(_seconds);
            _headsup.Caption += "~w~\nKills: ~b~" + _kills;

            _headsup.Draw();
            _headsupRectangle.Draw();
            if (_seconds < 0)
            {
                UI.Notify("You ran out of time!\nThe ~r~criminals~w~ have escaped.");
                StopMissions();
            }
            else
            {
                for (int i = 0; i < _criminals.Count; i++)
                {
                    if (_criminals[i].IsDead)
                    {
                        _kills++;
                        SpookCriminal();
                        AddCash(20 * _level);
                        _criminals[i].MarkAsNoLongerNeeded();
                        _criminals.RemoveAt(i);
                        _criminalBlips[i].Remove();
                        _criminalBlips.RemoveAt(i);
                        if (_criminals.Count == 0)
                        {
                            _level++;
                            SpookCriminal();
                            //int secsadded = _rndGet.Next(60, 200);
                            //BigMessage.ShowMessage("~b~" + secsadded + " ~w~seconds added", 200, Color.White, 1.0f);
                            _seconds = 180;
                            UI.Notify("Good job officer! You've completed this level.");
                            StartMissions();
                        }
                    }
                    else
                    {
                        if (_criminals[i].IsInVehicle())
                        {
                            if (player.IsInVehicle())
                            {
                                if ((player.Position - _criminals[i].Position).Length() < 40.0f && (player.CurrentVehicle.SirenActive || _criminals[i].IsInCombat) && !_spotted)
                                {
                                    SpookCriminal(i);
                                }
                            }

                            if ((player.Position - _criminals[i].Position).Length() < 50.0f && _criminals[i].CurrentVehicle.Speed < 1.0f && _spotted)
                            {
                                if (!_fighting)
                                {
                                    _fighting = true;
                                    TaskSequence tasks = new TaskSequence();
                                    tasks.AddTask.LeaveVehicle();
                                    tasks.AddTask.FightAgainst(player, 100000);
                                    tasks.Close();
                                    _criminals[i].Task.PerformSequence(tasks);
                                }
                            }
                            else if (_fighting)
                            {
                                //this.Criminals[i].Task.ClearAll();
                                if (_criminals[i].IsInVehicle())
                                    _criminals[i].Task.CruiseWithVehicle(_criminals[i].CurrentVehicle, 60.0f, 6);
                                _fighting = false;
                            }
                        }
                        else
                        {
                            if ((player.Position - _criminals[i].Position).Length() < 60.0f)
                            {
                                if (!_fighting)
                                {
                                    _fighting = true;
                                    _criminals[i].Task.FightAgainst(player, 100000);
                                }
                            }
                            else
                            {
                                if (_fighting)
                                {
                                    TaskSequence tasks = new TaskSequence();
                                    tasks.AddTask.EnterVehicle();
                                    //tasks.AddTask.CruiseWithVehicle(this.Criminals[i].CurrentVehicle, 60.0f, 6);
                                    tasks.Close();
                                    _criminals[i].Task.PerformSequence(tasks);
                                    //this.Fighting = false;
                                }
                            }
                        }


                    }
                }
                if (player.IsDead)
                {
                    StopMissions();
                }
            }
        }
    }

    void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.D2)
        {
            if (!_tmpWorkaround)
            {
                _criminalGroup = World.AddRelationShipGroup("CRIMINALS_MOD"); //Wont work
                _tmpWorkaround = true;
            }
            if (!_onMission && IsInPoliceCar())
            {
                _seconds = 180;
                StartMissions();
                BigMessage.ShowMessage("Vigilante", 300, Color.Goldenrod);
            }
            else if (_onMission)
            {
                StopMissions();
            }
        }
    }

    void AddCash(int amount)
    {
        string statNameFull = string.Format("SP{0}_TOTAL_CASH", (Game.Player.Character.Model.Hash == new Model("player_zero").Hash) ? 0 :    //Michael
                                                                (Game.Player.Character.Model.Hash == new Model("player_one").Hash) ? 1 :     //Franklin
                                                                (Game.Player.Character.Model.Hash == new Model("player_two").Hash) ? 2 : 0); //Trevor
        var hash = Function.Call<int>(Hash.GET_HASH_KEY, statNameFull);
        OutputArgument outArg = new OutputArgument();
        Function.Call<bool>(Hash.STAT_GET_INT, hash, outArg, -1);
        var val = outArg.GetResult<int>() + amount;
        Function.Call(Hash.STAT_SET_INT, hash, val, true);
    }

    private string ParseTime(int seconds)
    {
        decimal mins = Math.Floor(Convert.ToDecimal(seconds) / 60.0M);
        int sec = seconds % 60;
        if (sec <= 9)
        {
            return String.Format("{0}:0{1}", mins, sec);
        }
        return String.Format("{0}:{1}", mins, sec);
    }

    private void SpookCriminal(int crimId = -1)
    {
        if (crimId != -1)
        {
            if (_criminals[crimId] != null)
            {
                if (_criminals[crimId].IsInVehicle())
                {
                    _spotted = true;
                    _headsupRectangle.Size = new Size(200, 85);
                    Function.Call(Hash.SET_DRIVE_TASK_CRUISE_SPEED, _criminals[crimId].Handle, 60.0f);
                }
            }
        }
        else
        {
            if (_hostVehicle.Exists())
            {
                Ped driver = _hostVehicle.GetPedOnSeat(VehicleSeat.Driver);
                if (driver != null)
                    if (driver.Exists() && driver.IsAlive)
                    {
                        {
                            _spotted = true;
                            _headsupRectangle.Size = new Size(200, 85);
                            Function.Call(Hash.SET_DRIVE_TASK_CRUISE_SPEED, driver.Handle, 60.0f);
                        }
                    }
            }
        }
    }

    Vector3 GetSafeRoadPos(Vector3 originalPos)
    {
        OutputArgument outArg = new OutputArgument();
        Function.Call<int>(Hash.GET_CLOSEST_VEHICLE_NODE, originalPos.X, originalPos.Y, originalPos.Z, outArg, 1, 1077936128, 0);
        Vector3 output = outArg.GetResult<Vector3>();
        return output;
    }

    private bool IsInPoliceCar()
    {
        Model[] copCars = {
            new Model(VehicleHash.Police),
            new Model(VehicleHash.Police2),
            new Model(VehicleHash.Police3),
            new Model(VehicleHash.Police4),
            new Model(VehicleHash.PoliceOld1),
            new Model(VehicleHash.PoliceOld2),
            new Model(VehicleHash.Hydra),
            new Model(VehicleHash.Rhino),
            new Model(VehicleHash.Annihilator),
            new Model(VehicleHash.Buzzard),
            new Model(VehicleHash.Savage),
            new Model(VehicleHash.FBI),
            new Model(VehicleHash.FBI2),
            new Model(VehicleHash.Policeb),
            new Model(VehicleHash.PoliceT),
            new Model(VehicleHash.Sheriff),
            new Model(VehicleHash.Sheriff2),
            new Model(VehicleHash.Lazer)
        };
        Ped player = Game.Player.Character;
        if (player.IsInVehicle())
        {
            if (copCars.Contains(player.CurrentVehicle.Model))
                return true;
            return false;
        }
        return false;
    }

    //CREATE_PED_INSIDE_VEHICLE(Vehicle vehicle, int pedType, Hash modelHash, int seat, BOOL p4, BOOL p5) // 
    //void SET_PED_INTO_VEHICLE(Ped PedHandle, Vehicle VehicleHandle, int SeatIndex)

    private void StartMissions()
    {
        _onMission = true;
        _spotted = false;
        UI.ShowSubtitle("Eliminate the ~r~suspects~w~.", 10000);
        Ped player = Game.Player.Character;
        _headsupRectangle.Size = new Size(200, 110);

        for (int i = 1; i <= Math.Ceiling((decimal)_level / 4); i++)
        {
            Model vehModel = _vehicleList[_rndGet.Next(0, _vehicleList.Length)];
            if (vehModel.Request(2000))
            {
                Vector3 pedSpawnPoint;
                if (i == 1)
                {
                    Vector3 playerpos = player.Position;

                    Vector3 v;
                    v.X = (float)(_rndGet.NextDouble() - 0.5);
                    v.Y = (float)(_rndGet.NextDouble() - 0.5);
                    v.Z = 0.0f;
                    v.Normalize();
                    playerpos += v * 500.0f;

                    pedSpawnPoint = GetSafeRoadPos(playerpos);
                }
                else
                {
                    Vector3 playerpos = _hostVehicle.Position;

                    Vector3 v;
                    v.X = (float)(_rndGet.NextDouble() - 0.5);
                    v.Y = (float)(_rndGet.NextDouble() - 0.5);
                    v.Z = 0.0f;
                    v.Normalize();
                    playerpos += v * 200.0f;

                    pedSpawnPoint = GetSafeRoadPos(playerpos);
                }
                Vehicle tmpVeh = World.CreateVehicle(vehModel, pedSpawnPoint);
                tmpVeh.PlaceOnGround(); //FUTURE
                tmpVeh.IsPersistent = true;

                int maxPasseng;

                if (i == Math.Ceiling((decimal)_level / 4))
                {
                    maxPasseng = _level % 4;
                    if (maxPasseng == 0)
                        maxPasseng = 4;
                }
                else
                {
                    maxPasseng = 4;
                }

                for (int d = 0; d < maxPasseng; d++)
                {
                    Ped tmpPed = Function.Call<Ped>(Hash.CREATE_RANDOM_PED, pedSpawnPoint.X, pedSpawnPoint.Y, pedSpawnPoint.Z);
                    var gunid = _level > _weaponList.Length ? _weaponList[_rndGet.Next(0, _weaponList.Length)] : _weaponList[_rndGet.Next(0, _level)];
                    tmpPed.Weapons.Give(gunid, 999, true, true);
                    if (d == 0)
                        Function.Call(Hash.SET_PED_INTO_VEHICLE, tmpPed.Handle, tmpVeh.Handle, -1); //-1 driver, -2 any
                    else
                        Function.Call(Hash.SET_PED_INTO_VEHICLE, tmpPed.Handle, tmpVeh.Handle, -2);

                    if (i == 1 && d == 0)
                    {
                        tmpPed.Task.CruiseWithVehicle(tmpPed.CurrentVehicle, 15.0f, 6);
                        _hostVehicle = tmpVeh;
                    }
                    else if (d == 0)
                    { //TASK_VEHICLE_ESCORT(Ped pedHandle, Vehicle vehicle, Vehicle targetVehicle, int p3, float speed, Any p5, float minDistance, int p7, float p8)
                        Function.Call(Hash.TASK_VEHICLE_ESCORT, tmpPed.Handle, tmpPed.CurrentVehicle.Handle, _hostVehicle.Handle, -1, 10.0f);
                    }


                    tmpPed.IsPersistent = true;
                    Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, tmpPed.Handle, _criminalGroup);
                    //tmpPed.RelationshipGroup = this.CriminalGroup; //FUTURE
                    tmpPed.IsEnemy = true;
                    tmpPed.CanSwitchWeapons = true;

                    Blip tmpBlip = tmpPed.AddBlip();
                    tmpBlip.Color = BlipColor.Red; //FUTURE

                    _criminalBlips.Add(tmpBlip);
                    _criminals.Add(tmpPed);
                }
                tmpVeh.MarkAsNoLongerNeeded();
            }
            else
            {
                UI.Notify("Error loading vehicle.");
            }
        }
    }

    private void StopMissions()
    {
        _onMission = false;
        _level = 1;
        _kills = 0;
        _spotted = false;
        _seconds = 180;
        UI.ShowSubtitle("");
        foreach (var item in _criminalBlips)
            item.Remove();
        foreach (var item in _criminals)
            item.MarkAsNoLongerNeeded();

        _criminals.Clear();
        _criminalBlips.Clear();
    }

    private void LogToFile(string text)
    {
        using (StreamWriter w = File.AppendText("log.txt"))
        {
            w.Write(text + "\n");
        }
    }
}