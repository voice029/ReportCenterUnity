namespace ReportCenterSystem
{
    public class ReportEntryPublisher
    {
        public delegate void Report(ReportEntryBase entry);
        public Report OnReport = entry => { };

        public delegate void Initialize();
        public Initialize OnInitialize = () => { };

        public delegate void AddedTo(IReportEntries entries, ReportEntryBase entry);
        public AddedTo OnAddedTo = (entries, entry) => { };
        
        public static ReportEntryPublisher Instance => s_Instance;
        
        private static ReportEntryPublisher s_Instance;

        static ReportEntryPublisher()
        {
            s_Instance = new ReportEntryPublisher();
        }
    }
}