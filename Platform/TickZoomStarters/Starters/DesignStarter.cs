#region Copyright
/*
 * Software: TickZoom Trading Platform
 * Copyright 2009 M. Wayne Walter
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, see <http://www.tickzoom.org/wiki/Licenses>
 * or write to Free Software Foundation, Inc., 51 Franklin Street,
 * Fifth Floor, Boston, MA  02110-1301, USA.
 * 
 */
#endregion

#region Copyright
/*
 * Software: TickZoom Trading Platform
 * Copyright 2008 M. Wayne Walter
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

 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

using TickZoom.Api;
using TickZoom.TickUtil;

namespace TickZoom.Common
{
	/// <summary>
	/// This starter is use by the GUI to initialize model for
	/// browsing of properties.
	/// </summary>
	public class DesignStarter : StarterCommon
	{
		Log log = Factory.Log.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		TickEngine engine;
   		
		public DesignStarter()
		{
		} 
		
		bool CancelPending {
			get { if( BackgroundWorker != null) {
					return BackgroundWorker.CancellationPending;
				} else {
					return false;
				}
			}
		}

		public override void Run(ModelInterface model)
		{
			engine = Factory.Engine.TickEngine;
			engine.MaxBarsBack = 2;
			engine.MaxTicksBack = 2;
			SymbolInfo symbolInfo = Factory.Symbol.LookupSymbol("Design");
			engine.SymbolInfo = new SymbolInfo[] { symbolInfo };
			engine.Providers = SetupProviders(false,false);
			engine.IntervalDefault = ProjectProperties.Starter.IntervalDefault;
			engine.RunMode = RunMode.Historical;
			engine.EnableTickFilter = ProjectProperties.Engine.EnableTickFilter;
	
			if(CancelPending) return;
			
			engine.Model = model;
			
			engine.QueueTask();
			engine.WaitTask();
		}
		
		public override void Wait() {
			engine.WaitTask();
		}
		
		public override Provider[] SetupProviders(bool quietMode, bool singleLoad) {
			return new Provider[] { new DesignProvider() };
		}
	}

	public class DesignProvider : Provider {
		private object taskLocker = new object();
 		private volatile bool isDisposed = false;
		
		public void StartSymbol(Receiver receiver, SymbolInfo symbol, object eventDetail)
		{
			TickImpl tickImpl = new TickImpl();
			tickImpl.Initialize();
			tickImpl.SetSymbol( symbol.BinaryIdentifier);
			tickImpl.SetTime( new TimeStamp(2000,1,1));
			tickImpl.SetQuote(100D, 100D);
			TickBinary tickBinary = tickImpl.Extract();
			receiver.OnEvent(symbol,(int)EventType.StartHistorical,symbol);
			receiver.OnEvent(symbol,(int)EventType.Tick, tickBinary );
			tickImpl.Initialize();
			tickImpl.SetSymbol( symbol.BinaryIdentifier);
			tickImpl.SetTime( new TimeStamp(2000,1,2));
			tickImpl.SetQuote(101D, 101D);
			tickBinary = tickImpl.Extract();
			receiver.OnEvent(symbol,(int)EventType.Tick, tickBinary );
			receiver.OnEvent(symbol,(int)EventType.EndHistorical,symbol);
		}
		
		public void StopSymbol(Receiver receiver, SymbolInfo symbol)
		{
		}

	    public void PositionChange(Receiver receiver, SymbolInfo symbol, double signal, IList<LogicalOrder> orders)
	    {
	        throw new NotImplementedException();
	    }

	    public void Signal(Receiver receiver, string symbol, double signal)
		{
		}

        public void Start(Receiver receiver)
		{
		}
		
		public void Stop(Receiver receiver)
		{
		}
		
	    public void Dispose() 
	    {
	        Dispose(true);
	        GC.SuppressFinalize(this);      
	    }
	
	    protected virtual void Dispose(bool disposing)
	    {
       		if( !isDisposed) {
	    		lock( taskLocker) {
		            isDisposed = true;   
	    		}
    		}
	    }
	    
		public void SendEvent( Receiver receiver, SymbolInfo symbol, int eventType, object eventDetail) {
			switch( (EventType) eventType) {
				case EventType.Connect:
					Start(receiver);
					break;
				case EventType.Disconnect:
					Stop(receiver);
					break;
				case EventType.StartSymbol:
					StartSymbol(receiver, symbol, eventDetail);
					break;
				case EventType.StopSymbol:
					StopSymbol(receiver,(SymbolInfo)eventDetail);
					break;
				case EventType.PositionChange:
					PositionChangeDetail positionChange = (PositionChangeDetail) eventDetail;
					PositionChange(receiver,symbol,positionChange.Position,positionChange.Orders);
					break;
				case EventType.Terminate:
					Dispose();
					break;
				default:
					throw new ApplicationException("Unexpected event type: " + (EventType) eventType);
			}
		}
	}
}