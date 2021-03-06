﻿#region Copyright
/*
 * Copyright 2008 M. Wayne Walter
 * Software: TickZoom Trading Platform
 * User: Wayne Walter
 * 
 * You can use and modify this software under the terms of the
 * TickZOOM General Public License Version 1.0 or (at your option)
 * any later version.
 * 
 * Businesses are restricted to 30 days of use.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * TickZOOM General Public License for more details.
 *
 * You should have received a copy of the TickZOOM General Public
 * License along with this program.  If not, see
 * <http://www.tickzoom.org/wiki/Licenses>.
 */
#endregion

using System;
using System.Collections.Generic;

namespace TickZoom.Api
{
	public interface VerifyFeed : Receiver, IDisposable
	{
		long VerifyEvent(Action<SymbolInfo,int,object> assertEvent, SymbolInfo symbol, int timeout);
		long Verify(Action<TickIO, TickIO, ulong> assertTick, SymbolInfo symbol, int timeout);
		long Verify(int expectedCount, Action<TickIO, TickIO, ulong> assertTick, SymbolInfo symbol, int timeout);
		double VerifyPosition(double expectedPosition, SymbolInfo symbol, int timeout);
		ReceiverState VerifyState(ReceiverState expectedState, SymbolInfo symbol, int timeout);
		void StartTimeTheFeed();
		int EndTimeTheFeed(int expectedTickCount, int timeoutSeconds);
		bool TimeTheFeedTask();
		TickQueue TickQueue { get; }
		bool IsRealTime { get; }
		TickIO LastTick { get; }
	}
}
