using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using Vintasoft.Imaging;
using Vintasoft.Imaging.Codecs.Decoders;
using Vintasoft.Imaging.Utils;

namespace WpfCommonCode.Imaging
{
    /// <summary>
    /// Interaction logic for EmailLayoutSettingsDialog.xaml
    /// </summary>
    public partial class EmailLayoutSettingsDialog : DocumentLayoutSettingsDialog
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailLayoutSettingsDialog"/> class.
        /// </summary>
        public EmailLayoutSettingsDialog()
        {
            InitializeComponent();

            // init "PageSize"
            pageSizeComboBox.Items.Add("Undefined");

            Array paperSizeKindValues = Enum.GetValues(typeof(PaperSizeKind));
            string[] paperSizeKindValuesText = new string[paperSizeKindValues.Length];
            for (int i = 0; i < paperSizeKindValues.Length; i++)
                paperSizeKindValuesText[i] = paperSizeKindValues.GetValue(i).ToString();
            Array.Sort(paperSizeKindValuesText, paperSizeKindValues);

            foreach (object item in paperSizeKindValues)
                pageSizeComboBox.Items.Add(item);

            pageWidthNumericUpDown.Minimum = 0;
            pageHeightNumericUpDown.Minimum = 0;

            pageWidthNumericUpDown.Maximum = 10000;
            pageHeightNumericUpDown.Maximum = 10000;
        }

        /// <summary>
        /// Inititalizes new instance of <see cref="EmailLayoutSettingsDialog"/>.
        /// </summary>
        public EmailLayoutSettingsDialog(ImageCollection images)
            : this()
        {
            LayoutSettingsManager = images.LayoutSettings;
        }

        #endregion



        #region Properties

        /// <summary>
        /// Gets the name of the codec.
        /// </summary>
        public override string CodecName
        {
            get
            {
                return "Email";
            }
        }

        /// <summary>
        /// Gets or sets the document layout settings.
        /// </summary>
        [Browsable(false)]
        [DefaultValue((DocumentLayoutSettings)null)]
        public override DocumentLayoutSettings LayoutSettings
        {
            get
            {
                return base.LayoutSettings;
            }
            set
            {
#if !REMOVE_EMAIL_CODEC
                EmailLayoutSettings emailLayoutSettings = value as EmailLayoutSettings;
                if (emailLayoutSettings == null)
                {
                    emailLayoutSettings = (EmailLayoutSettings)EmailLayoutSettings.DefaultEmailSettings.Clone();
                    if (value != null)
                        value.CopyTo(emailLayoutSettings);
                }

                base.LayoutSettings = emailLayoutSettings;

                // if new value equals to the default settings
                if (value.Equals(CreateDefaultLayoutSettings()))
                    // specify that default settings are used
                    defaultSettingsCheckBox.IsChecked = true;
                // if new value is not equal to the default settings
                else
                    // specify that custom settings are used
                    defaultSettingsCheckBox.IsChecked = false;


                // update controls
                if (PageLayoutSettings.PageSize != null)
                    pageSizeComboBox.SelectedItem = PageLayoutSettings.PageSize.PaperSizeKind;
                else
                    pageSizeComboBox.SelectedItem = "Undefined";

                createHeaderCheckBox.IsChecked = emailLayoutSettings.EmailConverterSettings.CreateHeader;
                createAttachmentsCheckBox.IsChecked = emailLayoutSettings.EmailConverterSettings.CreateAttachmentsInHeader;
                createUnknownHeadersCheckBox.IsChecked = emailLayoutSettings.EmailConverterSettings.CreateUnknownHeadersInHeader;
#endif
            }
        }

        /// <summary>
        /// Gets or sets the current page layout settings.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public PageLayoutSettings PageLayoutSettings
        {
            get
            {
                if (LayoutSettings == null)
                    return null;
                return LayoutSettings.PageLayoutSettings;
            }
        }

        #endregion



        #region Methods


