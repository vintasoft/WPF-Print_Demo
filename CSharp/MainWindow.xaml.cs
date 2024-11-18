using System;
using System.Globalization;
using System.IO;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Microsoft.Win32;

using Vintasoft.Imaging;
using Vintasoft.Imaging.Annotation;
using Vintasoft.Imaging.Annotation.Wpf.UI;
using Vintasoft.Imaging.Print;
using Vintasoft.Imaging.Wpf;
using Vintasoft.Imaging.Wpf.Print;
using Vintasoft.Imaging.Wpf.UI;

using WpfDemosCommonCode;
using WpfDemosCommonCode.Imaging;
using WpfDemosCommonCode.Imaging.Codecs;
#if !REMOVE_PDF_PLUGIN
using WpfDemosCommonCode.Pdf;
#endif

namespace WpfPrintDemo
{
    /// <summary>
    /// A main window of "Print Demo" application.
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Constants

        /// <summary>
        /// Page header or footer height.
        /// </summary>
        const double PageHeaderOrFooterHeight = 50.0;

        #endregion



        #region Fields

        /// <summary>
        /// Template of the application's title.
        /// </summary>
        string _titlePrefix = string.Format("VintaSoft WPF Print Demo v{0}", ImagingGlobalSettings.ProductVersion);

        /// <summary>
        /// Print manager.
        /// </summary>
        WpfImagePrintManager _imagePrintManager = new WpfImagePrintManager();

        /// <summary>
        /// Open File dialog.
        /// </summary>
        OpenFileDialog openFileDialog1 = new OpenFileDialog();

        /// <summary>
        /// Gets a value indicating whether the page header is shown.
        /// </summary>
        bool _showPageHeader;

        /// <summary>
        /// Gets a value indicating whether the page footer is shown.
        /// </summary>
        bool _showPageFooter;

        /// <summary>
        /// Gets a value indicating whether the image header is shown.
        /// </summary>
        bool _showImageHeader;

        /// <summary>
        /// Gets a value indicating whether the image footer is shown.
        /// </summary>
        bool _showImageFooter;

        /// <summary>
        /// Gets a value indicating whether the page area is shown.
        /// </summary>
        bool _showPageArea;

        /// <summary>
        /// Gets a value indicating whether the image area is shown.
        /// </summary>
        bool _showImageArea;

        /// <summary>
        /// Gets a value indicating whether the image rect is shown.
        /// </summary>
        bool _showImageRect;

        /// <summary>
        /// Gets a value indicating whether the annotations are shown.
        /// </summary>
        bool _printAnnotations;

        /// <summary>
        /// Contains page padding specified by user.
        /// </summary>
        Thickness _userPagePadding;

        /// <summary>
        /// Contains image padding specified by user.
        /// </summary>
        Thickness _userImagePadding;

        /// <summary>
        /// Manages the layout settings of DOCX document image collections.
        /// </summary>
        ImageCollectionDocxLayoutSettingsManager _imageCollectionDocxLayoutSettingsManager;

        /// <summary>
        /// Manages the layout settings of XLSX document image collections.
        /// </summary>
        ImageCollectionXlsxLayoutSettingsManager _imageCollectionXlsxLayoutSettingsManager;


        #region Hot keys

        public static RoutedCommand _openCommand = new RoutedCommand();
        public static RoutedCommand _addCommand = new RoutedCommand();
        public static RoutedCommand _printCommand = new RoutedCommand();
        public static RoutedCommand _exitCommand = new RoutedCommand();
        public static RoutedCommand _aboutCommand = new RoutedCommand();

        #endregion

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            // register the evaluation license for VintaSoft Imaging .NET SDK
            Vintasoft.Imaging.ImagingGlobalSettings.Register("REG_USER", "REG_EMAIL", "EXPIRATION_DATE", "REG_CODE");

            InitializeComponent();

            Jbig2AssemblyLoader.Load();
            Jpeg2000AssemblyLoader.Load();
            RawAssemblyLoader.Load();
            DicomAssemblyLoader.Load();
            DocxAssemblyLoader.Load();

            this.Title = _titlePrefix;

