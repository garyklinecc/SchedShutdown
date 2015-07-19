# SchedShutdown
Oxide 2 plugin for Rust - schedule the Rust server to automatically shutdown at a set UTC time.

<b>Acknowledgements:</b>

My thanks to Feramor for his HappyHour plugin, which gave me a starting point for creating this one.

<b>Description</b>

This plugin will backup the Rust server data and shut it down at a specified time.  

When paired with a server-side 'keep-alive' script that will restart the Rust server after it stops, this allows an admin to automatically restart their server at the same time each day. 

<b>Usage</b> (server console command)

<code>schedule.shutdown hh:mm:ss</code> - Updates the configuration file, and enables a shutdown timer with the new time. The time is expected to be a UTC time. For example: <code>schedule.shutdown 17:30:00</code> will schedule the shutdown for 5:30pm UTC time. 

<code>schedule.shutdown enable</code> - Enables the shutdown timer for the already configured shutdown time. If the shutdown time has not been configured yet, the timer will remain disabled. 

<code>schedule.shutdown disable</code> - Disables the shutdown timer, without changing the shutdown time setting.  Use <code>shedule.shutdown enable</code> to start the shutdown timer again. 

<code>schedule.shutdown   </code> - Use the command without any parameters to check the timer status (enabled/disabled) and see what shutdown time is currently configured. 

<b>Installation Instructions</b>
<ol>
<li>Download the plugin, and place in the Oxide\plugins folder.</li>
<li>Once the plugin has been loaded, use the server console command to set the shutdown time.</li>
</ol>

<b>Example Configuration File</b>

<code>
{
  "status": "enabled", 
  "UTC_Time": "09:30:00"
}
</code>

In the default config file, the status is "disabled" and the UTC_Time is left blank so the server will not shut down automatically until the admin sets an actual shutdown time.

<b>Notes:</b>

If the time is not in the form hh:mm:ss, the plugin will do its best to interpret the time.  If there are any letters  or symbols (besides ':'), the time will be considered unreadable and it will not be accepted. 

If an unreadable time is entered with the schedule.shutdown command, the command will not make any changes.  If an unreadable time is entered into the config file directly, the config file will be changed to "disabled" status with a blank time value. 



