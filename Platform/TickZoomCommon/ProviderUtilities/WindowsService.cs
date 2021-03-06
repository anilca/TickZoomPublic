#region Copyright
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
using System.ServiceProcess;
using TickZoom.Api;

namespace TickZoom.Common
{

	public class WindowsService : ServiceBase, ProviderService
	{
		Log log;
		ServiceConnection connection;
		
		public WindowsService()
		{
			this.ServiceName = AssemblyAttributes.GetTitle();
			try { 
				log = Factory.Log.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
			} catch( Exception problem) {
	        	System.Diagnostics.EventLog.WriteEntry(ServiceName, "Exception while starting service: " + problem.ToString());
			}
		}
		
		public void Run(string[] args) {
			ServiceBase.Run(new ServiceBase[] { this });
		}
		
		/// <summary>
		/// Start this service.
		/// </summary>
		protected override void OnStart(string[] args)
		{
            try
            {
            	connection.OnStart();
            }
            catch (Exception problem)
            {
            	System.Diagnostics.EventLog.WriteEntry(ServiceName, "Exception while starting service: " + problem.ToString());
            	log.Notice(problem.ToString());
            }
		}
		
		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
            try
            {
				connection.OnStop();
            }
            catch (Exception problem)
            {
            	System.Diagnostics.EventLog.WriteEntry(ServiceName, "Exception while stopping service: " + problem.ToString());
            	log.Notice(problem.ToString());
            }
		}

		
		public ServiceConnection Connection {
			get { return connection; }
			set { connection = value; }
		}
	}
}
