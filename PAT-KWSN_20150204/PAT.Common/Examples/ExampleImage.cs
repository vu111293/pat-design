using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using PAT.Common.Classes.Assertion;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;

namespace PAT.Common.Examples
{
    public class ExampleImage<P,V>
    {
        public enum PhilosopherStatus
        {
            Thinking,
            HaveLeftFork,
            Eating
        };

        public static Bitmap MapConfigurationToImage(ConfigurationBase<P, V> config, SpecificationBase<P,V> Spec, int imageSize)
        {
            string ExampleName = Spec.SpecificationName;
            try
            {

                ExpressionValue value;
                switch (ExampleName)
                {
                    case "Sliding Game":
                        value = config.GlobalEnv.Variables["board"];
                        return DrawSlidingBoard(EvaluatorDenotational.GetValueFromExpression(value) as int[], imageSize);
                        
                    case "Shunting Game":
                        value = config.GlobalEnv.Variables["board"];
                        int c = (config.GlobalEnv.Variables["c"] as IntConstant).Value;
                        int r = (config.GlobalEnv.Variables["r"] as IntConstant).Value;

                        //row
                        int N = (Spec.GlobalConstantDatabase["N"] as IntConstant).Value;
                        //col
                        int M = (Spec.GlobalConstantDatabase["M"] as IntConstant).Value;

                        Point shunter = new Point(c, r);

                        //DrawShunterBoard
                        //return DrawShunterBoard(EvaluatorDenotational.GetValueFromExpression(value) as int[], imageSize);
                        Point[] crossPos = { new Point(2, 2), new Point(2, 3), new Point(3, 2), new Point(3, 3) };
                        return DrawShunterBoard(EvaluatorDenotational.GetValueFromExpression(value) as int[], shunter, crossPos, M, N, imageSize, imageSize);
                    //case "Dining Philosopher":
                    //    int numberOfPhilosophers = 5;
                    //    PhilosopherStatus[] philosopherStatuses = new PhilosopherStatus[numberOfPhilosophers];
                    //    philosopherStatuses[0] = PhilosopherStatus.Thinking;
                    //    philosopherStatuses[1] = PhilosopherStatus.Eating;
                    //    philosopherStatuses[2] = PhilosopherStatus.Thinking;
                    //    philosopherStatuses[3] = PhilosopherStatus.Thinking;
                    //    philosopherStatuses[4] = PhilosopherStatus.HaveLeftFork;

                    //    return DrawDiningPhilosopher(imageSize, philosopherStatuses);

                }
            }
            catch (Exception)
            {


            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr">input number array, which suppose to be a square(thus can get the dimension by Math.Sqrt(arr.Length))</param>
        /// <param name="imageLen">the width and height of output image (output image is supposed to be a square)</param>
        /// <returns>bitmap image that represents by arr</returns>
        private static Bitmap DrawSlidingBoard(int[] arr, int imageLen)
        {
            //===Configuration=============            
            int dimension = (int)Math.Sqrt(arr.Length);
            //the thickness of the pen to draw grid
            int penPixelWidth = 3;
            //the number that represents the empty sqare
            int emptyNum = 0;
            Color lineColor = Color.Black;
            Brush fontColor = Brushes.Black;
            //Rectangle is a linear gradient, thus need to color to form it
            Color upperRect = Color.FromArgb(208, 216, 226);
            Color lowerRect = Color.FromArgb(180, 190, 204);
            Color boardColor = Color.FromArgb(227, 176, 116);
            //===Configuration=============

            //Drawing on bitmap
            Bitmap myBitmap = new Bitmap(imageLen, imageLen,
                 System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics graphicsObj = Graphics.FromImage(myBitmap);
            Pen linePen = new Pen(lineColor, penPixelWidth);
            Brush brGradient = null;
            StringFormat drawFormat = null;
            Font useFont = null;
            try
            {

                graphicsObj.Clear(boardColor);
                //Length of each square on the board
                int squareLen = (imageLen - 2 * penPixelWidth) / dimension;


                //determine font size
                int fontSize = squareLen / 2;
                useFont = new Font("Arial", fontSize);

                for (int i = 0; i < arr.Length; i++)
                {
                    string printStr = arr[i].ToString();
                    SizeF size = graphicsObj.MeasureString(printStr, useFont);
                    while (size.Height >= squareLen || size.Width >= squareLen)
                    {
                        fontSize -= 5;
                        useFont = new Font("Arial", fontSize);
                        size = graphicsObj.MeasureString(printStr, useFont);
                    }
                }

                int boardOffset = (imageLen - (squareLen * dimension)) / 2;

                int fillColorOffset = penPixelWidth - 1;

                for (int col = 0; col < dimension; col++)
                {

                    for (int row = 0; row < dimension; row++)
                    {

                        int squareX = boardOffset + row * (squareLen);
                        int squareY = boardOffset + col * (squareLen);
                        Rectangle squareEdge = new Rectangle(squareX, squareY, squareLen, squareLen);

                        //to fill color avoid touching the edge               
                        //Rectangle squareFill = new Rectangle(squareX + fillColorOffset, squareY + fillColorOffset, squareLen - fillColorOffset, squareLen - fillColorOffset);
                        Rectangle squareFill = new Rectangle(squareX + fillColorOffset, squareY + fillColorOffset, squareLen - fillColorOffset - 1, squareLen - fillColorOffset - 1);
                        brGradient = new LinearGradientBrush(squareFill, upperRect, lowerRect, 45, false);
                        graphicsObj.DrawRectangle(linePen, squareEdge);
                        int currentNum = arr[dimension * col + row];
                        if (currentNum != emptyNum)
                        {
                            graphicsObj.FillRectangle(brGradient, squareFill);
                            drawFormat = new StringFormat();
                            drawFormat.Alignment = StringAlignment.Center;
                            drawFormat.LineAlignment = StringAlignment.Center;
                            string printStr = currentNum.ToString();
                            graphicsObj.DrawString(printStr,
                              useFont, fontColor, squareEdge, drawFormat);

                        }

                    }

                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                graphicsObj.Dispose();
                linePen.Dispose();
                if (brGradient != null)
                    brGradient.Dispose();
                if (drawFormat != null)
                    drawFormat.Dispose();
                if (useFont != null)
                    useFont.Dispose();
            }

            return myBitmap;
        }

        /*input-
         board: Position for cross, value for white pieces is 0, for offboard is -1, for available is 1
         shunter: x, y position for shunter
         crosspos: array of x,y position for cross
         rowLength: width of the board
         colLength: height of the board
         imageWidth: width of output image
         imageHeight: height of output image     

         output- bitmap image that represents the shunter board configuration
         */
        private static Bitmap DrawShunterBoard(int[] board, Point shunter, Point[] crossPos, int rowLength, int colLength, int imageWidth, int imageHeight)
        {
            //===Configuration=============           
            //int dimension = (int)Math.Sqrt(crossPos.Length);
            //the thickness of the pen to draw grid            
            const int WHITE_NUM = 0;
            const int OFFBOARD = -1;
            const int AVAILABLE = 1;

            int penPixelWidth = 3;
            ///the number that represents the empty square
            int emptyNum = 0;
            Color lineColor = Color.Black;
            Brush fontColor = Brushes.Black;

            Color boardColor = Color.FromArgb(227, 176, 116);

            //Rectangle is a linear gradient, thus need two color to form it
            Color upperAvaiRect = Color.FromArgb(208, 216, 226);
            Color lowerAvaiRect = Color.FromArgb(180, 190, 204);

            Color upperCrossRect = Color.FromArgb(247, 157, 128);
            Color lowerCrossRect = Color.FromArgb(243, 0, 0);

            Color upperWhitePiece = Color.FromArgb(249, 255, 253);
            Color lowerWhitePiece = Color.FromArgb(156, 156, 156);

            Color upperBlackPiece = Color.FromArgb(200, 200, 200);
            Color lowerBlackPiece = Color.FromArgb(22, 11, 9);

            string cross = "X";
            //===Configuration=============           
            //Drawing on bitmap
            Bitmap myBitmap = new Bitmap(imageWidth, imageHeight,
                 System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics graphicsObj = Graphics.FromImage(myBitmap);
            graphicsObj.SmoothingMode = SmoothingMode.AntiAlias;
            Pen linePen = new Pen(lineColor, penPixelWidth);
            Brush brAvaiGradient = null;
            Brush brCrossGradient = null;
            StringFormat drawFormat = null;
            Font useFont = null;
            //for circle drawing
            GraphicsPath path = null;
            PathGradientBrush pthGrBrush = null;
            try
            {

                graphicsObj.Clear(boardColor);
                //Length of each square on the board
                //int squareLen = (imageLen - 2 * penPixelWidth) / dimension;
                int rectWidth = (imageWidth - 2 * penPixelWidth) / rowLength;
                int rectHeight = (imageHeight - 2 * penPixelWidth) / colLength;


                //determine font size
                int fontSize = Math.Min(rectHeight, rectWidth) / 2;
                useFont = new Font("Arial", fontSize);

                //for (int i = 0; i < crossPos.Length; i++)
                //{
                string printStr = cross;
                SizeF size = graphicsObj.MeasureString(printStr, useFont);
                while (size.Height >= rectHeight || size.Width >= rectWidth)
                {
                    fontSize -= 5;
                    useFont = new Font("Arial", fontSize);
                    size = graphicsObj.MeasureString(printStr, useFont);
                }
                //}

                int boardWidthOffset = (imageWidth - (rectWidth * rowLength)) / 2;
                int boardHeightOffset = (imageHeight - (rectHeight * colLength)) / 2;

                int fillRectOffset = penPixelWidth - 1;

                int circleDimension = Math.Min(rectWidth, rectHeight) - 5;
                int fillEllipseWidthOffset = (rectWidth - circleDimension) / 2 + 1;
                int fillEllipseHeightOffset = (rectHeight - circleDimension) / 2 + 1;

                for (int col = 0; col < colLength; col++)
                {

                    for (int row = 0; row < rowLength; row++)
                    {

                        int rectX = boardWidthOffset + row * (rectWidth);
                        int rectY = boardHeightOffset + col * (rectHeight);
                        Rectangle squareEdge = new Rectangle(rectX, rectY, rectWidth, rectHeight);

                        //the offset is to fill the color without touching the edge           
                        Rectangle rectFill = new Rectangle(rectX + fillRectOffset, rectY + fillRectOffset, rectWidth - fillRectOffset - 1, rectHeight - fillRectOffset - 1);
                        brAvaiGradient = new LinearGradientBrush(rectFill, upperAvaiRect, lowerAvaiRect, 45, false);
                        brCrossGradient = new LinearGradientBrush(rectFill, upperCrossRect, lowerCrossRect, 45, false);
                        graphicsObj.DrawRectangle(linePen, squareEdge);
                        int currIndex = rowLength * col + row;
                        bool isCross = false;
                        foreach (Point p in crossPos)
                        {
                            if (p.X == row && p.Y == col)
                            {
                                isCross = true;
                                break;
                            }
                        }
                        bool isWhitePiece = board[currIndex] == WHITE_NUM;
                        bool isShunter = (shunter.X == row && shunter.Y == col);
                        bool isOffBoard = board[currIndex] == OFFBOARD;
                        //bool isAvailable = board[currIndex] == AVAILABLE;

                        //blue color for avai position
                        if (!isOffBoard) graphicsObj.FillRectangle(brAvaiGradient, rectFill);
                        //red color for cross
                        if (isCross) graphicsObj.FillRectangle(brCrossGradient, rectFill);
                        //draw cross only when no pieces on it
                        if (isCross && !isWhitePiece && !isShunter)
                        {
                            drawFormat = new StringFormat();
                            drawFormat.Alignment = StringAlignment.Center;
                            drawFormat.LineAlignment = StringAlignment.Center;
                            graphicsObj.DrawString(cross,
                              useFont, fontColor, squareEdge, drawFormat);
                        }


                        //====Drawing Circle===
                        if (isWhitePiece || isShunter)
                        {

                            Rectangle EllipseFill = new Rectangle(rectX + fillEllipseWidthOffset, rectY + fillEllipseHeightOffset, circleDimension, circleDimension);

                            path = new GraphicsPath();

                            path.AddEllipse(EllipseFill);
                            pthGrBrush = new PathGradientBrush(path);

                            pthGrBrush.CenterPoint = new PointF(rectX + fillEllipseWidthOffset, rectY + fillEllipseHeightOffset);
                            Color[] colorSet = new Color[1];

                            if (isWhitePiece)
                            {
                                pthGrBrush.CenterColor = upperWhitePiece;
                                colorSet[0] = lowerWhitePiece;
                            }
                            else
                            {
                                pthGrBrush.CenterColor = upperBlackPiece;
                                colorSet[0] = lowerBlackPiece;
                            }

                            pthGrBrush.SurroundColors = colorSet;
                            graphicsObj.FillEllipse(pthGrBrush, EllipseFill);
                        }
                        //====Drawing Circle end===

                    }

                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                graphicsObj.Dispose();
                linePen.Dispose();
                if (brAvaiGradient != null)
                    brAvaiGradient.Dispose();
                if (brCrossGradient != null)
                    brCrossGradient.Dispose();
                if (drawFormat != null)
                    drawFormat.Dispose();
                if (useFont != null)
                    useFont.Dispose();
                if (path != null)
                    path.Dispose();
                if (pthGrBrush != null)
                    pthGrBrush.Dispose();
            }

            return myBitmap;
        }

        private static Bitmap DrawDiningPhilosopher(int imageSize, PhilosopherStatus[] philosopherStatuses)
        {
            Bitmap myBitmap = new Bitmap(imageSize, imageSize, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics graphic = Graphics.FromImage(myBitmap);
            //fill background color
            graphic.FillRectangle(Brushes.White, 0, 0, imageSize, imageSize);

            graphic.TranslateTransform(imageSize / 2, imageSize / 2);
            int numberOfPhilosopher = philosopherStatuses.Length;
            int R = 100;

            for (int i = 0; i < numberOfPhilosopher; i++)
            {
                float x = (float)(R * Math.Cos(Math.PI * 2 / numberOfPhilosopher * i));
                float y = (float)(-R * Math.Sin(Math.PI * 2 / numberOfPhilosopher * i));
                int degree = 90 - 360 / numberOfPhilosopher * i;

                graphic.TranslateTransform(x, y);
                graphic.RotateTransform(degree);
                
                //
                float ratio = 0;
                int penWidth = 0;
                if (numberOfPhilosopher <= 8)
                {
                    ratio = 0.4F;
                    penWidth = 3;
                }
                else if (numberOfPhilosopher <= 16)
                {
                    ratio = 0.2F;
                    penWidth = 2;
                }
                else
                {
                    ratio = 0.1F;
                    penWidth = 1;
                }
                Brush brush;
                if (philosopherStatuses[i] == PhilosopherStatus.Thinking)
                {
                    brush = Brushes.Blue;
                }
                else if (philosopherStatuses[i] == PhilosopherStatus.HaveLeftFork)
                {
                    brush = Brushes.Red;
                }
                else
                {
                    brush = Brushes.Green;
                }
                DrawPhylosopher(graphic, new PointF(0, 0), brush, ratio);
                
                //turn back original 
                graphic.RotateTransform(-degree);
                graphic.TranslateTransform(-x, -y);
                
                //distance of philosopher to the fork
                float distance = (float)(60 / (Math.Ceiling((double)numberOfPhilosopher / 16)));

                //Draw the philosopher's forks
                if (philosopherStatuses[i] == PhilosopherStatus.HaveLeftFork || philosopherStatuses[i] == PhilosopherStatus.Eating)
                {
                    //the left fork angle is Math.PI / numberOfPhilosopher/4 less than the philosopher
                    x = (float)((R + distance) * Math.Cos(Math.PI * 2 / numberOfPhilosopher * i - Math.PI / numberOfPhilosopher / 4));
                    y = (float)(-(R + distance) * Math.Sin(Math.PI * 2 / numberOfPhilosopher * i - Math.PI / numberOfPhilosopher / 4));
                    degree = 90 - (360 / numberOfPhilosopher * i - 180 / numberOfPhilosopher / 4);

                    graphic.TranslateTransform(x, y);
                    graphic.RotateTransform(degree);

                    DrawFork(graphic, new PointF(0, 0), new Pen(Color.Black, penWidth), ratio);

                    //turn back original 
                    graphic.RotateTransform(-degree);
                    graphic.TranslateTransform(-x, -y);

                    //draw the right fork if eating
                    if (philosopherStatuses[i] == PhilosopherStatus.Eating)
                    {
                        //the right fork angle is Math.PI / numberOfPhilosopher /4 greater than the philosopher
                        x = (float)((R + distance) * Math.Cos(Math.PI * 2 / numberOfPhilosopher * i + Math.PI / numberOfPhilosopher / 4));
                        y = (float)(-(R + distance) * Math.Sin(Math.PI * 2 / numberOfPhilosopher * i + Math.PI / numberOfPhilosopher / 4));
                        degree = 90 - (360 / numberOfPhilosopher * i + 180 / numberOfPhilosopher / 4);

                        graphic.TranslateTransform(x, y);
                        graphic.RotateTransform(degree);

                        DrawFork(graphic, new PointF(0, 0), new Pen(Color.Black, penWidth), ratio);

                        //turn back original 
                        graphic.RotateTransform(-degree);
                        graphic.TranslateTransform(-x, -y);
                    }
                }
                else if (philosopherStatuses[i] != PhilosopherStatus.HaveLeftFork && philosopherStatuses[(i - 1 + numberOfPhilosopher) % numberOfPhilosopher] != PhilosopherStatus.Eating)
                {
                    //the left fork angle is Math.PI / numberOfPhilosopher less than the philosopher
                    x = (float)(R * Math.Cos(Math.PI * 2 / numberOfPhilosopher * i - Math.PI / numberOfPhilosopher));
                    y = (float)(-R * Math.Sin(Math.PI * 2 / numberOfPhilosopher * i - Math.PI / numberOfPhilosopher));
                    degree = 90 - (360 / numberOfPhilosopher * i - 180 / numberOfPhilosopher);

                    graphic.TranslateTransform(x, y);
                    graphic.RotateTransform(degree);
                    //
                    DrawFork(graphic, new PointF(0, 0), new Pen(Color.Black, penWidth), ratio);

                    //turn back original 
                    graphic.RotateTransform(-degree);
                    graphic.TranslateTransform(-x, -y);
                }

            }
            graphic.Dispose();

            return myBitmap;
        }

        private static void DrawPhylosopher(Graphics graphic, PointF headCenter, Brush brush, float ratio)
        {
            float headDiameter = 30 * ratio;
            float headDegree = 30;
            float headAngle = (float)(headDegree * Math.PI / 180);

            float handWidth = 15 * ratio;
            float handLength = 75 * ratio;
            float handDegree = 15;
            float handAngle = (float)(handDegree * Math.PI / 180);

            float bodyLength = 50 * ratio;

            RectangleF headRectangle = new RectangleF(headCenter.X - headDiameter, headCenter.Y - headDiameter, headDiameter * 2, headDiameter * 2);
            GraphicsPath graphicPath = new GraphicsPath();

            graphicPath.AddArc(headRectangle, 90 + headDegree, 360 - 2 * headDegree);


            PointF leftHeadBase = new PointF((float)(headCenter.X - headDiameter * Math.Sin(headAngle)),
                                                (float)(headCenter.Y + headDiameter * Math.Cos(headAngle)));

            PointF rightHeadBase = new PointF((float)(headCenter.X + headDiameter * Math.Sin(headAngle)),
                                                (float)(headCenter.Y + headDiameter * Math.Cos(headAngle)));

            PointF leftHand1 = new PointF(leftHeadBase.X - handWidth, leftHeadBase.Y);
            PointF leftHand2 = new PointF((float)(leftHand1.X - handLength * Math.Sin(handAngle)),
                                                (float)(leftHand1.Y + handLength * Math.Cos(handAngle)));
            PointF leftHand3 = new PointF(leftHand2.X + handWidth, leftHand2.Y);
            PointF leftHand4 = new PointF(leftHand1.X, (float)(leftHand3.Y - (leftHand1.X - leftHand3.X) / Math.Tan(handAngle)));
            PointF rightHand1 = new PointF(rightHeadBase.X + handWidth, rightHeadBase.Y);
            PointF rightHand2 = new PointF((float)(rightHand1.X + handLength * Math.Sin(handAngle)),
                                                (float)(rightHand1.Y + handLength * Math.Cos(handAngle)));
            PointF rightHand3 = new PointF(rightHand2.X - handWidth, rightHand2.Y);
            PointF rightHand4 = new PointF(rightHand1.X, (float)(rightHand3.Y - (rightHand3.X - rightHand1.X) / Math.Tan(handAngle)));

            PointF body1 = new PointF(leftHand4.X, leftHand4.Y + bodyLength);
            PointF body2 = new PointF(rightHand4.X, rightHand4.Y + bodyLength);

            graphicPath.AddLine(leftHeadBase, leftHand1);
            graphicPath.AddLine(leftHand1, leftHand2);
            graphicPath.AddLine(leftHand2, leftHand3);
            graphicPath.AddLine(leftHand3, leftHand4);

            graphicPath.AddLine(leftHand4, body1);
            graphicPath.AddLine(body1, body2);
            graphicPath.AddLine(body2, rightHand4);

            graphicPath.AddLine(rightHand4, rightHand3);
            graphicPath.AddLine(rightHand3, rightHand2);
            graphicPath.AddLine(rightHand2, rightHand1);
            graphicPath.AddLine(rightHand1, rightHeadBase);

            graphic.FillPath(brush, graphicPath);
        }

        private static void DrawFork(Graphics graphic, PointF body1, Pen pen, float ratio)
        {
            float forkBodyLength = 60 * ratio;
            float forkSpikeLength = 40 * ratio;
            float forkSpikeAngle = (float)(20 * Math.PI / 180);

            PointF body2 = new PointF(body1.X, body1.Y + forkBodyLength);
            PointF spike1 = new PointF(body2.X - (float)Math.Sin(forkSpikeAngle) * forkSpikeLength,
                                        body2.Y + (float)Math.Cos(forkSpikeAngle) * forkSpikeLength);
            PointF spike2 = new PointF(body2.X, body2.Y + forkSpikeLength);
            PointF spike3 = new PointF(body2.X + (float)Math.Sin(forkSpikeAngle) * forkSpikeLength,
                                        body2.Y + (float)Math.Cos(forkSpikeAngle) * forkSpikeLength);
            graphic.DrawLine(pen, body1, body2);
            graphic.DrawLine(pen, body2, spike1);
            graphic.DrawLine(pen, body2, spike2);
            graphic.DrawLine(pen, body2, spike3);
        }

    }
}