            printScaleModeComboBox.Items.Add(PrintScaleMode.None);
            printScaleModeComboBox.Items.Add(PrintScaleMode.BestFit);
            printScaleModeComboBox.Items.Add(PrintScaleMode.FitToHeight);
            printScaleModeComboBox.Items.Add(PrintScaleMode.FitToWidth);
            printScaleModeComboBox.Items.Add(PrintScaleMode.Mosaic);
            printScaleModeComboBox.Items.Add(PrintScaleMode.CropToPageSize);
            printScaleModeComboBox.Items.Add(PrintScaleMode.Stretch);

            printScaleModeComboBox.SelectedItem = PrintScaleMode.BestFit;

            openFileDialog1.Multiselect = true;

            // load XPS codec
            DemosTools.LoadXpsCodec();
            CodecsFileFilters.SetFilters(openFileDialog1);

            DemosTools.SetTestFilesFolder(openFileDialog1);

            thumbnailViewer1.Images.ImageCollectionChanged += new EventHandler<ImageCollectionChangeEventArgs>(Images_ImageCollectionChanged);
            thumbnailViewer1.ShowAnnotations = false;

            _imagePrintManager.PrintScaleMode = PrintScaleMode.BestFit;
            _imagePrintManager.MosaicColumnCount = (int)columnsOnPageNumericUpDown.Value;
            _imagePrintManager.MosaicRowCount = (int)rowsOnPageNumericUpDown.Value;
            _imagePrintManager.DistanceBetweenImages = new Size(
                distanceBetweenImagesNumericUpDown.Value,
                distanceBetweenImagesNumericUpDown.Value);

            _userPagePadding = _imagePrintManager.PagePadding;
            _userImagePadding = _imagePrintManager.ImagePadding;

            _imagePrintManager.PreviewZoom = zoomSlider.Value / 100.0;

            _imagePrintManager.Images = thumbnailViewer1.Images;
            _imagePrintManager.Preview = thumbnailViewerPreview;
            // clear input bindings such as Cut, Copy, Delete, etc.
            _imagePrintManager.Preview.InputBindings.Clear();
            _imagePrintManager.PrintingProgress += new EventHandler<ProgressEventArgs>(PrintingProgress);
            _imagePrintManager.PrintingException += new EventHandler<WpfPrintingExceptionEventArgs>(PrintingException);

            thumbnailViewer1.AnnotationDataController.AnnotationDataDeserializationException += new EventHandler<AnnotationDataDeserializationExceptionEventArgs>(AnnotationDataController_AnnotationDataDeserializationException);

            DocumentPasswordWindow.EnableAuthentication(thumbnailViewer1);

            // set CustomFontProgramsController for all opened PDF documents
            CustomFontProgramsController.SetDefaultFontProgramsController();

#if !REMOVE_OFFICE_PLUGIN
            // specify that image collection of thumbnail viewer must handle layout settings requests
            _imageCollectionDocxLayoutSettingsManager = new ImageCollectionDocxLayoutSettingsManager(thumbnailViewer1.Images);
            _imageCollectionXlsxLayoutSettingsManager = new ImageCollectionXlsxLayoutSettingsManager(thumbnailViewer1.Images);
#endif

#if REMOVE_OFFICE_PLUGIN
            documentLayoutSettingsMenuItem.Visibility = Visibility.Collapsed;
#endif

            // initialize color management
            ColorManagementHelper.EnableColorManagement(thumbnailViewer1);
            _imagePrintManager.PreviewColorManagement = thumbnailViewer1.ImageDecodingSettings.ColorManagement;
            _imagePrintManager.PrintColorManagement = thumbnailViewer1.ImageDecodingSettings.ColorManagement;
            _imagePrintManager.ReloadPreviewAsync();

