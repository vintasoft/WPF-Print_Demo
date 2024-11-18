using System;
using System.Windows;

using Vintasoft.Imaging.Codecs.Decoders;

namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A window that allows to view and edit XLSX document layout settings.
    /// </summary>
    public partial class XlsxLayoutSettingsDialog : DocumentLayoutSettingsDialog
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XlsxLayoutSettingsDialog"/> class.
        /// </summary>
        public XlsxLayoutSettingsDialog()
        {
            InitializeComponent();

            LayoutSettings = CreateDefaultLayoutSettings();
        }

        #endregion



        #region Properties

        /// <summary>
        /// Gets or sets the document layout settings.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if new value is not <see cref="XlsxDocumentLayoutSettings"/>.</exception>
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
                // cast settings to XLSX document layout settings
                XlsxDocumentLayoutSettings settings = (XlsxDocumentLayoutSettings)value;

                base.LayoutSettings = settings;

                // if new settings are equal to the default settings
                if (settings.Equals(CreateDefaultLayoutSettings()))
                    // specify that default settings are used
                    defaultSettingsCheckBox.IsChecked = true;
                // if new settings are not equal to the default settings
                else
                    // specify that custom settings are used
                    defaultSettingsCheckBox.IsChecked = false;

                XlsxPageLayoutSettingsTypeEditorControl1.Settings = settings.PageLayoutSettingsType;
                showHiddenSheetsCheckBox.IsChecked = settings.ShowHiddenSheets;
                showHiddenGraphicsCheckBox.IsChecked = settings.ShowHiddenGraphics;

                if (settings.WorksheetIndex != null)
                {
                    worksheetIndexCheckBox.IsChecked = true;
                    worksheetIndexNumericUpDown.Value = settings.WorksheetIndex.Value;
                }
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
            return new XlsxDocumentLayoutSettings();
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
        /// Handles the CheckedChanged event of worksheetIndexCheckBox object.
        /// </summary>
        private void worksheetIndexCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            worksheetIndexNumericUpDown.IsEnabled = worksheetIndexCheckBox.IsChecked.Value;
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
                // get settings
                base.LayoutSettings = documentLayoutSettingsEditorControl1.LayoutSettings;
#if !REMOVE_OFFICE_PLUGIN
                XlsxDocumentLayoutSettings xlsxSettings = ((XlsxDocumentLayoutSettings)LayoutSettings);
                xlsxSettings.PageLayoutSettingsType = XlsxPageLayoutSettingsTypeEditorControl1.Settings;
                xlsxSettings.ShowHiddenSheets = showHiddenSheetsCheckBox.IsChecked.Value;
                xlsxSettings.ShowHiddenGraphics = showHiddenGraphicsCheckBox.IsChecked.Value;

                if (worksheetIndexCheckBox.IsChecked == true)
                    xlsxSettings.WorksheetIndex = (int)worksheetIndexNumericUpDown.Value;
                else
                    xlsxSettings.WorksheetIndex = null;
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
