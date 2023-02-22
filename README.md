# FSAutomator

FSAutomator is a tool which allows you to automate part of your flight in MSFS (tested only under MSFS2020 on January 2023). 

The application supports several actions and some "complex" actions (this means that the action gets some data for you, calculates/does something with it, and returns it to the main screen so it's available and usable by the user.

Loading DLLs libraries as automation is also supported as long as you are compliant with some requirements.

## Supported actions

| Action name                          | JSON available | DLL available | Description                                                                                                                           |
|--------------------------------------|:--------------:|:-------------:|---------------------------------------------------------------------------------------------------------------------------------------|
| ConditionalAction                    | x              |               | Compares two values and executes an action or another (or no action) depending on the comparison outcome. Supports numbers and string |
| ExecuteCodeFromDLL                   | x              |               | Executes the specified method located in the specified class given a DLL                                                              |
| ExpectVariableValue                  | x              | x             | Checks if the asked variable has the asked value                                                                                      |
| GetVariable                          | x              | x             | Gets a variable from the simulator                                                                                                    |
| MemoryRegisterRead                   | x              |               | Reads a value from the memory register given its unique ID                                                                            |
| MemoryRegisterWrite                  | x              |               | Writes a value to the memory register, associating it to a unique ID                                                                  |
| OperateValue                         | x              |               | Operates over a value. Supports numbers and booleans.                                                                                 |
| SendEvent                            | x              | x             | Sends the specified value to the specified event in the simulator                                                                     |
| WaitSeconds                          | x              | x             | Waits for 'n' seconds before continuing                                                                                               |
| WaitUntilVariableReachesNumericValue | x              | x             | Waits until a specified variable value reaches the specified value (or meets criteria)                                                |

NOTE: Unsupported actions when using automations via DLL library are not supported because they can be executed much more easily via code.

## JSON actions

JSON actions can be executed by using JSON files and placing them in the application *Automations* directory. When importing a *.json file using the provided UI, it is placed in the mentioned directory automatically.

NOTE: All properties in the JSON file **must** be entered as a string, even if they are numbers or booleans. 

All JSON actions have some properties in common: 

| Property    | Type     | Description                                                                                                   | Default value: |
|-------------|----------|---------------------------------------------------------------------------------------------------------------|----------------|
| Name        | string   | Action Name. This must be written exactly as stated in 'SupportedActions'.                                    | (none)         |
| UniqueID    | string   | A unique ID (can be any string you like, but must be unique.                                                  | Random GUID    |
| IsAuxiliary | boolean  | If true, and in case the action is ExecuteCodeFromDLL, the DLL will also be shown as a spare automation.      | false          |
| StopOnError | boolean  | If true, the automation will stop running in case an error occurs.                                            | false          |
| Parameters  | json obj | An ActionObject defining the action parameters					                                             | (none)         |
