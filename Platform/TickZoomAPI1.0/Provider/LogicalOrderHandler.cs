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

	public interface LogicalOrderHandler {
		void SetActualPosition(double position);
		void ClearPhysicalOrders();
		void AddPhysicalOrder( OrderType type, double price, int size, int logicalOrderId, object brokerOrder);
		
		void SetDesiredPosition(double position);
		void SetLogicalOrders(IList<LogicalOrder> logicalOrders);
		void PerformCompare();
	}
}
