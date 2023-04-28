using FSAutomator.Backend.AutomatorInterface;
using FSAutomator.Backend.Automators;
using FSAutomator.SimConnectInterface;

namespace FSAutomator.Backend.AutomatorInterface.Managers
{
    public class AutoPilotManager : FSAutomatorInterfaceBaseActions
    {


        public AutoPilotManager(Automator automator, ISimConnectBridge connection) : base(automator, connection)
        {

        }


        #region Variables

        public string GetAutopilotAirspeedAcquisition()
        {
            var result = GetVariable("AUTOPILOT AIRSPEED ACQUISITION").ComputedResult;
            return result;
        }

        public string GetAutopilotAirspeedHold()
        {
            var result = GetVariable("AUTOPILOT AIRSPEED HOLD").ComputedResult;
            return result;
        }

        public string GetAutopilotAirspeedHoldCurrent()
        {
            var result = GetVariable("AUTOPILOT AIRSPEED HOLD CURRENT").ComputedResult;
            return result;
        }

        public string GetAutopilotAirspeedHoldVar()
        {
            var result = GetVariable("AUTOPILOT AIRSPEED HOLD VAR").ComputedResult;
            return result;
        }

        public string GetAutopilotAirspeedMaxCalculated()
        {
            var result = GetVariable("AUTOPILOT AIRSPEED MAX CALCULATED").ComputedResult;
            return result;
        }

        public string GetAutopilotAirspeedMinCalculated()
        {
            var result = GetVariable("AUTOPILOT AIRSPEED MIN CALCULATED").ComputedResult;
            return result;
        }

        public string GetAutopilotAltRadioMode()
        {
            var result = GetVariable("AUTOPILOT ALT RADIO MODE").ComputedResult;
            return result;
        }

        public string GetAutopilotAltitudeArm()
        {
            var result = GetVariable("AUTOPILOT ALTITUDE ARM").ComputedResult;
            return result;
        }

        public string GetAutopilotAltitudeLock()
        {
            var result = GetVariable("AUTOPILOT ALTITUDE LOCK").ComputedResult;
            return result;
        }

        public string GetAutopilotAltitudeLockVar()
        {
            var result = GetVariable("AUTOPILOT ALTITUDE LOCK VAR").ComputedResult;
            return result;
        }

        public string GetAutopilotAltitudeManuallyTunable()
        {
            var result = GetVariable("AUTOPILOT ALTITUDE MANUALLY TUNABLE").ComputedResult;
            return result;
        }

        public string GetAutopilotAltitudeSlotIndex()
        {
            var result = GetVariable("AUTOPILOT ALTITUDE SLOT INDEX").ComputedResult;
            return result;
        }

        public string GetAutopilotApproachActive()
        {
            var result = GetVariable("AUTOPILOT APPROACH ACTIVE").ComputedResult;
            return result;
        }

        public string GetAutopilotApproachArm()
        {
            var result = GetVariable("AUTOPILOT APPROACH ARM        ").ComputedResult;
            return result;
        }

        public string GetAutopilotApproachCaptured()
        {
            var result = GetVariable("AUTOPILOT APPROACH CAPTURED").ComputedResult;
            return result;
        }

        public string GetAutopilotApproachHold()
        {
            var result = GetVariable("AUTOPILOT APPROACH HOLD").ComputedResult;
            return result;
        }

        public string GetAutopilotApproachIsLocalizer()
        {
            var result = GetVariable("AUTOPILOT APPROACH IS LOCALIZER").ComputedResult;
            return result;
        }

        public string GetAutopilotAttitudeHold()
        {
            var result = GetVariable("AUTOPILOT ATTITUDE HOLD").ComputedResult;
            return result;
        }

        public string GetAutopilotAvailable()
        {
            var result = GetVariable("AUTOPILOT AVAILABLE").ComputedResult;
            return result;
        }

        public string GetAutopilotAvionicsManaged()
        {
            var result = GetVariable("AUTOPILOT AVIONICS MANAGED").ComputedResult;
            return result;
        }

        public string GetAutopilotBackcourseHold()
        {
            var result = GetVariable("AUTOPILOT BACKCOURSE HOLD").ComputedResult;
            return result;
        }

        public string GetAutopilotBankHold()
        {
            var result = GetVariable("AUTOPILOT BANK HOLD").ComputedResult;
            return result;
        }

        public string GetAutopilotBankHoldRef()
        {
            var result = GetVariable("AUTOPILOT BANK HOLD REF").ComputedResult;
            return result;
        }

        public string GetAutopilotCruiseSpeedHold()
        {
            var result = GetVariable("AUTOPILOT CRUISE SPEED HOLD").ComputedResult;
            return result;
        }

        public string GetAutopilotDefaultPitchMode()
        {
            var result = GetVariable("AUTOPILOT DEFAULT PITCH MODE").ComputedResult;
            return result;
        }

        public string GetAutopilotDefaultRollMode()
        {
            var result = GetVariable("AUTOPILOT DEFAULT ROLL MODE").ComputedResult;
            return result;
        }

        public string GetAutopilotDisengaged()
        {
            var result = GetVariable("AUTOPILOT DISENGAGED").ComputedResult;
            return result;
        }

        public string GetAutopilotFlightDirectorActive()
        {
            var result = GetVariable("AUTOPILOT FLIGHT DIRECTOR ACTIVE").ComputedResult;
            return result;
        }

        public string GetAutopilotFlightDirectorBank()
        {
            var result = GetVariable("AUTOPILOT FLIGHT DIRECTOR BANK").ComputedResult;
            return result;
        }