        /// <summary>
        /// Handles the SelectionChanged event of pageSizeComboBox object.
        /// </summary>
        private void pageSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PageLayoutSettings == null)
                return;

            if (pageSizeComboBox.SelectedItem.ToString() != "Undefined")
            {
                ImageSize size;

                // if custom page size selected
                if (pageSizeComboBox.SelectedItem.ToString() == "Custom")
                {
                    pageWidthNumericUpDown.IsEnabled = true;
                    pageHeightNumericUpDown.IsEnabled = true;

                    // if page size already set
                    if (PageLayoutSettings.PageSize != null)
                    {
                        // create custom page size with current values
                        size = ImageSize.FromInches(
                            PageLayoutSettings.PageSize.WidthInInch,
                            PageLayoutSettings.PageSize.HeightInInch,
                            PageLayoutSettings.PageSize.Resolution);
                    }
                    else
                    {
                        // create custom page size with default values
                        size = ImageSize.FromMillimeters(100, 100, ImagingEnvironment.ScreenResolution);
                    }
                }
                else
                {
                    // get page size from paper kind
                    size = ImageSize.FromPaperKind((PaperSizeKind)pageSizeComboBox.SelectedItem);
                    pageWidthNumericUpDown.IsEnabled = false;
                    pageHeightNumericUpDown.IsEnabled = false;
                }

                PageLayoutSettings.PageSize = size;

                // update page width and height containers
                pageWidthNumericUpDown.Value = (int)Math.Round(UnitOfMeasureConverter.ConvertToMillimeters(size.WidthInInch, UnitOfMeasure.Inches));
                pageHeightNumericUpDown.Value = (int)Math.Round(UnitOfMeasureConverter.ConvertToMillimeters(size.HeightInInch, UnitOfMeasure.Inches));
            }
            else
            {
                PageLayoutSettings.PageSize = null;
                pageWidthNumericUpDown.IsEnabled = false;
                pageHeightNumericUpDown.IsEnabled = false;
            }
        }

        /// <summary>
        /// Handles the ValueChanged event of pageSizeNumericUpDown object.
        /// </summary>
        private void pageSizeNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (pageSizeComboBox.SelectedItem.ToString() == "Custom")
            {
                // create custom page size
                PageLayoutSettings.PageSize = ImageSize.FromMillimeters(
                    (int)pageWidthNumericUpDown.Value,
                    (int)pageHeightNumericUpDown.Value,
                    ImagingEnvironment.ScreenResolution);
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of defaultSettingsCheckBox object.
        /// </summary>
        private void defaultSettingsCheckBox_CheckedChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            settingsGroupBox.IsEnabled = defaultSettingsCheckBox.IsChecked.Value == false;
        }

        /// <summary>
        /// Handles the Click event of okButton object.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_EMAIL_CODEC
            EmailLayoutSettings layoutSettings;

            if (defaultSettingsCheckBox.IsChecked == true)
            {
                // create default settings
                layoutSettings = (EmailLayoutSettings)CreateDefaultLayoutSettings();
            }
            // if custom settings must be used
            else
            {
                layoutSettings = (EmailLayoutSettings)LayoutSettings;
                layoutSettings.EmailConverterSettings.CreateHeader = createHeaderCheckBox.IsChecked.Value == true;
                layoutSettings.EmailConverterSettings.CreateAttachmentsInHeader = createAttachmentsCheckBox.IsChecked.Value == true;
                layoutSettings.EmailConverterSettings.CreateUnknownHeadersInHeader = createUnknownHeadersCheckBox.IsChecked.Value == true;
            }

            try
            {
                LayoutSettingsManager[CodecName] = layoutSettings;
                DialogResult = true;
            }
            catch (Exception ex)
            {
                DemosTools.ShowErrorMessage(ex);
                DialogResult = false;
            }
#endif
        }

        /// <summary>
        /// Handles the Click event of cancelButton object.
        /// </summary>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #endregion

    }
}