            // update the UI
            UpdateUI();
        }

        #endregion



        #region Properties

        bool _isFileOpening = false;
        bool IsFileOpening
        {
            get
            {
                return _isFileOpening;
            }
            set
            {
                _isFileOpening = value;
                UpdateUI();
            }
        }

        bool _isFilePrinting = false;
        bool IsFilePrinting
        {
            get
            {
                return _isFilePrinting;
            }
            set
            {
                _isFilePrinting = value;
                UpdateUI();
            }
        }

        #endregion



        #region Methods

        #region UI state

        /// <summary>
        /// Updates the user interface of this window.
        /// </summary>
        private void UpdateUI()
        {
            // get the current status of application
            bool isFileOpening = IsFileOpening;
            bool isFileLoaded = thumbnailViewer1.Images != null;
            bool isFileEmpty = isFileLoaded ? thumbnailViewer1.Images.Count == 0 : true;
            bool isFilePrinting = IsFilePrinting;

            bool isMosaic = (PrintScaleMode)printScaleModeComboBox.SelectedItem == PrintScaleMode.Mosaic;

            // "File" menu
            fileMenuItem.IsEnabled = !isFileOpening && !isFilePrinting;
            pageSettingsMenuItem.IsEnabled = !isFileEmpty;
            printMenuItem.IsEnabled = !isFileEmpty;

            // "Page" menu
            pageMenuItem.IsEnabled = !isFileOpening && !isFilePrinting && !isFileEmpty;

            // images per page
            imagesPerPageGroupBox.IsEnabled = isMosaic;

            // all controls
            panel1.IsEnabled = !isFileOpening && !isFilePrinting && !isFileEmpty;
        }

        #endregion


        #region 'File' menu

        /// <summary>
        /// Handles the Click event of openImageMenuItem object.
        /// </summary>
        private void openImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            IsFileOpening = true;

            if (openFileDialog1.ShowDialog().Value)
            {
                thumbnailViewer1.Images.ClearAndDisposeItems();
                foreach (string filename in openFileDialog1.FileNames)
                {
                    try
                    {
                        thumbnailViewer1.Images.Add(filename);
                    }
                    catch (Exception ex)
                    {
                        DemosTools.ShowErrorMessage(ex);
                    }
                }
            }

            IsFileOpening = false;
        }

        /// <summary>
        /// Adds image(s) to the image collection of the thumbnail viewer.
        /// </summary>
        private void addImagesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            IsFileOpening = true;

            if (openFileDialog1.ShowDialog().Value)
            {
                foreach (string filename in openFileDialog1.FileNames)
                {
                    try
                    {
                        thumbnailViewer1.Images.Add(filename);
                    }
                    catch (Exception ex)
                    {
                        DemosTools.ShowErrorMessage(ex);
                    }
                }
            }

            IsFileOpening = false;
        }

        /// <summary>
        /// Handles the Click event of docxLayoutSettingsMenuItem object.
        /// </summary>
        private void docxLayoutSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageCollectionDocxLayoutSettingsManager.EditLayoutSettingsUseDialog(this);
        }

        /// <summary>
        /// Handles the Click event of xlsxLayoutSettingsMenuItem object.
        /// </summary>
        private void xlsxLayoutSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageCollectionXlsxLayoutSettingsManager.EditLayoutSettingsUseDialog(this);
        }

        /// <summary>
        /// Shows dialog of page settings.
        /// </summary>
        private void pageSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            PageSettingsWindow pageSettingsForm = new PageSettingsWindow(_imagePrintManager, _userPagePadding, _userImagePadding);
            pageSettingsForm.Owner = this;
            if (pageSettingsForm.ShowDialog().Value)
            {
                _userPagePadding = pageSettingsForm.PagePadding;
                _userImagePadding = pageSettingsForm.ImagePadding;
                UpdatePrintParams();
            }
        }

        /// <summary>
        /// Edits the color management settings of printing.
        /// </summary>
        private void printColorManagementMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditPrintColorManagement();
        }

        /// <summary>
        /// Show print dialog and
        /// start print if dialog results is OK.
        /// </summary>
        private void printMenuItem_Click(object sender, RoutedEventArgs e)
        {
            IsFilePrinting = true;

            PrintDialog printDialog = _imagePrintManager.PrintDialog;
            printDialog.MinPage = 1;
            printDialog.MaxPage = (uint)_imagePrintManager.GetPrintingPageCount();
            printDialog.UserPageRangeEnabled = true;

            // show print dialog
            if (printDialog.ShowDialog().Value)
            {
                try
                {
                    _imagePrintManager.Print(this.Title);
                }
                catch (Exception ex)
                {
                    DemosTools.ShowErrorMessage(ex);
                }
            }

            IsFilePrinting = false;
        }

        /// <summary>
        /// Exits the application.
        /// </summary>
        private void exitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion


        #region 'View' menu

        /// <summary>
        /// Edits the color management settings of preview.
        /// </summary>
        private void colorManagementMenuItem_Click(object sender, RoutedEventArgs e)
        {
            bool previewColorManagementIsNull = _imagePrintManager.PreviewColorManagement == null;
            if (ColorManagementSettingsWindow.EditColorManagement(thumbnailViewer1))
            {
                if (thumbnailViewer1.ImageDecodingSettings == null)
                    _imagePrintManager.PreviewColorManagement = null;
                else
                    _imagePrintManager.PreviewColorManagement = thumbnailViewer1.ImageDecodingSettings.ColorManagement;

                // if color management settings could be changed
                if (!previewColorManagementIsNull || _imagePrintManager.PreviewColorManagement != null)
                    // reload preview
                    _imagePrintManager.ReloadPreviewAsync();
            }
        }

        #endregion


        #region 'Page' menu

        /// <summary>
        /// Enables/disables showing of page header.
        /// </summary>
        private void showPageHeaderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            showPageHeaderMenuItem.IsChecked = !showPageHeaderMenuItem.IsChecked;
            _showPageHeader = showPageHeaderMenuItem.IsChecked;

            UpdatePrintParams();
        }

        /// <summary>
        /// Enables/disables showing of page footer.
        /// </summary>
        private void showPageFooterMenuItem_Click(object sender, RoutedEventArgs e)
        {
            showPageFooterMenuItem.IsChecked = !showPageFooterMenuItem.IsChecked;
            _showPageFooter = showPageFooterMenuItem.IsChecked;

            UpdatePrintParams();
        }

        /// <summary>
        /// Enables/disables showing of image header.
        /// </summary>
        private void showImageHeaderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            showImageHeaderMenuItem.IsChecked = !showImageHeaderMenuItem.IsChecked;
            _showImageHeader = showImageHeaderMenuItem.IsChecked;

            UpdatePrintParams();
        }

        /// <summary>
        /// Enables/disables showing of image footer.
        /// </summary>
        private void showImageFooterMenuItem_Click(object sender, RoutedEventArgs e)
        {
            showImageFooterMenuItem.IsChecked = !showImageFooterMenuItem.IsChecked;
            _showImageFooter = showImageFooterMenuItem.IsChecked;

            UpdatePrintParams();
        }

        /// <summary>
        /// Handles the Click event of showPageAreaMenuItem object.
        /// </summary>
        private void showPageAreaMenuItem_Click(object sender, RoutedEventArgs e)
        {
            showPageAreaMenuItem.IsChecked = !showPageAreaMenuItem.IsChecked;
            _showPageArea = showPageAreaMenuItem.IsChecked;

            UpdatePrintParams();
        }

        /// <summary>
        /// Handles the Click event of showImageAreaMenuItem object.
        /// </summary>
        private void showImageAreaMenuItem_Click(object sender, RoutedEventArgs e)
        {
            showImageAreaMenuItem.IsChecked = !showImageAreaMenuItem.IsChecked;
            _showImageArea = showImageAreaMenuItem.IsChecked;

            UpdatePrintParams();
        }

        /// <summary>
        /// Handles the Click event of showImageRectMenuItem object.
        /// </summary>
        private void showImageRectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            showImageRectMenuItem.IsChecked = !showImageRectMenuItem.IsChecked;
            _showImageRect = showImageRectMenuItem.IsChecked;

            UpdatePrintParams();
        }

        #endregion


        #region 'Help' menu

        /// <summary>
        /// Handles the Click event of aboutMenuItem object.
        /// </summary>
        private void aboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder description = new StringBuilder();

            description.AppendLine("This demo demonstrates the following SDK capabilities:");
            description.AppendLine();
            description.AppendLine("- Display and print all supported image and document file formats.");
            description.AppendLine();
            description.AppendLine("- Change image print preview settings: scale, center on page, visible pages, zoom, column/row count.");
            description.AppendLine();
            description.AppendLine("- Show page and image header and footer, highlight page and image area, image rectangle.");
            description.AppendLine();
            description.AppendLine("- Enable/disable image printing with or without annotations.");
            description.AppendLine();
            description.AppendLine();
            description.AppendLine("The project is available in C# and VB.NET for Visual Studio .NET.");

            WpfAboutBoxBaseWindow dlg = new WpfAboutBoxBaseWindow("vsimaging-dotnet");
            dlg.Description = description.ToString();
            dlg.Owner = this;
            dlg.ShowDialog();
        }

        #endregion


        #region Print settings

        /// <summary>
        /// Scale mode is changed.
        /// </summary>
        private void printScaleModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count == 1)
            {
                _imagePrintManager.PrintScaleMode = (PrintScaleMode)e.AddedItems[0];
                // update the UI
                UpdateUI();
                UpdateMaximumPageIndex();
            }
        }

        /// <summary>
        /// Enabled/disabled centering of image on page.
        /// </summary>
        private void centerImageOnPageCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            _imagePrintManager.Center = centerImageOnPageCheckBox.IsChecked.Value;
        }

        /// <summary>
        /// Enabled/disabled printing of image with annotations.
        /// </summary>
        private void printImageWithAnnotationsCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            thumbnailViewer1.ShowAnnotations = (bool)printImageWithAnnotationsCheckBox.IsChecked;
            _printAnnotations = thumbnailViewer1.ShowAnnotations;

            UpdatePrintParams();
        }

        /// <summary>
        /// Update image columns count in mosaic mode.
        /// </summary>
        private void columnsOnPageNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _imagePrintManager.MosaicColumnCount = (int)columnsOnPageNumericUpDown.Value;

            if (_imagePrintManager.Images != null)
                UpdateMaximumPageIndex();
        }

        /// <summary>
        /// Update image rows count in mosaic mode.
        /// </summary>
        private void rowsOnPageNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _imagePrintManager.MosaicRowCount = (int)rowsOnPageNumericUpDown.Value;

            if (_imagePrintManager.Images != null)
                UpdateMaximumPageIndex();
        }

        /// <summary>
        /// Update distance between images image in mosaic mode.
        /// </summary>
        private void distanceBetweenImagesNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            double newDistance = distanceBetweenImagesNumericUpDown.Value;
            _imagePrintManager.DistanceBetweenImages = new Size(newDistance, newDistance);
        }

        /// <summary>
        /// Edits a color management settings of printing images.
        /// </summary>
        private void EditPrintColorManagement()
        {
            ColorManagementSettingsWindow colorManagementSettingsForm = new ColorManagementSettingsWindow();

            colorManagementSettingsForm.ColorManagementSettings = _imagePrintManager.PrintColorManagement;

            if (colorManagementSettingsForm.ShowDialog().Value)
            {
                _imagePrintManager.PrintColorManagement = colorManagementSettingsForm.ColorManagementSettings;
            }
        }

        #endregion


        #region Thumbnail viewer

        /// <summary>
        /// AnnotationData deserialization exception handler.
        /// </summary>
        void AnnotationDataController_AnnotationDataDeserializationException(object sender, Vintasoft.Imaging.Annotation.AnnotationDataDeserializationExceptionEventArgs e)
        {
            DemosTools.ShowErrorMessage("AnnotationData deserialization exception", e.Exception);
        }

        /// <summary>
        /// Handles the FocusedIndexChanged event of thumbnailViewer1 object.
        /// </summary>
        private void thumbnailViewer1_FocusedIndexChanged(object sender, PropertyChangedEventArgs<int> e)
        {
            if (thumbnailViewer1.FocusedIndex >= 0)
            {
                pageIndexNumericUpDown.Value = _imagePrintManager.GetFirstPageIndex(thumbnailViewer1.FocusedIndex);
            }
        }

        #endregion


        #region Print preview

        /// <summary>
        /// Sets new zoom value.
        /// </summary>
        private void zoomTrackBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // if window is initialized
            if (IsInitialized)
            {
                // set new zoom value
                _imagePrintManager.PreviewZoom = zoomSlider.Value / 100.0;
                zoomSlider.ToolTip = string.Format("{0:f0}%", zoomSlider.Value);
            }
        }

        /// <summary>
        /// Handles the ValueChanged event of pageIndexNumericUpDown object.
        /// </summary>
        private void pageIndexNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _imagePrintManager.PreviewFirstPageIndex = (int)pageIndexNumericUpDown.Value;
        }

        /// <summary>
        /// Handles the ValueChanged event of columnsNumericUpDown object.
        /// </summary>
        private void columnsNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _imagePrintManager.PreviewColumnCount = (int)columnsNumericUpDown.Value;
        }

        /// <summary>
        /// Handles the ValueChanged event of rowsNumericUpDown object.
        /// </summary>
        private void rowsNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _imagePrintManager.PreviewRowCount = (int)rowsNumericUpDown.Value;
        }

        private void UpdatePrintParams()
        {
            bool subscribeToPagePrinted = _showPageHeader || _showPageFooter || _showPageArea;
            bool subscribeToImageTilePrinted = _showImageHeader || _showImageFooter ||
                _showImageArea || _showImageRect || _printAnnotations;

            // disable auto update
            _imagePrintManager.PreviewAutoUpdate = false;

            // set additional page padding for page header and footer
            Thickness newPagePadding = _userPagePadding;
            if (_showPageHeader)
                newPagePadding.Top = newPagePadding.Top + PageHeaderOrFooterHeight;
            if (_showPageFooter)
                newPagePadding.Bottom = newPagePadding.Bottom + PageHeaderOrFooterHeight;
            _imagePrintManager.PagePadding = newPagePadding;

            // set additional image padding for image header and footer
            Thickness newImagePadding = _userImagePadding;
            if (_showImageHeader)
                newImagePadding.Top = newImagePadding.Top + PageHeaderOrFooterHeight;
            if (_showImageFooter)
                newImagePadding.Bottom = newImagePadding.Bottom + PageHeaderOrFooterHeight;
            _imagePrintManager.ImagePadding = newImagePadding;

            _imagePrintManager.PagePrinted -= new EventHandler<WpfPagePrintEventArgs>(ImagePrintManager_PagePrinted);
            _imagePrintManager.ImageTilePrinted -= new EventHandler<WpfImageTilePrintEventArgs>(ImagePrintManager_ImageTilePrinted);

            if (subscribeToImageTilePrinted)
                _imagePrintManager.ImageTilePrinted += new EventHandler<WpfImageTilePrintEventArgs>(ImagePrintManager_ImageTilePrinted);
            if (subscribeToPagePrinted)
                _imagePrintManager.PagePrinted += new EventHandler<WpfPagePrintEventArgs>(ImagePrintManager_PagePrinted);

            // enable auto update
            _imagePrintManager.PreviewAutoUpdate = true;

            UpdateMaximumPageIndex();
            _imagePrintManager.ReloadPreviewAsync();
        }

        #endregion


        #region Hot keys

        /// <summary>
        /// Handles the CanExecute event of openCommandBinding object.
        /// </summary>
        private void openCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = openImageMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of addCommandBinding object.
        /// </summary>
        private void addCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = addImagesMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of printCommandBinding object.
        /// </summary>
        private void printCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = printMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of exitCommandBinding object.
        /// </summary>
        private void exitCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = exitMenuItem.IsEnabled;
        }

        #endregion


        /// <summary>
        /// Handles the ImageTilePrinted event of ImagePrintManager object.
        /// </summary>
        private void ImagePrintManager_ImageTilePrinted(object sender, WpfImageTilePrintEventArgs e)
        {
            if (_showImageArea)
            {
                Rect imageArea = e.ImageArea;
                Pen imageAreaPen = new Pen(Brushes.Red, 10);
                e.DrawingContext.DrawRectangle(null, imageAreaPen, imageArea);
            }

            if (_showImageRect)
            {
                Rect imageRect = e.ImageRect;
                Pen imageRectPen = new Pen(Brushes.Lime, 5);
                e.DrawingContext.DrawRectangle(null, imageRectPen, imageRect);
            }

            if (_printAnnotations)
            {
                VintasoftImage sourceImage = e.SourceImage;
                using (WpfAnnotationViewCollection viewCollection = new WpfAnnotationViewCollection(thumbnailViewer1.AnnotationDataController.GetAnnotations(sourceImage)))
                {
                    Rect imageRect = e.ImageRect;
                    WpfDrawingSurface drawingSurface = e.DrawingSurface;
                    DrawingContext dc = e.DrawingContext;

                    dc.PushClip(new RectangleGeometry(imageRect));
                    viewCollection.Render(dc, drawingSurface);
                    dc.Pop();
                }
            }

            // if image header or footer should be shown
            if (_showImageHeader || _showImageFooter)
                // draw page header and/or footer
                DrawImageHeaderAndFooter(e);
        }

        /// <summary>
        /// Handles the PagePrinted event of ImagePrintManager object.
        /// </summary>
        private void ImagePrintManager_PagePrinted(object sender, WpfPagePrintEventArgs e)
        {
            if (_showPageArea)
            {
                Rect pageArea = e.PageArea;
                Pen pageAreaPen = new Pen(Brushes.Blue, 2);
                pageAreaPen.DashStyle = DashStyles.Dot;
                e.DrawingContext.DrawRectangle(null, pageAreaPen, pageArea);
            }

            // if page header or footer should be shown
            if (_showPageHeader || _showPageFooter)
                // draw page header and/or footer
                DrawPageHeaderAndFooter(e);
        }

        /// <summary>
        /// Draws image header and/or footer.
        /// </summary>
        private void DrawImageHeaderAndFooter(WpfImageTilePrintEventArgs e)
        {
            VintasoftImage image = e.SourceImage;

            // if image header should be shown
            if (_showImageHeader)
            {
                string printText = string.Empty;
                // if print scale mode is Mosaic
                if (_imagePrintManager.PrintScaleMode == Vintasoft.Imaging.Print.PrintScaleMode.Mosaic)
                {
                    // get image filename and page index
                    printText = string.Format(
                        "{0}, {1}",
                        Path.GetFileName(image.SourceInfo.Filename),
                        image.SourceInfo.PageIndex);
                }
                // if print scale mode is not Mosaic
                else
                {
                    // get image filename and page index
                    printText = string.Format(
                        "Filename: {0}, Page index: {1}",
                        Path.GetFileName(image.SourceInfo.Filename),
                        image.SourceInfo.PageIndex);
                }

                // get formatted text
                FormattedText formattedText = new FormattedText(
                    printText,
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                    25.0,
                    Brushes.Black);

                // set new font size
                formattedText.SetFontSize(SelectFontSize(printText, formattedText, e.ImageRect.Width, 25));

                Size headerSize = formattedText.BuildGeometry(new Point()).Bounds.Size;
                double headerXPos = e.ImageRect.X;
                double headerYPos = e.ImageRect.Y - headerSize.Height * 1.2;
                if (headerYPos < 0)
                    headerYPos = 0;

                // draw page header on the context
                e.DrawingContext.DrawText(formattedText, new Point(headerXPos, headerYPos));
            }

            // if image footer should be shown
            if (_showImageFooter)
            {
                string printText = string.Format("Image info: {0}x{1}, {2}, {3}",
                    image.Width, image.Height, image.PixelFormat, image.Resolution);

                // get formatted text
                FormattedText formattedText = new FormattedText(
                    printText,
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                    25.0,
                    Brushes.Black);

                // set new font size
                formattedText.SetFontSize(SelectFontSize(printText, formattedText, e.ImageRect.Width, 25));

                Size footerSize = formattedText.BuildGeometry(new Point()).Bounds.Size;
                double footerXPos = e.ImageRect.X;
                double footerYPos = e.ImageRect.Y + e.ImageRect.Height + footerSize.Height * 0.25;
                // draw page footer on the context
                e.DrawingContext.DrawText(formattedText, new Point(footerXPos, footerYPos));
            }
        }

        /// <summary>
        /// Selects the font size by the width of the string.
        /// </summary>
        /// <param name="text">The text string.</param>
        /// <param name="maxWidth">The maximum width of the text.</param>
        /// <returns>The font size.</returns>
        private double SelectFontSize(string text, FormattedText formattedText, double maxWidth, float fontSize)
        {
            return (fontSize / formattedText.WidthIncludingTrailingWhitespace) * maxWidth;
        }

        /// <summary>
        /// Draws page header and/or footer.
        /// </summary>
        private void DrawPageHeaderAndFooter(WpfPagePrintEventArgs e)
        {
            // if page header should be shown
            if (_showPageHeader)
            {
                string printText = string.Format("Page: {0}; Scale mode: {1}",
                                                 e.PageIndex,
                                                 _imagePrintManager.PrintScaleMode);
                FormattedText formattedText = new FormattedText(
                    printText,
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                    25.0,
                    Brushes.Black);
                Size headerSize = formattedText.BuildGeometry(new Point()).Bounds.Size;
                double headerXPos = e.PageArea.X;
                double headerYPos = (e.PageArea.Y - headerSize.Height) / 2;
                if (headerYPos < 0)
                    headerYPos = 0;

                // draw page header on the context
                e.DrawingContext.DrawText(formattedText, new Point(headerXPos, headerYPos));
            }

            // if page footer should be shown
            if (_showPageFooter)
            {
                string printText = string.Format("{0}", DateTime.Now);
                FormattedText formattedText = new FormattedText(
                    printText,
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                    25.0,
                    Brushes.Black);
                Size footerSize = formattedText.BuildGeometry(new Point()).Bounds.Size;

                double footerXPos = e.PageArea.X;
                double footerYPos = e.PageArea.Y + e.PageArea.Height;
                footerYPos += (e.PageSize.Height - footerYPos) / 2;

                // draw page footer on the context
                e.DrawingContext.DrawText(formattedText, new Point(footerXPos, footerYPos));
            }
        }

        /// <summary>
        /// Handles the ImageCollectionChanged event of Images object.
        /// </summary>
        private void Images_ImageCollectionChanged(object sender, ImageCollectionChangeEventArgs e)
        {
            UpdateMaximumPageIndex();
            UpdateUI();
        }

        /// <summary>
        /// Updates the maximum value of the page index numeric up down.
        /// </summary>
        private void UpdateMaximumPageIndex()
        {
            pageIndexNumericUpDown.Maximum = _imagePrintManager.GetPrintingPageCount() - 1;
        }

        /// <summary>
        /// Printing progress.
        /// </summary>
        private void PrintingProgress(object sender, ProgressEventArgs e)
        {
            Dispatcher.Invoke(new UpdatePrintingProgressDelegate(UpdatePrintingProgress), e.Progress);
        }

        private void UpdatePrintingProgress(int progress)
        {
            actionLabel.Content = "Printing:";
            printingProgressBar.Value = progress;

            if (progress == 100)
            {
                printingProgressBar.Visibility = Visibility.Collapsed;
                actionLabel.Visibility = Visibility.Collapsed;
            }
            else
            {
                printingProgressBar.Visibility = Visibility.Visible;
                actionLabel.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Printing exception.
        /// </summary>
        private void PrintingException(object sender, WpfPrintingExceptionEventArgs e)
        {
            Dispatcher.Invoke(new ShowExceptionDelegate(ShowException), e.Exception);
        }

        private void ShowException(Exception ex)
        {
            DemosTools.ShowErrorMessage(ex);
        }

        /// <summary>
        /// Handles the Closing event of Window object.
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _imagePrintManager.Dispose();
        }

        #endregion



        #region Delegates

        private delegate void UpdatePrintingProgressDelegate(int progress);

        private delegate void ShowExceptionDelegate(Exception exception);

        #endregion

    }
}
