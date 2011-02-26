﻿#region Copyright & License Information
/*
 * Copyright 2007-2011 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made 
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation. For more information,
 * see COPYING.
 */
#endregion

using System;
using OpenRA.Traits;

namespace OpenRA.Mods.RA
{
	class ReservableInfo : TraitInfo<Reservable> {}

	public class Reservable : ITick
	{
		Actor reservedFor;

		public void Tick(Actor self)
		{
			if (reservedFor == null) 
				return;		/* nothing to do */

			if (!reservedFor.IsInWorld || reservedFor.IsDead())	// todo: replace with Target.IsValid?
				reservedFor = null;		/* not likely to arrive now. */
		}

		public IDisposable Reserve(Actor self, Actor forActor)
		{
			reservedFor = forActor;

			return new DisposableAction(() => reservedFor = null,
										() => Game.RunAfterTick(() => 
			                            {throw new InvalidOperationException("Attempted to finalize an undisposed DisposableAction. {0} ({1}) reserved {2} ({3})"
			                                                            .F(forActor.Info.Name, forActor.ActorID, self.Info.Name, self.ActorID));})
			);
		}

		public static bool IsReserved(Actor a)
		{
			var res = a.TraitOrDefault<Reservable>();
			return res != null && res.reservedFor != null;
		}
	}
}
