using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DynaDocs
{
    public class TemplateProcessor
    {
        private const string ComponentTagRegexPattern = @"\{\{([^{}]+?\.docx)\}\}";
        private const string FieldTagRegexPattern = @"\{%([^%]+?)%\}";

        public TemplateAnalysisResult AnalyzeTemplate(string filePath)
        {
            var result = new TemplateAnalysisResult();

            try
            {
                if (!File.Exists(filePath))
                {
                    result.Success = false;
                    result.ErrorMessage = "Arquivo de template não encontrado: " + filePath;
                    return result;
                }

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
                {
                    MainDocumentPart mainPart = wordDoc.MainDocumentPart;
                    if (mainPart?.Document?.Body != null)
                    {
                        foreach (Paragraph paragraph in mainPart.Document.Body.Descendants<Paragraph>())
                        {
                            ExtractTagsFromParagraphText(paragraph.InnerText, result);
                        }
                    }
                    else
                    {
                        result.Success = false;
                        result.ErrorMessage = "O corpo do documento está vazio ou é inválido.";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = "Exceção ao processar template: " + ex.Message;
            }
            return result;
        }

        private void ExtractTagsFromParagraphText(string paragraphText, TemplateAnalysisResult result)
        {
            // Encontra tags de componente
            MatchCollection componentMatches = Regex.Matches(paragraphText, ComponentTagRegexPattern);
            foreach (Match match in componentMatches)
            {
                if (match.Success && match.Groups.Count > 1)
                {
                    result.AllFoundComponentTagsLog.Add(match.Value + " (Componente: " + match.Groups[1].Value + ")");
                    result.UniqueComponentFiles.Add(match.Groups[1].Value);
                }
            }

            // Encontra tags de campo
            MatchCollection fieldMatches = Regex.Matches(paragraphText, FieldTagRegexPattern);
            foreach (Match match in fieldMatches)
            {
                if (match.Success && match.Groups.Count > 1)
                {
                    result.AllFoundFieldTagsLog.Add(match.Value + " (Campo: " + match.Groups[1].Value + ")");
                    result.UniqueFieldNames.Add(match.Groups[1].Value);
                }
            }
        }
    }
}