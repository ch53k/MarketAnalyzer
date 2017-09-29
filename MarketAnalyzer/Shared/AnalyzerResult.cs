namespace MarketAnalyzer.Shared
{
    public class AnalyzerResult
    {
        public bool Succeeded { get; }
        public string Error { get; }

        protected AnalyzerResult()
        {
            Succeeded = true;
        }

        protected AnalyzerResult(string error)
        {
            Succeeded = false;
            Error = error;
        }

        public static AnalyzerResult Sucess()
        {
            return new AnalyzerResult();
        }

        public static AnalyzerResult Fail(string error)
        {
            return new AnalyzerResult(error);
        }
    }
}