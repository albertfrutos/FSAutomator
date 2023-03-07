# FSAutomator

**NOTICE: Feel free to modify the application or any component associated to it. Just let me know about your modifications so I can continue learning!. All the code and work (except the external NuGet packages included) belongs to Albert Frutos (2023), albertfg89@gmail.com **

FSAutomator is a tool which allows you to automate part of your flight in MSFS (tested only under MSFS2020 on January 2023). 

The application supports several actions and some "complex" actions (this means that the action gets some data for you, calculates/does something with it, and returns it to the main screen so it's available and usable by the user.

Loading DLLs libraries as automation is also supported as long as you are compliant with some requirements.

## Solution content

The solution contains 4 projects:

 * **FSAutomator.BackEnd** - This is the backend of the application, where al the interaction with the automations, actions and the simulator occurs.
 * **FSAutomatior.UI** - The UI of the application in WPF.
 * **ExternalAutomationExample** - An example on how you can use the DLLAutomation action
 * **Auxiliary** - An auxiliary method I used to generate the methods used by the public interface that is used by DLLAutomation. It connects to the MSFS SDK web documentation to get some data and generate the code. Not necessary.

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

All actions also return the same object type, which is an ActionResult object containing 3 properties:

* **VisibleResult**: A message containing the result, which can be used for (example) a UI. Example: "Variable value is 143"
* **ComputedResult**: The raw message received from FS, always as a string. Example: "143";
* **Error**: Informs whether if the return is because of an error (true) or not (false). Example: True (if error happened), False (if no error happened)

Each action has its own particular properties/parameters.

### GetVariable 

This action recovers a variable value from the simulator.

| Property    | Type     | Description                                                                                                   | Default value: |
|-------------|----------|---------------------------------------------------------------------------------------------------------------|----------------|
| VariableName| string   | Name of the variable to be retrieved from the simulator

```yaml
    {
        "Name": "GetVariable",
        "Parameters": {
        "VariableName": "ATC ID"
        }
    }
```


### DLLAutomation

This automation action does not rely on any Json files, as it is automatically generated when the automation selected is  a DLL file provided by the user, which contains the code to execute. This DLL needs to comply with the following rules:

 * Belong to the namespace _FSAutomator.ExternalAutomation_
 * Have a class called _ExternalAutomation_
 * Inside that class, have a method called _Execute_ which receives a FSAutomatorInterface instance

When using this action type, you must always finish with _FSAutomator.AutomationHasEnded();_. The returned string will be the result shown in the UI.

NOTE: Some ready-to-use methods can be found in classes under the backend, in AutomatorInterface/Managers

```cs
using FSAutomator.Backend.AutomatorInterface;
using FSAutomator.Backend.Entities;
using FSAutomator.BackEnd.Entities;
using Microsoft.FlightSimulator.SimConnect;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FSAutomator.ExternalAutomation
{
    public class ExternalAutomation
    {
        public string Execute(FSAutomatorInterface FSAutomator)
        {
			// your method
			FSAutomator.AutomationHasEnded();
			return "Execution Finished".
		}
	}
}
```

### SendEvent 

This action sends an event to the simulator, i.e. modify auto pilot values, for example.

| Property    | Type     | Description                                                                                                   | Default value: |
|-------------|----------|---------------------------------------------------------------------------------------------------------------|----------------|
| EventName   | string   | Name of the event to be sent to the simulator
| EventValue  | string   | Value of the event to be sent to the simulator


```yaml
    {
      "Name": "SendEvent",
      "Parameters": {
        "EventName": "HEADING_BUG_SET",
        "EventValue": "155"
      }
    }
```

### WaitSeconds 

This action waits a number of seconds (integer) before continuing.

| Property    | Type     | Description                                                                                                   | Default value: |
|-------------|----------|---------------------------------------------------------------------------------------------------------------|----------------|
| WaitTime    | string   | Number of (integer) seconds to wait before continuing.


```yaml
    {
      "Name": "WaitSeconds",
      "Parameters": {
        "WaitTime": "10"
      }
    }
```

### WaitUntilVariableReachesNumericValue 

This action continously checks for a variable value and lets the automation continue once the variable has reached the specified value.

| Property   	    | Type     | Description                                                                                                   | Default value: |
|-------------------|----------|---------------------------------------------------------------------------------------------------------------|----------------|
| VariableName		| string   | Name of the variable to monitor
| Comparison  		| string   | Comparison to run and check. Supported values: <, >, =
| ThresholdValue  	| string   | Value to be reached/overpassed buy the variable
| CheckInterval	  	| string   | Number of milliseconds between checks. **This interval blocks the application**.

NOTE: Take into account that when using the '=' comparison, a too low or too high value for CheckInterval could cause the variable value to overpass the value, causing a never-finishing wait. Use of > or < is recommended. 

It must be taken into account that the CheckInterval parameter stops the thread for the specified millisecons, meaning that the application is blocked/frozen!

Example: the following will wait until ground velocity is higher than 150 knots.

```yaml
    {
      "Name": "WaitUntilVariableReachesNumericValue",
      "Parameters": {
        "VariableName": "GROUND VELOCITY",
        "Comparison": ">",
        "ThresholdValue": "150",
        "CheckInterval": "200"
      }
    }
```


### OperateValue 

This action performs operations on a certaing value.

| Property   	    | Type     | Description                                                                                                   | Default value: |
|------------------|----------|---------------------------------------------------------------------------------------------------------------|----------------|
| Operation			| string   | Operation to perform. Supported value: +, -, *, /, NOT (only for booleans as 0/1 - if operand is not a boolean, '0' is returned).
| Number	  		| string   | Value to use as second operation member.
| ItemToOperateOver	| string   | Value to operate on. Tags are used.



```yaml
    {
      "Name": "OperateValue",
      "Parameters": {
        "Operation": "+",
        "Number": "100",
        "ItemToOperateOver": "%PrevValue%"
      }
    }
```

### MemoryRegisterWrite 

This action writes a value to the specified memory register.

| Property   	    | Type     | Description                                                                                                   | Default value: |
|------------------|----------|---------------------------------------------------------------------------------------------------------------|----------------|
| Value				| string   | Value to write.
| Id		  		| string   | Id of the register to write (it must not exist).



```yaml
    {
      "Name": "MemoryRegisterWrite",
      "Parameters": {
        "Value": "01a-",
        "Id": "Id_01"
      }
    }
```


### MemoryRegisterRead

This action returns the value of the specified register.

| Property   	    | Type     | Description                                                                                                   | Default value: |
|------------------|----------|---------------------------------------------------------------------------------------------------------------|----------------|
| RemoveAfterRead	| bool	   | Specifies whether if the register should be kept (false) or removed (true) after being read.
| Id		  		| string   | Id of the register to read (it must exist).



```yaml
	{
      "Name": "MemoryRegisterRead",
      "Parameters": {
        "RemoveAfterRead": "True",
        "Id": "Id_01"
      }
    }
```

### Expect variable value 

This action returns if the specifies variable value is the expected one (true) or not (false).

| Property   	    		| Type     | Description                                                                                                   | Default value: |
|--------------------------|----------|---------------------------------------------------------------------------------------------------------------|----------------|
| VariableName				| string 	| Variable name to check
| VariableExpectedValue		| string   | Expected variable value.



```yaml
    {
      "Name": "ExpectVariableValue",
      "Parameters": {
        "VariableName": "AUTOPILOT HEADING LOCK DIR",
        "VariableExpectedValue": "80"
      }
    }
```

### ExecuteCodeFromDLL 

This action executes code from a given DLL in the specified method of the specified class.

| Property   	    				| Type     | Description                                                                                                   | Default value: |
|----------------------------------|----------|---------------------------------------------------------------------------------------------------------------|----------------|
| SLLName							| string   | Name of the dll file
| ClassName  						| string   | Name of the class
| MethodName	  					| string   | Name of the method
| IncludeAsExternalAutomator	  	| string   | If true, the DLL is also included as an external automator (will also become and work as a DLLAutomation action).

NOTE: Take into account that when using the '=' comparison, a too low or too high value for CheckInterval could cause the variable value to overpass the value, causing a never-finishing wait. Use of > or < is recommended. 

The method headers must be the following:

```cs
        public string MyLonelyMethod(object sender, SimConnect connection, AutoResetEvent evento, Dictionary<string, string> memoryRegisters, string lastValue, ObservableCollection<FSAutomatorAction> actionList)
```

Example: the following will wait until ground velocity is higher than 150 knots.

```yaml
    {
      "Name": "ExecuteCodeFromDLL",
      "UniqueID": "5h0t3o",
      "IsAuxiliary": "true",
      "Parameters": {
        "DLLName": "ExternalAutomationExample.dll",
        "ClassName": "Lalala",
        "MethodName": "LalalaTest",
        "IncludeAsExternalAutomator": "false"
      }
    }
```


### ConditionalAction 

This action executes conditional actions and code within the automation.

| Property   	    				| Type     | Description                                                                                                   | Default value: |
|----------------------------------|----------|---------------------------------------------------------------------------------------------------------------|----------------|
| FirstMember						| string   | First member to compare
| Comparison  						| string   | Comparison to use. Supported comparisons: <, >, =/== (same behaviour), <> (different)
| SecondMember	  					| string   | Second membert to compare
| ActionIfTrueUniqueID			  	| string   | UniqueID of the action to execute if the comparison is true.
| ActionIfFalseUniqueID			  	| string   | UniqueID of the action to execute if the comparison is false.

NOTE: Take into account that when using the '=' comparison, a too low or too high value for CheckInterval could cause the variable value to overpass the value, causing a never-finishing wait. Use of > or < is recommended. 

Example: the following will wait until ground velocity is higher than 150 knots.

```yaml
    {
      "Name": "ConditionalAction",
      "UniqueID": "2vy54",
      "Parameters": {
        "FirstMember": "abcd",
        "Comparison": "=",
        "SecondMember": "abcd",
        "ActionIfTrueUniqueID": "5h0t3o",
        "ActionIfFalseUniqueID": "5h0t3oAA"
      }
    }
```

## Supported tags

 * **%PrevValue%**: returns the value of the last action in the automation.
 * **%FM%parameter**: returns the parameter specified in _parameter_ from the currently loaded aircraft flight model. Example: %FM%FullFlapsStallSpeed% would return the FullFlapsStallSpeed of the flight model for the currently loaded aircraft.
 * **%AutomationId%UniqueID**: returns the result that the action with UniqueID returned as a result. Example: %AutomationId%abcd123 would return the result that the action  with UniqueID abcd123 returned.
 * **%MemoryRegister%Id**: returns the content of the memory register with identifier 'Id'
 * **%Variable%myVar**: returns the value of the variable 'myVar'.


## Examples

 * There are example for json automation under the "Automations" folder
 * For DLLAutomations, there is a project in the solution called ExternalAutomationExample
 * Regargins automation, the following json and c# code to use as DLLAutomation do the same:

