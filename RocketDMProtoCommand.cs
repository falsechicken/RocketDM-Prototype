 /*****
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
using Rocket.RocketAPI;
using SDG;

namespace FC.RocketDMProto
{
	public class RocketDMProtoCommand : IRocketCommand
	{
		public RocketDMProtoCommand()
		{
		}

		#region IRocketCommand implementation

		public void Execute(RocketPlayer caller, string[] command)
		{
			if (caller.IsAdmin == false)
				return;
			
			if (command[0].ToLower().Equals("add"))
				ProcessAddCommand(caller, command);
			
			if (command[0].ToLower().Equals("match"))
				ProcessMatchCommand(caller, command);
					
			if (command[0].ToLower().Equals("set"))
				ProcessSetCommand(caller, command);
			
			if (command[0].ToLower().Equals("list"))
				RocketDMProto.ListTeams();
			
			if (command[0].ToLower().Equals("reset"))
				ProcessResetCommand(caller, command);
			
		}

		public bool RunFromConsole {
			get {
				return false;
			}
		}

		public string Name {
			get {
				return "rdm";
			}
		}

		public string Help {
			get {
				return "rdm command.";
			}
		}
		
		private void ProcessMatchCommand(RocketPlayer _caller, string[] _cmds)
		{
			if (_cmds[1].ToLower().Equals("start"))
				RocketDMProto.StartMatch();
			
			if (_cmds[1].ToLower().Equals("stop"))
				RocketDMProto.StopMatch();
			
			if (_cmds[1].ToLower().Equals("load"))
				ProcessMatchLoadCommand(_caller, _cmds);
			
			if (_cmds[1].ToLower().Equals("save"))
				ProcessMatchSaveCommand(_caller, _cmds);
		}
		
		private void ProcessAddCommand(RocketPlayer _caller, string[]  _cmds)
		{
		    if (_cmds[1].ToLower().Equals("player"))
			{
				SteamPlayer steamPlayer;
				
				if (PlayerTool.tryGetSteamPlayer(_cmds[2], out steamPlayer))
				{
					RocketDMProto.AddPlayerToTeam(RocketPlayer.FromName(_cmds[2]), null);
					RocketChatManager.Say(_caller, "Player " + RocketPlayer.FromSteamPlayer(steamPlayer).CharacterName + " added to match.");
					return;
				}
				
				RocketChatManager.Say(_caller, "Player not found.");
				return;
			}
		    
		 	if (_cmds[1].ToLower().Equals("spawn"))
			{
		 		RocketDMProto.AddSpawnPoint(_caller.Position);
		 		RocketChatManager.Say(_caller, "Spawn Point Added.");
		 		return;
			}
		}
		
		private void ProcessSetCommand(RocketPlayer _caller, string[] _cmds)
		{
			if (_cmds[1].ToLower().Equals("time"))
			{
				RocketDMProto.SetMatchTimeLimit(int.Parse(_cmds[2]));
				RocketChatManager.Say(_caller, "Match time set to " + int.Parse(_cmds[2]) + ".");
				return;
			}
			
			if (_cmds[1].ToLower().Equals("center"))
			{
				RocketDMProto.SetMatchCenter(_caller.Position);
				RocketChatManager.Say(_caller, "Match center set to " + _caller.Position + ".");
				return;
			}
		}
		
		private void ProcessResetCommand(RocketPlayer _caller, string[] _cmds)
		{
			if (_cmds[1].ToLower().Equals("hard"))
			{
				ResetMatchHard();
				RocketChatManager.Say(_caller, "Match hard reset.");
				return;
			}
			
			if (_cmds[1].ToLower().Equals("soft"))
			{
				ResetMatchSoft();
				RocketChatManager.Say(_caller, "Match soft reset.");
				return;
			}
		}
		
		private void ProcessMatchLoadCommand(RocketPlayer _caller, string[] _cmds)
		{
			
			RocketDMProto.QueueMatchLoad(_caller, _cmds[2]);

		}
				
		private void ProcessMatchSaveCommand(RocketPlayer _caller, string[] _cmds)
		{
			RocketDMProto.QueueMatchSave(_caller, _cmds[2]);
		}
		
		private void ResetMatchHard()
		{
			RocketDMProto.QueueHardReset();
		}
		
		private void ResetMatchSoft()
		{
			RocketDMProto.QueueSoftReset();
		}
		

		#endregion
	}
}
