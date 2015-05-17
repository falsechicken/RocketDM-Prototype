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
using Rocket.API;
using Rocket.Unturned;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace FC.RocketDMProto
{
	public class RocketDMProtoConfiguration : IRocketPluginConfiguration
	{
		
		public bool enabled;
		
		public List<Match> matches;
		
		public IRocketPluginConfiguration DefaultConfiguration
		{
			get
			{
				return new RocketDMProtoConfiguration()
				{
					 enabled = true,
					
					 matches = new List<Match>
					{
						new Match
						{
							matchName = "DefaultExample",
							
							matchTimeLimit = 300,
							
							MatchCenter = new Vector3()
							{
								x = 0,
								y = 0,
								z = 0,
							},
							
							spawnPoints = new List<Vector3>()
							{
								new Vector3(1, 1, 1),
								new Vector3(1, 2, 3),
								new Vector3(3, 2, 1),
							}

						},
					},
				};
			}
		}
	}
}
