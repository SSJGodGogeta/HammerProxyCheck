# How to use this tool:

a) Compile it yourself and run it.
b) Download the code, open the Tools folder and run the .exe file

A folder named "LogsForProxyCheck" will automatically get created in the directory where the .exe is running. 
Upload your RagePluginHook.logs in that folder


Every time you start this tool, it will ask you if you want to have a "detailed mode". 
Detailed Mode enabled will spam the Console with all possible informations gathered from the logs.
Answer with Y/N.

The application will then also ask if you want to clean the "LogsForProxyCheck" folder (in case you are too lazy to do it on your own or forgot before starting the .exe). 
Answer with Y/N.
If Detailed Mode is enabled, you will see the file names that got removed. It will ONLY REMOVE RagePluginHook.logs (Anything that contains RagePluginHook and .log)


# Current features:

- Checks if the Log path has changed
- Checks if the characters have changed
- Displays various informations and checks if the log is valid in general or if it got modified. (Full Detail available by enabling Detailed Mode)

# Planned features:
- Date, version and platform checks.
- Piracy check
- Percentage system (depending on what check fails, the probability of a proxy case rises)
