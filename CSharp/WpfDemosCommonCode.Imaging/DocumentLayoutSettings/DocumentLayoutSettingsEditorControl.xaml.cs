using System;
using System.ComponentModel;
using System.Windows.Controls;
using Vintasoft.Imaging;
using Vintasoft.Imaging.Codecs.Decoders;

namespace WpfCommonCode.Imaging
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

        /// <summary>
        /// Gets the name of the codec.
        /// </summary>
        [Browsable(false)]
        public virtual string CodecName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

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

        ImageCollectionLayoutSettingsManager _layoutSettingsManager;
        /// <summary>
        /// Gets or sets the manager of document layout settings.
        /// </summary>
        /// <value>
        /// Default value is <b>null</b>.
        /// </value>
        [Browsable(false)]
        public ImageCollectionLayoutSettingsManager LayoutSettingsManager
        {
            get
            {
                return _layoutSettingsManager;
            }
            set
            {
                _layoutSettingsManager = value;
                if (value != null)
                    LayoutSettings = _layoutSettingsManager[CodecName];
            }
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates the default layout settings.
        /// </summary>
        protected virtual DocumentLayoutSettings CreateDefaultLayoutSettings()
        {
            return LayoutSettingsManager.GetDefaultSettings(CodecName);
        }

        #endregion

    }
}
