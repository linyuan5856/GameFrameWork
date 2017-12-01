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
using System.Threading;

namespace Pandora
{
    public class TimeBomb
    {
        #region Variable

        private string name;
        private Action cb_bomb = null;
        private Action cb_tip = null;
        private AtomicLong countdown_ms = null;
        private long tip_ms;
        private Thread thread_idle = null;
        private AtomicBoolean runState = new AtomicBoolean(false);

        #endregion

        #region Public Method

        public TimeBomb(string name, long countdown_ms, long tip_ms, Action cb_bomb = null, Action cb_tip = null)
        {
            this.name = name;
            this.countdown_ms = new AtomicLong(countdown_ms);
            this.tip_ms = tip_ms;

            this.thread_idle = new Thread(new ThreadStart(delegate ()
            {
                GameLogger.LogError("[TimeBomb]thread is startip!");
                while (this.runState.Get())
                {
                    long timeLeft = this.countdown_ms.AddAndGet(1000);
                    if (timeLeft <= 0)
                    {
                        this.Bomb();
                    }
                    else if (timeLeft <= this.tip_ms)
                    {
                        if (this.cb_tip != null)
                            this.cb_tip();
                    }

                    Core.Sleep(1000);
                }
                GameLogger.LogError("[TimeBomb] thread is free!");
            }));
        }

        public string Name { get { return this.name; } }

        public void SetCallback(Action cb_bomb = null, Action cb_tip = null)
        {
            this.cb_bomb = cb_bomb;
            this.cb_tip = cb_tip;
        }

        public void Start()
        {
            this.runState.Set(true);
            this.thread_idle.IsBackground = true;
            this.thread_idle.Start();
        }

        public void Stop()
        {
            this.runState.Set(false);
        }

        public void AddTime(long ms)
        {
            this.countdown_ms.Add(ms);
        }

        public void SetCountdown(long ms)
        {
            this.countdown_ms.Set(ms);
        }

        public void Bomb()
        {
            if (this.cb_bomb != null)
                this.cb_bomb();
        }

        #endregion
    }

}

