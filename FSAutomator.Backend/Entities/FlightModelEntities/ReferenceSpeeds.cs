using FSAutomator.Backend.Utilities;

namespace FSAutomator.Backend.Entities
{
    public class ReferenceSpeeds
    {
        public string FullFlapsStallSpeed { get; set; }
        public string FlapsUpStallSpeed { get; set; }
        public string CruiseSpeed { get; set; }
        public string MaxMach { get; set; }
        public string MaxIndicatedSpeed { get; set; }
        public string MaxFlapsExtended { get; set; }
        public string NormalOperatingSpeed { get; set; }
        public string AirspeedIndicatorMax { get; set; }
        public string RotationSpeedMin { get; set; }
        public string ClimbSpeed { get; set; }
        public string CruiseAlt { get; set; }
        public string TakeoffSpeed { get; set; }
        public string SpawnCruiseAltitude { get; set; }
        public string SpawnDescentAltitude { get; set; }
        public string BestAngleClimbSpeed { get; set; }
        public string ApproachSpeed { get; set; }
        public string BestGlide { get; set; }
        public string BestSingleEngineRateOfClimbSpeed { get; set; }
        public string MinimumControlSpeed { get; set; }

        private readonly IniFile ini;

        public ReferenceSpeeds(IniFile ini)
        {
            this.ini = ini;

            this.FullFlapsStallSpeed = GetValue("full_flaps_stall_speed", "REFERENCE SPEEDS");
            this.FlapsUpStallSpeed = GetValue("flaps_up_stall_speed", "REFERENCE SPEEDS");
            this.CruiseSpeed = GetValue("cruise_speed", "REFERENCE SPEEDS");
            this.MaxMach = GetValue("max_mach", "REFERENCE SPEEDS");
            this.MaxIndicatedSpeed = GetValue("max_indicated_speed", "REFERENCE SPEEDS");
            this.MaxFlapsExtended = GetValue("max_flaps_extended", "REFERENCE SPEEDS");
            this.NormalOperatingSpeed = GetValue("normal_operating_speed", "REFERENCE SPEEDS");
            this.AirspeedIndicatorMax = GetValue("airspeed_indicator_max", "REFERENCE SPEEDS");
            this.RotationSpeedMin = GetValue("rotation_speed_min", "REFERENCE SPEEDS");
            this.ClimbSpeed = GetValue("climb_speed", "REFERENCE SPEEDS");
            this.CruiseAlt = GetValue("cruise_alt", "REFERENCE SPEEDS");
            this.TakeoffSpeed = GetValue("takeoff_speed", "REFERENCE SPEEDS");
            this.SpawnCruiseAltitude = GetValue("spawn_cruise_altitude", "REFERENCE SPEEDS");
            this.SpawnDescentAltitude = GetValue("spawn_descent_altitude", "REFERENCE SPEEDS");
            this.BestAngleClimbSpeed = GetValue("best_angle_climb_speed", "REFERENCE SPEEDS");
            this.ApproachSpeed = GetValue("approach_speed", "REFERENCE SPEEDS");
            this.BestGlide = GetValue("best_glide", "REFERENCE SPEEDS");
            this.BestSingleEngineRateOfClimbSpeed = GetValue("best_single_engine_rate_of_climb_speed", "REFERENCE SPEEDS");
            this.MinimumControlSpeed = GetValue("minimum_control_speed", "REFERENCE SPEEDS");
        }

        public string GetValue(string key, string section)
        {
            return ini.Read(key, section).Split(';')[0].Trim();
        }
    }
}
