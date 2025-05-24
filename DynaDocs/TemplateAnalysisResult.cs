namespace DynaDocs
{
    public class TemplateAnalysisResult
    {
        public HashSet<string> UniqueFieldNames { get; }
        public HashSet<string> UniqueComponentFiles { get; }
        public List<string> AllFoundFieldTagsLog { get; }
        public List<string> AllFoundComponentTagsLog { get; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public TemplateAnalysisResult()
        {
            UniqueFieldNames = new HashSet<string>();
            UniqueComponentFiles = new HashSet<string>();
            AllFoundFieldTagsLog = new List<string>();
            AllFoundComponentTagsLog = new List<string>();
            Success = true; // Assume sucesso por padrão
            ErrorMessage = string.Empty;
        }
    }
}