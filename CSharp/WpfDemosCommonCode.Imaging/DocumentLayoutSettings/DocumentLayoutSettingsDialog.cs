using System.ComponentModel;
using System.Windows;

using Vintasoft.Imaging;
using Vintasoft.Imaging.Codecs.Decoders;

namespace WpfCommonCode.Imaging
{
    /// <summary>
    /// Provides a base class for dialog that allows to view and edit document layout settings.
    /// </summary>
    public abstract class DocumentLayoutSettingsDialog : Window
    {

        #region Properties

        /// <summary>
        /// Gets the name of the codec.
        /// </summary>
        public abstract string CodecName
        {
            get;
        }

        DocumentLayoutSettings _layoutSettings;
        /// <summary>
        /// Gets or sets the document layout settings.
        /// </summary>
        /// <value>
        /// Default value is <b>null</b>.
        /// </value>
        public virtual DocumentLayoutSettings LayoutSettings
        {
            get
            {
                return _layoutSettings;
            }
            set
            {
                _layoutSettings = value;
            }
        }

        #endregion



        #region Methods

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
