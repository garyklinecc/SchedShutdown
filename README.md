# SchedShutdown
Oxide 2 plugin for Rust - schedule the Rust server to automatically shutdown at a set UTC time.

<b>Acknowledgements:</b>

My thanks to Feramor for his HappyHour plugin, which gave me a starting point for creating this one.

<b>Description</b>

This plugin will backup the Rust server data and shut it down at a specified time.  

When paired with a server-side 'keep-alive' script that will restart the Rust server after it stops, this allows an admin to automatically restart their server at the same time each day. 

<b>Usage</b> (server console command)

schedule.shutdown <hh:mm:ss> - Resets the shutdown timer, and updates the configuration file. The time is expected to be a UTC time. For example:

<code>schedule.shutdown 17:30:00</code>

will schedule the shutdown for 5:30pm UTC time. If the shutdown time is left blank, like so:

<code>schedule.shutdown   </code>

then any existing shutdown time will be erased, and no shutdown will be scheduled. 

<b>Installation Instructions</b>
<ol>
<li>Download the plugin, and place in the Oxide\plugins folder.</li>
<li>Once the plugin has been loaded, use the server console command to set the shutdown time.</li>
</ol>

<b>Example Configuration File</b>

<code>
{
  "UTC_Time": "09:30:00"
}
</code>

In the default config file, the UTC_Time is left blank so the server will not shut down automatically until the admin sets an actual shutdown time.

<b>Notes:</b>

If the time is not in the form hh:mm:ss, the plugin will do its best to interpret the time.  If there are any letters  or symbols (besides ':'), the time will be considered a blank time, and disable the automatic shutdown.



