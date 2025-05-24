namespace DynaDocs
{
    public partial class InputTagsForm : Form
    {
        private readonly IEnumerable<string> _fieldNames;
        private readonly IEnumerable<string> _componentFiles;
        private readonly Dictionary<string, TextBox> _fieldTextBoxes = new Dictionary<string, TextBox>();
        private readonly Dictionary<string, CheckBox> _componentCheckBoxes = new Dictionary<string, CheckBox>();

        public Dictionary<string, string> FieldValues { get; private set; }
        public HashSet<string> SelectedComponentFiles { get; private set; }

        public InputTagsForm(IEnumerable<string> fieldNames, IEnumerable<string> componentFiles)
        {
            InitializeComponent();
            _fieldNames = fieldNames ?? new List<string>();
            _componentFiles = componentFiles ?? new List<string>();

            FieldValues = new Dictionary<string, string>();
            SelectedComponentFiles = new HashSet<string>();
        }

        private void InputTagsForm_Load(object sender, EventArgs e)
        {
            // Gerar campos para os nomes de campos (Fields)
            foreach (string fieldName in _fieldNames)
            {
                Label lbl = new Label
                {
                    Text = fieldName + ":",
                    AutoSize = true,
                    Margin = new Padding(3, 6, 3, 0)
                };

                TextBox txt = new TextBox
                {
                    Name = "txt" + fieldName.Replace(" ", ""),
                    Width = 300,
                    Margin = new Padding(3, 0, 3, 6)
                };

                _fieldTextBoxes.Add(fieldName, txt);

                flowLayoutPanelInputs.Controls.Add(lbl);
                flowLayoutPanelInputs.Controls.Add(txt);
            }

            var fieldList = _fieldNames as ICollection<string> ?? new List<string>(_fieldNames);
            var componentList = _componentFiles as ICollection<string> ?? new List<string>(_componentFiles);

            // Gerar CheckBoxes para os arquivos de componentes
            foreach (string componentFile in _componentFiles)
            {
                CheckBox chkComponent = new CheckBox
                {
                    Text = componentFile,
                    Name = "chk" + componentFile.Replace(" ", "").Replace(".", ""), // Nome único
                    Checked = false, // desmarcado por padrão
                    AutoSize = true,
                    Margin = new Padding(10, 3, 3, 3)
                };
                _componentCheckBoxes.Add(componentFile, chkComponent); // Guarda referência ao CheckBox

                flowLayoutPanelInputs.Controls.Add(chkComponent);
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            FieldValues.Clear(); // Limpa valores anteriores, se houver
            foreach (var pair in _fieldTextBoxes)
            {
                FieldValues[pair.Key] = pair.Value.Text;
            }

            SelectedComponentFiles.Clear(); // Limpa seleções anteriores
            foreach (var pair in _componentCheckBoxes)
            {
                if (pair.Value.Checked)
                {
                    SelectedComponentFiles.Add(pair.Key);
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}