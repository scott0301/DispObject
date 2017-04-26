using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace DispObject
{

    public delegate void deleDisplayImage(byte [] rawImage, int imageW, int imageH);
    public delegate void deleDisplayImageBitmap(Bitmap bmp);

    public class CPolygon
    {
        public static double GetPolygonArea(List<PointF> list)
        {
            return Math.Abs(list.Take(list.Count - 1).Select((p, index) => (p.X * list[index + 1].Y) - (p.Y * list[index + 1].X)).Sum() / 2);;
        }
    }

    public class CLine
    {
        float x1 = 0;
        float y1 = 0;
        float x2 = 0;
        float y2 = 0;

        public CLine() { x1 = y1 = x2 = y2 = 0; }
        public CLine(PointF p1, PointF p2) { this.x1 = p1.X; this.y1 = p1.Y; this.x2 = p2.X; this.y2 = p2.Y; }
        public CLine(double x1, double y1, double x2, double y2) { this.x1 = (float)x1; this.y1 = (float)y1; this.x2 = (float)x2; this.y2 = (float)y2; }
     

        public void SetLine(float x1, float y1, float x2, float y2) { this.x1 = x1; this.y1 = y1; this.x2 = x2; this.y2 = y2; }
        public void SetLine(PointF p1, PointF p2) { this.x1 = p1.X; this.y1 = p1.Y; this.x2 = p2.X; this.y2 = p2.Y; }
        public void OffsetLine(double x, double y) { this.x1 += (float)x; this.y1 += (float)y; this.x2 += (float)x; this.y2 += (float)y; }
        public void OffsetLine(PointF pt) { OffsetLine(pt.X, pt.Y); }
        public void OffsetLine(RectangleF rt) { x1 += rt.X; y1 += rt.Y; x2 += rt.X; y2 += rt.Y; }

        public PointF P1 { get { return new PointF(x1, y1); } set { x1 = value.X; y1 = value.Y; } }
        public PointF P2 { get { return new PointF(x2, y2); } set { x2 = value.X; y2 = value.Y; } }
        public PointF CENTER { get { return new PointF((float)((x1 + x2) / 2.0), (float)((y1 + y2) / 2.0)); ;} }
        public double LENGTH { get { return Math.Sqrt(Math.Pow((x2 - x1), 2.0) + Math.Pow((y2 - y1), 2.0)); } }
        public double DegreeToRadians(double degree) { return degree * Math.PI / 180.0; }
        public double RadiansToDegree(double radians) { return radians * 180.0 / Math.PI; }

        public double GetLineLength(PointF pt1, PointF pt2) { return Math.Sqrt(Math.Pow((pt2.X - pt1.X), 2.0) + Math.Pow((pt2.Y - pt1.Y), 2.0)); }
        public static double GetPointsLength(PointF pt1, PointF pt2) { return Math.Sqrt(Math.Pow((pt2.X - pt1.X), 2.0) + Math.Pow((pt2.Y - pt1.Y), 2.0)); }
        public double GetSlantofLine() { return Math.Atan((y2 - y1) / (x2 - x1)); }

        public CLine CopyTo() { return new CLine(x1, y1, x2, y2); }
        /// <summary>
        /// line을 offset 만큼 수평, 수직 이동시킨다.
        /// </summary>
        /// <param name="offsetX">x 축 이동량</param>
        /// <param name="offsetY">y 축 이동량</param>
        public CLine ShiftLine(int offsetX, int offsetY)
        {
            x1 += offsetX;
            y1 += offsetY;
            x2 += offsetX;
            y2 += offsetY;

            CLine line = new CLine(x1, y1, x2, y2);
            return line;
        }
        
        public double GetLineAngle()
        {
            double fx = x2 - x1;
            double fy = y2 - y1;
            double fd = this.LENGTH;
            double ft = RadiansToDegree(Math.Acos(fx / fd));

            double fAngle = 0;
            if (fy < 0) fAngle = ft * -1;
            else fAngle = ft;

            // if(  fAngle < 0 ) // - angle compensation for + angle 
            // {
            //     // - 0. 5 -> 359.5
            //     //  -83  -> 277
            //     fAngle = 360 + fAngle;
            // }

            return fAngle;
        }

        public double GetLineAngle(PointF pt1, PointF pt2)
        {
            double fx = pt2.X - pt1.X;
            double fy = pt2.Y - pt1.Y;
            double fd = this.LENGTH;
            double ft = RadiansToDegree(Math.Acos(fx / fd));

            double fAngle = 0;
            if (fy < 0) fAngle = ft * -1;
            else fAngle = ft;

            //if (fAngle < 0) // - angle compensation for + angle 
            //{
            //    fAngle = 360 + fAngle;
            //}
            return fAngle;
        }
        /// <summary>
        ///  line의 각도를 구한다.
        ///  line의 시작 점과 끝점을 입력받는다. Manual 계산 버전
        /// </summary>
        /// <param name="ptStart">line의 시작점</param>
        /// <param name="ptEnd">line의 끝점</param>
        /// <returns></returns>
        public double GetLineAngle2(PointF ptStart, PointF ptEnd)
        {
            // quadrant 2 | quadrant 1
            // quadrant 3 | quadrant 4

            CLine line = new CLine(ptStart.X, ptStart.Y, ptEnd.X, ptEnd.Y);

            double m = GetSlantofLine();

            double fAngle = RadiansToDegree(m);

            //*******************************************************
            // overapped line
            //*******************************************************
            /***/if (ptStart.X < ptEnd.X && ptStart.Y == ptEnd.Y) fAngle = 0;
            else if (ptStart.X == ptEnd.X && ptStart.Y < ptEnd.Y) fAngle = 90;
            else if (ptStart.X == ptEnd.X && ptStart.Y > ptEnd.Y) fAngle = -90;
            else if (ptStart.X > ptEnd.X && ptStart.Y == ptEnd.Y) fAngle = -180;

            // quadrant 1
            else if (ptStart.X < ptEnd.X && ptStart.Y > ptEnd.Y) { } // quadrant 1 : minus coordinates
            // quadrant 2
            else if (ptStart.X > ptEnd.X && ptStart.Y > ptEnd.Y) { fAngle = (90 + (90 - fAngle)) * -1; }
            // quadrant 3
            else if (ptStart.X > ptEnd.X && ptStart.Y < ptEnd.Y) { fAngle = 90 + (90 + fAngle); }
            // quadrant 4
            else if (ptStart.X < ptEnd.X && ptStart.Y < ptEnd.Y) { } // quadrant 4 : plus coordinates

            if (fAngle < 0) // - angle compensation for + angle 
            {
                fAngle = 360 + fAngle;
            }
            return fAngle;
        }
        /// <summary>
        /// 임의의 두 점을 선으로 간주하고, 해당 선분에서 x 좌표에 따른 lying 좌표를 생성한다.
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static PointF GetLyingPointBy_X(PointF pt1, PointF pt2, double x) // 161215 
        {
            double diffX = pt2.X - pt1.X;
            double diffY = pt2.Y - pt1.Y;

            double m = diffY / diffX;

            double y = (m * x) - (m * pt2.X) + pt2.Y;
            return new PointF((float)x, (float)y);
        }
        public static PointF GetLyingPointBy_Y(PointF pt1, PointF pt2, double y) // 161215
        {
            double diffX = pt2.X - pt1.X;
            double diffY = pt2.Y - pt1.Y;

            if (diffX == 0) diffX = 0.1; // to avoid zero slope
            double m = diffY / diffX;

            double x = ((m * pt2.X) - pt2.Y + y) / m;
            return new PointF((float)x, (float)y);
        }
        /// <summary>
        /// 주어진 임의의 두 점을 라인으로 간주하고 slope을 구한다 y = mx + b --> m
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double GetSlope(PointF p1, PointF p2)
        {
            double diffX = p2.X - p1.X;
            double diffY = p2.Y - p1.Y;

            if (diffX == 0) diffX = 0.1; // to avoid zero slope
            double m = diffY / diffX;
            return m;
        }
        /// <summary>
        /// 주어진 라인으로부터 slope을 구한다. y = mx + b --> m
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static double GetSlope(CLine line)
        {
            return GetSlope(line.P1, line.P2);
        }
        /// <summary>
        /// 주어진 두 점으로부터 intercept를 구한다 y = mx + b --> b
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double GetIntercept(PointF p1, PointF p2)
        {
            double m = GetSlope(p1, p2);

            double x1 = p1.X;
            double y1 = p1.Y;

            double b = y1 - (x1 * m);
            return b;
        }
        /// <summary>
        /// 임의의 라인에서 intercept를 받는다. y = mx + b
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static double GetIntercept(CLine line)
        {
            return GetIntercept(line.P1, line.P2);
        }
        /// <summary>
        /// slope이 같으면 같은 평행이다.
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <returns></returns>
        public static bool isParallelLine(CLine l1, CLine l2)
        {
            double slope1 = GetSlope(l1);
            double slope2 = GetSlope(l2);

            if (Math.Abs(slope1 - slope2) <= 0.1) return true;
            return false;
        }
        /// <summary>
        /// 임의의 라인으로부터 인접하지 않은 좌표를 제거한 후 유효 좌표를 리턴한다.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<PointF> GetFilteredFitLineFromVariationY(CLine line, List<PointF> list)
        {
            List<PointF> listBaseLine = GetLyingPointsFromVariationY(line);

            List<PointF> listPointsFiltered = new List<PointF>();

            double[] fArrDistance = new double[listBaseLine.Count];

            for (int candidate = 0; candidate < list.Count; candidate++)
            {
                PointF ptCandidate = list.ElementAt(candidate);

                Array.Clear(fArrDistance, 0, fArrDistance.Length);

                for (int baseTarget = 0; baseTarget < listBaseLine.Count; baseTarget++)
                {
                    PointF ptBase = listBaseLine.ElementAt(baseTarget);
                    fArrDistance[baseTarget] = CPoint.GetDistance(ptBase, ptCandidate);
                }

                if (fArrDistance.Min() < 5)
                {
                    listPointsFiltered.Add(ptCandidate);
                }
            }
            return listPointsFiltered;
        }
        /// <summary>
        /// 주어진 라인의 세로값을 기준으로 X좌표생성해서 전체 포인트리스트 생성
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static List<PointF> GetLyingPointsFromVariationY(CLine line)
        {
            List<PointF> listBaseLine = new List<PointF>();

            int max = (int)Math.Max(line.P1.Y, line.P2.Y);
            int min = (int)Math.Min(line.P1.Y, line.P2.Y);

            for (int i = min; i < max; i++)
            {
                listBaseLine.Add(CLine.GetLyingPointBy_Y(line.P1, line.P2, i));
            }
            return listBaseLine;
        }
        public static List<PointF> GetLyingPointsFromVariationX(CLine line)
        {
            List<PointF> listBaseLine = new List<PointF>();

            int max = (int)Math.Max(line.P1.X, line.P2.X);
            int min = (int)Math.Min(line.P1.X, line.P2.X);

            for (int i = min; i < max; i++)
            {
                listBaseLine.Add(CLine.GetLyingPointBy_X(line.P1, line.P2, i));
            }
            return listBaseLine;
        }
        /// <summary>
        /// 주어진 두 점을 라인으로 처리하여, 세로값을 기준으로 X좌표 생성해서 포인트 리스트 생성
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static List<PointF> GetLyingPointsFromVariationY(PointF p1, PointF p2) {return GetLyingPointsFromVariationY(new CLine(p1, p2));}
        public static List<PointF> GetLyingPointsFromVariationX(PointF p1, PointF p2){return GetLyingPointsFromVariationX(new CLine(p1, p2));}

        /// <summary>
        /// 그냥 수평선 기준 기본 fitting
        /// </summary>
        /// <param name="listPoints"></param>
        /// <returns></returns>
        public static CLine GetFitLineHOR(List<PointF> listPoints)
        {
            CLine line = new CLine();

            if (listPoints.Count != 0)
            {
                float[] objX = listPoints.Select(element => element.X).ToArray();
                float[] objY = listPoints.Select(element => element.Y).ToArray();

                double meanX = objX.Average();
                double meanY = objY.Average();

                double numer = 0;
                double denom = 0;

                for (int i = 0; i < listPoints.Count; i++)
                {
                    numer += (objX[i] - meanX) * (objY[i] - meanY);
                    denom += (objX[i] - meanX) * (objX[i] - meanX);
                }

                double m = numer / denom;

                if (m == 0) m = 0.001;
                double b = meanY - (m * meanX);

                float fMinX = objX.Min();
                float fMaxX = objX.Max();

                line.P1 = new PointF((float)fMinX, (float)(m * fMinX + b));
                line.P2 = new PointF((float)fMaxX, (float)(m * fMaxX + b));
            }

            return line;
        }

        /// <summary>
        /// 주어진 좌표 리스트를 전체 회전하여 라인 생성
        /// 수직이므로 임의의 좌표를 중심으로 회전하고,  최소y, 최대x 기준으로 값을 생성 
        /// Horizontal Fitting 기준임--> 90도 rotation --> 수평선으로 변환 --> fitting --> 수직선으로 원복
        /// </summary>
        /// <param name="listPoints"></param>
        /// <returns></returns>
        public static CLine GetFitLineVER(List<PointF> listPoints)
        {
            CLine line = new CLine();

            // arbitrary gravity
            double w = 3000.0;
            double h = 3000.0;

            if (listPoints.Count != 0)
            {
                List<PointF> ptTemp = new List<PointF>();

                float[] arrOrgX = listPoints.Select(element => element.X).ToArray();
                float[] arrOrgY = listPoints.Select(element => element.Y).ToArray();

                float orgMinX = arrOrgX.Min();
                float orgMaxX = arrOrgX.Max();
                float orgMinY = arrOrgY.Min();
                float orgMaxY = arrOrgY.Max();
                // rotate points 
                ptTemp = HC_EDGE_GetRotatedPoints(listPoints, 90.0, w, h);

                float[] objX = ptTemp.Select(element => element.X).ToArray();
                float[] objY = ptTemp.Select(element => element.Y).ToArray();

                double meanX = objX.Average();
                double meanY = objY.Average();

                double numer = 0;
                double denom = 0;

                for (int i = 0; i < ptTemp.Count; i++)
                {
                    numer += (objX[i] - meanX) * (objY[i] - meanY);
                    denom += (objX[i] - meanX) * (objX[i] - meanX);
                }

                double a = numer / denom;

                if (a == 0) a = 0.001;

                double b = meanY - (a * meanX);

                float fMinX = objX.Min();
                float fMaxX = objX.Max();


                double y1 = a * fMinX + b;
                double x1 = (y1 - b) / a;
                double y2 = a * fMaxX + b;
                double x2 = (y2 - b) / a;

                line.P1 = new PointF((float)x1, (float)y1);
                line.P2 = new PointF((float)x2, (float)y2);
            }
            // recover angle
            return line.RotateLinebyPoints(-90.0, w / 2, h / 2);

        }
        /// <summary>
        /// 포인트 리스트들을 특정 각도로 돌리고, 돌릴 때 중심은 영역 기준 센터(x,y) 임의의 좌표를 넣고 돌려 도 됨. 동일한 기준만 된다면.
        /// </summary>
        /// <param name="listPoints"></param>
        /// <param name="fAngle"></param>
        /// <param name="w">영상전체 회전 기준좌표라 영상 너비를 넣으면 센터기준 회전</param>
        /// <param name="h">영상전제 회전 기준좌표라 영상 높이를 넣으면 센터기준 회전</param>
        /// <returns></returns>
        public static List<PointF> HC_EDGE_GetRotatedPoints(List<PointF> listPoints, double fAngle, double w, double h)
        {
            List<PointF> ptRotated = new List<PointF>();

            double fDegree = fAngle * Math.PI / 180.0;

            for (int i = 0; i < listPoints.Count; i++)
            {
                PointF pt = _GetRotatePos(listPoints.ElementAt(i).X, listPoints.ElementAt(i).Y, fDegree, w / 2, h / 2);
                ptRotated.Add(pt);
            }
            return ptRotated;
        }

        /// <summary>
        /// 특정 점을 특정 각도로 돌리자 : 버티칼 라인피팅할때 참조용
        /// </summary>
        /// <param name="X">input croodinate X</param>
        /// <param name="Y">input croodinate Y</param>
        /// <param name="fAngle">돌릴 앵글</param>
        /// <param name="fOffsetX"> rotation gravity X</param>
        /// <param name="fOffsetY"> rotation gravity Y</param>
        /// <returns></returns>
        public static PointF _GetRotatePos(double X, double Y, double fAngle, double fOffsetX, double fOffsetY)
        {
            double rotX = ((X - fOffsetX) * Math.Cos(fAngle)) - ((Y - fOffsetY) * Math.Sin(fAngle)) + fOffsetX;
            double rotY = ((X - fOffsetY) * Math.Sin(fAngle)) + ((Y - fOffsetY) * Math.Cos(fAngle)) + fOffsetY;

            return new PointF((float)rotX, (float)rotY);
        }
        public double GetLineAngle(CLine line) { return GetLineAngle(line.P1, line.P2); }
        public double GetLineAngle2(CLine line) { return GetLineAngle2(line.P1, line.P2); }
        /// <summary>
        /// Line을 기준으로 주어진 Point와 가까운 라인의 Point를 반환한다.
        /// </summary>
        /// <param name="pt">비교할 Point</param>
        /// <returns></returns>
        public PointF GetClosePointOfLine(PointF pt)
        {
            double fDistStart = GetLineLength(P1, pt);
            double fDistEnd = GetLineLength(P2, pt);

            if (fDistStart < fDistEnd) return P1;
            else if (fDistStart > fDistEnd) return P2;
            else return P1;
        }
        public PointF GetIntersectPointOfInfiniteLines(CLine line) { return GetIntersectPointOfInfiniteLines(line.P1, line.P2); }
        /// <summary>
        /// 내자신(line)과 입력받은 Line(시작점, 끝점)사이에서 만나는 점을 구한다.
        /// 선분이 교차하지 않을 경우 무한한 점에서 만나는 지점을 구한다.
        /// </summary>
        /// <param name="pt1">비교 대상 line의 시작점</param>
        /// <param name="pt2">비교 대상 line의 끝점</param>
        /// <returns></returns>
        /// 
        public PointF GetIntersectPointOfInfiniteLines(PointF pt1, PointF pt2)
        {
            double fDx1 = x2 - x1;
            double fDy1 = y2 - y1;

            double fDx2 = pt2.X - pt1.X;
            double fDy2 = pt2.Y - pt1.Y;

            double b_dot_d_perp = fDx1 * fDy2 - fDy1 * fDx2;


            PointF ptIntersect = new PointF(0, 0);

            if (b_dot_d_perp == 0)
            {
                return ptIntersect;
            }

            double cx = pt1.X - x1;
            double cy = pt1.Y - y1;

            double t = (cx * fDy2 - cy * fDx2) / b_dot_d_perp;

            ptIntersect.X = (float)(x1 + t * fDx1);
            ptIntersect.Y = (float)(y1 + t * fDy1);

            return ptIntersect;
        }
        /// <summary>
        /// 내 자신(line)과 입력한 선(시작점, 끝점)이 만나는 지점을 구한다.
        /// 실제 선분간의 교차점을 구하므로, 두 선분이 만나지 않을 경우 교차점이 나오지 않는다.
        /// </summary>
        /// <param name="pt1">비교 대상 line의 시작점</param>
        /// <param name="pt2">비교 대상 line의 끝 점</param>
        /// <returns></returns>
        public PointF GetIntersectionOfSegmentedLines(PointF pt1, PointF pt2)
        {
            double fUnderlying = (pt1.Y - pt2.Y) * (x2 - x1) - (pt2.X - pt1.X) * (y2 - y1);

            PointF ptIntersect = new PointF(0, 0);

            // never meet case
            if (fUnderlying == 0) return ptIntersect;

            double t = (pt2.X - pt1.X) * (y1 - pt1.Y) - (pt2.Y - pt1.Y) * (x1 - pt1.X);
            double s = (x2 - x1) * (y1 - pt1.Y) - (y2 - y1) * (x1 - pt1.X);

            t = t / fUnderlying;
            s = s / fUnderlying;

            if (t < 0.0 || t > 1.0 || s < 0.0 || s > 1.0) return ptIntersect;
            if (t == 0.0 && s == 0.0) return ptIntersect;

            ptIntersect.X = (float)(x1 + t * (x2 - x1));
            ptIntersect.Y = (float)(y1 + t * (y2 - y1));

            return ptIntersect;
        }

        public CLine GetExpandedLine_CrossLineBased(CLine baseline, PointF h1, PointF h2, PointF t1, PointF t2)
        {
            PointF ptHead = GetIntersectPointOfInfiniteLines(h1, h2);
            PointF ptTail = GetIntersectPointOfInfiniteLines(t1, t2);

            CLine line = CopyTo();
            line.P1 = ptHead;
            line.P2 = ptTail;
            return line;
        }
        /// <summary>
        /// 내자신(line)을 line center를 기준으로 입력받은 Angle만큼 회전시킨다.
        /// </summary>
        /// <param name="fAngle">회전할 Angle</param>
        /// <returns></returns>
        public CLine RotateLineByCenter(double fAngle)
        {
            CLine line = CopyTo();
            PointF ptRotatedHead = CPoint.RotatePointByGravity(P1, CENTER, fAngle);
            PointF ptRotatedTail = CPoint.RotatePointByGravity(P2, CENTER, fAngle);
            line.SetLine(ptRotatedHead, ptRotatedTail);
            return line;
        }
        public CLine RotateLinebyPoints(double fAngle, double x, double y)
        {
            double fDegree = fAngle * Math.PI / 180.0;
            double rotX1 = ((this.x1 - x) * Math.Cos(fDegree) - (this.y1 - y) * Math.Sin(fDegree)) + x;
            double rotY1 = ((this.x1 - y) * Math.Sin(fDegree) + (this.y1 - y) * Math.Cos(fDegree)) + y;
            double rotX2 = ((this.x2 - x) * Math.Cos(fDegree) - (this.y2 - y) * Math.Sin(fDegree)) + x;
            double rotY2 = ((this.x2 - y) * Math.Sin(fDegree) + (this.y2 - y) * Math.Cos(fDegree)) + y;
            return new CLine(rotX1, rotY1, rotX2, rotY2);
        }

    }
    public class CPoint
    {
        public static PointF SetInvalid(PointF pt) 
        {
            pt.X = -1; pt.Y = -1;
            return pt;
        }
        public static PointF SetValue(PointF pt, double x, double y)
        {
            pt.X += (float)x;
            pt.Y += (float)y;
            return pt;
        }
        public static Point OffsetPoint(Point pt, double tx, double ty)
        {
            pt.X += (int)tx; pt.Y += (int)ty;
            return pt;
        }
        public static List<PointF> OffsetPoints(List<PointF> list, double tx, double ty)
        {
            int nCount = list.Count;

            PointF[] arrPoints = list.ToArray();
            
            Parallel.For(0, nCount, i =>
            {
                arrPoints[i].X += (float)tx; 
                arrPoints[i].Y += (float)ty;
            });

            return arrPoints.ToList();
        }
        public static List<PointF> GetListSampling(List<PointF> list , int nSampling)
        {
            double step = list.Count / (double)nSampling;

            List<PointF> listSampled = new List<PointF>();

            for (int i = 0; i < nSampling; i++ )
            {
                int nIndex = (int)Math.Floor(i * step);
                listSampled.Add(list.ElementAt(nIndex));
            }
            return listSampled;
        }
        public static PointF RotatePointByGravity(PointF ptTarget, PointF ptGravity, double fAngle)
        {
            //x' = (x-a) * cosR - (y-b)sinR + a
            //y' = (x-a) * sinR + (y-b)cosR + b


            fAngle = fAngle * Math.PI / 180.0;

            PointF ptRotated = new PointF(0, 0);

            ptRotated.X = (float)(((ptTarget.X - ptGravity.X) * Math.Cos(fAngle) - (ptTarget.Y - ptGravity.Y) * Math.Sin(fAngle)) + ptGravity.X);
            ptRotated.Y = (float)(((ptTarget.X - ptGravity.X) * Math.Sin(fAngle) + (ptTarget.Y - ptGravity.Y) * Math.Cos(fAngle)) + ptGravity.Y);

            return ptRotated;
        }
        public static List<PointF> RoatedPointsByCenter(List<PointF> points, double fAngle)
        {
            List<PointF> listNew = new List<PointF>();

            PointF pt = CPoint.GetCentroid(points);

            for (int i = 0; i < points.Count; i++)
            {
                PointF ptRotated = CPoint.RotatePointByGravity(points.ElementAt(i), pt, fAngle);
                listNew.Add(ptRotated);
            }
            return listNew;
        }

        public static List<PointF> RoatedPointsByGravity(List<PointF> points, PointF ptGravity, double fAngle)
        {
            List<PointF> listNew = new List<PointF>();

            for (int i = 0; i < points.Count; i++)
            {
                PointF ptRotated = CPoint.RotatePointByGravity(points.ElementAt(i), ptGravity, fAngle);
                listNew.Add(ptRotated);
            }
            return listNew;
        }
        
        public static PointF[] GetCrossPointsOfLine(PointF pt, float size)
        {
            PointF[] arrPoints = new PointF[4];
            arrPoints[0] = new PointF(pt.X - size, pt.Y);
            arrPoints[1] = new PointF(pt.X + size, pt.Y);
            arrPoints[2] = new PointF(pt.X, pt.Y - size);
            arrPoints[3] = new PointF(pt.X, pt.Y + size);
            return arrPoints;
        }

        public string ToStringX(PointF pt) 
        {
            return string.Format("X = " + pt.X.ToString("N0"));
        }
        public string ToStringY(PointF pt)
        {
            return string.Format("Y = " + pt.Y.ToString("N0"));
        }
        public string ToStringXY(PointF pt)
        {
            return string.Format(string.Format("X = {0}, Y = {1}", pt.X.ToString("N0"), pt.Y.ToString("N0")));
        }
        public static string ToString_XY(PointF pt)
        {
            return string.Format(string.Format("X = {0}, Y = {1}", pt.X.ToString("N0"), pt.Y.ToString("N0")));
        }


        public static PointF OffsetPoint(PointF pt, double tx, double ty) 
        {
            pt.X += (float)tx; pt.Y += (float)ty;
            return pt;
        }
        public static PointF OffsetPoint(PointF pt, RectangleF rc) 
        {
            pt.X += (float)rc.X; pt.Y += (float)rc.Y;
            return pt;
        }
        public static PointF OffsetPoint(PointF ptSrc, PointF ptTrans) 
        {
            ptSrc.X += (float)ptTrans.X; ptSrc.Y += (float)ptTrans.Y;
            return ptSrc;
        }
        public static PointF GetDistancePoint(PointF ptSrc, PointF ptTarget)
        {
            PointF ptDist = new PointF(ptSrc.X - ptTarget.X, ptSrc.Y - ptTarget.Y);
            return ptDist;  
        }
        public static double GetDistance(PointF ptSrc, PointF ptTarget) 
        { 
            return Math.Sqrt(((ptTarget.X - ptSrc.X)*(ptTarget.X - ptSrc.X)) + ((ptTarget.Y - ptSrc.Y)*(ptTarget.Y - ptSrc.Y))); ;
        }

        public static bool IsValid(PointF pt){if (pt.X < 0 && pt.Y < 0) return false; return true;}

        public static  List<PointF> GetMergedList(List<PointF> list1, List<PointF> list2)
        {
            List<PointF> listMerged = new List<PointF>();

            listMerged.AddRange(list1);
            listMerged.AddRange(list2);
            return listMerged;
        }

        public static Point ToPoint(PointF pt) { return new Point((int)pt.X, (int)pt.Y); }

        public static List<Point> ToPoints(List<PointF> ptList)
        {
            List<Point> listPoints = new List<Point>();

            for (int i = 0; i < ptList.Count; i++)
            {
                listPoints.Add( ToPoint(ptList.ElementAt(i)));
            }
            return listPoints;
        }
        public static void GetMinMax_X(List<PointF> list, ref float minX, ref float maxX)// 170110 
        {
            if (list.Count == 0) return;
            PointF[] arrPoints = list.ToArray();

            minX = arrPoints.Min(element => element.X);
            maxX = arrPoints.Max(element => element.X);
        }
        public static void GetMinMax_Y(List<PointF> list, ref float minY, ref float maxY)// 170110 
        {
            if (list.Count != 0)
            {
                PointF[] arrPoints = list.ToArray();

                minY = arrPoints.Min(element => element.Y);
                maxY = arrPoints.Max(element => element.Y);
            }
        }
        public static PointF GetPos_LT(List<PointF> list)// 170110 
        {
            float x = 0;float y = 0;

            if (list.Count != 0)
            {
                PointF[] arrPoints = list.ToArray();

                x = arrPoints.Min(element => element.X);
                y = arrPoints.Min(element => element.Y);
            }

            return new PointF(x, y);
        }
        public static PointF GetPos_RT(List<PointF> list)// 170110 
        {
            float x = 0;float y = 0;

            if( list.Count != 0)
            {
                PointF[] arrPoints = list.ToArray();

                x = arrPoints.Max(element => element.X);
                y = arrPoints.Min(element => element.Y);
            }

            return new PointF(x, y);
        }
        public static PointF GetPos_LB(List<PointF> list)// 170110 
        {
            float x = 0;float y = 0;

            if (list.Count != 0)
            {
                PointF[] arrPoints = list.ToArray();

                x = arrPoints.Min(element => element.X);
                y = arrPoints.Max(element => element.Y);
            }

            return new PointF(x, y);
        }
        public static PointF GetPos_RB(List<PointF> list)// 170110 
        {
            float x = 0; float y = 0;

            if (list.Count != 0)
            {
                PointF[] arrPoints = list.ToArray();
                x = arrPoints.Max(element => element.X);
                y = arrPoints.Max(element => element.Y);
            }

            return new PointF(x, y);
        }
        public static PointF GetPos_Center(List<PointF> list) // 170110 
        {
            float minX = 0; float maxX = 0;
            float minY = 0; float maxY = 0;

            CPoint.GetMinMax_X(list, ref minX, ref maxX);
            CPoint.GetMinMax_Y(list, ref minY, ref maxY);

            float cx = (float)(minX + ((maxX - minX) / 2.0));
            float cy = (float)(minY + ((maxY - minY) / 2.0));
                
            return new PointF(cx, cy);
        }
        public static PointF GetCentroid(List<PointF> list)
        {
            PointF[] arrPoints = list.ToArray();

            float fx = 0;
            float fy = 0; 
            Parallel.Invoke(() =>
            {
                fx = arrPoints.Sum(element => (float)element.X);
                fy = arrPoints.Sum(element => (float)element.Y);
            });
            
            //for( int i = 0; i < arrPoints.Length; i++)
            //{x, (float)x
            //    fX += (float)arrPoints[i].X;
            //    fY += (float)arrPoints[i].Y;
            //}

            PointF ptCenter = new PointF( fx/ list.Count, fy / list.Count);
            return ptCenter;
        }
    }

    public class CRect
    {
        public static Rectangle ToRectangle(RectangleF rc)
        {
            Rectangle rect = new Rectangle((int)rc.X, (int)rc.Y, (int)rc.Width, (int)rc.Height);
            return rect;
        }
        public static float GetCX(RectangleF rc) { return (float)(rc.X + rc.Width / 2.0); }
        public static float GetCY(RectangleF rc) {return (float)(rc.Y + rc.Height / 2.0); }
        public static PointF GetCenter(RectangleF rc)
        {
            return new PointF(GetCX(rc), GetCY(rc));
        }
        public static PointF GetLT(RectangleF rc) { return new PointF(rc.X, rc.Y); }
        public static PointF GetLB(RectangleF rc) { return new PointF(rc.X, rc.Y + rc.Height); }
        public static PointF GetRT(RectangleF rc) { return new PointF(rc.X + rc.Width, rc.Y); }
        public static PointF GetRB(RectangleF rc) { return new PointF(rc.X + rc.Width, rc.Y + rc.Height); }

        public static PointF GetLC(Rectangle rc){return new PointF(rc.X, (float)((rc.Y + rc.Height)/2));}
        public static PointF GetRC(Rectangle rc) { return new PointF(rc.X + rc.Width, (float)((rc.Y + rc.Height) / 2)); }
        public static PointF GetTC(RectangleF rc){return new PointF((float)((rc.X + rc.Width) / 2), rc.Y);}
        public static PointF GetBC(RectangleF rc){return new PointF((float)((rc.X+rc.Width)/2), rc.Y+rc.Height);}

        public static double GetArea(RectangleF rc) { return rc.Width * rc.Height; }
        public static double GetMajorLength(RectangleF rc) { return Math.Max(rc.Width, rc.Height); } 
        public static bool IsIntersectPoint(RectangleF rc, PointF pt){return IsIntersectPoint(rc, pt.X, pt.Y);}
        public static bool IsIntersectPoint(RectangleF rc, float x, float y)
        {
            if (rc.X <= x && rc.X+rc.Width >= x && rc.Y<= y && rc.Y+rc.Height>= y) { return true; }
            return false;
        }
        public static List<PointF> GetAnglarEdgePoints(RectangleF rc, int nSample) // 161215
        {
            List<PointF> listEdges = new List<PointF>();
            #region body
            float nCenterX = GetCX(rc);
            float nCenterY = GetCY(rc);

            float width = rc.Width;
            float height = rc.Height;

            int nRadius = Convert.ToInt32(Math.Max(width, height));

            int nAngleStep = (int)(360.0 / nSample);

            for (int nAngle = 0; nAngle < 360; nAngle += nAngleStep)
            {
                float ex = 0;
                float ey = 0;

                float fDegree =  (float)((nAngle - 90.0) * Math.PI / 180.0);

                for (int moveRadius = 0; moveRadius < nRadius; moveRadius++)
                {
                    int x = Convert.ToInt32(nCenterX +((moveRadius) * Math.Cos(fDegree)));
                    int y = Convert.ToInt32(nCenterY +((moveRadius) * Math.Sin(fDegree)));

                    if ( CRect.IsIntersectPoint(rc, x, y) == true)
                    {
                        ex = x;
                        ey = y;
                    }
                    else
                    {
                        break;
                    }
                }

                listEdges.Add(new PointF(ex, ey));
            }// angle loop
            

            #endregion
            return listEdges;
        } //
        public static bool IsIntersectRect(RectangleF rcSrc, RectangleF rcTarget )
        {
            if (IsIntersectPoint(rcSrc, CRect.GetLT(rcTarget)) || 
                IsIntersectPoint(rcSrc, CRect.GetRT(rcTarget))||
                IsIntersectPoint(rcSrc, CRect.GetLB(rcTarget)) ||
                IsIntersectPoint(rcSrc, CRect.GetRB(rcTarget)))
            {
                return true;
            }

            return false;
        }

        public static Rectangle GenRect(Point p1, Point p2)
        {
            int minX = Math.Min(p1.X, p2.X);
            int maxX = Math.Max(p1.X, p2.X);
            int minY = Math.Min(p1.Y, p2.Y);
            int maxY = Math.Max(p1.Y, p2.Y);

            Rectangle rc = new Rectangle(minX, minY, maxX - minX, maxY - minY);

            return rc;
        }
        public static RectangleF GenRect(PointF p1, PointF p2)
        {
            float minX = Math.Min(p1.X, p2.X);
            float maxX = Math.Max(p1.X, p2.X);
            float minY = Math.Min(p1.Y, p2.Y);
            float maxY = Math.Max(p1.Y, p2.Y);

            RectangleF rc = new RectangleF(minX, minY, maxX - minX, maxY - minY);

            return rc;
        }

        public static Rectangle OffsetRect(Rectangle rc, double x, double y)
        {
            rc.X += (int)x; rc.Y += (int)y;
            return rc;
        }
        // Convenient Direct Component Changing Functions for rectangle origin 170412 
        public static RectangleF ReplaceOrigin(RectangleF rc, PointF pt)
        {
            RectangleF rcNovel = new RectangleF(pt.X, pt.Y, rc.Width, rc.Height);
            return rcNovel;
        }
        // Convenient Direct Component Changing Functions for rectangle origin 170412 
        public static Rectangle ReplaceOrigin(Rectangle rc, Point pt)
        {
            Rectangle rcNovel = new Rectangle(pt.X, pt.Y, rc.Width, rc.Height);
            return rcNovel;
        }
        // Convenient Direct Component Changing Functions for width and height 170412 
        public static RectangleF ReplaceSize(RectangleF rc, float width, float height)
        {
            RectangleF rcNovel = new RectangleF(rc.X, rc.Y, width, height);
            return rcNovel;
        }
        // Convenient Direct Component Changing Functions : for width and height 170412 
        public static Rectangle ReplaceSize(Rectangle rc, int width, int height)
        {
            Rectangle rcNovel = new Rectangle(rc.X, rc.Y, width, height);
            return rcNovel;
        }
        public static RectangleF OffsetRect(RectangleF rc, double x, double y){rc.X += (float)x;rc.Y += (float)y;return rc;}
        public static RectangleF OffsetRect(RectangleF rc, PointF pt){rc.X += (float)pt.X;rc.Y += (float)pt.Y;return rc;}
        public static RectangleF OffsetRect(RectangleF rcSrc, RectangleF rcTarget){rcSrc.X += (float)rcTarget.X;rcSrc.Y += (float)rcTarget.Y;return rcSrc;}
        public static RectangleF InflateRect(RectangleF rc, double value){rc.X -= (float)value;rc.Y -= (float)value;rc.Width += (float)(value * 2);rc.Height += (float)(value * 2);return rc;}
        public static RectangleF InflateRect(RectangleF rc, double vx, double vy)
        {
            rc.Width += (float)(vx);rc.Height += (float)(vy);
            rc = OffsetRect(rc, (float)(vx / 2), (float)(vy/ 2));
            return rc;}

        public static RectangleF GetMergedRect(RectangleF rcSrc, RectangleF rcTarget)
        {
            float x = Math.Min(rcSrc.X, rcTarget.X);
            float y = Math.Min(rcSrc.Y, rcTarget.Y);
            
            float rx = Math.Max(rcSrc.X + rcSrc.Width, rcTarget.X + rcTarget.Width);
            float by = Math.Max(rcSrc.Y = rcSrc.Height, rcTarget.Y + rcTarget.Height);

            float w = rx - x;
            float h = by - y;

            return new RectangleF( x, y, w, h);
        }
        public static PointF[] GetRotatedRectPoints(Rectangle rc, double fAngle)
        {
            List<PointF> list = new List<PointF>();

            PointF Center = GetCenter(rc);

            PointF LT = RotatedPoint(GetLT(rc), Center, fAngle); list.Add(LT);
            PointF RT = RotatedPoint(GetRT(rc), Center, fAngle); list.Add(RT);
            PointF LB = RotatedPoint(GetLB(rc), Center, fAngle); list.Add(LB);
            PointF RB = RotatedPoint(GetRB(rc), Center, fAngle); list.Add(RB);
            return list.ToArray();
        }
        public static PointF RotatedPoint(PointF pt, PointF ptCenter, double fAngle)
        {
            double dx = (Math.Round((pt.X - ptCenter.X) * Math.Cos(fAngle) - (pt.Y - ptCenter.Y) * Math.Sin(fAngle))) + ptCenter.X;
            double dy = (Math.Round((pt.X - ptCenter.Y) * Math.Sin(fAngle) + (pt.Y - ptCenter.Y) * Math.Cos(fAngle))) + ptCenter.Y;
            return new PointF((float)dx, (float)dy);
        }

        public static RectangleF CopyTo(RectangleF rc)
        {
            return new RectangleF(rc.X, rc.Y, rc.Width, rc.Height);
        }

        //public static DLine GetLine_L {get { return new DLine(this.LT, this.LB); } }
        //public DLine LINE_RV { get { return new DLine(this.RT, this.RB); } }
        //public DLine LINE_UH { get { return new DLine(this.LT, this.RT); } }
        //public DLine LINE_DH { get { return new DLine(this.LB, this.RB); } }

        public static RectangleF EnsureValidity(RectangleF rc , int nLimitW, int nLimitH)
        {
            if (rc.X < 0) rc.X = 0;
            if (rc.Y < 0) rc.Y = 0;

            if (rc.X + rc.Width > nLimitW)
            {
                float fDiff = (rc.X + rc.Width+ 1) - nLimitW;
                rc.Width -= fDiff;
            }
            if (rc.Y + rc.Height> nLimitH)
            {
                float fDiff = (rc.Y + rc.Height) - nLimitH;
                rc.Height -= fDiff;
            }
            return rc;
        }
        public static bool isValid(ref RectangleF rc, int nLimitW, int nLimitH)
        {
            if (rc.X < 0 || rc.Y < 0 || rc.X > nLimitW || rc.Y > nLimitH)
            {
                return false;
            }
            else return true;
        }
        public static bool IsBoarderPosition( RectangleF rc, int nLimitW, int nLimitH)
        {
            if (rc.X <= 0 || rc.Y <= 0 || rc.X+rc.Width>= nLimitW || rc.Y+rc.Height>= nLimitH)
            {
                return true;
            }
            else return false;
        }

        public static List<PointF> GetContourPoints(RectangleF rc) // 161215
        {
            List<PointF> listContour = new List<PointF>();

            PointF LT = GetLT(rc);
            PointF RT = GetRT(rc);
            PointF LB = GetLB(rc);
            PointF RB = GetRB(rc);

            for (int x = (int)LT.X + 1; x < (int)RT.X; x++)
            {
                PointF pt = CLine.GetLyingPointBy_X(LT, RT, x);
                listContour.Add(pt);
            }
            for (int x = (int)LB.X + 1; x < (int)RB.X; x++)
            {
                PointF pt = CLine.GetLyingPointBy_X(LB, RB, x);
                listContour.Add(pt);
            }
            for (int y = (int)LT.Y + 1; y < (int)LB.Y; y++)
            {
                PointF pt = CLine.GetLyingPointBy_Y(LT, LB, y);
                listContour.Add(pt);
            }
            for (int y = (int)RT.Y + 1; y < (int)RB.Y; y++)
            {
                PointF pt = CLine.GetLyingPointBy_Y(RT, RB, y);
                listContour.Add(pt);
            }
            return listContour;
        } //

        public static List<PointF> GetAnglarEdgePoints(RectangleF rc) // 161215
        {
            List<PointF> listContour = new List<PointF>();

            PointF LT = GetLT(rc);
            PointF RT = GetRT(rc);
            PointF LB = GetLB(rc);
            PointF RB = GetRB(rc);


            int var = (int)((RT.X - LT.X) / 90.0);
            for (int x = (int)LT.X + 1; x < (int)RT.X; x += var)
            {
                PointF pt = CLine.GetLyingPointBy_X(LT, RT, x);
                listContour.Add(pt);
            }
            var = (int)((RB.Y - RT.Y) / 90.0);
            for (int y = (int)RT.Y + 1; y < (int)RB.Y; y += var)
            {
                PointF pt = CLine.GetLyingPointBy_Y(RT, RB, y);
                listContour.Add(pt);
            }
            var = (int)((RB.X - LB.X) / 90.0);
            for (int x = (int)LB.X + 1; x < (int)RB.X; x += var)
            {
                PointF pt = CLine.GetLyingPointBy_X(LB, RB, x);
                listContour.Add(pt);
            }
            var = (int)((LB.Y - LT.Y) / 90.0);
            for (int y = (int)LT.Y + 1; y < (int)LB.Y; y += var)
            {
                PointF pt = CLine.GetLyingPointBy_Y(LT, LB, y);
                listContour.Add(pt);
            }

            return listContour;
        } //

      
    }
    public class DispObj : ICloneable
    {
        
        public List<DLine>/****/dispLine/****/= new List<DLine>();
        public List<DString>/**/dispString/**/= new List<DString>();
        public List<DPoint>/***/dispPoint/***/= new List<DPoint>();
        public List<DRect>/****/dispRect/****/= new List<DRect>();
        public List<DCircle>/**/dispCircle/**/= new List<DCircle>();
        public List<DBlob>/**/dispBlob/**/= new List<DBlob>();

        
        public DispObj()
        {
            Clear();
        }

        public void Clear()
        {
            dispLine.Clear();
            dispString.Clear();
            dispPoint.Clear();
            dispRect.Clear();
            dispCircle.Clear();
            dispBlob.Clear();
        }


        public void InsertLine(PointF p1, PointF p2, Color c, float thick)
        {

            dispLine.Add(new DLine( p1.X, p1.Y, p2.X, p2.Y, thick, c));
        }

        public void InsertString(string text, int x, int y, int size, Color c)
        {
            dispString.Add(new DString(text, x, y, size, c));
        }

        public void InsertPoint(float x, float y, float size, float thick, Color c)
        {
            dispPoint.Add(new DPoint(x, y, size, thick, c));
        }

        public void InsertRect(RectangleF rc, Color c)
        {
            dispRect.Add(new DRect(rc.X, rc.Y, rc.Width, rc.Height, 2, c));
        }
        public void InsertRect(double x, double y, double width, double height, Color c, float Thick)
        {
            dispRect.Add(new DRect(x, y, width, height, Thick, c));
        }

        public void InsertCircle(PointF pt, double rx, double ry, Color c, double thick)
        {
            DCircle circle = new DCircle(pt.X, pt.Y, rx, ry, (float)thick, c);
            dispCircle.Add(circle);
        }

        public void InsertBlob(int nID, int nType, PointF ptCenter, RectangleF rc, List<PointF> ptArr)
        {
            this.dispBlob.Add(new DBlob(nID, nType,ptCenter, rc, ptArr));
        }
        public void InsertBlob(DBlob contour)
        {
            this.dispBlob.Add(contour);
        }
        public object Clone()
        {
            DispObj obj = new DispObj();
            obj.dispPoint = this.dispPoint.ToList();
            obj.dispLine = this.dispLine.ToList();
            obj.dispRect = this.dispRect.ToList();
            obj.dispCircle = this.dispCircle.ToList();
            obj.dispString = this.dispString.ToList();
            obj.dispBlob = this.dispBlob.ToList();
            return obj;
        }



    }
    
    public class DBlob
    {
        public int i01_nID = 0;
        public int i02_nType = 0;
        public PointF i03_ptCenter = new PointF(0, 0);
        public RectangleF i04_rc = new RectangleF(0, 0, 0, 0);
        public List<PointF> i05_list_Edges = new List<PointF>();

        public DBlob(int nID, int nType, PointF ptCenter, RectangleF rc, List<PointF> arrPoints)
        {
            this.i01_nID = nID;
            this.i02_nType = nType;
            this.i03_ptCenter = ptCenter;
            this.i04_rc = rc;
            this.i05_list_Edges = arrPoints.ToList();
        }


        
    }
    
    public class DPoint 
    {
        private float x = 0;
        private float y = 0;
        private float size = 1;
        private float thick = 1;
        private Color color = Color.Red;

        public DPoint() { x = 0; y = 0; }
        public DPoint(float x, float y) { this.x = x; this.y = y; }
        public DPoint(float x, float y, float size) { this.x = x; this.y = y; this.size = size; }
        public DPoint(float x, float y, float size, float thick) { this.x = x; this.y = y; this.size = size; this.thick = thick; }
        public DPoint(float x, float y, float size, float thick, Color c) { this.x = x; this.y = y; this.size = size; this.thick = thick; this.color = c; }
        public DPoint(Point pt) { this.x = pt.X; this.y = pt.Y; }
        public DPoint(PointF pt) { this.x = pt.X; this.y = pt.Y; }

        public float X { get { return x; } set { this.x = value; } }
        public float Y { get { return y; } set { this.y = value; } }
        public float Thick { get { return thick; } set { this.thick = value; } }
        public Color Color { get { return color; } set { this.color = value; } }
        public float Size { get { return size; } set { this.size = value; } }


        
        public PointF OffsetPoint(double tx, double ty)
        {
            return new PointF(x + (float)tx, y + (float)ty);
        }
        public DPoint CopyTo()
        {
            return new DPoint(x, y, size, thick, color);
        }
        public Point ToPoint()
        {
            return new Point((int)x, (int)y);
        }
        public PointF ToPointf()
        {
            return new PointF(x, y);
        }
    }
    
    public class DString
    {
        private string msg = "";
        private int x = 0;
        private int y = 0;
        private int size = 1;
        private Color color = Color.Green;

        public DString(string text) { msg = text; }
        public DString(string text, int x, int y) { msg = text; this.x = x; this.y = y; }
        public DString(string text, int x, int y, int size) { msg = text; this.x = x; this.y = y; this.size = size; }
        public DString(string text, int x, int y, int size, Color color) { msg = text; this.x = x; this.y = y; this.size = size; this.color = color; }

        public string MSG { get { return msg; } set { this.msg = value; } }
        public int X { get { return x; } set { this.x = value; } }
        public int Y { get { return y; } set { this.y = value; } }
        public int SIZE { get { return size; } set { this.size = value; } }
        public Color Color { get { return color; } set { this.color = value; } }
        public void OffsetString(double x, double y){ this.x += (int)x; this.y += (int)y;}
        public DString Copy() { return new DString(msg, x, y, size, color); }
        public Point ToPoint() {return new Point( this.x, this.y);}
        public PointF ToPointF() {return new PointF( this.x, this.y);}
    }
    
    public class DRect
    {
        private double x = 0;
        private double y = 0;
        private double w = 0;
        private double h = 0;
        private float thick = 1;
        private Color color = Color.Red;

        public DRect() { this.x = 0; this.y = 0; }
        public DRect(double x, double y, double w, double h) { this.x = x; this.y = y; this.w = w; this.h = h; }
        public DRect(double x, double y, double w, double h, float thick) { this.x = x; this.y = y; this.w = w; this.h = h; this.thick = thick; }
        public DRect(double x, double y, double w, double h, float thick, Color c) { this.x = x; this.y = y; this.w = w; this.h = h; this.thick = thick; this.color = c; }
        public DRect(RectangleF rc) { x = rc.X; y = rc.Y; w = rc.Width; h = rc.Height; }
        public DRect(Rectangle rc) { x = rc.X; y = rc.Y; w = rc.Width; h = rc.Height; }

        public double X { get { return x; } set { this.x = value; } }
        public double Y { get { return y; } set { this.y = value; } }
        public double WIDTH { get { return w; } set { this.w = value; } }
        public double HEIGHT { get { return h; } set { this.h = value; } }
        public float Thick { get { return thick; } set { this.thick = value; } }
        public Color Color { get { return color; } set { this.color = value; } }

        public void OffsetRect(double tx, double ty)
        {
            x += (float)tx;
            y += (float)ty;
        }
        public DRect CopyTo()
        {
            return new DRect(x, y, w, h, thick, color);
        }
        public Rectangle ToRectangle() { Rectangle rc = new Rectangle((int)X, (int)Y, (int)WIDTH, (int)HEIGHT); return rc; }
        public RectangleF ToRectangleF() { RectangleF rc = new RectangleF((float)X, (float)Y, (float)WIDTH, (float)HEIGHT); return rc; }
    }
    
    public class DLine
    {
        private float x1 = 0;
        private float y1 = 0;
        private float x2 = 0;
        private float y2 = 0;
        private float thick = 1;
        private Color color = Color.Red;

        public DLine() { x1 = y1 = x2 = y2 = thick = 0; color = Color.Red; }
        public DLine(double x1, double y1, double x2, double y2, float thick, Color c) 
        {
            this.x1 = (float)x1; this.y1 = (float)y1; this.x2 = (float)x2; this.y2 = (float)y2; this.thick = thick; this.color = c;
        }
        
        public float Thick { get { return thick; } set { this.thick = value; } }
        public float X1 { get { return x1; } set { this.x1 = value; } }
        public float Y1 { get { return y1; } set { this.y1 = value; } }
        public float X2 { get { return x2; } set { this.x2 = value; } }
        public float Y2 { get { return y2; } set { this.y2 = value; } }
        public Color Color { get { return color; } set { this.color = value; } }
        public PointF P1 { get { return new PointF((float)x1, (float)y1); } set { x1 = value.X; y1 = value.Y; } }
        public PointF P2 { get { return new PointF((float)x2, (float)y2); } set { x2 = value.X; y2 = value.Y; } }

        public double LENGTH { get { return Math.Sqrt(Math.Pow((x2 - x1), 2.0) + Math.Pow((y2 - y1), 2.0)); } }
        public PointF CENTER{get{return new PointF((float)((x1 + x2) / 2.0), (float)((y1 + y2) / 2.0)); ;}}

        public DLine CopyTo() 
        {
            return new DLine(x1, y1, x2, y2, this.thick, this.color);
        }
        public void OffsetLine(double tx, double ty){x1 += (float)tx; y1 += (float)ty; x2 += (float)tx; y2 += (float)ty;}
    };
    
    public class DCircle
    {
        private double cx = 0;
        private double cy = 0;
        private double rx = 0;
        private double ry = 0;
        private float thick = 1;
        private Color color = Color.Green;

        public DCircle() { this.cx = this.cy = this.rx = this.ry = this.thick = (float)0.0; }
        public DCircle(double cx, double cy, double rx, double ry, float thick, Color color) { this.cx = cx; this.cy = cy; this.rx = rx; this.ry = ry; this.thick = thick; this.color = color; }
        public void SetCircle(double cx, double cy, double rx, double ry, float thick) { this.cx = cx; this.cy = cy; this.rx = rx; this.ry = ry; this.thick = thick; }
        public void InflateCircle(int rx, int ry) { this.rx += rx; this.ry += ry; }

        public double CX { get { return cx; } set { this.cx = value; } }
        public double CY { get { return cy; } set { this.cy = value; } }
        public double RX { get { return rx; } set { this.rx = value; } }
        public double RY { get { return ry; } set { this.ry = value; } }
        public Color Color { get { return color; } set { this.color = value; } }
        public float Thick { get { return thick; } set { this.thick = value; } }

        public double LTX { get { return cx - rx; } }
        public double LTY { get { return cy - ry; } }
        public double RTX { get { return cx + rx; } }
        public double RTY { get { return cy + ry; } }
        public double WIDTH { get { return RTX - LTX; } }
        public double HEIGHT { get { return RTY - LTY; } }
        public DCircle Copy() { return new DCircle(this.cx, this.cy, this.rx, this.ry, this.thick, this.color); ;}

        public void OffsetCircle(double x, double y){this.cx += x; this.cy += y;}
        public Rectangle ToRectangle() { return new Rectangle((int)LTX, (int)LTY, (int)WIDTH, (int)HEIGHT); }
        public RectangleF ToRectangleF() { return new RectangleF((float)LTX, (float)LTY, (float)WIDTH, (float)HEIGHT); }
    }
    
    public class CDonut
    {
        private PointF[] P1 = new PointF[360];
        private PointF[] P2 = new PointF[360];

        public CDonut()
        {
            Parallel.For(0, P1.Length, i => { P1[i] = new PointF(); });
            Parallel.For(0, P2.Length, i => { P2[i] = new PointF(); });
        }
        public void InsertP1(int nAngle, PointF p1) { P1[nAngle] = p1; }
        public void InsertP2(int nAngle, PointF p2) { P2[nAngle] = p2; }

        public List<PointF> GetAllPoints()
        {
            List<PointF> listP1 = GetListP1();
            List<PointF> listP2 = GetListP2();

            List<PointF> merged = CPoint.GetMergedList(listP1, listP2);

            return merged;
        }
        public List<PointF> GetListP1()
        {
            List<PointF> ptList = new List<PointF>();

            for (int i = 0; i < P1.Length; i++)
            {
                if (CPoint.IsValid(P1[i]) == true )
                {
                    ptList.Add(P1[i]);
                }
            }
            return ptList;
        }
        public List<PointF> GetListP2()
        {
            List<PointF> ptList = new List<PointF>();

            for (int i = 0; i < P2.Length; i++)
            {
                if (CPoint.IsValid(P2[i]) == true )
                {
                    ptList.Add(P2[i]);
                }
            }
            return ptList;
        }

        public double THICKNESS
        {
            get
            {
                double fThick = 0;
                int nCount = 0;

                for (int i = 0; i < 360; i++)
                {
                    if (P1[i].X != 0 && P1[i].Y != 0 && P2[i].X != 0 && P2[i].Y != 0)
                    {
                        fThick += Math.Sqrt(Math.Pow(P1[i].X - P2[i].X, 2) + Math.Pow(P1[i].Y - P2[i].Y, 2));
                        nCount++;
                    }
                }
                fThick /= nCount;
                return fThick;
            }
        }

        public PointF GetP1(int nAngle) { return P1[nAngle]; }
        public PointF GetP2(int nAngle) { return P2[nAngle]; }
        public Double GetDistance(int nAngle)
        {
            double fThickness = 0;
            if (P1[nAngle].X != 0 && P1[nAngle].Y != 0 && P2[nAngle].X != 0 && P2[nAngle].Y != 0)
            {
                fThickness = Math.Sqrt(Math.Pow(P1[nAngle].X - P2[nAngle].X, 2) + Math.Pow(P1[nAngle].Y - P2[nAngle].Y, 2));
            }
            return fThickness;
        }
        public Double GetDiaIn(int nAngle)
        {
            double fThickness = 0;

            PointF p1 = new PointF();
            PointF p2 = new PointF();

            int nSymetry = 0;
            if (nAngle >= 0 && nAngle < 180)
            {
                nSymetry = nAngle + 180;
                p1 = P1[nAngle];
                p2 = P1[nSymetry];
            }
            else
            {
                nSymetry = nAngle - 180;
                p1 = P1[nAngle];
                p2 = P1[nSymetry];
            }

            if (p1.X != 0 && p1.Y != 0 && p2.X != 0 && p2.Y != 0)
            {
                fThickness = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
            }
            return fThickness;
        }
        public Double GetDiaEx(int nAngle)
        {
            double fThickness = 0;

            PointF p1 = new PointF();
            PointF p2 = new PointF();

            int nSymetry = 0;
            if (nAngle >= 0 && nAngle < 180)
            {
                nSymetry = nAngle + 180;
                p1 = P2[nAngle];
                p2 = P2[nSymetry];
            }
            else
            {
                nSymetry = nAngle - 180;
                p1 = P2[nAngle];
                p2 = P2[nSymetry];
            }

            if (p1.X != 0 && p1.Y != 0 && p2.X != 0 && p2.Y != 0)
            {
                fThickness = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
            }
            return fThickness;
        }
    }

    public sealed class ResultPTRN
    {
        public string time { get; set; }

        public class PTRN_Data
        {
            public RectangleF rc;
            public double fScore;
            public double fAngle;
            public PointF ptCenter;
            public PointF ptDist;

            public PTRN_Data()
            {
                this.rc = new RectangleF(0, 0, 0, 0);
                this.fScore = 0;
                this.fAngle = 0;
                this.ptCenter = new PointF(0, 0);
                this.ptDist = new PointF(0, 0);
                
                
            }
            public void InsertData(RectangleF rc, PointF ptCenter, PointF ptDist, double Score, double Angle)
            {
                this.rc = rc;
                this.ptCenter = ptCenter;
                this.ptDist = ptDist;
                this.fScore = Score;
                this.fAngle = Angle;
            }

        }

        public List<PTRN_Data> resultList = new List<PTRN_Data>();

        public int Count { get { return resultList.Count; } }

        public void removeAll()
        {
            resultList.Clear();
        }
        public void AddData(RectangleF rc, PointF ptCenter, PointF ptDist, double fScore, double fAngle)
        {
            PTRN_Data res = new PTRN_Data();
            res.InsertData(rc, ptCenter, ptDist, fScore, fAngle);

            resultList.Add(res);
        }
        public List<RectangleF> GetROIList() // 싸그리다.
        {
            List<RectangleF> rectList = new List<RectangleF>();

            foreach (PTRN_Data data in resultList)
            {
                rectList.Add(data.rc);
            }

            RectSorter rs = new RectSorter();
            rectList.Sort(rs);

            return rectList;
        }
        public List<RectangleF> GetROIList(int imageW, int imageH) // 경계부 제외하고 다
        {
            List<RectangleF> rectList = GetROIList();

            List<RectangleF> rectListFiltered = new List<RectangleF>();

            foreach (RectangleF rc in rectList)
            {
                if (CRect.IsBoarderPosition(rc, imageW, imageH) == true) continue;
                 
                rectListFiltered.Add(rc);
            }
            return rectListFiltered;
        }
        public PointF GetFirstCenterPoint() // 첫번째 데이터의 센터
        {
            PointF pt = new PointF(0, 0);

            if (resultList.Count >= 1)
            {
                RectangleF rc = resultList.ElementAt(0).rc;

                pt.X = CRect.GetCX(rc);
                pt.Y = CRect.GetCY(rc);
            }
            return pt;
        }
        public PointF GetData(int nIndex)
        {
            PointF pt = new PointF(0, 0);

            if (resultList.Count >= nIndex)
            {
                RectangleF rc = resultList.ElementAt(nIndex).rc;

                pt.X = CRect.GetCX(rc);
                pt.Y = CRect.GetCY(rc);
            }
            return pt;
        }
         
    }

    public class CircleSorter : IComparer<DCircle>
    {
        #region CircleSorter Class

        public int Compare(DCircle c1, DCircle c2)
        {
            CLine line1 = new CLine(0, 0, c1.CX, c1.CY);
            CLine line2 = new CLine(0, 0, c2.CX, c2.CY);

            double fDist1 = line1.LENGTH;
            double fDist2 = line2.LENGTH;

            if (c1.CY < c2.CY) fDist2 *= 2;
            else if (c1.CY > c2.CY) fDist1 *= 2;


            if (fDist1 > fDist2) return 1;
            else if (fDist1 < fDist2) return -1;

            return 0;
        }
        #endregion
    }

    

    public class RectSorter : IComparer<RectangleF>
    {
        #region CircleSorter Class

        public int Compare(RectangleF c1, RectangleF c2)
        {
            PointF first= CRect.GetCenter(c1);
            PointF second= CRect.GetCenter(c2);

            double fDist1 = 0;
            double fDist2 = 0;

            if (first.Y > second.Y)
            {
                fDist1 = 500 * Math.Abs(first.Y - second.Y);
            }
            else if (first.Y <= second.Y)
            {
                fDist2 = 500 * Math.Abs(first.Y - second.Y); ;
            }

            if (first.X > second.X)
            {
                fDist1 += 100 * Math.Abs(first.X - second.X);
            }
            else if (first.X <= second.X)
            {
                fDist2 += 100 * Math.Abs(first.X - second.X);
            }

            if (fDist1 > fDist2) return 1;
            else if (fDist1 < fDist2) return -1;

            return 0;
        }
        #endregion
    }
   

}
