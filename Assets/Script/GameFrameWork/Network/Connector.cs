// RingBuffer.cs --- This is where you apply your OCD.
//
// Copyright (C) 2016 Damon Kwok
//
// Author: gww <DamonKwok@msn.com>
// Date: 2015-09-01
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
//Code:

using System;

namespace GFW
{

    public abstract class Connector
    {
        //private AtomicBoolean running = new AtomicBoolean(false);

        public abstract void Connect(String ip, int port, int timeout_ms);
        public abstract void Disconnect();
        public abstract void Send(Packet pack);
    }

}
