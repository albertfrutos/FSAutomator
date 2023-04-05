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
        Automator automator;

        string currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

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
        public void CopyFullDirectory_Exists_IsCopiedAndDLLFilesDeleted()
        {
            const string sourceDir = @"TestAuxiliaries\TestFiles";
            const string targetDir = @"TestFilesCopy";

            Utils.CopyFullDirectory(sourceDir, targetDir);

            var filesSource = Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories).Select(f => Path.GetFileName(f)).ToList();
            var filesTarget = Directory.GetFiles(targetDir, "*.*", SearchOption.AllDirectories).Select(f => Path.GetFileName(f)).ToList();

            filesSource.Except(filesTarget).Count().Should().Be(0);

            Utils.DeleteFilesFromDirectoryWithExtension(targetDir, "dll");

            var dllFilesTargetCount = Directory.GetFiles(targetDir, "*.dll", SearchOption.AllDirectories).Count();

            dllFilesTargetCount.Should().Be(0);

            Directory.Delete(targetDir, true);
        }

        [TestMethod]
        public void GetDLLFilesInJSONActionList_ActionList_ReturnsDLLPaths()
        {
            const string dllName = "myTestDLL.dll";

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
        public void CheckIfAllDLLsInActionFileExist_AllDLLFilesExist_ReturnsTrue()
        {
            //Arrange
            const string relativePathToDLL = @"TestAuxiliaries\TestFiles\randomDLL.dll";

            var dllFiles = new List<string>()
            {
                Path.Combine(currentDir, relativePathToDLL)
            };

            //Act
            var result = Utils.CheckIfAllDLLsInActionFileExist(dllFiles);

            //Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void CheckIfAllDLLsInActionFileExist_ThereAreDLLsMissing_ReturnsTrue()
        {
            //Arrange
            const string relativePathToDLL = @"TestAuxiliaries\TestFiles\randomDLL.dll";

            var dllFiles = new List<string>()
            {
                Path.Combine(currentDir, relativePathToDLL),
                Path.Combine(currentDir, "fakeDLL.dll")
            };

            //Act
            var result = Utils.CheckIfAllDLLsInActionFileExist(dllFiles);

            //Assert
            result.Should().BeFalse();
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
        public void GetExistingActions_SomeActionExists_ReturnsTrue()
        {
            var result = Utils.GetExistingActions();

            result.Count().Should().BeGreaterThan(0);
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
        public void GetAutomationFilesList_AutomationsAreAvailable_ListIsReturned()
        {
            const string automationsFolder = @"TestAuxiliaries\TestFiles";

            Utils.Config.AutomationsFolder = automationsFolder;

            var automationFilesList = new List<AutomationFile>()
            {
                new AutomationFile()
                {
                    BasePath = Path.Combine(currentDir,automationsFolder),
                    FileName = @"Actions_Example.json",
                    FilePath = @"TestAuxiliaries\TestFiles\Actions_Example.json",
                    IsPackage = false,
                    PackageName = "",
                    VisibleName = "Actions_Example [.json]"
                },
                new AutomationFile()
                {
                    BasePath = Path.Combine(currentDir,automationsFolder),
                    FileName = @"randomDLL.dll",
                    FilePath = @"TestAuxiliaries\TestFiles\randomDLL.dll",
                    IsPackage = false,
                    PackageName = "",
                    VisibleName = "randomDLL [.dll]"
                }
            };

            var result = Utils.GetAutomationFilesList();

            result.Should().BeEquivalentTo(automationFilesList);

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

        [TestMethod]
        public void GetValueToOperateFromTag_IsNotATag_ReturnsSameValue()
        {
            const string theValue = "MyValue";

            var result = Utils.GetValueToOperateOnFromTag(null,null, theValue);
            result.Should().Be(theValue);
        }

        [TestMethod]
        public void GetValueToOperateFromTag_LastValueTag_ReturnsLastValue()
        {
            //Arrange
            const string lastValue = "ValueFromLastOperation";
            const string tag = "%PrevValue%";

            this.automator = new Automator();
            this.automator.lastOperationValue = lastValue;

            //Act
            var result = Utils.GetValueToOperateOnFromTag(this.automator, null, tag);

            //Assert
            result.Should().Be(lastValue);
        }

        [TestMethod]
        public void GetValueToOperateFromTag_FlightModelTagValid_ReturnsValue()
        {
            //Arrange
            const string propertyValue = "250";
            const string tag = "%FM%CruiseSpeed";

            var flightModel = new FlightModel(null)
            {
                ReferenceSpeeds = new ReferenceSpeeds()
                {
                    CruiseSpeed = "250"
                }
            };

            this.automator = new Automator();
            this.automator.flightModel = flightModel;

            //Act
            var result = Utils.GetValueToOperateOnFromTag(this.automator, null, tag);

            //Assert
            result.Should().Be(propertyValue);
        }
        
        [TestMethod]
        public void GetValueToOperateFromTag_FlightModelTagWithNonExistingProperty_ReturnsError()
        {
            //Arrange
            const string propertyValue = "250";
            const string tag = "%FM%XXXXX";

            var flightModel = new FlightModel(null)
            {
                ReferenceSpeeds = new ReferenceSpeeds()
                {
                    CruiseSpeed = "250"
                }
            };

            this.automator = new Automator();
            this.automator.flightModel = flightModel;

            //Act
            var result = Utils.GetValueToOperateOnFromTag(this.automator, null, tag);

            //Assert
            result.Should().Contain("not found in the flight model");
        }
        
        [TestMethod]
        public void GetValueToOperateFromTag_AutomationIdTag_ReturnsResultFromAutomationId()
        {
            //Arrange

            const string tag = "%AutomationId%1234abcd";

            var actionList = new ObservableCollection<FSAutomatorAction>();

            actionList.Add(
                new FSAutomatorAction()
                {
                    UniqueID = "1234abcd",
                    Status = FSAutomatorAction.ActionStatus.Pending,
                    Result = new ActionResult("VisibleResultValue", "ComputedResultValue", false),
                    AutomationFile = new AutomationFile("", "", "", "file_path", ""),
                    ActionObject = new ExecuteCodeFromDLL()
                    {
                        DLLName = "nameForTheDLL.dll"
                    }
                }
            );

            this.automator = new Automator();
            this.automator.ActionList = actionList;

            //Act
            var result = Utils.GetValueToOperateOnFromTag(this.automator, null, tag);

            //Assert
            result.Should().Be("ComputedResultValue");
        }
        
        [TestMethod]
        public void GetValueToOperateFromTag_MemoryRegisterTag_ReturnsResultFromAutomationId()
        {
            //Arrange

            const string tag = "%MemoryRegister%R1";

            this.automator = new Automator();
            automator.MemoryRegisters.Add("R1", "R1Content");
            
            //Act
            var result = Utils.GetValueToOperateOnFromTag(this.automator, null, tag);

            //Assert
            result.Should().Be("R1Content");
        }

        [TestMethod]
        public void TryParse_MatchingTypeAndValue_ReturnsTrueAndOutputsTheValue()
        {
            var result = Utils.TryParse<int>("100", out int outputVar);
            
            result.Should().BeTrue();
            outputVar.Should().Be(100);
        }
        
        [TestMethod]
        public void TryParse_IncorrectMatchingTypeAndValue_ReturnsFalse()
        {
            var result = Utils.TryParse<int>("asdfasdf", out int outputVar);

            result.Should().BeFalse();
            outputVar.Should().Be(0);
            
        }
    }
}