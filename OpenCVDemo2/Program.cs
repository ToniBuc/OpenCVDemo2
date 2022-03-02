using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Tracking;
using Yolov5Net.Scorer.Models;
using Yolov5Net.Scorer;

namespace OpenCVDemo2
{
    class Program
    {
        static void Main(string[] args)
        {
            var window = new Window("Video");
            var window2 = new Window("Mask");
            var image = new Mat();
            var mask = new Mat();
            var newSize = new OpenCvSharp.Size(1280, 720);
            var capture = new VideoCapture(@"D:\PETS09-S2L1-raw.mp4"); //place path to any video you see fit
            //var tracker = TrackerCSRT.Create();
            //var multiTracker = MultiTracker.Create();

            var objectDetector = BackgroundSubtractorMOG2.Create(100, 40, true);
            var run = true;
            while (run)
            {
                capture.Read(image);
                if (image.Empty())
                    break;
                Cv2.Resize(image, image, newSize);

                #region Filters

                //Cv2.CvtColor(image, image, ColorConversionCodes.BGR2GRAY);
                //Cv2.GaussianBlur(image, image, new OpenCvSharp.Size(5, 5), 0, 0);
                //Cv2.Threshold(mask, mask, 254, 255, ThresholdTypes.Binary);

                //Cv2.Dilate(image, mask, new Mat(), null, 3, BorderTypes.Constant, null);

                //Cv2.Erode(image, image, new Mat(), new Point(-1, -1), 2, BorderTypes.Constant, new Scalar(1));

                //Cv2.InRange(image, new Scalar(38, 86, 0), new Scalar(121, 255, 255), mask);

                #endregion

                #region Threshold, Contours & Lists

                objectDetector.Apply(image, mask);
                Cv2.Threshold(mask, mask, 254, 255, ThresholdTypes.Binary);
                Point[][] contours;
                HierarchyIndex[] hIndexes;

                //Setting up contours
                Cv2.FindContours(mask, out contours, out hIndexes, RetrievalModes.CComp, ContourApproximationModes.ApproxSimple);

                if (contours.Length == 0)
                {
                    continue;
                }

                List<int[]> detections = new List<int[]>();
                List<Rect> rects = new List<Rect>();

                #endregion

                var cIndex = 0;
                while (cIndex >= 0)
                {
                    
                    var area = Cv2.ContourArea(contours[cIndex]);
                    if (area > 250) //limiting size of detected objects
                    {

                        #region Setting Up Rectangle

                        var rect = Cv2.BoundingRect(contours[cIndex]);
                        rects.Add(rect);

                        //Cv2.Rectangle(image, rects.Last(), Scalar.All(1), thickness: 2, lineType: LineTypes.Link4, 0);

                        #endregion

                        #region Adding Detections to List

                        ////Writing rectangle coords to console (non rect list)
                        
                        //detections.Add(new int[] {rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height});
                        //Console.WriteLine(string.Join(",", detections.Last()));

                        //Writing rectangle coords to console (rect list)

                        detections.Add(new int[] { rects.Last().X, rects.Last().Y, rects.Last().Width, rects.Last().Height });
                        //Console.WriteLine(string.Join(",", detections.Last()));

                        #endregion

                        //Cv2.DrawContours(image, contours, cIndex, color: Scalar.All(255), thickness: 2, lineType: LineTypes.Link4, hierarchy: hIndexes, maxLevel: 0);
                    }

                    cIndex = hIndexes[cIndex].Next;
                }

                #region For Multitracker Experimentation

                for (int i = 0; i < rects.Count; i++)
                {
                    //PutText is here just for testing, doesn't really work because there's no proper tracking
                    //Cv2.PutText(image, i.ToString(), rects[i].TopLeft, HersheyFonts.HersheyPlain, 1, Scalar.All(1), 2, LineTypes.Link8);
                    Cv2.Rectangle(image, rects[i], Scalar.All(255), 2, LineTypes.Link4, 0);
                }

                #endregion

                window.ShowImage(image);
                window2.ShowImage(mask);

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
