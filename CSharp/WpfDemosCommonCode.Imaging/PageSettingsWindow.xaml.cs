using System.Printing;
using System.Windows;

using Vintasoft.Imaging.Wpf.Print;

namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A window that allows to view and edit page settings.
    /// </summary>
    public partial class PageSettingsWindow : Window
    {

        #region Fields

        PageMediaSize _pageSize;

        #endregion



        #region Constructors

        public PageSettingsWindow()
        {
            InitializeComponent();
        }

        public PageSettingsWindow(WpfImagePrintManager printManager, Thickness pagePadding, Thickness imagePadding)
            : this()
        {
            _pageSize = printManager.PrintDialog.PrintTicket.PageMediaSize;
            _pagePadding = pagePadding;
            _imagePadding = imagePadding;

            ShowSettings();
        }

        #endregion



        #region Properties

        Thickness _pagePadding;
        public Thickness PagePadding
        {
            get
            {
                return _pagePadding;
            }
        }

        Thickness _imagePadding;
        public Thickness ImagePadding
        {
            get
            {
                return _imagePadding;
            }
        }

        #endregion



        #region Methods

        private void ShowSettings()
        {
            int pageWidth = (int)_pageSize.Width.Value;
            int pageHeight = (int)_pageSize.Height.Value;

            pagePaddingLeftNumericUpDown.Maximum = pageWidth;
            pagePaddingRightNumericUpDown.Maximum = pageWidth;
            pagePaddingTopNumericUpDown.Maximum = pageHeight;
            pagePaddingBottomNumericUpDown.Maximum = pageHeight;

            imagePaddingLeftNumericUpDown.Maximum = pageWidth;
            imagePaddingRightNumericUpDown.Maximum = pageWidth;
            imagePaddingTopNumericUpDown.Maximum = pageHeight;
            imagePaddingBottomNumericUpDown.Maximum = pageHeight;

            pagePaddingLeftNumericUpDown.Value = (int)_pagePadding.Left;
            pagePaddingRightNumericUpDown.Value = (int)_pagePadding.Right;
            pagePaddingTopNumericUpDown.Value = (int)_pagePadding.Top;
            pagePaddingBottomNumericUpDown.Value = (int)_pagePadding.Bottom;

            imagePaddingLeftNumericUpDown.Value = (int)_imagePadding.Left;
            imagePaddingRightNumericUpDown.Value = (int)_imagePadding.Right;
            imagePaddingTopNumericUpDown.Value = (int)_imagePadding.Top;
            imagePaddingBottomNumericUpDown.Value = (int)_imagePadding.Bottom;
        }

        private void SetSettings()
        {
            _pagePadding = new Thickness(
                pagePaddingLeftNumericUpDown.Value,
                pagePaddingTopNumericUpDown.Value,
                pagePaddingRightNumericUpDown.Value,
                pagePaddingBottomNumericUpDown.Value);
            _imagePadding = new Thickness(
                imagePaddingLeftNumericUpDown.Value,
                imagePaddingTopNumericUpDown.Value,
                imagePaddingRightNumericUpDown.Value,
                imagePaddingBottomNumericUpDown.Value);
        }

        /// <summary>
        /// Handles the Click event of okBtn object.
        /// </summary>
        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            int pageWidth = (int)_pageSize.Width.Value;
            int pageHeight = (int)_pageSize.Height.Value;

            if (pagePaddingLeftNumericUpDown.Value + pagePaddingRightNumericUpDown.Value >= pageWidth ||
                pagePaddingTopNumericUpDown.Value + pagePaddingBottomNumericUpDown.Value >= pageHeight)
            {
                MessageBox.Show("Margins overlap. Please specify correct values.");
                return;
            }

            SetSettings();
            DialogResult = true;
        }

        /// <summary>
        /// Handles the Click event of clearPagePaddingButton object.
        /// </summary>
        private void clearPagePaddingButton_Click(object sender, RoutedEventArgs e)
        {
            pagePaddingLeftNumericUpDown.Value = 0;
            pagePaddingTopNumericUpDown.Value = 0;
            pagePaddingRightNumericUpDown.Value = 0;
            pagePaddingBottomNumericUpDown.Value = 0;
        }

        /// <summary>
        /// Handles the Click event of setDefaultPagePaddingButton object.
        /// </summary>
        private void setDefaultPagePaddingButton_Click(object sender, RoutedEventArgs e)
        {
            pagePaddingLeftNumericUpDown.Value = 96;
            pagePaddingTopNumericUpDown.Value = 96;
            pagePaddingRightNumericUpDown.Value = 96;
            pagePaddingBottomNumericUpDown.Value = 96;
        }

        /// <summary>
        /// Handles the Click event of clearImagePaddingButton object.
        /// </summary>
        private void clearImagePaddingButton_Click(object sender, RoutedEventArgs e)
        {
            imagePaddingLeftNumericUpDown.Value = 0;
            imagePaddingTopNumericUpDown.Value = 0;
            imagePaddingRightNumericUpDown.Value = 0;
            imagePaddingBottomNumericUpDown.Value = 0;
        }

        /// <summary>
        /// Handles the Click event of setDefaultImagePaddingButton object.
        /// </summary>
        private void setDefaultImagePaddingButton_Click(object sender, RoutedEventArgs e)
        {
            imagePaddingLeftNumericUpDown.Value = 0;
            imagePaddingTopNumericUpDown.Value = 0;
            imagePaddingRightNumericUpDown.Value = 0;
            imagePaddingBottomNumericUpDown.Value = 0;
        }

        #endregion

    }
}
