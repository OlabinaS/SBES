﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContracts;
namespace IPSService
{

    public class Audit : IDisposable
    {
        private static EventLog customLog = null;
        const string SourceName = "IPSService.Audit";
        const string LogName = "SBESLogName2";

        static Audit()
        {
            try
            {
                
                if (!EventLog.SourceExists(SourceName)){ 
                    EventLog.CreateEventSource(SourceName, LogName);
                }
                customLog = new EventLog(LogName, Environment.MachineName, SourceName);
            }
            catch (Exception e)
            {
                customLog = null;
                Console.WriteLine($"Error while trying to create log handle. Error = {e.Message}");
            }
        }

        public static void CriticalLog(Alarm alarm)
        {
            if (customLog != null)
            {
                string critical = AuditEvents.Critical;
                string message = "Level: " + critical + "\nFile name: [" + alarm.Filename + "]\nPath: " + alarm.Path + "\nData and Time: " + alarm.TimeStamp.ToString();
                customLog.WriteEntry(message, EventLogEntryType.Error);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.Critical));
            }
        }

        public static void InformationLog(Alarm alarm)
        {
            if (customLog != null)
            {
                string information = AuditEvents.Information;

                string message = "Level: " + information + "\nFile name: [" + alarm.Filename + "]\nPath: " + alarm.Path + "\nData and Time: " + alarm.TimeStamp.ToString();

                customLog.WriteEntry(message, EventLogEntryType.Information);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.Information));
            }
        }

        public static void WarningLog(Alarm alarm)
        {
            if (customLog != null)
            {
                string warning = AuditEvents.Warning;
                //string message = String.Format(warning, alarm.Filename, alarm.Path, alarm.TimeStamp.ToString());
                string message = "Level: " + warning + "\nFile name: [" + alarm.Filename + "]\nPath: " + alarm.Path + "\nData and Time: " + alarm.TimeStamp.ToString();

                customLog.WriteEntry(message, EventLogEntryType.Warning);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.Warning));
            }
        }

        public void Dispose()
        {
            if (customLog != null)
            {
                customLog.Dispose();
                customLog = null;
            }
        }
    }
}
