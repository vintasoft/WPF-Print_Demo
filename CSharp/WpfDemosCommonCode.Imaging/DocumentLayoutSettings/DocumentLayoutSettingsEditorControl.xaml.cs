using System;
using System.Windows.Controls;
using Vintasoft.Imaging.Codecs.Decoders;

namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A control that allows to edit document layout settings.
    /// </summary>
    public partial class DocumentLayoutSettingsEditorControl : UserControl
    {

        #region Constructors

        /// <summary>
        /// Inititalizes new instance of <see cref="DocumentLayoutSettingsEditorControl"/>.
        /// </summary>
        public DocumentLayoutSettingsEditorControl()
        {
            InitializeComponent();
        }

        #endregion



        #region Properties

        DocumentLayoutSettings _layoutSettings;
        /// <summary>
        /// Gets or sets document layout settings.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <b>value</b> is null.</exception>
        public DocumentLayoutSettings LayoutSettings
        {
            get
            {
                // update settings
                _layoutSettings.PageLayoutSettings = allPagesLayoutSettingsControl.PageLayoutSettings;
                _layoutSettings.EvenPageLayoutSettings = evenPagesLayoutSettingsControl.PageLayoutSettings;
                _layoutSettings.OddPageLayoutSettings = oddPagesLayoutSettingsControl.PageLayoutSettings;

                return _layoutSettings;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("", "Value can not be null.");

                // pass settings to controls
                allPagesLayoutSettingsControl.PageLayoutSettings = value.PageLayoutSettings;
                evenPagesLayoutSettingsControl.PageLayoutSettings = value.EvenPageLayoutSettings;
                oddPagesLayoutSettingsControl.PageLayoutSettings = value.OddPageLayoutSettings;

                _layoutSettings = value;
            }
        }

        #endregion

    }
}
