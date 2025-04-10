namespace CallReportCreator.Classes
{
    /// <summary>
    /// Calling logs.
    /// </summary> 
    public class Input
    {
        public string CallDate { get; set; }
        public string CallTime { get; set; }
        public string CallerNumber { get; set; }
        public string CalledPartyNumber { get; set; }
        public int CallDuration { get; set; }
    }
}
