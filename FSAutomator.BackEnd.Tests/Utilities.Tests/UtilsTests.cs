using FluentAssertions;
using FSAutomator.Backend.Actions;
using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using Microsoft.FlightSimulator.SimConnect;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Reflection;

namespace FSAutomator.Backend.Utilities.Tests
{
    [TestClass]
    public class UtilsTests
    {
        [TestInitialize()]

        public void TestInitialize()
        {

        }

        [TestMethod]
        public void IsNumericDouble_DoubleValue_ReturnsTrue()
        {
            string number = "2";
            var result = Utils.IsNumericDouble(number);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsNumericDouble_NonDoubleValue_ReturnsFalse()
        {
            string number = "a";
            var result = Utils.IsNumericDouble(number);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void GetType_TypeAsString_ReturnsType()
        {
            string typeAsString = "System.String";
            var result = Utils.GetType(typeAsString);

            result.Name.Should().Be("String");
        }

        [TestMethod]
        public void GetDLLFilesInJSONActionList_ActionList_ReturnsDLLPaths()
        {
            const string dllName = "myTestDLL.dll";

            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var expectedDLLDir = Path.Combine(currentDir, dllName);

            var actionList = new ObservableCollection<FSAutomatorAction>();

            actionList.Add(
                new FSAutomatorAction()
                {
                    UniqueID = "trueID",
                    Status = FSAutomatorAction.ActionStatus.Pending,
                    Result = new ActionResult(),
                    AutomationFile = new AutomationFile("", "", "", "file_path", ""),
                    ActionObject = new ExecuteCodeFromDLL()
                    {
                        DLLName = dllName
                    }
                }
            ); ;

            var result = Utils.GetDLLFilesInJSONActionList(actionList);

            result[0].Should().Be(expectedDLLDir);
            result.Count.Should().Be(1);

        }

        [TestMethod]
        public void CheckIfActionExists_ActionExists_ReturnsTrue()
        {
            const string actionName = "OperateValue";

            var result = Utils.CheckIfActionExists(actionName);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void CheckIfActionExists_ActionNotExists_ReturnsFalse()
        {
            const string actionName = "NotExistingAction";

            var result = Utils.CheckIfActionExists(actionName);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void GetActionsList_JSONIsRead_ReturnsCorrectActionList()
        {
            const string json = @" { 'Actions': [ { 'Name': 'GetVariable', 'Parameters':  { 'VariableName': 'ATC ID' } } ] } ";

            Directory.CreateDirectory("TestFiles");

            File.WriteAllText(@"TestFiles\jsonFile.json", json);

            var automationFile = new AutomationFile("jsonFile.json", "", "", @"TestFiles\jsonFile.json");

            var result = Utils.GetActionsList(automationFile);

            Directory.Delete("TestFiles", true);

            result[0].Name.Should().Be("GetVariable");
            (result[0].ActionObject as GetVariable).VariableName.Should().Be("ATC ID");
        }


        [TestMethod]
        public void GetJSONTextFromAutomationList_ANActionIsSaved_GeneratesJSONCorrectly()
        {
            var expectedJson = JToken.Parse(@"{  'Actions': [    {      'Name': 'GetVariable',      'UniqueID': null,      'StopOnError': false,      'Parameters': {        'VariableName': 'ATC ID'      }    }  ]}");

            var actionList = new ObservableCollection<FSAutomatorAction>()
            {
                new FSAutomatorAction()
                {
                    Name = "GetVariable",
                    ActionObject = new GetVariable()
                    {
                        VariableName = "ATC ID"
                    }
                }
            };

            var result = JToken.Parse(Utils.GetJSONTextFromAutomationList(actionList).Replace("\"", "'"));

            var areJSONEquals = JToken.DeepEquals(expectedJson, result);

            areJSONEquals.Should().BeTrue();
        }

        [TestMethod]
        public void RecalculateColorForKML_AColorInRGB_ReturnsTheBGRColor()
        {
            var result = Utils.RecalculateColorForKML("010203");
            result.Should().Be("030201");

        }
    }
}