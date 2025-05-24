using System.Diagnostics;

namespace DynaDocs
{
    public partial class Form1 : Form
    {
        private string selectedTemplatePath;

        private HashSet<string> uniqueFieldNames;
        private HashSet<string> uniqueComponentFiles;

        private const string DefaultTemplatesSubdirectory = "templates";
        private const string WordDocumentFilter = "Documentos Word (*.docx)|*.docx|Todos os arquivos (*.*)|*.*";
        private const string SelectTemplateDialogTitle = "Selecionar Template DOCX";

        public Form1()
        {
            InitializeComponent();
            uniqueFieldNames = new HashSet<string>();
            uniqueComponentFiles = new HashSet<string>();
        }

        private void btnSelectTemplate_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = CreateOpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedTemplatePath = openFileDialog.FileName;
                    txtTemplatePath.Text = selectedTemplatePath;

                    MessageBox.Show("Template selecionado: " + selectedTemplatePath, "Template Selecionado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ProcessAndLogTemplateTags(selectedTemplatePath);
                }
            }
        }

        private OpenFileDialog CreateOpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string templatesDirectoryPath = Path.Combine(Application.StartupPath, DefaultTemplatesSubdirectory);

            openFileDialog.InitialDirectory = Directory.Exists(templatesDirectoryPath) ? templatesDirectoryPath : Application.StartupPath;
            openFileDialog.Filter = WordDocumentFilter;
            openFileDialog.FilterIndex = 1;
            openFileDialog.Title = SelectTemplateDialogTitle;
            openFileDialog.RestoreDirectory = true;

            return openFileDialog;
        }

        private void ProcessAndLogTemplateTags(string filePath)
        {
            this.uniqueFieldNames.Clear();
            this.uniqueComponentFiles.Clear();

            var processor = new TemplateProcessor();
            TemplateAnalysisResult analysisResult = processor.AnalyzeTemplate(filePath);

            if (analysisResult.Success)
            {
                // Atualiza as coleções do Form1 com os resultados únicos
                foreach (var fieldName in analysisResult.UniqueFieldNames)
                {
                    this.uniqueFieldNames.Add(fieldName);
                }
                foreach (var componentFile in analysisResult.UniqueComponentFiles)
                {
                    this.uniqueComponentFiles.Add(componentFile);
                }

                if (this.uniqueFieldNames.Count > 0 || this.uniqueComponentFiles.Count > 0)
                {
                    // Abre o formulário para o usuário inserir os dados das tags
                    using (InputTagsForm inputForm = new InputTagsForm(this.uniqueFieldNames, this.uniqueComponentFiles))
                    {
                        if (inputForm.ShowDialog(this) == DialogResult.OK) // Passa 'this' para centralizar no Form1
                        {
                            // Usuário clicou em "Gerar Documento"
                            Dictionary<string, string> userFieldValues = inputForm.FieldValues;

                            // TODO: Implementar a lógica de geração do documento aqui,
                            // usando userFieldValues e this.uniqueComponentFiles
                            // e o selectedTemplatePath.

                            MessageBox.Show("Pronto para gerar o documento com os seguintes valores de campo:\n\n" +
                                            string.Join("\n", userFieldValues.Select(kvp => $"'{kvp.Key}': '{kvp.Value}'")),
                                            "Valores Coletados",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information);
                        }
                        else
                        {
                            // Usuário clicou em "Cancelar" ou fechou o formulário
                            MessageBox.Show("Geração de documento cancelada pelo usuário.", "Operação Cancelada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Nenhuma tag foi encontrada no template para preenchimento.",
                                    "Nenhuma Tag Dinâmica",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }
            else
            {
                Debug.WriteLine("Erro ao processar o template: " + analysisResult.ErrorMessage);
                MessageBox.Show("Ocorreu um erro ao tentar processar o template: \n" + analysisResult.ErrorMessage, "Erro de Processamento", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}