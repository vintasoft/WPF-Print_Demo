using System.Windows;

using Vintasoft.Imaging.Codecs.Decoders;

namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A window that allows to view and edit DOCX document layout settings.
    /// </summary>
    public partial class DocxLayoutSettingsDialog : DocumentLayoutSettingsDialog
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DocxLayoutSettingsDialog"/> class.
        /// </summary>
        public DocxLayoutSettingsDialog()
        {
            InitializeComponent();

            LayoutSettings = CreateDefaultLayoutSettings();
        }

        #endregion



        #region Properties

        /// <summary>
        /// Gets or sets the document layout settings.
        /// </summary>
        public override DocumentLayoutSettings LayoutSettings
        {
            get
            {
                if (defaultSettingsCheckBox.IsChecked.Value == true)
                    return null;

                return base.LayoutSettings;
            }
            set
            {
#if !REMOVE_OFFICE_PLUGIN
                // cast settings to DOCX document layout settings
                DocxDocumentLayoutSettings settings = (DocxDocumentLayoutSettings)value;

                base.LayoutSettings = value;

                // if new value equals to the default settings
                if (value.Equals(CreateDefaultLayoutSettings()))
                    // specify that default settings are used
                    defaultSettingsCheckBox.IsChecked = true;
                // if new value is not equal to the default settings
                else
                    defaultSettingsCheckBox.IsChecked = false;

                showHiddenContentCheckBox.IsChecked = settings.ShowHiddenContent;
#endif
                // pass the settings to control
                documentLayoutSettingsEditorControl1.LayoutSettings = base.LayoutSettings;
            }
        }

        #endregion



        #region Methods

        #region PROTECTED

        /// <summary>
        /// Returns the default document layout settings.
        /// </summary>
        /// <returns>
        /// Default document layout settings.
        /// </returns>
        protected override DocumentLayoutSettings CreateDefaultLayoutSettings()
        {
#if REMOVE_OFFICE_PLUGIN
            return new DocumentLayoutSettings();
#else
            return new DocxDocumentLayoutSettings();
#endif
        }

        #endregion


        #region PRIVATE

        #region UI

        /// <summary>
        /// Handles the CheckedChanged event of defaultSettingsCheckBox object.
        /// </summary>
        private void defaultSettingsCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            settingsGroupBox.IsEnabled = !defaultSettingsCheckBox.IsChecked.Value;
        }

        /// <summary>
        /// Handles the Click event of okButton object.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (defaultSettingsCheckBox.IsChecked.Value == true)
            {
                // create default settings
                LayoutSettings = CreateDefaultLayoutSettings();
            }
            else
            {
                // get settings form control
                base.LayoutSettings = documentLayoutSettingsEditorControl1.LayoutSettings;
#if !REMOVE_OFFICE_PLUGIN
                ((DocxDocumentLayoutSettings)LayoutSettings).ShowHiddenContent = showHiddenContentCheckBox.IsChecked.Value;
#endif
            }

            DialogResult = true;
        }

        /// <summary>
        /// Handles the Click event of cancelButton object.
        /// </summary>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #endregion

        #endregion

        #endregion

    }
}
