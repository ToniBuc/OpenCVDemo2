using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Tracking;

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
            var newSize = new Size(1280, 720);
            var capture = new VideoCapture(@"C:\Users\PR2\Videos\HighwayTrafficVideo2.mp4");
            var tracker = TrackerCSRT.Create();

            //Object detection
            var objectDetector = BackgroundSubtractorMOG2.Create();

            var run = true;
            while (run)
            {
                capture.Read(image);
                if (image.Empty())
                    break;
                Cv2.Resize(image, image, newSize);

                objectDetector.Apply(image, mask);

                Point[][] contours;
                HierarchyIndex[] hIndexes;

                Cv2.Threshold(mask, mask, 254, 255, ThresholdTypes.Binary);
                Cv2.FindContours(mask, out contours, out hIndexes, mode: RetrievalModes.CComp, method: ContourApproximationModes.ApproxSimple);

                if (contours.Length == 0)
                {
                    continue;
                }

                List<int[]> detections = new List<int[]>();
                var cIndex = 0;
                while (cIndex >= 0)
                {
                    var area = Cv2.ContourArea(contours[cIndex]);
                    if (area > 200)
                    {
                        //Cv2.DrawContours(image, contours, cIndex, color: Scalar.All(1), thickness: 2, lineType: LineTypes.Link4, hierarchy: hIndexes, maxLevel: 0);
                        var rectangle = Cv2.BoundingRect(contours[cIndex]);

                        Cv2.Rectangle(image, rectangle, Scalar.All(1), thickness: 2, lineType: LineTypes.Link4, 0);

                        detections.Add(new int[] {rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height});
                        Console.WriteLine(string.Join(",", detections.Last()));

                        
                        //tracker.Update(image, ref rectangle);
                    }
                    cIndex = hIndexes[cIndex].Next;
                }

                window.ShowImage(image);
                window2.ShowImage(mask);

                var key = (char)Cv2.WaitKey(10);
                switch ((char)Cv2.WaitKey(10))
                {
                    case (char)27: // ESC
                        run = false;
                        break;
                }
            }
        }
    }
}
