using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Tracking;
using Yolov5Net.Scorer.Models;
using Yolov5Net.Scorer;
using System.Drawing;

namespace OpenCVDemo2
{
    public class Yolov5
    {
        static void Main(string[] args)
        {
            var window = new Window("Video");
            var image = new Mat();
            var newSize = new OpenCvSharp.Size(1280, 720);
            var capture = new VideoCapture(@"D:\PETS09-S2L1-raw.mp4"); //place path to any video you see fit
            var scorer = new YoloScorer<YoloCocoP5Model>(@"D:\repos\OpenCVDemo2\yolov5s.onnx");

            #region Yolov5 (still image)

            //var stockImage = Image.FromFile(@"D:\road2.jpg");

            //List<YoloPrediction> predictions = scorer.Predict(stockImage);
            //var graphics = Graphics.FromImage(stockImage);

            //foreach (var prediction in predictions)
            //{
            //    double score = Math.Round(prediction.Score, 2);

            //    graphics.DrawRectangles(new Pen(prediction.Label.Color, 1), new[] { prediction.Rectangle });

            //    var (x, y) = (prediction.Rectangle.X - 3, prediction.Rectangle.Y - 23);

            //    graphics.DrawString($"{prediction.Label.Name} ({score})", new Font("Arial", 16, GraphicsUnit.Pixel), new SolidBrush(prediction.Label.Color), new PointF(x, y));
            //}

            //stockImage.Save(@"D:\repos\result.jpg");

            #endregion
            var run = true;
            while (run)
            {
                capture.Read(image);
                if (image.Empty())
                    break;
                Cv2.Resize(image, image, newSize);

                #region Yolov5 (video)

                var tempImage = Image.FromStream(image.ToMemoryStream());

                List<YoloPrediction> predictions = scorer.Predict(tempImage);
                var graphics = Graphics.FromImage(tempImage);

                foreach (var prediction in predictions)
                {
                    double score = Math.Round(prediction.Score, 2);

                    graphics.DrawRectangles(new Pen(prediction.Label.Color, 1), new[] { prediction.Rectangle });

                    var (x, y) = (prediction.Rectangle.X - 3, prediction.Rectangle.Y - 23);

                    graphics.DrawString($"{prediction.Label.Name} ({score})", new Font("Arial", 16, GraphicsUnit.Pixel), new SolidBrush(prediction.Label.Color), new PointF(x, y));
                }

                #endregion

                Bitmap tempBmp = new Bitmap(tempImage);
                window.ShowImage(OpenCvSharp.Extensions.BitmapConverter.ToMat(tempBmp));

                var key = (char)Cv2.WaitKey(10);
                switch ((char)Cv2.WaitKey(10))
                {
                    case (char)27:
                        run = false;
                        break;
                }
            }
        }
    }
}
