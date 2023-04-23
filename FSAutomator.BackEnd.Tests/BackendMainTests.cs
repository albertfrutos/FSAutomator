using FluentAssertions;
using FSAutomator.Backend.Actions;
using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.BackEnd.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using static FSAutomator.Backend.Entities.InternalMessage;

namespace FSAutomator.Backend.Utilities.Tests
{
    [TestClass]
    public class BackendMainTests
    {
        BackendMain backend;
        GeneralStatus status;
        string currentDir;

        [TestInitialize()]
        public void TestInitialize()
        {
            backend = new BackendMain();
            status = GeneralStatus.GetInstance;
            currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        [TestMethod]
        public void ValidateActions_ActionsWithNoIssuesAreUsed_ReturnsNoIssues()
        {
            //Arrange
            backend.automator.ActionList.Add(
                new FSAutomatorAction()
                {
                    Name = "GetVariable",
                    ActionObject = new GetVariable("ATC ID"),
                    Parameters = "{\"VariableName\":\"ATC ID\"}"
                }
            );

            //Act
            var isActionValidated = ActionJSONValidator.ValidateActions(backend.automator.ActionList.ToArray());

            //Assert
            isActionValidated.Should().BeEmpty();
            backend.automator.ActionList[0].IsValidated.Should().BeTrue();
        }

        [TestMethod]
        public void ValidateActions_ActionsWithOneIssueIsUsed_ReturnsOneIssue()
        {
            //Arrange
            backend.automator.ActionList.Add(
                new FSAutomatorAction()
                {
                    Name = "GetVariable",
                    ActionObject = new GetVariable("UNEXISTING VARIABLE"),
                    Parameters = "{\"VariableName\":\"UNEXISTING VARIABLE\"}"
                }
            );

            //Act
            var isActionValidated = ActionJSONValidator.ValidateActions(backend.automator.ActionList.ToArray());

            //Assert
            isActionValidated.Count.Should().Be(1);
            isActionValidated[0].Should().Be("GetVariable [0]: Variable UNEXISTING VARIABLE does not exist.");
            backend.automator.ActionList[0].IsValidated.Should().BeFalse();
        }

        [TestMethod]
        public void SaveAutomation_FileNameIsEmpty_ReturnsInfoMessage()
        {
            //Arrange
            const string fileName = "";

            //Act
            var result = backend.SaveAutomation(null, fileName);

            //Assert
            result.Message.Should().Be("Please enter an automation name.");
            result.Type.Should().Be(MsgType.Info);
        }

        [TestMethod]
        public void SaveAutomation_AutomationIsDLLAutomation_ReturnsInfoMessage()
        {
            //Arrange
            const string fileName = "testFileName.json";
            const string dllName = "randomDLL.dll";


            string basePath = Path.Combine(currentDir, "Automations");
            string dllFilePath = Path.Combine("Automations", dllName);

            File.Copy(@"TestAuxiliaries\TestFiles\randomDLL.dll", dllFilePath);

            backend.automator.ActionList.Add(
                new FSAutomatorAction()
                {
                    Name = "DLLAutomation",
                    ActionObject = new ExternalAutomator("randomDLL.dll", dllFilePath),
                    Parameters = "{\"VariableName\":\"ATC ID\"}"
                }
            );

            AutomationFile automationFile = new AutomationFile()
            {
                BasePath = basePath,
                FileName = dllName,
                FilePath = dllFilePath,
                IsPackage = false,
                PackageName = "",
                VisibleName = "randomDLL [.dll]"
            };

            //Act
            var result = backend.SaveAutomation(automationFile, fileName);

            //Assert
            result.Message.Should().Be("Saving DLL automations is not supported");
            result.Type.Should().Be(MsgType.Info);

            File.Delete(dllFilePath);
        }

        [TestMethod]
        public void SaveAutomation_ActionListHasNonDLLAutomationActions_ReturnsInfoMessage()
        {
            //Arrange
            const string fileName = "testFileName.json";
            const string expectedJSON = "{\r\n  \"Actions\": [\r\n    {\r\n      \"Name\": \"GetVariable\",\r\n      \"UniqueID\": \"id123\",\r\n      \"StopOnError\": false,\r\n      \"Parameters\": {\r\n        \"VariableName\": \"VARIABLE NAME\"\r\n      }\r\n    }\r\n  ]\r\n}";

            string basePath = Path.Combine(currentDir, "Automations");
            string filePath = Path.Combine("Automations", fileName);

            backend.automator.ActionList.Add(
                new FSAutomatorAction()
                {
                    Name = "GetVariable",
                    UniqueID = "id123",
                    ActionObject = new GetVariable("VARIABLE NAME"),
                    Parameters = "{\"VariableName\":\"VARIABLE NAME\"}"
                }
            );

            AutomationFile automationFile = new AutomationFile()
            {
                BasePath = basePath,
                FileName = fileName,
                FilePath = filePath,
                IsPackage = false,
                PackageName = "",
                VisibleName = "visibleName"
            };

            //Act
            var result = backend.SaveAutomation(automationFile, fileName);

            //Assert
            result.Message.Should().Be("Automation saved successfully");
            result.Type.Should().Be(MsgType.Info);

            Assert.IsTrue(File.Exists(filePath));
            Assert.IsTrue(File.ReadAllText(filePath) == expectedJSON);
        }

        [TestMethod]
        public void LoadAction_AutomationJSONIsLoaded_LoadsAutomation()
        {
            //Arrange
            const string fileName = "Simple_Action.json";

            string basePath = Path.Combine(currentDir, "TestAuxiliaries", "TestFiles");
            string filePath = Path.Combine("TestAuxiliaries", "TestFiles", fileName);

            AutomationFile automationFile = new AutomationFile()
            {
                BasePath = basePath,
                FileName = fileName,
                FilePath = filePath,
                IsPackage = false,
                PackageName = "",
                VisibleName = "visibleName"
            };

            //Act
            backend.LoadActions(automationFile);

            var loadedAction = backend.automator.ActionList[0];

            //Assert
            loadedAction.Name.Should().Be("GetVariable");
            loadedAction.UniqueID.Should().Be("id1234");
            loadedAction.StopOnError.Should().BeFalse();
            loadedAction.ActionObject.Should().BeOfType<GetVariable>();
        }

        [TestMethod]
        public void LoadAction_AutomationDLLIsLoaded_LoadsAutomation()
        {
            //Arrange
            const string fileName = "randomDLL.dll";

            string basePath = Path.Combine(currentDir, "TestAuxiliaries", "TestFiles");
            string filePath = Path.Combine("TestAuxiliaries", "TestFiles", fileName);

            AutomationFile automationFile = new AutomationFile()
            {
                BasePath = basePath,
                FileName = fileName,
                FilePath = filePath,
                IsPackage = false,
                PackageName = "",
                VisibleName = "visibleName"
            };

            //Act
            backend.LoadActions(automationFile);

            var loadedAction = backend.automator.ActionList[0];

            //Assert
            loadedAction.Name.Should().Be("DLLAutomation");
            loadedAction.StopOnError.Should().BeTrue();
            loadedAction.ActionObject.Should().BeOfType<ExternalAutomator>();
        }

    }
}