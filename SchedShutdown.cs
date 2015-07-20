// Reference: Oxide.Ext.Unity

/* 
 * Many thanks to feramor@computer.org for his HappyHour.cs plugin, which 
 * gave me examples of how to work with dates and timers in a plugin.
 */

/*
 * The MIT License (MIT)
 * Copyright (c) 2015 #db_arcane
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Core.Libraries;

namespace Oxide.Plugins
{
    [Info("SchedShutdown", "db_arcane", "1.1.0")]
    public class SchedShutdown : RustPlugin
    {   
        static List<Oxide.Core.Libraries.Timer.TimerInstance> Timers = new List<Oxide.Core.Libraries.Timer.TimerInstance>();
        DateTime MainTime = new DateTime();
        Oxide.Core.Libraries.Timer MainTimer;
        string ErrorStr = "error";

        void Init()
        {
            LoadConfig();
            CleanupConfig();
            MainTimer = Interface.GetMod().GetLibrary<Oxide.Core.Libraries.Timer>("Timer");
            MainTime = DateTime.UtcNow;
        }

        [ConsoleCommand("schedule.shutdown")]
        private void ScheduleShutdown(ConsoleSystem.Arg arg)
        {
            string param = arg.ArgsStr.ToString();
            
            if (param == "")
            {
                 PrintShutdownStatus();
                 return;
            }    
            
            if (param == "enable")
            {
                if (Config["UTC_Time"].ToString() == "") {
                    This.Puts("The shutdown time has not been configured yet. The shutdown timer remains disabled.");
                    return;
                }
                
                Config["Status"] = "enabled";
                ResetShutdownTimer();
                return;
            }
            
            if (param == "disable")
            {
                Config["Status"] = "disabled";
                ResetShutdownTimer();
                return;
            }
            
            string scheduleTime = ParseTimeSetting(param);
			if (scheduleTime == ErrorStr) 
			{
				this.Puts("The time entered was unreadable. No changes have been made. ");
                PrintShutdownStatus();
                return;
			}
            Config["Status"] = "enabled";
            Config["UTC_Time"] = scheduleTime;
            ResetShutdownTimer();
        }
        
        
        [HookMethod("OnServerInitialized")]
        void myOnServerInitialized()
        {
            if (Config["Status"].ToString() == "disabled") {
                PrintShutdownStatus();
				return;
			}
                
            // Set up timer for server save and shutdown
            string shutdownSetting = Config["UTC_Time"].ToString();
            string[] shutdownHour = shutdownSetting.Split(':');
            Int32 hours = Convert.ToInt32(shutdownHour[0]);
            Int32 mins = Convert.ToInt32(shutdownHour[1]);
            Int32 secs = Convert.ToInt32(shutdownHour[2]);
            
            DateTime shutdownTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, hours, mins, secs, DateTimeKind.Utc);
            if (MainTime > shutdownTime) 
            {
                shutdownTime = shutdownTime.AddDays(1);
            }
            long shutdownInterval = Convert.ToInt64((shutdownTime - MainTime).TotalSeconds);

            // schedule the server save command.
            Oxide.Core.Libraries.Timer.TimerInstance newTimer = MainTimer.Once(shutdownInterval, () => ConsoleSystem.Run.Server.Normal("server.save"));
            Timers.Add(newTimer);

            // schedule the restart command.  Restart simply shuts down the server after a 60-second countdown
            newTimer = MainTimer.Once(shutdownInterval, () => ConsoleSystem.Run.Server.Normal("restart"));
            Timers.Add(newTimer);
            
            PrintShutdownStatus();
        }
        
        [HookMethod("Unload")]
        void myUnload()
        {
            foreach (Oxide.Core.Libraries.Timer.TimerInstance CurrentTimer in Timers)
            {
                if (CurrentTimer != null)
                    if (CurrentTimer.Destroyed == false)
                        CurrentTimer.Destroy();
            }
            Timers.Clear();
        }

        [HookMethod("LoadDefaultConfig")]
        void myLoadDefaultConfig()
        {
            Config["Status"] = "disabled";
            Config["UTC_Time"] = "";
            SaveConfig();
        }

        private void ResetShutdownTimer()
        {
            SaveConfig();
            myUnload();
            myOnServerInitialized();
        }
        
        private void PrintShutdownStatus()
        {
            string status = (Config["Status"].ToString() == "disabled") ? "DISABLED" : "ENABLED";
            string schedTime = Config["UTC_Time"].ToString();
            schedTime = (schedTime == "") ? "blank" : schedTime + " UTC" ;

            this.Puts("Shutdown timer is " + status + ", configured shutdown time is " + schedTime);
        }
        
        private void CleanupConfig()
        {
            string status = Config["Status"].ToString().ToLower();
            Config["Status"] = ((status != "enabled") && (status != "disabled")) ? "disabled" : status;
            
            string schedTime = ParseTimeSetting(Config["UTC_Time"].ToString()); 
            Config["UTC_Time"] = (schedTime == ErrorStr) ? "" : schedTime;
            SaveConfig();
        }
        
        private string ParseTimeSetting(string str)
        {
            Int16 hr = 0;
            Int16 min = 0;
            Int16 sec = 0;

            if (str == "") {
                return ErrorStr;
            }

            string[] timeSet = str.Split(':');

            if (timeSet.Length > 0)
            {
                try
                {
                    hr = Convert.ToInt16(timeSet[0]);
                }
                catch (FormatException e)
                {
                    return ErrorStr;
                }
                if ((hr < 0) || (hr > 23)) 
                {
                    return ErrorStr; 
                }
            }

            if (timeSet.Length > 1)
            {
                try
                {
                    min = Convert.ToInt16(timeSet[1]);
                }
                catch (FormatException e)
                {
                    return ErrorStr;
                }
                min = ((min < 0) || (min > 59)) ? (Int16)0 : min;
            }

            if (timeSet.Length > 2)
            {
                try
                {
                    sec = Convert.ToInt16(timeSet[2]);
                }
                catch (FormatException e)
                {
                    return ErrorStr;
                }
                sec = ((sec < 0) || (sec > 59)) ? (Int16)0 : sec;
            }

            return (String.Format("{0:00}:{1:00}:{2:00}", hr, min, sec));
        }
        
    }
}
