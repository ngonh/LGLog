using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenCvSharp;
using System.Windows;
using System.Windows.Input;

namespace LGLog.WPFApp
{
    public class MainWindowViewModel : ObservableObject
    {
        #region Constructor(s)
        public MainWindowViewModel()
        {
            ContinuousCaptureCommand = new RelayCommand(ContinuousCapture);
        }
        #endregion

        #region Properties
        public Mat? ImageSource
        {
            get => imageSource;
            set
            {
                imageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }
        public int NG
        {
            get => ng;
            set
            {
                ng = value;
                OnPropertyChanged(nameof(NG));
            }
        }
        public int OK
        {
            get => ok;
            set
            {
                ok = value;
                OnPropertyChanged(nameof(OK));
            }
        }
        #endregion

        #region Command(s)
        public ICommand ContinuousCaptureCommand { get; set; }
        #endregion

        #region Method(s)
        private async void ContinuousCapture()
        {
#if SIMULATION
            Random random = new Random();
            await Task.Run(() =>
            {
                while (true)
                {
                    int imageIndex = random.Next(1, 3);
                    string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Images/img{imageIndex}.png");
                    if (System.IO.File.Exists(imagePath))
                    {
                        Mat mat = new Mat(imagePath, ImreadModes.Grayscale);
                        if (IsOk(mat)) OK++;
                        else NG++;
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            imageSource?.Dispose();
                            ImageSource = mat.Clone();
                        });

                        mat.Dispose();
                    }
                    Thread.Sleep(200);
                }
            });
#endif
            // TODO Screen Capture
        }
        private bool IsOk(Mat imageSource)
        {
            string imageTemplatePath = "Images/NG.png";
            if (imageSource != null) 
            imageSource.ConvertTo(imageSource, MatType.CV_8U);
            Mat tem = new Mat(imageTemplatePath, ImreadModes.Grayscale);
            Mat result = new Mat();
            tem.ConvertTo(tem, MatType.CV_8U);
            Cv2.MatchTemplate(imageSource, tem, result, TemplateMatchModes.CCoeffNormed);
            Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out OpenCvSharp.Point maxLox);
            return !(maxVal > 0.91d);
        }
        #endregion

        #region Field(s)
        private Mat? imageSource;
        private int ok = 0;
        private int ng = 0;
        #endregion
    }
}
