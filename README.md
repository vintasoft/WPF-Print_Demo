# VintaSoft WPF Print Demo

This C# project uses <a href="https://www.vintasoft.com/vsimaging-dotnet-index.html">VintaSoft Imaging .NET SDK</a> and demonstrates how to print images and documents in WPF:
* Display and print all supported image and document file formats.
* Change image print preview settings: scale, center on page, visible pages, zoom, column/row count.
* Show page and image header and footer, highlight page and image area, image rectangle.
* Enable/disable image printing with or without annotations.


## Screenshot
<img src="vintasoft-wpf-print-demo.png" alt="VintaSoft WPF Print Demo">


## Usage
1. Get the 30 day free evaluation license for <a href="https://www.vintasoft.com/vsimaging-dotnet-index.html" target="_blank">VintaSoft Imaging .NET SDK</a> as described here: <a href="https://www.vintasoft.com/docs/vsimaging-dotnet/Licensing-Evaluation.html" target="_blank">https://www.vintasoft.com/docs/vsimaging-dotnet/Licensing-Evaluation.html</a>

2. Update the evaluation license in "CSharp\MainForm.cs" file:
   ```
   Vintasoft.Imaging.ImagingGlobalSettings.Register("REG_USER", "REG_EMAIL", "EXPIRATION_DATE", "REG_CODE");
   ```

3. Build the project ("WpfPrintDemo.Net7.csproj" file) in Visual Studio or using .NET CLI:
   ```
   dotnet build WpfPrintDemo.Net7.csproj
   ```

4. Run compiled application and try to print images and documents.


## Documentation
VintaSoft Imaging .NET SDK on-line User Guide and API Reference for .NET developer is available here: https://www.vintasoft.com/docs/vsimaging-dotnet/


## Support
Please visit our <a href="https://myaccount.vintasoft.com/">online support center</a> if you have any question or problem.