        public string GetAutopilotFlightDirectorBankEx1()
        {
            var result = GetVariable("AUTOPILOT FLIGHT DIRECTOR BANK EX1").ComputedResult;
            return result;
        }

        public string GetAutopilotFlightDirectorPitch()
        {
            var result = GetVariable("AUTOPILOT FLIGHT DIRECTOR PITCH").ComputedResult;
            return result;
        }

        public string GetAutopilotFlightDirectorPitchEx1()
        {
            var result = GetVariable("AUTOPILOT FLIGHT DIRECTOR PITCH EX1").ComputedResult;
            return result;
        }

        public string GetAutopilotFlightLevelChange()
        {
            var result = GetVariable("AUTOPILOT FLIGHT LEVEL CHANGE").ComputedResult;
            return result;
        }

        public string GetAutopilotGlideslopeActive()
        {
            var result = GetVariable("AUTOPILOT GLIDESLOPE ACTIVE").ComputedResult;
            return result;
        }

        public string GetAutopilotGlideslopeArm()
        {
            var result = GetVariable("AUTOPILOT GLIDESLOPE ARM").ComputedResult;
            return result;
        }

        public string GetAutopilotGlideslopeHold()
        {
            var result = GetVariable("AUTOPILOT GLIDESLOPE HOLD").ComputedResult;
            return result;
        }

        public string GetAutopilotHeadingLock()
        {
            var result = GetVariable("AUTOPILOT HEADING LOCK").ComputedResult;
            return result;
        }

        public string GetAutopilotHeadingLockDir()
        {
            var result = GetVariable("AUTOPILOT HEADING LOCK DIR").ComputedResult;
            return result;
        }

        public string GetAutopilotHeadingManuallyTunable()
        {
            var result = GetVariable("AUTOPILOT HEADING MANUALLY TUNABLE").ComputedResult;
            return result;
        }

        public string GetAutopilotHeadingSlotIndex()
        {
            var result = GetVariable("AUTOPILOT HEADING SLOT INDEX").ComputedResult;
            return result;
        }

        public string GetAutopilotMachHold()
        {
            var result = GetVariable("AUTOPILOT MACH HOLD").ComputedResult;
            return result;
        }

        public string GetAutopilotMachHoldVar()
        {
            var result = GetVariable("AUTOPILOT MACH HOLD VAR").ComputedResult;
            return result;
        }

        public string GetAutopilotManagedIndex()
        {
            var result = GetVariable("AUTOPILOT MANAGED INDEX").ComputedResult;
            return result;
        }

        public string GetAutopilotManagedSpeedInMach()
        {
            var result = GetVariable("AUTOPILOT MANAGED SPEED IN MACH").ComputedResult;
            return result;
        }

        public string GetAutopilotManagedThrottleActive()
        {
            var result = GetVariable("AUTOPILOT MANAGED THROTTLE ACTIVE").ComputedResult;
            return result;
        }

        public string GetAutopilotMaster()
        {
            var result = GetVariable("AUTOPILOT MASTER").ComputedResult;
            return result;
        }

        public string GetAutopilotMaxBank()
        {
            var result = GetVariable("AUTOPILOT MAX BANK").ComputedResult;
            return result;
        }

        public string GetAutopilotMaxBankId()
        {
            var result = GetVariable("AUTOPILOT MAX BANK ID").ComputedResult;
            return result;
        }

        public string GetAutopilotMaxSpeedHold()
        {
            var result = GetVariable("AUTOPILOT MAX SPEED HOLD").ComputedResult;
            return result;
        }

        public string GetAutopilotNav1Lock()
        {
            var result = GetVariable("AUTOPILOT NAV1 LOCK").ComputedResult;
            return result;
        }

        public string GetAutopilotNavSelected()
        {
            var result = GetVariable("AUTOPILOT NAV SELECTED").ComputedResult;
            return result;
        }

        public string GetAutopilotPitchHold()
        {
            var result = GetVariable("AUTOPILOT PITCH HOLD").ComputedResult;
            return result;
        }

        public string GetAutopilotPitchHoldRef()
        {
            var result = GetVariable("AUTOPILOT PITCH HOLD REF").ComputedResult;
            return result;
        }

        public string GetAutopilotRpmHold()
        {
            var result = GetVariable("AUTOPILOT RPM HOLD").ComputedResult;
            return result;
        }

        public string GetAutopilotRpmHoldVar()
        {
            var result = GetVariable("AUTOPILOT RPM HOLD VAR").ComputedResult;
            return result;
        }

        public string GetAutopilotRpmSlotIndex()
        {
            var result = GetVariable("AUTOPILOT RPM SLOT INDEX").ComputedResult;
            return result;
        }

        public string GetAutopilotSpeedSetting()
        {
            var result = GetVariable("AUTOPILOT SPEED SETTING").ComputedResult;
            return result;
        }

        public string GetAutopilotSpeedSlotIndex()
        {
            var result = GetVariable("AUTOPILOT SPEED SLOT INDEX").ComputedResult;
            return result;
        }

        public string GetAutopilotTakeoffPowerActive()
        {
            var result = GetVariable("AUTOPILOT TAKEOFF POWER ACTIVE").ComputedResult;
            return result;
        }

        public string GetAutopilotThrottleArm()
        {
            var result = GetVariable("AUTOPILOT THROTTLE ARM").ComputedResult;
            return result;
        }

        public string GetAutopilotThrottleMaxThrust()
        {
            var result = GetVariable("AUTOPILOT THROTTLE MAX THRUST").ComputedResult;
            return result;
        }

