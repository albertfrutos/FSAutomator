namespace FSAutomator.BackEnd.Configuration
{
    public class FSPackagesPaths
    {
        private string s_FSPathCommunity { get; set; }
        private string s_FSPathOfficial { get; set; }

        public string FSPathOfficial
        {
            get { return this.s_FSPathOfficial; }
            set { this.s_FSPathOfficial = value; }
        }

        public string FSPathCommunity
        {
            get { return this.s_FSPathCommunity; }
            set { this.s_FSPathCommunity = value; }
        }
    }
}
