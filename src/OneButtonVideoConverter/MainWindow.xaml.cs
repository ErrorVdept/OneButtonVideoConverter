
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace OneButtonVideoConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            fileFormatComboBox.Items.Add("mp4");
            fileFormatComboBox.Items.Add("avi");
            fileFormatComboBox.Items.Add("mov");
            fileFormatComboBox.Items.Add("mkv");
            fileFormatComboBox.Items.Add("wmv");
            fileFormatComboBox.Items.Add("flv");

            fileResolutionComboBox.Items.Add("480p");
            fileResolutionComboBox.Items.Add("720p");
            fileResolutionComboBox.Items.Add("1080p");
        }
        private string inputFileDirectroy = "File not selected";
        private string fullInputFileDirectroy = "";
        private string inputFileRash = "";
        private void ChooseFileBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Video files|*.mp4;*.avi;*.mov;*.mkv;*wmv;*flv";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {
                inputFileDirectroy = openFileDialog.SafeFileName;
                fullInputFileDirectroy = openFileDialog.FileName;
                inputFileRash = Path.GetExtension(openFileDialog.FileName);
            }
            textboxInputFile.Text = inputFileDirectroy;

            if (!String.IsNullOrEmpty(fullInputFileDirectroy))
            {
                outputfolderpanel.IsEnabled = true;
            }
        }

        private string outputPath = "";
        private void ChooseOutputFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                outputPath = dialog.SelectedPath;
            }
            outFolderTextBox.Text = outputPath;
            if (!String.IsNullOrEmpty(outputPath))
            {
                fileformatsettings.IsEnabled = true;
            }
            ConvertVideoButton.IsEnabled = true;
        }

        private void ConvertVideoButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (String.IsNullOrEmpty(fileFormatComboBox.SelectedItem.ToString()) || String.IsNullOrEmpty(fileResolutionComboBox.SelectedItem.ToString()))
            {
                System.Windows.MessageBox.Show("Please Choose format and resolution");
            }
            else
            {
                outputfolderpanel.IsEnabled = false;
                fileformatsettings.IsEnabled = false;
                inputfolderpanel.IsEnabled = false;
                ConvertVideoButton.IsEnabled = false;
                var inputFile = new MediaFile { Filename = fullInputFileDirectroy };
                var outputFile = new MediaFile { Filename = outputPath + "/" + inputFileDirectroy.Replace(inputFileRash, "." + fileFormatComboBox.SelectedItem.ToString()) };
                var VideoSz = VideoSize.Hd480;

                switch (fileResolutionComboBox.SelectedItem.ToString())
                {
                    case "480p":
                        VideoSz = VideoSize.Hd480;
                        break;
                    case "720p":
                        VideoSz = VideoSize.Hd720;
                        break;
                    case "1080p":
                        VideoSz = VideoSize.Hd1080;
                        break;
                }
                var conversionOptions = new ConversionOptions
                {

                    VideoAspectRatio = VideoAspectRatio.R16_9,
                    VideoSize = VideoSz,
                    AudioSampleRate = AudioSampleRate.Hz44100
                };
                progressBarConverter.Minimum = 0;
                try
                {
                    new Thread(() => Converting(inputFile, outputFile, conversionOptions)).Start();
                }
                catch
                {
                    System.Windows.MessageBox.Show("Error. Please retry");
                }
            }
            
            

        }
        private void Converting(MediaFile InputFile, MediaFile OutputFile, ConversionOptions Options )
        {

            
            using (var engine = new Engine())
            {
                engine.ConvertProgressEvent += ConvertProgressEvent;
                engine.ConversionCompleteEvent += engine_ConversionCompleteEvent;
                engine.GetMetadata(InputFile);
                Dispatcher.Invoke(() => progressBarConverter.Maximum = InputFile.Metadata.Duration.TotalSeconds);
                engine.Convert(InputFile, OutputFile, Options);
            }
        }
        private void ConvertProgressEvent(object sender, ConvertProgressEventArgs e)
        {
             
            Dispatcher.Invoke(() => {
                progressBarConverter.Value = e.ProcessedDuration.TotalSeconds;
                
                } );


        }
        private void engine_ConversionCompleteEvent(object sender, ConversionCompleteEventArgs e)
        {
            Dispatcher.Invoke(() => {
                System.Windows.MessageBox.Show("Convertation Complete!");
                progressBarConverter.Value = 0;
                outputfolderpanel.IsEnabled = false;
                fileformatsettings.IsEnabled = false;
                inputfolderpanel.IsEnabled = true;
                ConvertVideoButton.IsEnabled = false;
            });
        }

        private void DonateBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SupportDev_Click(object sender, RoutedEventArgs e)
        {
            
            Process.Start("https://boosty.to/errorvdept");
        }

        private void MyWebsite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://errorvdept.github.io");
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            
            
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
