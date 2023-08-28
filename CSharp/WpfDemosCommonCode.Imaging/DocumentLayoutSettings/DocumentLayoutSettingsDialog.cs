using System;
using System.ComponentModel;
using System.Windows;

using Vintasoft.Imaging.Codecs.Decoders;

namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// Provides a base class for dialog that allows to view and edit document layout settings.
    /// </summary>
    public class DocumentLayoutSettingsDialog : Window
    {

        #region Properties

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

        /// <summary>
        /// Returns the default document layout settings.
        /// </summary>
        /// <returns>
        /// Default document layout settings.
        /// </returns>
        /// <exception cref="NotImplementedException">Thrown if method is not implemented.</exception>
        protected virtual DocumentLayoutSettings CreateDefaultLayoutSettings()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