        public string GetAutopilotVerticalHold()
        {
            var result = GetVariable("AUTOPILOT VERTICAL HOLD").ComputedResult;
            return result;
        }

        public string GetAutopilotVerticalHoldVar()
        {
            var result = GetVariable("AUTOPILOT VERTICAL HOLD VAR").ComputedResult;
            return result;
        }

        public string GetAutopilotVsSlotIndex()
        {
            var result = GetVariable("AUTOPILOT VS SLOT INDEX").ComputedResult;
            return result;
        }

        public string GetAutopilotWingLeveler()
        {
            var result = GetVariable("AUTOPILOT WING LEVELER").ComputedResult;
            return result;
        }

        public string GetAutopilotYawDamper()
        {
            var result = GetVariable("AUTOPILOT YAW DAMPER").ComputedResult;
            return result;
        }

        #endregion

        #region Events

        public string SetEventApAirspeedHold(string value)
        {
            var result = SendEvent("AP_AIRSPEED_HOLD", value).ComputedResult;
            return result;
        }

        public string SetEventApAirspeedOff(string value)
        {
            var result = SendEvent("AP_AIRSPEED_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventApAirspeedOn(string value)
        {
            var result = SendEvent("AP_AIRSPEED_ON", value).ComputedResult;
            return result;
        }

        public string SetEventApAirspeedSet(string value)
        {
            var result = SendEvent("AP_AIRSPEED_SET", value).ComputedResult;
            return result;
        }

        public string SetEventApAltVarDec(string value)
        {
            var result = SendEvent("AP_ALT_VAR_DEC", value).ComputedResult;
            return result;
        }

        public string SetEventApAltVarInc(string value)
        {
            var result = SendEvent("AP_ALT_VAR_INC", value).ComputedResult;
            return result;
        }

        public string SetEventApAltHold(string value)
        {
            var result = SendEvent("AP_ALT_HOLD", value).ComputedResult;
            return result;
        }

        public string SetEventApAltHoldOff(string value)
        {
            var result = SendEvent("AP_ALT_HOLD_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventApAltHoldOn(string value)
        {
            var result = SendEvent("AP_ALT_HOLD_ON", value).ComputedResult;
            return result;
        }

        public string SetEventApAltRadioModeOff(string value)
        {
            var result = SendEvent("AP_ALT_RADIO_MODE_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventApAltRadioModeOn(string value)
        {
            var result = SendEvent("AP_ALT_RADIO_MODE_ON", value).ComputedResult;
            return result;
        }

        public string SetEventApAltRadioModeSet(string value)
        {
            var result = SendEvent("AP_ALT_RADIO_MODE_SET", value).ComputedResult;
            return result;
        }

        public string SetEventApAltRadioModeToggle(string value)
        {
            var result = SendEvent("AP_ALT_RADIO_MODE_TOGGLE", value).ComputedResult;
            return result;
        }

        public string SetEventApAltVarSetEnglish(string value)
        {
            var result = SendEvent("AP_ALT_VAR_SET_ENGLISH", value).ComputedResult;
            return result;
        }

        public string SetEventApAltVarSetMetric(string value)
        {
            var result = SendEvent("AP_ALT_VAR_SET_METRIC", value).ComputedResult;
            return result;
        }

        public string SetEventApAprHold(string value)
        {
            var result = SendEvent("AP_APR_HOLD", value).ComputedResult;
            return result;
        }

        public string SetEventApAprHoldOff(string value)
        {
            var result = SendEvent("AP_APR_HOLD_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventApAprHoldOn(string value)
        {
            var result = SendEvent("AP_APR_HOLD_ON", value).ComputedResult;
            return result;
        }

        public string SetEventApAttHold(string value)
        {
            var result = SendEvent("AP_ATT_HOLD", value).ComputedResult;
            return result;
        }

        public string SetEventApAttHoldOff(string value)
        {
            var result = SendEvent("AP_ATT_HOLD_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventApAttHoldOn(string value)
        {
            var result = SendEvent("AP_ATT_HOLD_ON", value).ComputedResult;
            return result;
        }

        public string SetEventApAvionicsManagedOff(string value)
        {
            var result = SendEvent("AP_AVIONICS_MANAGED_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventApAvionicsManagedOn(string value)
        {
            var result = SendEvent("AP_AVIONICS_MANAGED_ON", value).ComputedResult;
            return result;
        }

        public string SetEventApAvionicsManagedSet(string value)
        {
            var result = SendEvent("AP_AVIONICS_MANAGED_SET", value).ComputedResult;
            return result;
        }

        public string SetEventApAvionicsManagedToggle(string value)
        {
            var result = SendEvent("AP_AVIONICS_MANAGED_TOGGLE", value).ComputedResult;
            return result;
        }

        public string SetEventApBankHold(string value)
        {
            var result = SendEvent("AP_BANK_HOLD", value).ComputedResult;
            return result;
        }

        public string SetEventApBankHoldOff(string value)
        {
            var result = SendEvent("AP_BANK_HOLD_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventApBankHoldOn(string value)
        {
            var result = SendEvent("AP_BANK_HOLD_ON", value).ComputedResult;
            return result;
        }

        public string SetEventApBcHold(string value)
        {
            var result = SendEvent("AP_BC_HOLD", value).ComputedResult;
            return result;
        }

        public string SetEventApBcHoldOff(string value)
        {
            var result = SendEvent("AP_BC_HOLD_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventApBcHoldOn(string value)
        {
            var result = SendEvent("AP_BC_HOLD_ON", value).ComputedResult;
            return result;
        }

        public string SetEventApHdgHold(string value)
        {
            var result = SendEvent("AP_HDG_HOLD", value).ComputedResult;
            return result;
        }

        public string SetEventApHdgHoldOff(string value)
        {
            var result = SendEvent("AP_HDG_HOLD_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventApHdgHoldOn(string value)
        {
            var result = SendEvent("AP_HDG_HOLD_ON", value).ComputedResult;
            return result;
        }

        public string SetEventApLocHold(string value)
        {
            var result = SendEvent("AP_LOC_HOLD", value).ComputedResult;
            return result;
        }

        public string SetEventApLocHoldOff(string value)
        {
            var result = SendEvent("AP_LOC_HOLD_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventApLocHoldOn(string value)
        {
            var result = SendEvent("AP_LOC_HOLD_ON", value).ComputedResult;
            return result;
        }

        public string SetEventApMachVarDec(string value)
        {
            var result = SendEvent("AP_MACH_VAR_DEC", value).ComputedResult;
            return result;
        }

        public string SetEventApMachVarInc(string value)
        {
            var result = SendEvent("AP_MACH_VAR_INC", value).ComputedResult;
            return result;
        }

        public string SetEventApMachHold(string value)
        {
            var result = SendEvent("AP_MACH_HOLD", value).ComputedResult;
            return result;
        }

        public string SetEventApMachOff(string value)
        {
            var result = SendEvent("AP_MACH_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventApMachOn(string value)
        {
            var result = SendEvent("AP_MACH_ON", value).ComputedResult;
            return result;
        }

        public string SetEventApMachSet(string value)
        {
            var result = SendEvent("AP_MACH_SET", value).ComputedResult;
            return result;
        }

        public string SetEventApMachVarSet(string value)
        {
            var result = SendEvent("AP_MACH_VAR_SET", value).ComputedResult;
            return result;
        }

        public string SetEventApMachVarSetEx1(string value)
        {
            var result = SendEvent("AP_MACH_VAR_SET_EX1", value).ComputedResult;
            return result;
        }

        public string SetEventApManagedSpeedInMachOff(string value)
        {
            var result = SendEvent("AP_MANAGED_SPEED_IN_MACH_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventApManagedSpeedInMachOn(string value)
        {
            var result = SendEvent("AP_MANAGED_SPEED_IN_MACH_ON", value).ComputedResult;
            return result;
        }

        public string SetEventApManagedSpeedInMachSet(string value)
        {
            var result = SendEvent("AP_MANAGED_SPEED_IN_MACH_SET", value).ComputedResult;
            return result;
        }

        public string SetEventApManagedSpeedInMachToggle(string value)
        {
            var result = SendEvent("AP_MANAGED_SPEED_IN_MACH_TOGGLE", value).ComputedResult;
            return result;
        }

        public string SetEventApMaster(string value)
        {
            var result = SendEvent("AP_MASTER", value).ComputedResult;
            return result;
        }

        public string SetEventApMasterAlt(string value)
        {
            var result = SendEvent("AP_MASTER_ALT", value).ComputedResult;
            return result;
        }

        public string SetEventApMaxBankAngleSet(string value)
        {
            var result = SendEvent("AP_MAX_BANK_ANGLE_SET", value).ComputedResult;
            return result;
        }

        public string SetEventApMaxBankInc(string value)
        {
            var result = SendEvent("AP_MAX_BANK_INC", value).ComputedResult;
            return result;
        }

        public string SetEventApMaxBankDec(string value)
        {
            var result = SendEvent("AP_MAX_BANK_DEC", value).ComputedResult;
            return result;
        }

        public string SetEventApMaxBankSet(string value)
        {
            var result = SendEvent("AP_MAX_BANK_SET", value).ComputedResult;
            return result;
        }

        public string SetEventApMaxBankVelocitySet(string value)
        {
            var result = SendEvent("AP_MAX_BANK_VELOCITY_SET", value).ComputedResult;
            return result;
        }

        public string SetEventApN1Hold(string value)
        {
            var result = SendEvent("AP_N1_HOLD", value).ComputedResult;
            return result;
        }

        public string SetEventApN1RefDec(string value)
        {
            var result = SendEvent("AP_N1_REF_DEC", value).ComputedResult;
            return result;
        }

        public string SetEventApN1RefInc(string value)
        {
            var result = SendEvent("AP_N1_REF_INC", value).ComputedResult;
            return result;
        }

        public string SetEventApN1RefSet(string value)
        {
            var result = SendEvent("AP_N1_REF_SET", value).ComputedResult;
            return result;
        }

        public string SetEventApNavSelectSet(string value)
        {
            var result = SendEvent("AP_NAV_SELECT_SET", value).ComputedResult;
            return result;
        }

        public string SetEventApNav1Hold(string value)
        {
            var result = SendEvent("AP_NAV1_HOLD", value).ComputedResult;
            return result;
        }

        public string SetEventApNav1HoldOff(string value)
        {
            var result = SendEvent("AP_NAV1_HOLD_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventApNav1HoldOn(string value)
        {
            var result = SendEvent("AP_NAV1_HOLD_ON", value).ComputedResult;
            return result;
        }

        public string SetEventApPanelAltitudeHold(string value)
        {
            var result = SendEvent("AP_PANEL_ALTITUDE_HOLD", value).ComputedResult;
            return result;
        }

        public string SetEventApPanelAltitudeOff(string value)
        {
            var result = SendEvent("AP_PANEL_ALTITUDE_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventApPanelAltitudeOn(string value)
        {
            var result = SendEvent("AP_PANEL_ALTITUDE_ON", value).ComputedResult;
            return result;
        }

        public string SetEventApPanelAltitudeSet(string value)
        {
            var result = SendEvent("AP_PANEL_ALTITUDE_SET", value).ComputedResult;
            return result;
        }

        public string SetEventApPanelHeadingHold(string value)
        {
            var result = SendEvent("AP_PANEL_HEADING_HOLD", value).ComputedResult;
            return result;
        }

        public string SetEventApPanelHeadingOff(string value)
        {
            var result = SendEvent("AP_PANEL_HEADING_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventApPanelHeadingOn(string value)
        {
            var result = SendEvent("AP_PANEL_HEADING_ON", value).ComputedResult;
            return result;
        }

        public string SetEventApPanelHeadingSet(string value)
        {
            var result = SendEvent("AP_PANEL_HEADING_SET", value).ComputedResult;
            return result;
        }

        public string SetEventApPanelMachHold(string value)
        {
            var result = SendEvent("AP_PANEL_MACH_HOLD", value).ComputedResult;
            return result;
        }

        public string SetEventApPanelMachOff(string value)
        {
            var result = SendEvent("AP_PANEL_MACH_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventApPanelMachOn(string value)
        {
            var result = SendEvent("AP_PANEL_MACH_ON", value).ComputedResult;
            return result;
        }

        public string SetEventApPanelMachSet(string value)
        {
            var result = SendEvent("AP_PANEL_MACH_SET", value).ComputedResult;
            return result;
        }

        public string SetEventApPanelSpeedHold(string value)
        {
            var result = SendEvent("AP_PANEL_SPEED_HOLD", value).ComputedResult;
            return result;
        }

        public string SetEventApPanelSpeedOff(string value)
        {
            var result = SendEvent("AP_PANEL_SPEED_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventApPanelSpeedOn(string value)
        {
            var result = SendEvent("AP_PANEL_SPEED_ON", value).ComputedResult;
            return result;
        }

        public string SetEventApPanelSpeedSet(string value)
        {
            var result = SendEvent("AP_PANEL_SPEED_SET", value).ComputedResult;
            return result;
        }

        public string SetEventApPanelVsOff(string value)
        {
            var result = SendEvent("AP_PANEL_VS_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventApPanelVsOn(string value)
        {
            var result = SendEvent("AP_PANEL_VS_ON", value).ComputedResult;
            return result;
        }

        public string SetEventApPanelVsSet(string value)
        {
            var result = SendEvent("AP_PANEL_VS_SET", value).ComputedResult;
            return result;
        }

        public string SetEventApPanelVsHold(string value)
        {
            var result = SendEvent("AP_PANEL_VS_HOLD", value).ComputedResult;
            return result;
        }

        public string SetEventApPitchLeveler(string value)
        {
            var result = SendEvent("AP_PITCH_LEVELER", value).ComputedResult;
            return result;
        }

        public string SetEventApPitchLevelerOff(string value)
        {
            var result = SendEvent("AP_PITCH_LEVELER_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventApPitchLevelerOn(string value)
        {
            var result = SendEvent("AP_PITCH_LEVELER_ON", value).ComputedResult;
            return result;
        }

        public string SetEventApPitchRefIncDn(string value)
        {
            var result = SendEvent("AP_PITCH_REF_INC_DN", value).ComputedResult;
            return result;
        }

        public string SetEventApPitchRefIncUp(string value)
        {
            var result = SendEvent("AP_PITCH_REF_INC_UP", value).ComputedResult;
            return result;
        }

        public string SetEventApPitchRefSelect(string value)
        {
            var result = SendEvent("AP_PITCH_REF_SELECT", value).ComputedResult;
            return result;
        }

        public string SetEventApPitchRefSet(string value)
        {
            var result = SendEvent("AP_PITCH_REF_SET", value).ComputedResult;
            return result;
        }

        public string SetEventApRpmSlotIndexSet(string value)
        {
            var result = SendEvent("AP_RPM_SLOT_INDEX_SET", value).ComputedResult;
            return result;
        }

        public string SetEventApSpdVarDec(string value)
        {
            var result = SendEvent("AP_SPD_VAR_DEC", value).ComputedResult;
            return result;
        }

        public string SetEventApSpdVarInc(string value)
        {
            var result = SendEvent("AP_SPD_VAR_INC", value).ComputedResult;
            return result;
        }

        public string SetEventApSpdVarSet(string value)
        {
            var result = SendEvent("AP_SPD_VAR_SET", value).ComputedResult;
            return result;
        }

        public string SetEventApSpdVarSetEx1(string value)
        {
            var result = SendEvent("AP_SPD_VAR_SET_EX1", value).ComputedResult;
            return result;
        }

        public string SetEventApSpeedSlotIndexSet(string value)
        {
            var result = SendEvent("AP_SPEED_SLOT_INDEX_SET", value).ComputedResult;
            return result;
        }

        public string SetEventApVsHold(string value)
        {
            var result = SendEvent("AP_VS_HOLD", value).ComputedResult;
            return result;
        }

        public string SetEventApVsOff(string value)
        {
            var result = SendEvent("AP_VS_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventApVsOn(string value)
        {
            var result = SendEvent("AP_VS_ON", value).ComputedResult;
            return result;
        }

        public string SetEventApVsSet(string value)
        {
            var result = SendEvent("AP_VS_SET", value).ComputedResult;
            return result;
        }

        public string SetEventApVsSlotIndexSet(string value)
        {
            var result = SendEvent("AP_VS_SLOT_INDEX_SET", value).ComputedResult;
            return result;
        }

        public string SetEventApVsVarDec(string value)
        {
            var result = SendEvent("AP_VS_VAR_DEC", value).ComputedResult;
            return result;
        }

        public string SetEventApVsVarInc(string value)
        {
            var result = SendEvent("AP_VS_VAR_INC", value).ComputedResult;
            return result;
        }

        public string SetEventApVsVarSetCurrent(string value)
        {
            var result = SendEvent("AP_VS_VAR_SET_CURRENT", value).ComputedResult;
            return result;
        }

        public string SetEventApVsVarSetEnglish(string value)
        {
            var result = SendEvent("AP_VS_VAR_SET_ENGLISH", value).ComputedResult;
            return result;
        }

        public string SetEventApVsVarSetMetric(string value)
        {
            var result = SendEvent("AP_VS_VAR_SET_METRIC", value).ComputedResult;
            return result;
        }

        public string SetEventApWingLeveler(string value)
        {
            var result = SendEvent("AP_WING_LEVELER", value).ComputedResult;
            return result;
        }

        public string SetEventApWingLevelerOff(string value)
        {
            var result = SendEvent("AP_WING_LEVELER_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventApWingLevelerOn(string value)
        {
            var result = SendEvent("AP_WING_LEVELER_ON", value).ComputedResult;
            return result;
        }

        public string SetEventApPanelSpeedHoldToggle(string value)
        {
            var result = SendEvent("AP_PANEL_SPEED_HOLD_TOGGLE", value).ComputedResult;
            return result;
        }

        public string SetEventApPanelMachHoldToggle(string value)
        {
            var result = SendEvent("AP_PANEL_MACH_HOLD_TOGGLE", value).ComputedResult;
            return result;
        }

        public string SetEventAutopilotAirspeedAcquire(string value)
        {
            var result = SendEvent("AUTOPILOT_AIRSPEED_ACQUIRE", value).ComputedResult;
            return result;
        }

        public string SetEventAutopilotDisengageSet(string value)
        {
            var result = SendEvent("AUTOPILOT_DISENGAGE_SET", value).ComputedResult;
            return result;
        }

        public string SetEventAutopilotDisengageToggle(string value)
        {
            var result = SendEvent("AUTOPILOT_DISENGAGE_TOGGLE", value).ComputedResult;
            return result;
        }

        public string SetEventAutopilotOff(string value)
        {
            var result = SendEvent("AUTOPILOT_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventAutopilotOn(string value)
        {
            var result = SendEvent("AUTOPILOT_ON", value).ComputedResult;
            return result;
        }

        public string SetEventAutopilotPanelAirspeedSet(string value)
        {
            var result = SendEvent("AUTOPILOT_PANEL_AIRSPEED_SET", value).ComputedResult;
            return result;
        }

        public string SetEventAutopilotPanelCruiseSpeed(string value)
        {
            var result = SendEvent("AUTOPILOT_PANEL_CRUISE_SPEED", value).ComputedResult;
            return result;
        }

        public string SetEventAutopilotPanelMaxSpeed(string value)
        {
            var result = SendEvent("AUTOPILOT_PANEL_MAX_SPEED", value).ComputedResult;
            return result;
        }

        public string SetEventAltitudeSlotIndexSet(string value)
        {
            var result = SendEvent("ALTITUDE_SLOT_INDEX_SET", value).ComputedResult;
            return result;
        }

        public string SetEventFlightLevelChange(string value)
        {
            var result = SendEvent("FLIGHT_LEVEL_CHANGE", value).ComputedResult;
            return result;
        }

        public string SetEventFlightLevelChangeOff(string value)
        {
            var result = SendEvent("FLIGHT_LEVEL_CHANGE_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventFlightLevelChangeOn(string value)
        {
            var result = SendEvent("FLIGHT_LEVEL_CHANGE_ON", value).ComputedResult;
            return result;
        }

        public string SetEventHeadingSlotIndexSet(string value)
        {
            var result = SendEvent("HEADING_SLOT_INDEX_SET", value).ComputedResult;
            return result;
        }

        public string SetEventRpmSlotIndexSet(string value)
        {
            var result = SendEvent("RPM_SLOT_INDEX_SET", value).ComputedResult;
            return result;
        }

        public string SetEventSpeedSlotIndexSet(string value)
        {
            var result = SendEvent("SPEED_SLOT_INDEX_SET", value).ComputedResult;
            return result;
        }

        public string SetEventVsSlotIndexSet(string value)
        {
            var result = SendEvent("VS_SLOT_INDEX_SET", value).ComputedResult;
            return result;
        }

        public string SetEventAirspeedBugSelect(string value)
        {
            var result = SendEvent("AIRSPEED_BUG_SELECT", value).ComputedResult;
            return result;
        }

        public string SetEventAltitudeBugSelect(string value)
        {
            var result = SendEvent("ALTITUDE_BUG_SELECT", value).ComputedResult;
            return result;
        }

        public string SetEventAutoThrottleArm(string value)
        {
            var result = SendEvent("AUTO_THROTTLE_ARM", value).ComputedResult;
            return result;
        }

        public string SetEventAutoThrottleDisconnect(string value)
        {
            var result = SendEvent("AUTO_THROTTLE_DISCONNECT", value).ComputedResult;
            return result;
        }

        public string SetEventAutoThrottleToGa(string value)
        {
            var result = SendEvent("AUTO_THROTTLE_TO_GA", value).ComputedResult;
            return result;
        }

        public string SetEventAutobrakeDisarm(string value)
        {
            var result = SendEvent("AUTOBRAKE_DISARM", value).ComputedResult;
            return result;
        }

        public string SetEventAutobrakeHiSet(string value)
        {
            var result = SendEvent("AUTOBRAKE_HI_SET", value).ComputedResult;
            return result;
        }

        public string SetEventAutobrakeLoSet(string value)
        {
            var result = SendEvent("AUTOBRAKE_LO_SET", value).ComputedResult;
            return result;
        }

        public string SetEventAutobrakeMedSet(string value)
        {
            var result = SendEvent("AUTOBRAKE_MED_SET", value).ComputedResult;
            return result;
        }

        public string SetEventDecreaseAutobrakeControl(string value)
        {
            var result = SendEvent("DECREASE_AUTOBRAKE_CONTROL", value).ComputedResult;
            return result;
        }

        public string SetEventFlyByWireElacToggle(string value)
        {
            var result = SendEvent("FLY_BY_WIRE_ELAC_TOGGLE", value).ComputedResult;
            return result;
        }

        public string SetEventFlyByWireFacToggle(string value)
        {
            var result = SendEvent("FLY_BY_WIRE_FAC_TOGGLE", value).ComputedResult;
            return result;
        }

        public string SetEventFlyByWireSecToggle(string value)
        {
            var result = SendEvent("FLY_BY_WIRE_SEC_TOGGLE", value).ComputedResult;
            return result;
        }

        public string SetEventGpwsSwitchToggle(string value)
        {
            var result = SendEvent("GPWS_SWITCH_TOGGLE", value).ComputedResult;
            return result;
        }

        public string SetEventHeadingBugDec(string value)
        {
            var result = SendEvent("HEADING_BUG_DEC", value).ComputedResult;
            return result;
        }

        public string SetEventHeadingBugInc(string value)
        {
            var result = SendEvent("HEADING_BUG_INC", value).ComputedResult;
            return result;
        }

        public string SetEventHeadingBugSelect(string value)
        {
            var result = SendEvent("HEADING_BUG_SELECT", value).ComputedResult;
            return result;
        }

        public string SetEventHeadingBugSet(string value)
        {
            var result = SendEvent("HEADING_BUG_SET", value).ComputedResult;
            return result;
        }

        public string SetEventApHeadingBugSetEx1(string value)
        {
            var result = SendEvent("AP_HEADING_BUG_SET_EX1", value).ComputedResult;
            return result;
        }

        public string SetEventIncreaseAutobrakeControl(string value)
        {
            var result = SendEvent("INCREASE_AUTOBRAKE_CONTROL", value).ComputedResult;
            return result;
        }

        public string SetEventSetAutobrakeControl(string value)
        {
            var result = SendEvent("SET_AUTOBRAKE_CONTROL", value).ComputedResult;
            return result;
        }

        public string SetEventSyncFlightDirectorPitch(string value)
        {
            var result = SendEvent("SYNC_FLIGHT_DIRECTOR_PITCH", value).ComputedResult;
            return result;
        }

        public string SetEventToggleFlightDirector(string value)
        {
            var result = SendEvent("TOGGLE_FLIGHT_DIRECTOR", value).ComputedResult;
            return result;
        }

        public string SetEventYawDamperToggle(string value)
        {
            var result = SendEvent("YAW_DAMPER_TOGGLE", value).ComputedResult;
            return result;
        }

        public string SetEventYawDamperOn(string value)
        {
            var result = SendEvent("YAW_DAMPER_ON", value).ComputedResult;
            return result;
        }

        public string SetEventYawDamperOff(string value)
        {
            var result = SendEvent("YAW_DAMPER_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventYawDamperSet(string value)
        {
            var result = SendEvent("YAW_DAMPER_SET", value).ComputedResult;
            return result;
        }

        public string SetEventVsiBugSelect(string value)
        {
            var result = SendEvent("VSI_BUG_SELECT", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdClearButton(string value)
        {
            var result = SendEvent("G1000_MFD_CLEAR_BUTTON", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdCursorButton(string value)
        {
            var result = SendEvent("G1000_MFD_CURSOR_BUTTON", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdDirecttoButton(string value)
        {
            var result = SendEvent("G1000_MFD_DIRECTTO_BUTTON", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdEnterButton(string value)
        {
            var result = SendEvent("G1000_MFD_ENTER_BUTTON", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdFlightplanButton(string value)
        {
            var result = SendEvent("G1000_MFD_FLIGHTPLAN_BUTTON", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdGroupKnobDec(string value)
        {
            var result = SendEvent("G1000_MFD_GROUP_KNOB_DEC", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdGroupKnobInc(string value)
        {
            var result = SendEvent("G1000_MFD_GROUP_KNOB_INC", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdMenuButton(string value)
        {
            var result = SendEvent("G1000_MFD_MENU_BUTTON", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdPageKnobDec(string value)
        {
            var result = SendEvent("G1000_MFD_PAGE_KNOB_DEC", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdPageKnobInc(string value)
        {
            var result = SendEvent("G1000_MFD_PAGE_KNOB_INC", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdProcedureButton(string value)
        {
            var result = SendEvent("G1000_MFD_PROCEDURE_BUTTON", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdSoftkey1(string value)
        {
            var result = SendEvent("G1000_MFD_SOFTKEY1", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdSoftkey2(string value)
        {
            var result = SendEvent("G1000_MFD_SOFTKEY2", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdSoftkey3(string value)
        {
            var result = SendEvent("G1000_MFD_SOFTKEY3", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdSoftkey4(string value)
        {
            var result = SendEvent("G1000_MFD_SOFTKEY4", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdSoftkey5(string value)
        {
            var result = SendEvent("G1000_MFD_SOFTKEY5", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdSoftkey6(string value)
        {
            var result = SendEvent("G1000_MFD_SOFTKEY6", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdSoftkey7(string value)
        {
            var result = SendEvent("G1000_MFD_SOFTKEY7", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdSoftkey8(string value)
        {
            var result = SendEvent("G1000_MFD_SOFTKEY8", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdSoftkey9(string value)
        {
            var result = SendEvent("G1000_MFD_SOFTKEY9", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdSoftkey10(string value)
        {
            var result = SendEvent("G1000_MFD_SOFTKEY10", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdSoftkey11(string value)
        {
            var result = SendEvent("G1000_MFD_SOFTKEY11", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdSoftkey12(string value)
        {
            var result = SendEvent("G1000_MFD_SOFTKEY12", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdZoominButton(string value)
        {
            var result = SendEvent("G1000_MFD_ZOOMIN_BUTTON", value).ComputedResult;
            return result;
        }

        public string SetEventG1000MfdZoomoutButton(string value)
        {
            var result = SendEvent("G1000_MFD_ZOOMOUT_BUTTON", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdClearButton(string value)
        {
            var result = SendEvent("G1000_PFD_CLEAR_BUTTON", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdCursorButton(string value)
        {
            var result = SendEvent("G1000_PFD_CURSOR_BUTTON", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdDirecttoButton(string value)
        {
            var result = SendEvent("G1000_PFD_DIRECTTO_BUTTON", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdEnterButton(string value)
        {
            var result = SendEvent("G1000_PFD_ENTER_BUTTON", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdFlightplanButton(string value)
        {
            var result = SendEvent("G1000_PFD_FLIGHTPLAN_BUTTON", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdGroupKnobInc(string value)
        {
            var result = SendEvent("G1000_PFD_GROUP_KNOB_INC", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdGroupKnobDec(string value)
        {
            var result = SendEvent("G1000_PFD_GROUP_KNOB_DEC", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdMenuButton(string value)
        {
            var result = SendEvent("G1000_PFD_MENU_BUTTON", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdPageKnobInc(string value)
        {
            var result = SendEvent("G1000_PFD_PAGE_KNOB_INC", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdPageKnobDec(string value)
        {
            var result = SendEvent("G1000_PFD_PAGE_KNOB_DEC", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdProcedureButton(string value)
        {
            var result = SendEvent("G1000_PFD_PROCEDURE_BUTTON", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdSoftkey1(string value)
        {
            var result = SendEvent("G1000_PFD_SOFTKEY1", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdSoftkey2(string value)
        {
            var result = SendEvent("G1000_PFD_SOFTKEY2", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdSoftkey3(string value)
        {
            var result = SendEvent("G1000_PFD_SOFTKEY3", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdSoftkey4(string value)
        {
            var result = SendEvent("G1000_PFD_SOFTKEY4", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdSoftkey5(string value)
        {
            var result = SendEvent("G1000_PFD_SOFTKEY5", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdSoftkey6(string value)
        {
            var result = SendEvent("G1000_PFD_SOFTKEY6", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdSoftkey7(string value)
        {
            var result = SendEvent("G1000_PFD_SOFTKEY7", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdSoftkey8(string value)
        {
            var result = SendEvent("G1000_PFD_SOFTKEY8", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdSoftkey9(string value)
        {
            var result = SendEvent("G1000_PFD_SOFTKEY9", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdSoftkey10(string value)
        {
            var result = SendEvent("G1000_PFD_SOFTKEY10", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdSoftkey11(string value)
        {
            var result = SendEvent("G1000_PFD_SOFTKEY11", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdSoftkey12(string value)
        {
            var result = SendEvent("G1000_PFD_SOFTKEY12", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdZoominButton(string value)
        {
            var result = SendEvent("G1000_PFD_ZOOMIN_BUTTON", value).ComputedResult;
            return result;
        }

        public string SetEventG1000PfdZoomoutButton(string value)
        {
            var result = SendEvent("G1000_PFD_ZOOMOUT_BUTTON", value).ComputedResult;
            return result;
        }

        public string SetEventVirtualCopilotAction(string value)
        {
            var result = SendEvent("VIRTUAL_COPILOT_ACTION", value).ComputedResult;
            return result;
        }

        public string SetEventVirtualCopilotSet(string value)
        {
            var result = SendEvent("VIRTUAL_COPILOT_SET", value).ComputedResult;
            return result;
        }

        public string SetEventVirtualCopilotToggle(string value)
        {
            var result = SendEvent("VIRTUAL_COPILOT_TOGGLE", value).ComputedResult;
            return result;
        }

        public string SetEventGLimiterOff(string value)
        {
            var result = SendEvent("G_LIMITER_OFF", value).ComputedResult;
            return result;
        }

        public string SetEventGLimiterOn(string value)
        {
            var result = SendEvent("G_LIMITER_ON", value).ComputedResult;
            return result;
        }

        public string SetEventGLimiterSet(string value)
        {
            var result = SendEvent("G_LIMITER_SET", value).ComputedResult;
            return result;
        }

        public string SetEventGLimiterToggle(string value)
        {
            var result = SendEvent("G_LIMITER_TOGGLE", value).ComputedResult;
            return result;
        }


        #endregion
    }
}
