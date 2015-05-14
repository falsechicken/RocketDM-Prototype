﻿/*****
 * -- <TITLE>  --
 *
 * Copyright (C) 2015 False_Chicken
 * Contact: jmdevsupport@gmail.com
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, Get it here: https://www.gnu.org/licenses/gpl-2.0.html
 *****/

using System;
using System.Collections.Generic;
using Rocket.RocketAPI;
using Rocket.RocketAPI.Events;
using SDG;
using Steamworks;
using UnityEngine;

namespace FC.RocketDMProto
{
	public class RocketDMProto : RocketPlugin
	{
		
		static System.Random rand = new System.Random();
		
		static DateTime now;
		
		static DateTime lastCalled = DateTime.Now;
		
		RocketPlayer tempTele;
		static bool teleQueued = false;
		
		static bool softResetQueued = false;
		static bool hardResetQueued = false;
		
		static bool teamPlay;
		
		static List<Vector3> spawnPoints = new List<Vector3>();
		
		static Dictionary<ulong, ushort> scoreTable = new Dictionary<ulong, ushort>();
		
		
		static double matchTimeLimit;
		
		static DateTime matchEndTime;
		
		static bool isGameRunning;
		
		protected override void Load()
		{
			RocketPlayerEvents.OnPlayerRevive += OnPlayerRespawn;
			RocketPlayerEvents.OnPlayerDeath += OnPlayerDeath;
			
			RocketServerEvents.OnPlayerConnected += OnPlayerConnect;
		}
		
		protected void FixedUpdate()
		{
			if (isGameRunning)
			{
				if (teleQueued)
				{
					int r = rand.Next(spawnPoints.Count);
					
					tempTele.Teleport(spawnPoints[r], 0);
					teleQueued = false;
				}
				
				DoMatchUpdate();
			}
			
			if (softResetQueued)
			{
				ResetScores();
				softResetQueued = false;
			}
			
			if (hardResetQueued)
			{
				ResetScores(); //TODO NEEDS TO CLEAR THE WHOLE THING.
				ResetSpawnPoints();
				hardResetQueued = false;
			}
		}
		
		private void OnPlayerRespawn(RocketPlayer _player, Vector3 position, byte angle)
		{
			if (isGameRunning == false)
				return;
			
			if (scoreTable.ContainsKey(_player.CSteamID.m_SteamID) == false)
				return;

			tempTele = _player;
			teleQueued = true;
		}
		
		private void OnPlayerConnect(RocketPlayer _rPlayer)
		{
			
		}
		
		private void OnPlayerDeath(RocketPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
		{
			RocketPlayer killer = RocketPlayer.FromCSteamID(murderer);
			
			if (isGameRunning == false)
				return;
			
			if (scoreTable.ContainsKey(player.CSteamID.m_SteamID) == false)
				return;
			
			player.Inventory.Clear(); //Clear inventory if the player is in the game. To prevent loot build-up and crashes.
			
			if (WasKillAMurder(murderer) == false)
				return;
			
			if (WasDeathASuicide(player, murderer))
			    return;
			
			if (scoreTable.ContainsKey(killer.CSteamID.m_SteamID) == false)
			{
				RocketChatManager.Say("Deathmatch player " + player.CharacterName + " was killed by non-match player " + killer.CharacterName);
				return;
			}
			
			scoreTable[killer.CSteamID.m_SteamID]++;
		}
		
		private bool WasKillAMurder(CSteamID _killer)
		{
			if (RocketPlayer.FromCSteamID(_killer) == null)
				return false;
			
			return true;
		}
		
		private bool WasDeathASuicide(RocketPlayer _player, CSteamID _killer)
		{
			if (_player.CSteamID.m_SteamID == _killer.m_SteamID)
				return true;
			
			return false;
		}
		
		private static void EndMatch()
		{
			string scoreString = "";
			
			RocketChatManager.Say("Match Over!");
			foreach (ulong steamID in scoreTable.Keys)
			{
				
				scoreString = scoreString + RocketPlayer.FromCSteamID((CSteamID)steamID).CharacterName + " : " + scoreTable[steamID] + " , ";
			}
			
			RocketChatManager.Say("Scores: " + scoreString);
			
			isGameRunning = false;
			
		}
		
		public void DoMatchUpdate()
		{
			now = DateTime.Now;
			
			if ((DateTime.Now - lastCalled).TotalSeconds > 1)
			{
				 
				var timeSpan = now - matchEndTime;
				if ((ushort)timeSpan.TotalSeconds == 0)
					EndMatch();
				
				lastCalled = now;
			}
		}
		
		public static void AddPlayerToTeam(RocketPlayer _player, string _team)
		{
			//teamA.Add(_player);
			scoreTable.Add(_player.CSteamID.m_SteamID, 0);
		}
		
		public static void ResetTeam(string _team)
		{
			//teamA.Clear();
			scoreTable.Clear();
		}
		
		public static void ResetScores()
		{
			
			List<ulong> tempPlayerList = new List<ulong>();
			
			foreach (ulong steamID in scoreTable.Keys)
			{
				tempPlayerList.Add(steamID);
				RocketChatManager.Say("" + steamID);
			}
			
			scoreTable.Clear();
			
			foreach (ulong steamID in tempPlayerList)
			{
				scoreTable.Add(steamID, 0);
			}
			
			tempPlayerList = null;
		}
		
		public static void SetMatchTimeLimit(double _timeInSecs)
		{
			matchTimeLimit = _timeInSecs;
		}
		
		public static void AddSpawnPoint(Vector3 _point)
		{
			spawnPoints.Add(_point);
		}
		
		public static void ResetSpawnPoints()
		{
			spawnPoints.Clear();
		}
		
		public static void SetTeamPlay(bool _teamplay)
		{
			teamPlay = _teamplay;
		}
		
		public static void ListTeams()
		{
			string players = "";
			
			foreach (ulong steamID in scoreTable.Keys)
			{
				players = players + RocketPlayer.FromCSteamID((CSteamID)steamID).CharacterName + ", ";
			}
			
			RocketChatManager.Say("Current DM players: " + players);
		}
		
		public static void QueueSoftReset()
		{
			softResetQueued = true;
		}
		
		public static void QueueHardReset()
		{
			hardResetQueued = true;
		}
		
		public static List<RocketPlayer> GetTeam()
		{
			return null;
		}
		
		public static void StartMatch()
		{
			RocketChatManager.Say("Match Started!");
			
			matchEndTime = DateTime.Now.AddSeconds(matchTimeLimit);
			isGameRunning = true;
		}
		
		public static void StopMatch()
		{
			RocketChatManager.Say("Match Stopped!");
			isGameRunning = false;
			softResetQueued = true;
		}
	}
}