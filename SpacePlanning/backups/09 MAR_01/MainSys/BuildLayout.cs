﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using stuffer;
using Autodesk.DesignScript.Runtime;
using System.Diagnostics;
using Autodesk.DesignScript.Geometry;


namespace SpacePlanning
{
    public class BuildLayout
    {
        

        private static double spacingSet = 8; // 0.5 worked great
        internal static Point2d reference = new Point2d(0,0);

        //RECURSIVE SPLITS A POLY
        [MultiReturn(new[] { "PolyAfterSplit", "AreaEachPoly", "EachPolyPoint" })]
        public static Dictionary<string, object> RecursiveSplitPoly(Polygon2d poly,   double ratioA = 0.5, int recompute = 1)
        {

            /*PSUEDO CODE:
            stack polylist = new polylist
            polylist.push(input poly)
            while(polyList != empty )
            {

                poly = polyList[i]
                splittedpoly = splitpolyintowtwocheck(poly, ratio, extents, dir)
                if (splipoly[0].area > thresharea)
                polylist.addrange(splitpoly)
                i += 1
                count += 1

            }
                
            */
            double ratio = 0.5;

            List<Polygon2d> polyList = new List<Polygon2d>();
            List<Point2d> pointsList = new List<Point2d>();
            List<double> areaList = new List<double>();
            Stack<Polygon2d> polyRetrieved = new Stack<Polygon2d>();
            polyRetrieved.Push(poly);
            int count = 0;
            //int thresh = 10;
            //double areaThreshold = GraphicsUtility.AreaPolygon2d(poly.Points)/10;
            double areaThreshold = 1000;
            int dir = 1;
            List<Polygon2d> polyAfterSplit = null;
            Dictionary<string, object> splitReturn = null;
            Polygon2d currentPoly;
            Random rand = new Random();
            double maximum = 0.9;
            double minimum = 0.3;
            while (polyRetrieved.Count > 0)
            {
                //double mul = rand.NextDouble() * (maximum - minimum) + minimum;
                //ratio *= mul;
                ratio = rand.NextDouble() * (maximum - minimum) + minimum;
                currentPoly = polyRetrieved.Pop();
                try
                {
                    splitReturn = BasicSplitPolyIntoTwo(currentPoly, ratio, dir);
                    polyAfterSplit = (List<Polygon2d>)splitReturn["PolyAfterSplit"];
                }
                catch (Exception)
                {
                    //toggle dir between 0 and 1
                    dir = BasicUtility.toggleInputInt(dir);
                    splitReturn = BasicSplitPolyIntoTwo(currentPoly, ratio, dir);
                    if (splitReturn == null)
                    {
                        //Trace.WriteLine("Could Not Split due to Aspect Ration Problem : Sorry");
                        continue;
                    }
                    polyAfterSplit = (List<Polygon2d>)splitReturn["PolyAfterSplit"];
                    //throw;
                }
                List<List<Point2d>> pointsOnPoly = (List<List<Point2d>>)splitReturn["EachPolyPoint"];
                double area1 = GraphicsUtility.AreaPolygon2d(polyAfterSplit[0].Points);
                double area2 = GraphicsUtility.AreaPolygon2d(polyAfterSplit[1].Points);
                if (area1 > areaThreshold)
                {
                    polyRetrieved.Push(polyAfterSplit[0]);
                    polyList.Add(polyAfterSplit[0]);
                    pointsList.AddRange(pointsOnPoly[0]);
                    areaList.Add(area1);

                }
                if (area2 > areaThreshold)
                {
                    polyRetrieved.Push(polyAfterSplit[1]);
                    polyList.Add(polyAfterSplit[1]);
                    pointsList.AddRange(pointsOnPoly[1]);
                    areaList.Add(area2);

                }
                //pointsList.AddRange(pointsOnPoly);
                //polyList.AddRange(polyAfterSplit);
                //pointsList.AddRange(pointsOnPoly);
                //areaList.Add(area1);
                //areaList.Add(area2);


                //toggle dir between 0 and 1
                dir = BasicUtility.toggleInputInt(dir);
                count += 1;
            }

            //Trace.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            //return polyList;
            return new Dictionary<string, object>
            {
                { "PolyAfterSplit", (polyList) },
                { "AreaEachPoly", (areaList) },
                { "EachPolyPoint", (pointsList) }
            };


        }



        //RECURSIVE SPLITS A POLY
        [MultiReturn(new[] { "PolyAfterSplit", "AreaEachPoly", "EachPolyPoint", "UpdatedProgramData" })]
        public static Dictionary<string, object> RecursiveSplitPolyPrograms(List<Polygon2d> polyInputList,List<ProgramData> progData, double ratioA = 0.5, int recompute = 1)
        {

            /*PSUEDO CODE:
            stack polylist = new polylist
            polylist.push(input poly)
            while(polyList != empty )
            {

                poly = polyList[i]
                splittedpoly = splitpolyintowtwocheck(poly, ratio, extents, dir)
                if (splipoly[0].area > thresharea)
                polylist.addrange(splitpoly)
                i += 1
                count += 1

            }
                
            */
            double ratio = 0.5;

            List<Polygon2d> polyList = new List<Polygon2d>();            
            List<Point2d> pointsList = new List<Point2d>();
            List<double> areaList = new List<double>();
            Stack<Polygon2d> polyRetrieved = new Stack<Polygon2d>();
            Stack<ProgramData> programDataRetrieved = new Stack<ProgramData>();

            for(int j = 0; j < progData.Count; j++)
            {
                programDataRetrieved.Push(progData[j]);
            }

            //////////////////////////////////////////////////////////////////////
            for(int i=0; i < polyInputList.Count; i++)
            {

           
            Polygon2d poly = polyInputList[i];
            if (poly == null || poly.Points == null || poly.Points.Count == 0)
            {
                return null;
            }


                polyRetrieved.Push(poly);
            int count = 0;
            //int thresh = 10;
            //double areaThreshold = GraphicsUtility.AreaPolygon2d(poly.Points)/10;
            double areaThreshold = 1000;
            int dir = 1;
            List<Polygon2d> polyAfterSplit = null;
            Dictionary<string, object> splitReturn = null;
            Polygon2d currentPoly;
            Random rand = new Random();
            double maximum = 0.9;
            double minimum = 0.3;
            while (polyRetrieved.Count > 0 && programDataRetrieved.Count>0)
            {
                //double mul = rand.NextDouble() * (maximum - minimum) + minimum;
                //ratio *= mul;
                ProgramData progItem = programDataRetrieved.Pop();
                ratio = rand.NextDouble() * (maximum - minimum) + minimum;
                currentPoly = polyRetrieved.Pop();
                try
                {
                    splitReturn = BasicSplitPolyIntoTwo(currentPoly, ratio, dir);
                    polyAfterSplit = (List<Polygon2d>)splitReturn["PolyAfterSplit"];
                }
                catch (Exception)
                {
                    //toggle dir between 0 and 1
                    dir = BasicUtility.toggleInputInt(dir);
                    splitReturn = BasicSplitPolyIntoTwo(currentPoly, ratio, dir);
                    if (splitReturn == null)
                    {
                        //Trace.WriteLine("Could Not Split due to Aspect Ration Problem : Sorry");
                        continue;
                    }
                    polyAfterSplit = (List<Polygon2d>)splitReturn["PolyAfterSplit"];
                    //throw;
                }
                List<List<Point2d>> pointsOnPoly = (List<List<Point2d>>)splitReturn["EachPolyPoint"];
                double area1 = GraphicsUtility.AreaPolygon2d(polyAfterSplit[0].Points);
                double area2 = GraphicsUtility.AreaPolygon2d(polyAfterSplit[1].Points);
                if (area1 > areaThreshold)
                {
                    polyRetrieved.Push(polyAfterSplit[0]);
                    polyList.Add(polyAfterSplit[0]);
                    progItem.AreaProvided = area1;
                    pointsList.AddRange(pointsOnPoly[0]);
                    areaList.Add(area1);

                }
                if (area2 > areaThreshold)
                {
                    polyRetrieved.Push(polyAfterSplit[1]);
                    polyList.Add(polyAfterSplit[1]);
                    progItem.AreaProvided = area2;
                    pointsList.AddRange(pointsOnPoly[1]);
                    areaList.Add(area2);

                }
                    //pointsList.AddRange(pointsOnPoly);
                    //polyList.AddRange(polyAfterSplit);
                    //pointsList.AddRange(pointsOnPoly);
                    //areaList.Add(area1);
                    //areaList.Add(area2);
                    
                //toggle dir between 0 and 1
                dir = BasicUtility.toggleInputInt(dir);
                count += 1;
            }// end of while loop
            }//end of for loop

            List<ProgramData> AllProgramDataList = new List<ProgramData>();
            for (int i = 0; i < progData.Count; i++)
            {
                ProgramData progItem = progData[i];
                ProgramData progNew = new ProgramData(progItem);
                AllProgramDataList.Add(progNew);
            }

            //Trace.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            //return polyList;
            return new Dictionary<string, object>
            {
                { "PolyAfterSplit", (polyList) },
                { "AreaEachPoly", (areaList) },
                { "EachPolyPoint", (pointsList) },
                { "UpdatedProgramData", (AllProgramDataList) }
                
            };


        }





        public static Line2d PolygonPolygonCommonEdge(Polygon2d poly, Polygon2d other)
        {
            /*
            first reduce number of pts in both polys
            find their centers
            make a vec between their center
            get horizontal comp of vec
            get vertical comp of vec
            which length is long will be our vector

            then for both polys
                check line line intersection between line between two centers and each line of the poly
                    if no intersect, no edge
                    find the line intersects 
                    find the perpendicular projection of centers on these linese

            */

            bool check = false;
            if (poly == null || other == null)
            {
                return null;
            }
            //reduce number of points
            Polygon2d polyReg = new Polygon2d(poly.Points);
            Polygon2d otherReg = new Polygon2d(other.Points);

            //find centers
            Point2d centerPoly = GraphicsUtility.CentroidInPointLists(polyReg.Points);
            Point2d centerOther = GraphicsUtility.CentroidInPointLists(otherReg.Points);

            //make vectors
            Vector2d centerToCen = new Vector2d(centerPoly, centerOther);
            Vector2d centerToCenX = new Vector2d(centerToCen.X, 0);
            Vector2d centerToCenY = new Vector2d(0, centerToCen.Y);

            //make centerLine
            Line2d centerLine = new Line2d(centerPoly, centerOther);
            Vector2d keyVec;
            if(centerToCenX.Length > centerToCenY.Length)
            {
                keyVec = new Vector2d(centerToCenX.X, centerToCenX.Y);
            }
            else
            {
                keyVec = new Vector2d(centerToCenY.X, centerToCenY.Y);
            }

            //check line poly intersection between centertocen vector and each polys            
            Line2d lineInPolyReg = GraphicsUtility.LinePolygonIntersectionReturnLine(polyReg.Points, centerLine);
            Line2d lineInOtherReg = GraphicsUtility.LinePolygonIntersectionReturnLine(otherReg.Points, centerLine);

            //extend the line
            lineInPolyReg.extend();

            //check = GraphicsUtility.AreLinesCollinear(lineInPolyReg, lineInOtherReg);
            check = GraphicsUtility.CheckLineCollinear(lineInPolyReg, lineInOtherReg);

            if (check)
            {
                return lineInPolyReg;
            }
            else
            {
                return null;
            }
            
        }









        internal static List<Polygon2d> BasicSplitWrapper(Polygon2d currentPoly, double ratio, int dir)
        {

            Dictionary<string, object> splitReturned = null;
            List<Polygon2d> polyAfterSplitting = new List<Polygon2d>();
            try
            {
                splitReturned = BasicSplitPolyIntoTwo(currentPoly, ratio, dir);
                polyAfterSplitting = (List<Polygon2d>)splitReturned["PolyAfterSplit"];
            }
            catch (Exception)
            {
                //toggle dir between 0 and 1
                dir = BasicUtility.toggleInputInt(dir);
                splitReturned = BasicSplitPolyIntoTwo(currentPoly, ratio, dir);
                if (splitReturned == null)
                {
                    //Trace.WriteLine("Split Polys did not work");
                    return null;
                }
                polyAfterSplitting = (List<Polygon2d>)splitReturned["PolyAfterSplit"];
                //throw;
            }

            return polyAfterSplitting;
        }


        internal static List<Polygon2d> EdgeSplitWrapper(Polygon2d currentPoly,Random ran, double distance, int dir, double dummy =0)
        {

            Dictionary<string, object> splitReturned = null;
            List<Polygon2d> polyAfterSplitting = new List<Polygon2d>();
            try
            {
                splitReturned = SplitByDistance(currentPoly, ran, distance, dir, dummy);
                polyAfterSplitting = (List<Polygon2d>)splitReturned["PolyAfterSplit"];
            }
            catch (Exception)
            {
                //toggle dir between 0 and 1
                dir = BasicUtility.toggleInputInt(dir);
                splitReturned = SplitByDistance(currentPoly, ran, distance, dir,dummy);
                if (splitReturned == null)
                {
                    //Trace.WriteLine("Split Polys did not work");
                    return null;
                }
                polyAfterSplitting = (List<Polygon2d>)splitReturned["PolyAfterSplit"];

                if(polyAfterSplitting[0] == null && polyAfterSplitting[1] == null)
                {
                    return null;
                }
                //throw;
            }

            

            return polyAfterSplitting;
        }



        //RECURSIVE SPLITS A POLY
        [MultiReturn(new[] { "DeptPolys", "LeftOverPolys", "DepartmentNames", "UpdatedDeptData" })]
        internal static Dictionary<string, object> DeptSplitRefined(Polygon2d poly, List<DeptData> deptData, List<Cell> cellInside, double offset, int recompute = 1)
        {

            /*
            get the poly
            get the poly area
            maintain a leftoverstack
            if dept is inpatients
                split by distance from edge ( distance = 32 )
            else
                split by basicsplitwrapper
            */

            List<List<Polygon2d>> AllDeptPolys = new List<List<Polygon2d>>();
            List<string> AllDepartmentNames = new List<string>();
            List<double> AllDeptAreaAdded = new List<double>();
            Stack<Polygon2d> leftOverPoly = new Stack<Polygon2d>();


            SortedDictionary<double, DeptData> sortedD = new SortedDictionary<double, DeptData>();
            for (int i = 0; i < deptData.Count; i++)
            {
                double area = deptData[i].AreaEachDept();
                DeptData deptD = deptData[i];
                sortedD.Add(area, deptD);

            }



            List<DeptData> sortedDepartmentData = new List<DeptData>();
            foreach (KeyValuePair<double, DeptData> p in sortedD)
            {
                DeptData deptItem = p.Value;
                sortedDepartmentData.Add(deptItem);
            }

            //SORT THE DEPT BASED ON THE AREA
            sortedDepartmentData.Reverse();
            leftOverPoly.Push(poly);
            int dir = 0;
            int maxRound = 1000;
            double count3 = 0;

            for (int i = 0; i < sortedD.Count; i++)
            {

                DeptData deptItem = sortedDepartmentData[i];
                double areaDeptNeeds = deptItem.DeptAreaNeeded;
                double areaAddedToDept = 0;
                double areaLeftOverToAdd = areaDeptNeeds - areaAddedToDept;
                double areaCurrentPoly = 0;
                double perc = 0.2;
                double limit = areaDeptNeeds * perc;

                Polygon2d currentPolyObj = poly;
                List<Polygon2d> everyDeptPoly = new List<Polygon2d>();
                List<Polygon2d> polyAfterSplitting = new List<Polygon2d>();
                double count1 = 0;
                double count2 = 0;
                double areaCheck = 0;



                //areaCurrentPoly = Polygon2d.AreaCheckPolygon(currentPolyObj);

                Random ran = new Random();
                // when inpatient--------------------------------------------------------------------------
                if (i == 0)
                {
                    while (areaLeftOverToAdd > limit && leftOverPoly.Count > 0 && count1 < maxRound)
                    {
                        dir = BasicUtility.toggleInputInt(dir);
                        currentPolyObj = leftOverPoly.Pop();
                        areaCurrentPoly = Polygon2d.AreaCheckPolygon(currentPolyObj);
                        List<Polygon2d> edgeSplitted = EdgeSplitWrapper(currentPolyObj, ran, offset, dir, 0.75); //////////////////////

                        if (edgeSplitted == null)
                        {
                            return null;
                            Trace.WriteLine("Found Null");
                            int countTry = 0;
                            Random ran1 = new Random();
                            while (edgeSplitted == null && countTry < 4)
                            {

                                dir = BasicUtility.toggleInputInt(dir);
                                double percentage = BasicUtility.RandomBetweenNumbers(ran1, 0.75, 0.25);
                                double offsetNew = offset * percentage;

                                edgeSplitted = EdgeSplitWrapper(currentPolyObj, ran, offsetNew, dir, percentage);
                                Trace.WriteLine("Trying to Split By Edge for :  " + countTry);
                                Trace.WriteLine("Direction is :  " + dir + " | Offset is : " + offsetNew +
                                    " | Outer While Iteration is : " + count1);
                                countTry += 1;
                            }

                            //continue;
                        }
                        double areaA = Polygon2d.AreaCheckPolygon(edgeSplitted[0]);
                        double areaB = Polygon2d.AreaCheckPolygon(edgeSplitted[1]);
                        if (areaA < areaB)
                        {
                            everyDeptPoly.Add(edgeSplitted[0]);
                            areaLeftOverToAdd = areaLeftOverToAdd - areaA;
                            areaCheck += areaA;
                            leftOverPoly.Push(edgeSplitted[1]);
                        }
                        else
                        {
                            everyDeptPoly.Add(edgeSplitted[1]);
                            areaLeftOverToAdd = areaLeftOverToAdd - areaB;
                            areaCheck += areaB;
                            leftOverPoly.Push(edgeSplitted[0]);
                        }
                        count1 += 0;
                    }
                }
                //when other depts------------------------------------------------------------------------
                else
                {
                    Random rn = new Random();
                    while (areaLeftOverToAdd > limit && leftOverPoly.Count > 0 && count2 < maxRound)
                    {
                        dir = BasicUtility.toggleInputInt(dir);
                        //double ratio = rn.NextDouble() * (0.85 - 0.15) + 0.15;
                        double ratio = BasicUtility.RandomBetweenNumbers(rn, 0.85, 0.15);
                        currentPolyObj = leftOverPoly.Pop();
                        areaCurrentPoly = Polygon2d.AreaCheckPolygon(currentPolyObj);
                        dir = BasicUtility.toggleInputInt(dir);
                        //dir = BasicUtility.RandomToggleInputInt();
                        //Trace.WriteLine("Area left over is : " + areaLeftOverToAdd);
                        if (areaLeftOverToAdd > areaCurrentPoly)
                        {
                            everyDeptPoly.Add(currentPolyObj);
                            areaLeftOverToAdd = areaLeftOverToAdd - areaCurrentPoly;
                            areaCheck += areaCurrentPoly;
                            //Trace.WriteLine("Area left over after assigning when area is greater than current : " + areaLeftOverToAdd);

                        }
                        else
                        {

                            Dictionary<string, object> basicSplit = BasicSplitPolyIntoTwo(currentPolyObj, ratio, dir); ///////////////////////////////
                            if(basicSplit == null)
                            {
                                return null;
                            }
                            List<Polygon2d> polyS = (List<Polygon2d>)basicSplit["PolyAfterSplit"];
                            double areaA = Polygon2d.AreaCheckPolygon(polyS[0]);
                            double areaB = Polygon2d.AreaCheckPolygon(polyS[1]);

                            if (areaA < areaB)
                            {
                                everyDeptPoly.Add(polyS[0]);
                                areaLeftOverToAdd = areaLeftOverToAdd - areaA;
                                areaCheck += areaA;
                                leftOverPoly.Push(polyS[1]);
                            }
                            else
                            {
                                everyDeptPoly.Add(polyS[1]);
                                areaLeftOverToAdd = areaLeftOverToAdd - areaB;
                                areaCheck += areaB;
                                leftOverPoly.Push(polyS[0]);
                            }


                        }

                        //Trace.WriteLine("Poly After Splitting Length is : " + polyAfterSplitting.Count);

                        //Trace.WriteLine("\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
                        count2 += 1;
                    } // end of while loop
                }

                AllDeptAreaAdded.Add(areaCheck);
                AllDeptPolys.Add(everyDeptPoly);
                AllDepartmentNames.Add(deptItem.DepartmentName);

            }// end of for loop

            Random ran2 = new Random();
            if (recompute > 3)
            {
                //there is any left over poly
                double minArea = 10;
                double areaMoreCheck = 0;
                if (leftOverPoly.Count > 0)
                {

                    while (leftOverPoly.Count > 0 && count3 < maxRound)
                    {
                        dir = BasicUtility.toggleInputInt(dir);
                        Polygon2d currentPolyObj = leftOverPoly.Pop();
                        double areaCurrentPoly = Polygon2d.AreaCheckPolygon(currentPolyObj);
                        List<Polygon2d> edgeSplitted = EdgeSplitWrapper(currentPolyObj,ran2, offset, dir);
                        if (edgeSplitted == null)
                        {
                            return null;
                        }
                        double areaA = Polygon2d.AreaCheckPolygon(edgeSplitted[0]);
                        double areaB = Polygon2d.AreaCheckPolygon(edgeSplitted[1]);
                        if (areaA < areaB)
                        {
                            AllDeptPolys[0].Add(edgeSplitted[0]);
                            //areaLeftOverToAdd = areaLeftOverToAdd - areaA;
                            areaMoreCheck += areaA;
                            if (areaB > minArea) { leftOverPoly.Push(edgeSplitted[1]); }

                        }
                        else
                        {
                            AllDeptPolys[0].Add(edgeSplitted[1]);
                            //areaLeftOverToAdd = areaLeftOverToAdd - areaA;
                            areaMoreCheck += areaB;
                            if (areaA > minArea) { leftOverPoly.Push(edgeSplitted[0]); }
                        }
                        count3 += 1;
                    }// end of while loop



                }// end of if loop for leftover count
                AllDeptAreaAdded[0] += areaMoreCheck;
            }// end of if loop







            List<DeptData> UpdatedDeptData = new List<DeptData>();
            //make the new deptdata to output
            for (int i = 0; i < sortedDepartmentData.Count; i++)
            {

                DeptData newDeptData = new DeptData(sortedDepartmentData[i]);
                newDeptData.AreaProvided = AllDeptAreaAdded[i];
                UpdatedDeptData.Add(newDeptData);

            }




            List<Polygon2d> AllLeftOverPolys = new List<Polygon2d>();
            AllLeftOverPolys.AddRange(leftOverPoly);

            //Trace.WriteLine("Dept Splitting Done ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            //return polyList;
            return new Dictionary<string, object>
            {
                { "DeptPolys", (AllDeptPolys) },
                { "LeftOverPolys", (AllLeftOverPolys) },
                { "DepartmentNames", (AllDepartmentNames) },
                { "UpdatedDeptData", (UpdatedDeptData) }
            };


        }



        //RECURSIVE SPLITS A POLY - USES EdgeSplitWrapper (spltbydistance) & BasicSplitPolyIntoTwo
        [MultiReturn(new[] { "DeptPolys", "LeftOverPolys", "DepartmentNames", "UpdatedDeptData" })]
        public static Dictionary<string, object> DeptArrangeOnSite(Polygon2d poly, List<DeptData> deptData, List<Cell> cellInside, double offset, int recompute = 1)
        {
            Dictionary<string, object> deptArrangement = DeptSplitRefined(poly, deptData, cellInside, offset, 1);
            double count = 0;
            int maxCount = 10;
            Random rand = new Random();
            while(deptArrangement == null && count < maxCount)
            {
                Trace.WriteLine("Lets Go Again for : " + count);
                int reco = rand.Next();
                deptArrangement = DeptSplitRefined(poly, deptData, cellInside, offset, reco);
                count += 1;
            }


            return deptArrangement;

        }







        //RECURSIVE SPLITS A POLY - USES EdgeSplitWrapper (spltbydistance) & BasicSplitPolyIntoTwo
        [MultiReturn(new[] { "DeptPolys", "LeftOverPolys", "DepartmentNames","UpdatedDeptData" })]
        public static Dictionary<string, object> DeptSplitRefineChange(Polygon2d poly, List<DeptData> deptData, List<Cell> cellInside, double offset, int recompute = 1)
        {
            /*
            get the poly
            get the poly area
            maintain a leftoverstack
            if dept is inpatients
                split by distance from edge ( distance = 32 )
            else
                split by basicsplitwrapper
            */

            List<List<Polygon2d>> AllDeptPolys = new List<List<Polygon2d>>();
            List<string> AllDepartmentNames = new List<string>();
            List<double> AllDeptAreaAdded = new List<double>();
            Stack<Polygon2d> leftOverPoly = new Stack<Polygon2d>();


            SortedDictionary<double, DeptData> sortedD = new SortedDictionary<double, DeptData>();
            for (int i = 0; i < deptData.Count; i++)
            {
                double area = deptData[i].AreaEachDept();
                DeptData deptD = deptData[i];
                sortedD.Add(area, deptD);

            }



            List<DeptData> sortedDepartmentData = new List<DeptData>();
            foreach (KeyValuePair<double, DeptData> p in sortedD)
            {
                DeptData deptItem = p.Value;
                sortedDepartmentData.Add(deptItem);
            }

            //SORT THE DEPT BASED ON THE AREA
            sortedDepartmentData.Reverse();
            leftOverPoly.Push(poly);
            int dir = 0;
            int maxRound = 500;
            double count3 = 0;

            for (int i = 0; i < sortedD.Count; i++)
            {
                
                DeptData deptItem = sortedDepartmentData[i];
                double areaDeptNeeds = deptItem.DeptAreaNeeded;
                double areaAddedToDept = 0;
                double areaLeftOverToAdd = areaDeptNeeds - areaAddedToDept;
                double areaCurrentPoly = 0;
                double perc = 0.2;
                double limit = areaDeptNeeds * perc;

                Polygon2d currentPolyObj = poly;
                List<Polygon2d> everyDeptPoly = new List<Polygon2d>();
                List<Polygon2d> polyAfterSplitting = new List<Polygon2d>();
                double count1 = 0;
                double count2 = 0;
                double areaCheck = 0;



                //areaCurrentPoly = Polygon2d.AreaCheckPolygon(currentPolyObj);

                Random ran3 = new Random();
                // when inpatient--------------------------------------------------------------------------
                if (i == 0)
                {
                    while (areaLeftOverToAdd > limit && leftOverPoly.Count > 0 && count1 < maxRound)
                    {
                        dir = BasicUtility.toggleInputInt(dir);
                        currentPolyObj = leftOverPoly.Pop();
                        areaCurrentPoly = Polygon2d.AreaCheckPolygon(currentPolyObj);
                        List<Polygon2d> edgeSplitted = EdgeSplitWrapper(currentPolyObj, ran3, offset, dir,0.75); //////////////////////
                        
                        if(edgeSplitted == null)
                        {
                            int countTry = 0;
                            Random ran = new Random();
                            while(edgeSplitted == null && countTry < 300)
                            {
                                dir = BasicUtility.toggleInputInt(dir);
                                double percentage = BasicUtility.RandomBetweenNumbers(ran, 0.75, 0.25);
                                double offsetNew = offset * percentage;
                                 
                                edgeSplitted = EdgeSplitWrapper(currentPolyObj, ran3, offsetNew, dir, percentage);
                                Trace.WriteLine("Trying to Split By Edge for :  " + countTry);
                                Trace.WriteLine("Direction is :  " + dir + " | Offset is : " + offsetNew + 
                                    " | Outer While Iteration is : " + count1 );
                                countTry += 1;
                            }
                            
                            //continue;
                        }
                        double areaA = Polygon2d.AreaCheckPolygon(edgeSplitted[0]);
                        double areaB = Polygon2d.AreaCheckPolygon(edgeSplitted[1]);
                        if (areaA < areaB)
                        {
                            everyDeptPoly.Add(edgeSplitted[0]);
                            areaLeftOverToAdd = areaLeftOverToAdd - areaA;
                            areaCheck += areaA;
                            leftOverPoly.Push(edgeSplitted[1]);
                        }
                        else
                        {
                            everyDeptPoly.Add(edgeSplitted[1]);
                            areaLeftOverToAdd = areaLeftOverToAdd - areaB;
                            areaCheck += areaB;
                            leftOverPoly.Push(edgeSplitted[0]);
                        }
                        count1 += 0;
                    }
                }
                //when other depts------------------------------------------------------------------------
                else
                {
                    Random rn = new Random();
                    while (areaLeftOverToAdd > limit && leftOverPoly.Count > 0 && count2 < maxRound)
                    {
                        dir = BasicUtility.toggleInputInt(dir);
                        //double ratio = rn.NextDouble() * (0.85 - 0.15) + 0.15;
                        double ratio = BasicUtility.RandomBetweenNumbers(rn, 0.85, 0.15);
                        currentPolyObj = leftOverPoly.Pop();
                        areaCurrentPoly = Polygon2d.AreaCheckPolygon(currentPolyObj);
                        dir = BasicUtility.toggleInputInt(dir);
                        //dir = BasicUtility.RandomToggleInputInt();
                        //Trace.WriteLine("Area left over is : " + areaLeftOverToAdd);
                        if (areaLeftOverToAdd > areaCurrentPoly)
                        {
                            everyDeptPoly.Add(currentPolyObj);
                            areaLeftOverToAdd = areaLeftOverToAdd - areaCurrentPoly;
                            areaCheck += areaCurrentPoly;
                            //Trace.WriteLine("Area left over after assigning when area is greater than current : " + areaLeftOverToAdd);

                        }
                        else
                        {

                            Dictionary<string,object> basicSplit = BasicSplitPolyIntoTwo(currentPolyObj, ratio, dir); ///////////////////////////////
                            List<Polygon2d> polyS = (List<Polygon2d>)basicSplit["PolyAfterSplit"];
                            double areaA = Polygon2d.AreaCheckPolygon(polyS[0]);
                            double areaB = Polygon2d.AreaCheckPolygon(polyS[1]);

                            if (areaA < areaB)
                            {
                                everyDeptPoly.Add(polyS[0]);
                                areaLeftOverToAdd = areaLeftOverToAdd - areaA;
                                areaCheck += areaA;
                                leftOverPoly.Push(polyS[1]);
                            }
                            else
                            {
                                everyDeptPoly.Add(polyS[1]);
                                areaLeftOverToAdd = areaLeftOverToAdd - areaB;
                                areaCheck += areaB;
                                leftOverPoly.Push(polyS[0]);
                            }


                        }

                        //Trace.WriteLine("Poly After Splitting Length is : " + polyAfterSplitting.Count);

                        //Trace.WriteLine("\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
                        count2 += 1;
                    } // end of while loop
                }

                AllDeptAreaAdded.Add(areaCheck);
                AllDeptPolys.Add(everyDeptPoly);
                AllDepartmentNames.Add(deptItem.DepartmentName);

            }// end of for loop

            Random ran2 =new Random();
            if (recompute > 3)
            {
                //there is any left over poly
                double minArea = 50;
                double areaMoreCheck = 0;
                if (leftOverPoly.Count > 0)
                {

                    while (leftOverPoly.Count > 0 && count3 < maxRound)
                    {
                        dir = BasicUtility.toggleInputInt(dir);
                        Polygon2d currentPolyObj = leftOverPoly.Pop();
                        double areaCurrentPoly = Polygon2d.AreaCheckPolygon(currentPolyObj);
                        List<Polygon2d> edgeSplitted = EdgeSplitWrapper(currentPolyObj,ran2, offset, dir);
                        double areaA = Polygon2d.AreaCheckPolygon(edgeSplitted[0]);
                        double areaB = Polygon2d.AreaCheckPolygon(edgeSplitted[1]);
                        if (areaA < areaB)
                        {
                            AllDeptPolys[0].Add(edgeSplitted[0]);
                            //areaLeftOverToAdd = areaLeftOverToAdd - areaA;
                            areaMoreCheck += areaA;
                            if (areaB > minArea) { leftOverPoly.Push(edgeSplitted[1]); }

                        }
                        else
                        {
                            AllDeptPolys[0].Add(edgeSplitted[1]);
                            //areaLeftOverToAdd = areaLeftOverToAdd - areaA;
                            areaMoreCheck += areaB;
                            if (areaA > minArea) { leftOverPoly.Push(edgeSplitted[0]); }
                        }
                        count3 += 1;
                    }// end of while loop



                }// end of if loop for leftover count
                AllDeptAreaAdded[0] += areaMoreCheck;
            }// end of if loop







            List<DeptData> UpdatedDeptData = new List<DeptData>();
            //make the new deptdata to output
            for (int i = 0; i < sortedDepartmentData.Count; i++)
            {

                DeptData newDeptData = new DeptData(sortedDepartmentData[i]);
                newDeptData.AreaProvided = AllDeptAreaAdded[i];
                UpdatedDeptData.Add(newDeptData);

            }




                List<Polygon2d> AllLeftOverPolys = new List<Polygon2d>();
            AllLeftOverPolys.AddRange(leftOverPoly);

            //Trace.WriteLine("Dept Splitting Done ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            //return polyList;
            return new Dictionary<string, object>
            {
                { "DeptPolys", (AllDeptPolys) },
                { "LeftOverPolys", (AllLeftOverPolys) },
                { "DepartmentNames", (AllDepartmentNames) },
                { "UpdatedDeptData", (UpdatedDeptData) }
            };


        }












        //RECURSIVE SPLITS A POLY
        [MultiReturn(new[] { "DeptPolys", "LeftOverPolys", "DepartmentNames" })]
        internal static Dictionary<string, object> DeptSplitMake(Polygon2d poly, List<DeptData> deptData, int recompute = 1)
        {

            double limit   = 300;
            double lowArea = 4000;
            

            List<List<Polygon2d>> AllDeptPolys = new List<List<Polygon2d>>();
            List<string> AllDepartmentNames = new List<string>();
            Stack<Polygon2d> leftOverPoly = new Stack<Polygon2d>();

            
            SortedDictionary<double, DeptData> sortedD = new SortedDictionary<double, DeptData>();
            for ( int i=0; i < deptData.Count; i++ )
                {
                    double area = deptData[i].AreaEachDept();
                    DeptData deptD = deptData[i];
                    sortedD.Add(area, deptD);

                }



            List<DeptData> sortedDepartmentData = new List<DeptData>();

            foreach (KeyValuePair<double, DeptData> p in sortedD)
            {
                DeptData deptItem = p.Value;
                sortedDepartmentData.Add(deptItem);
            }

            //SORT THE DEPT BASED ON THE AREA
            sortedDepartmentData.Reverse();


                for (int i = 0; i < sortedD.Count; i++)
                {
                //foreach (KeyValuePair<double, DeptData> p in sortedD)
                //{
                
                //p.Key,p.Value
                //SortedDictionary<double,DeptData>.Enumerator sortedEnumerator = sortedD.GetEnumerator();
                //sortedEnumerator.Current;
                DeptData deptItem = sortedDepartmentData[i];
                // DeptData deptItem = p.Value;
                double areaDeptNeeds = deptItem.DeptAreaNeeded;
                double areaAddedToDept = 0;
                double areaLeftOverToAdd = areaDeptNeeds - areaAddedToDept;
                double areaCurrentPoly = 0;
                double proportion = 0.5;
                int direction = 0;

                Polygon2d currentPolyObj = poly;
                List<Polygon2d> everyDeptPoly = new List<Polygon2d>();
                List<Polygon2d> polyAfterSplitting = new List<Polygon2d>();
                int iter = 0;



                do
                {
                    //toggle direction
                    direction = BasicUtility.toggleInputInt(direction);

                    //make split of current poly
                    //splitReturned = SplitPolyIntoTwoCheckNew(currentPolyObj, proportion, direction);
                    polyAfterSplitting = BasicSplitWrapper(currentPolyObj, proportion, direction);

                    //check which poly assigned to dept and which poly goes to leftOverPoly
                    double areaPoly1 = GraphicsUtility.AreaPolygon2d(polyAfterSplitting[0].Points);
                    double areaPoly2 = GraphicsUtility.AreaPolygon2d(polyAfterSplitting[1].Points);

                    double diff1 = areaPoly1 - areaDeptNeeds;
                    double diff2 = areaPoly2 - areaDeptNeeds;

                    if (diff1 < diff2)
                    {
                        currentPolyObj = polyAfterSplitting[0];
                        leftOverPoly.Push(polyAfterSplitting[1]);
                        areaCurrentPoly = areaPoly1;
                    }
                    else
                    {
                        currentPolyObj = polyAfterSplitting[1];
                        leftOverPoly.Push(polyAfterSplitting[0]);
                        areaCurrentPoly = areaPoly2;
                    }

                    iter += 1;

                    //area of polyAssigned to dept is LESS THAN areaLeftOverToAdd - CASE 1-------------------------------------
                    if (areaCurrentPoly < areaLeftOverToAdd)
                    {
                        everyDeptPoly.Add(currentPolyObj);
                        currentPolyObj = leftOverPoly.Pop();
                        areaAddedToDept += areaCurrentPoly;
                        areaLeftOverToAdd = areaDeptNeeds - areaAddedToDept;
                        areaCurrentPoly = GraphicsUtility.AreaPolygon2d(currentPolyObj.Points);
                    }


                    //                      -----OR------

                    //area of polyAssigned to dept is MORE THAN areaLeftOverToAdd - CASE 2--------------------------------------
                    else
                    {
                        //while()

                    }


                    if (areaCurrentPoly < lowArea)
                    {
                        break;
                    }
                } while (areaLeftOverToAdd > limit && leftOverPoly.Count > 0);

                AllDeptPolys.Add(everyDeptPoly);
                AllDepartmentNames.Add(deptItem.DepartmentName);
            }
            

            List<Polygon2d> AllLeftOverPolys = new List<Polygon2d>();

            AllLeftOverPolys.AddRange(leftOverPoly);

            //Trace.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            //return polyList;
            return new Dictionary<string, object>
            {
                { "DeptPolys", (AllDeptPolys) },
                { "LeftOverPolys", (AllLeftOverPolys) },
                { "DepartmentNames", (AllDepartmentNames) }
            };


        }
        


        // USING NOW 
        //SPLITS A POLYGON2D INTO TWO POLYS, BASED ON A DIRECTION AND RATIO
        [MultiReturn(new[] { "PolyAfterSplit", "SplitLine", "IntersectedPoints", "SpansBBox", "EachPolyPoint" })]
        public static Dictionary<string, object> BasicSplitPolyIntoTwo(Polygon2d polyOutline, double ratio = 0.5, int dir = 0)
        {
            if(polyOutline == null)
            {
                Trace.WriteLine("-----Basic Poly is Null found");
                return null;
            }
            if(polyOutline != null && polyOutline.Points == null)
            {
                Trace.WriteLine("-----Basic Poly Points are Null found");
                return null;
            }

            double extents = 5000;
            double spacing = spacingSet;
            double minimumLength = 2;
            double minWidth = 10;
            // dir = 0 : horizontal split line
            // dir = 1 : vertical split line

            List<Point2d> polyOrig = polyOutline.Points;
            double eps = 0.1;
            //CHECKS
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //List<Point2d> poly = GraphicsUtility.AddPointsInBetween(polyOrig, 5);
            List<Point2d> poly = Polygon2d.SmoothPolygon(polyOrig, spacing);
            // compute bounding box ( set of four points ) for the poly
            // find x Range, find y Range
            List<Point2d> polyBBox = Polygon2d.FromPointsGetBoundingPoly(poly);
            Range2d polyRange = Polygon2d.GetRang2DFromBBox(poly);

            Point2d span = polyRange.Span;
            double horizontalSpan = span.X;
            double verticalSpan = span.Y;
            List<double> spans = new List<double>();
            spans.Add(horizontalSpan);
            spans.Add(verticalSpan);
            //compute centroid
            Point2d polyCenter = Polygon2d.CentroidFromPoly(poly);
            //check aspect ratio
            double aspectRatio = 0;



            // check if width or length is enough to make split
            if (horizontalSpan < minimumLength || verticalSpan < minimumLength)
            {
                return null;
            }


            //should check direction of split ( flip dir value )
            if (horizontalSpan > verticalSpan)
            {
                dir = 1;
                aspectRatio = horizontalSpan / verticalSpan;
            }
            else
            {
                dir = 0;
                aspectRatio = verticalSpan / horizontalSpan;
            }

            if (aspectRatio < 2)
            {
                //dir = BasicUtility.toggleInputInt(dir);
                //return null;
            }



            // adjust ratio
            if (ratio < 0.15)
            {
                ratio = ratio + eps;
            }
            if (ratio > 0.85)
            {
                ratio = ratio - eps;
            }

            if(horizontalSpan < minWidth || verticalSpan < minWidth)
            {
                ratio = 0.5;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            Line2d splitLine = new Line2d(polyCenter, extents, dir);


            //compute vertical or horizontal line via centroid
            double basic = 0.5;
            double shift = ratio - basic;

            // push this line right or left or up or down based on ratio
            if (dir == 0)
            {
                splitLine.move(0, shift * verticalSpan);
            }
            else
            {
                splitLine.move(shift * horizontalSpan, 0);
            }


            List<Point2d> intersectedPoints = GraphicsUtility.LinePolygonIntersection(poly, splitLine);

            ////////////////////////////////////////////////////////////////////////////////////////////
            
            

            // find all points on poly which are to the left or to the right of the line
            Polygon2d polyA, polyB;

            List<int> pIndexA = new List<int>();
            List<int> pIndexB = new List<int>();
            for (int i = 0; i < poly.Count; i++)
            {
                bool check = GraphicsUtility.CheckPointSide(splitLine, poly[i]);
                if (check)
                {
                    pIndexA.Add(i);
                    //ptA.Add(poly[i]);
                }
                else
                {
                    pIndexB.Add(i);
                    //ptB.Add(poly[i]);
                }
            }

            //organize the points to make closed poly
            List<List<Point2d>> twoSets = new List<List<Point2d>>();
            //List<Point2d> sortedA = makePolyPointsStraight(poly, intersectedPoints, pIndexA);
            //List<Point2d> sortedB = makePolyPointsStraight(poly, intersectedPoints, pIndexB);
            List<Point2d> sortedA = DoSortClockwise(poly, intersectedPoints, pIndexA);
            List<Point2d> sortedB = DoSortClockwise(poly, intersectedPoints, pIndexB);
            twoSets.Add(sortedA);
            twoSets.Add(sortedB);
            polyA = new Polygon2d(twoSets[0], 0);
            polyB = new Polygon2d(twoSets[1], 0);


            List<Polygon2d> splittedPoly = new List<Polygon2d>();

            splittedPoly.Add(polyA);
            splittedPoly.Add(polyB);
            
            return new Dictionary<string, object>
            {
                { "PolyAfterSplit", (splittedPoly) },
                { "SplitLine", (splitLine) },
                { "IntersectedPoints", (intersectedPoints) },
                { "SpansBBox", (spans) },
                { "EachPolyPoint", (twoSets) }
            };

        }







        //internal function for Recursive Split By Area to work
        internal static double DistanceEditBasedOnRatio(double distance, double areaPoly, double areaFound, double area, double setSpan,double areaDifference)
        {
            double distanceNew = 0;
            /*
            if (areaDifference < 0)
            {
                // need to split less - decrease distance
                double ratio = Math.Abs(areaDifference) / areaPoly;
                distance -= ratio * setSpan;
                Trace.WriteLine("Reducing Distance by : " + ratio * setSpan);
                //double ratio = areaFound / area;
                //distance -= distance*Math.Sqrt(ratio);


            }
            else
            {
                //need to split more -  increase distance
                double ratio = Math.Abs(areaDifference) / areaPoly;
                distance += ratio * setSpan;
                Trace.WriteLine("Increading Distance by : " + ratio * setSpan);
                //double ratio = areaFound / area;
                //distance += distance * Math.Sqrt(ratio);
            }
            */

            distanceNew = distance * (area / areaFound);
            //Trace.WriteLine("Ratio multiplied to distance is : " + (area / areaFound));
            //distanceNew = distance;
            return distanceNew;
        }

        //RECURSIVE SPLITS A POLY
        [MultiReturn(new[] { "PolyAfterSplit", "AreaEachPoly", "EachPolyPoint" })]
        public static Dictionary<string, object> RecursiveSplitByArea(Polygon2d poly, double area, int dir, int recompute = 1)
        {

            /*PSUEDO CODE:
            get poly's vertical and horizontal span
            based on that get the direction of split
            get polys area and compare with given area ( if area bigger than polyarea then return null )
            based on the proportion calc distance
            bigpoly 
                split that into two
                save distance
                check area of both
                get the smaller poly and compare with asked area
                if area more increase distance by that much
                if area less decrease distance by that much
                split the bigger poly again
                repeat                
            */



            List<Polygon2d> polyList = new List<Polygon2d>();
            List<double> areaList = new List<double>();
            List<Point2d> pointsList = new List<Point2d>();
            List<Point2d> polyBBox = Polygon2d.FromPointsGetBoundingPoly(poly.Points);
            Range2d polyRange = Polygon2d.GetRang2DFromBBox(poly.Points);
            double minimumLength = 200;
            double perc = 0.2;
            //set limit of 10%
            double limit = area*perc;

            // increase required area by 10%
            //area += area * perc/4;

            Point2d span = polyRange.Span;
            double horizontalSpan = span.X; 
            double verticalSpan = span.Y;
            List<double> spans = new List<double>();
            spans.Add(horizontalSpan);
            spans.Add(verticalSpan);
            double setSpan = 1000000000000;
            //int dir = 0;
            if (horizontalSpan > verticalSpan)
            {
                //dir = 1;
                setSpan = horizontalSpan;

            }
            else
            {
                //dir = 0;
                setSpan = verticalSpan;

            }
            double prop = 0;
            double areaPoly = GraphicsUtility.AreaPolygon2d(poly.Points);
            double areaDifference = 200000;
            if (areaPoly < area)
            {
                return null;
            }
            else
            {
                prop = area / areaPoly;
            }

            List<Polygon2d> polyAfterSplitting = new List<Polygon2d>();
            double distance = prop * setSpan;
            Polygon2d currentPoly = poly;
            int count = 0;
            //Trace.WriteLine("Initial Distance set is : " + distance);
            //Trace.WriteLine("Set Span found is : " + setSpan);
            //Trace.WriteLine("Limit accepted is : " + limit);
            Random ran2 = new Random();
            while (Math.Abs(areaDifference) > limit && count < 300)
            {
                if (currentPoly.Points == null || distance > setSpan)
                {
                    //Trace.WriteLine("Breaking This---------------------------------");
                    break;
                }

                polyAfterSplitting = EdgeSplitWrapper(currentPoly,ran2, distance, dir);
                double area1 = GraphicsUtility.AreaPolygon2d(polyAfterSplitting[0].Points);
                

                areaDifference = area - area1;
                distance = DistanceEditBasedOnRatio(distance, areaPoly, area1,area, setSpan, areaDifference);
                //Trace.WriteLine("Updated Distance for 1 is : " + distance);
                //Trace.WriteLine("Area Difference found for 1 is : " + areaDifference);
                

                if (areaDifference < 0)
                {
                    //Trace.WriteLine("Reducing Distance");
                }






                //reduce number of points
                //currentPoly = new Polygon2d(currentPoly.Points);
                areaList.Add(distance);
                //Trace.WriteLine("Distance Now is : " + distance);
                //Trace.WriteLine("Iteration is : " + count);
                //Trace.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                count += 1;
            }

            polyList.AddRange(polyAfterSplitting);
            pointsList = null;
            //return polyList;
            return new Dictionary<string, object>
            {
                { "PolyAfterSplit", (polyList) },
                { "AreaEachPoly", (areaList) },
                { "EachPolyPoint", (pointsList) }
            };
        }

      


    //RECURSIVE SPLITS A POLY
    [MultiReturn(new[] { "PolyAfterSplit", "AreaEachPoly", "EachPolyPoint","UpdatedProgramData" })]
    public static Dictionary<string, object> RecursiveSplitProgramsOneDirByDistance(List<Polygon2d> polyInputList, List<ProgramData> progData, double distance, int recompute = 1)
    {

            /*PSUEDO CODE:
            get poly's vertical and horizontal span
            based on that get the direction of split
            bigpoly 
                split that into two
                push the smaller one in a list
                take the bigger one 
                make it big poly
                repeat

            */
        List<Polygon2d> polyList = new List<Polygon2d>();
        List<double> areaList = new List<double>();
        List<Point2d> pointsList = new List<Point2d>();
        Stack<ProgramData> programDataRetrieved = new Stack<ProgramData>();

        for (int j = 0; j < progData.Count; j++)
        {
            programDataRetrieved.Push(progData[j]);
        }

        ////////////////////////////////////////////////////////////////////////////
        for (int i = 0; i < polyInputList.Count; i++)
        {

        Polygon2d poly = polyInputList[i]; 


        if (poly == null || poly.Points == null || poly.Points.Count == 0)
        {
            return null;
        }
        
        
        List<Point2d> polyBBox = Polygon2d.FromPointsGetBoundingPoly(poly.Points);
        Range2d polyRange = Polygon2d.GetRang2DFromBBox(poly.Points);
        double minimumLength = 200;

        Point2d span = polyRange.Span;
        double horizontalSpan = span.X;
        double verticalSpan = span.Y;
        List<double> spans = new List<double>();
        spans.Add(horizontalSpan);
        spans.Add(verticalSpan);
        double setSpan = 1000000000000;
        int dir = 0;
        if (horizontalSpan > verticalSpan)
        {
            dir = 1;
            setSpan = horizontalSpan;

        }
        else
        {
            dir = 0;
            setSpan = verticalSpan;

        }


        Polygon2d currentPoly = poly;
        int count = 0;


        Random ran2 = new Random();
        while (setSpan > 0 && programDataRetrieved.Count>0)
        {
            ProgramData progItem = programDataRetrieved.Pop();
            List<Polygon2d> polyAfterSplitting = EdgeSplitWrapper(currentPoly, ran2, distance, dir);
            double selectedArea = 0;
            double area1 = GraphicsUtility.AreaPolygon2d(polyAfterSplitting[0].Points);
            double area2 = GraphicsUtility.AreaPolygon2d(polyAfterSplitting[1].Points);
            if (area1 > area2)
            {
                currentPoly = polyAfterSplitting[0];
                if (polyAfterSplitting[1] == null)
                {
                    break;
                }
                polyList.Add(polyAfterSplitting[1]);
                progItem.AreaProvided = area1;
                areaList.Add(area2);
                selectedArea = area2;


            }
            else
            {
                currentPoly = polyAfterSplitting[1];
                polyList.Add(polyAfterSplitting[0]);
                progItem.AreaProvided = area2;
                areaList.Add(area1);
                selectedArea = area1;

            }


            if (currentPoly.Points == null)
            {
                Trace.WriteLine("Breaking This");
                break;
            }



            //reduce number of points
            //currentPoly = new Polygon2d(currentPoly.Points);

            setSpan -= distance;
            count += 1;
        }// end of while loop


            }// end of for loop
        List<ProgramData> UpdatedProgramDataList = new List<ProgramData>();
        for (int i = 0; i < progData.Count; i++)
        {
            ProgramData progItem = progData[i];
            ProgramData progNew = new ProgramData(progItem);
            UpdatedProgramDataList.Add(progNew);
        }

            pointsList = null;
        //return polyList;
        return new Dictionary<string, object>
            {
                { "PolyAfterSplit", (polyList) },
                { "AreaEachPoly", (areaList) },
                { "EachPolyPoint", (pointsList) },
                { "UpdatedProgramData",(UpdatedProgramDataList) }
            };
    }




    //RECURSIVE SPLITS A POLY
    [MultiReturn(new[] { "PolyAfterSplit", "AreaEachPoly", "EachPolyPoint" })]
        public static Dictionary<string, object> RecursiveSplitOneDirByDistance(Polygon2d poly, double distance, int recompute = 1)
        {

            /*PSUEDO CODE:
            get poly's vertical and horizontal span
            based on that get the direction of split
            bigpoly 
                split that into two
                push the smaller one in a list
                take the bigger one 
                make it big poly
                repeat
                
            */

            if(poly == null || poly.Points ==null || poly.Points.Count ==0)
            {
                return null;
            }


            List<Polygon2d> polyList = new List<Polygon2d>();
            List<double> areaList = new List<double>();
            List<Point2d> pointsList = new List<Point2d>();
            List<Point2d> polyBBox = Polygon2d.FromPointsGetBoundingPoly(poly.Points);
            Range2d polyRange = Polygon2d.GetRang2DFromBBox(poly.Points);
            double minimumLength = 200;

            Point2d span = polyRange.Span;
            double horizontalSpan = span.X;
            double verticalSpan = span.Y;
            List<double> spans = new List<double>();
            spans.Add(horizontalSpan);
            spans.Add(verticalSpan);
            double setSpan = 1000000000000;
            int dir = 0;
            if (horizontalSpan > verticalSpan)
            {
                dir = 1;
                setSpan = horizontalSpan;
        
            }
            else
            {
                dir = 0;
                setSpan = verticalSpan;
             
            }


            Polygon2d currentPoly = poly;
            int count = 0;


            Random ran2 = new Random();
            while (setSpan > 0)
            {
                
                List<Polygon2d> polyAfterSplitting = EdgeSplitWrapper(currentPoly,ran2, distance, dir);
                double selectedArea = 0;
                double area1 = GraphicsUtility.AreaPolygon2d(polyAfterSplitting[0].Points);
                double area2 = GraphicsUtility.AreaPolygon2d(polyAfterSplitting[1].Points);
                if (area1 > area2)
                {
                    currentPoly = polyAfterSplitting[0];
                    if (polyAfterSplitting[1] == null)
                    {
                       break;
                    }
                    polyList.Add(polyAfterSplitting[1]);
                    areaList.Add(area2);
                    selectedArea = area2;
                    

                }
                else
                {
                    currentPoly = polyAfterSplitting[1];
                    polyList.Add(polyAfterSplitting[0]);
                    areaList.Add(area1);
                    selectedArea = area1;
                   
                }


                if (currentPoly.Points == null)
                {
                    Trace.WriteLine("Breaking This");
                    break;
                }


               
                //reduce number of points
                //currentPoly = new Polygon2d(currentPoly.Points);
               
                setSpan -= distance;
                count += 1;
            }

            pointsList = null;
            //return polyList;
            return new Dictionary<string, object>
            {
                { "PolyAfterSplit", (polyList) },
                { "AreaEachPoly", (areaList) },
                { "EachPolyPoint", (pointsList) }
            };
        }




        internal static Dictionary<int, object> pointSelector(Random ran,List<Point2d> poly)
        {
            Dictionary<int, object> output = new Dictionary<int, object>();
 
            double num = ran.NextDouble();
            Trace.WriteLine("Point Selector Random Found is : " + num);
            int highInd = GraphicsUtility.ReturnHighestPointFromListNew(poly);
            Point2d hiPt = poly[highInd];
            int lowInd = GraphicsUtility.ReturnLowestPointFromListNew(poly);
            Point2d lowPt = poly[lowInd];


            if (num < 0.5)
            {
                output[0] = lowPt;
                output[1] = 1;
            }
            else
            {
                output[0] = hiPt; //hiPt
                output[1] = -1; //lowPt
            }


            return output;
        }



        internal static List<double> PolySpanCheck(Polygon2d poly)
        {
            List<double> spanList = new List<double>();
            Range2d polyRange = Polygon2d.GetRang2DFromBBox(poly.Points);

            Point2d span = polyRange.Span;
            double horizontalSpan = span.X;
            double verticalSpan = span.Y;

            //place longer span first
            if (horizontalSpan > verticalSpan)
            {
                spanList.Add(horizontalSpan);
                spanList.Add(verticalSpan);
            }
            else
            {
                spanList.Add(verticalSpan);
                spanList.Add(horizontalSpan);                
            }
            
            return spanList;
        }


       


        // USING NOW 
        //SPLITS A POLYGON2D INTO TWO POLYS, BASED ON A DIRECTION AND DISTANCE
        [MultiReturn(new[] { "PolyAfterSplit", "SplitLine", "IntersectedPoints", "SpansBBox", "EachPolyPoint" })]
        public static Dictionary<string, object> SplitByDistance(Polygon2d polyOutline, Random ran, double distance = 10, int dir = 0, double dummy =0)
        {
            if(polyOutline ==null || polyOutline.Points ==null || polyOutline.Points.Count == 0)
            {
                return null;
            }
           
            double extents = 5000;
            double spacing = spacingSet;
            double minimumLength = 10;
            double minValue = 10;
            bool horizontalSplit = false;
            bool verticalSplit = true;
            // dir = 0 : horizontal split line
            // dir = 1 : vertical split line

            List<Point2d> polyOrig = polyOutline.Points;
            double eps = 0.1;
            //CHECKS
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //List<Point2d> poly = GraphicsUtility.AddPointsInBetween(polyOrig, 5);
            List<Point2d> poly = Polygon2d.SmoothPolygon(polyOrig, spacing);
            if (poly == null || poly.Count == 0)
            {
                return null;
                if (dummy> 0.65)
                {
                    //Trace.WriteLine("Killing it as poly is null");
                    //return null;
                }  
                
            }
            // compute bounding box ( set of four points ) for the poly
            // find x Range, find y Range
            List<Point2d> polyBBox = Polygon2d.FromPointsGetBoundingPoly(poly);
            Range2d polyRange = Polygon2d.GetRang2DFromBBox(poly);

            Point2d span = polyRange.Span;
            double horizontalSpan = span.X;
            double verticalSpan = span.Y;
            List<double> spans = new List<double>();
            spans.Add(horizontalSpan);
            spans.Add(verticalSpan);
            //compute centroid
            //Point2d polyCenter = Polygon2d.CentroidFromPoly(poly);

            //compute lowest point
            // int lowInd = GraphicsUtility.ReturnLowestPointFromListNew(poly);
            //Point2d lowPt = poly[lowInd];
            //check aspect ratio
            double aspectRatio = 0;



            // check if width or length is enough to make split
            if (horizontalSpan < minimumLength || verticalSpan < minimumLength)
            {
                //return null;
            }


            //should check direction of split ( flip dir value )
            if (horizontalSpan > verticalSpan)
            {
                //dir = 1;
                aspectRatio = horizontalSpan / verticalSpan;
            }
            else
            {
                //dir = 0;
                aspectRatio = verticalSpan / horizontalSpan;
            }

            if (aspectRatio > 2)
            {
                //return null;
            }

            //set split style
            if (dir == 0)
            {
                horizontalSplit = true;
            }
            else
            {
                verticalSplit = true;
            }



            // adjust distance if less than some value
            if (distance < minValue)
            {
                //distance = minValue;
            }
            // adjust distance if more than total length of split possible
            if (verticalSplit)
            {
                if (distance > verticalSpan)
                {
                    //distance = verticalSpan - minValue; //CHANGED
                }
            }

            if (horizontalSplit)
            {
                if (distance > horizontalSpan)
                {
                    //distance = horizontalSpan - minValue; //CHANGED
                }
            }


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            Dictionary<int, object> obj = pointSelector(ran,poly);
            Point2d pt = (Point2d)obj[0];
            int orient = (int)obj[1];

            Line2d splitLine = new Line2d(pt, extents, dir);
            //compute vertical or horizontal line via centroid



            // push this line right or left or up or down based on ratio
            if (dir == 0)
            {
                splitLine.move(0, orient*distance);
            }
            else
            {
                splitLine.move(orient*distance, 0);
            }



            List<Point2d> intersectedPoints = GraphicsUtility.LinePolygonIntersection(poly, splitLine);

            ////////////////////////////////////////////////////////////////////////////////////////////



            // find all points on poly which are to the left or to the right of the line
            Polygon2d polyA, polyB;

            List<int> pIndexA = new List<int>();
            List<int> pIndexB = new List<int>();
            for (int i = 0; i < poly.Count; i++)
            {
                bool check = GraphicsUtility.CheckPointSide(splitLine, poly[i]);
                if (check)
                {
                    pIndexA.Add(i);
                    //ptA.Add(poly[i]);
                }
                else
                {
                    pIndexB.Add(i);
                    //ptB.Add(poly[i]);
                }
            }

            //organize the points to make closed poly
            List<List<Point2d>> twoSets = new List<List<Point2d>>();
            //List<Point2d> sortedA = makePolyPointsStraight(poly, intersectedPoints, pIndexA);
            //List<Point2d> sortedB = makePolyPointsStraight(poly, intersectedPoints, pIndexB);
            List<Point2d> sortedA = DoSortClockwise(poly, intersectedPoints, pIndexA);
            List<Point2d> sortedB = DoSortClockwise(poly, intersectedPoints, pIndexB);
            twoSets.Add(sortedA);
            twoSets.Add(sortedB);
            polyA = new Polygon2d(twoSets[0], 0);
            polyB = new Polygon2d(twoSets[1], 0);


            List<Polygon2d> splittedPoly = new List<Polygon2d>();

            splittedPoly.Add(polyA);
            splittedPoly.Add(polyB);



            return new Dictionary<string, object>
            {
                { "PolyAfterSplit", (splittedPoly) },
                { "SplitLine", (splitLine) },
                { "IntersectedPoints", (intersectedPoints) },
                { "SpansBBox", (spans) },
                { "EachPolyPoint", (twoSets) }
            };

        }


        // USING NOW 
        //SPLITS A POLYGON2D INTO TWO POLYS, BASED ON A DIRECTION AND RATIO
        [MultiReturn(new[] { "PolyAfterSplit", "SplitLine", "IntersectedPoints", "SpansBBox", "EachPolyPoint" })]
        internal static Dictionary<string, object> SplitFromEdgePolyIntoTwo(Polygon2d polyOutline, double distance = 10, int dir = 0)
        {
            double extents = 5000;
            double spacing = spacingSet;
            double minimumLength = 10;
            double minValue = 10;
            bool horizontalSplit = false;
            bool verticalSplit = true;
            // dir = 0 : horizontal split line
            // dir = 1 : vertical split line

            List<Point2d> polyOrig = polyOutline.Points;
            double eps = 0.1;
            //CHECKS
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //List<Point2d> poly = GraphicsUtility.AddPointsInBetween(polyOrig, 5);
            List<Point2d> poly = Polygon2d.SmoothPolygon(polyOrig, spacing);
            // compute bounding box ( set of four points ) for the poly
            // find x Range, find y Range
            List<Point2d> polyBBox = Polygon2d.FromPointsGetBoundingPoly(poly);
            Range2d polyRange = Polygon2d.GetRang2DFromBBox(poly);

            Point2d span = polyRange.Span;
            double horizontalSpan = span.X;
            double verticalSpan = span.Y;
            List<double> spans = new List<double>();
            spans.Add(horizontalSpan);
            spans.Add(verticalSpan);
            //compute centroid
            //Point2d polyCenter = Polygon2d.CentroidFromPoly(poly);

            //compute lowest point
            int lowInd = GraphicsUtility.ReturnLowestPointFromListNew(poly);
            Point2d lowPt = poly[lowInd];
            //check aspect ratio
            double aspectRatio = 0;



            // check if width or length is enough to make split
            if (horizontalSpan < minimumLength || verticalSpan < minimumLength)
            {
                return null;
            }


            //should check direction of split ( flip dir value )
            if (horizontalSpan > verticalSpan)
            {
                //dir = 1;
                aspectRatio = horizontalSpan / verticalSpan;
            }
            else
            {
                //dir = 0;
                aspectRatio = verticalSpan / horizontalSpan;
            }

            if (aspectRatio > 2)
            {
                //return null;
            }

            //set split style
            if(dir == 0)
            {
                horizontalSplit = true;
            }
            else
            {
                verticalSplit = true;
            }


            
            // adjust distance if less than some value
            if (distance < minValue)
            {
                distance = minValue;
            }
            // adjust distance if more than total length of split possible
            if (verticalSplit)
            {
                if(distance > verticalSpan)
                {
                    distance = verticalSpan - minValue;
                }
            }

            if (horizontalSplit)
            {
                if (distance > horizontalSpan)
                {
                    distance = horizontalSpan - minValue;
                }
            }


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            Line2d splitLine = new Line2d(lowPt, extents, dir);
            //compute vertical or horizontal line via centroid

         

            // push this line right or left or up or down based on ratio
            if (dir == 0)
            {
                splitLine.move(0, distance);
            }
            else
            {
                splitLine.move(distance, 0);
            }



            List<Point2d> intersectedPoints = GraphicsUtility.LinePolygonIntersection(poly, splitLine);

            ////////////////////////////////////////////////////////////////////////////////////////////



            // find all points on poly which are to the left or to the right of the line
            Polygon2d polyA, polyB;

            List<int> pIndexA = new List<int>();
            List<int> pIndexB = new List<int>();
            for (int i = 0; i < poly.Count; i++)
            {
                bool check = GraphicsUtility.CheckPointSide(splitLine, poly[i]);
                if (check)
                {
                    pIndexA.Add(i);
                    //ptA.Add(poly[i]);
                }
                else
                {
                    pIndexB.Add(i);
                    //ptB.Add(poly[i]);
                }
            }

            //organize the points to make closed poly
            List<List<Point2d>> twoSets = new List<List<Point2d>>();
            //List<Point2d> sortedA = makePolyPointsStraight(poly, intersectedPoints, pIndexA);
            //List<Point2d> sortedB = makePolyPointsStraight(poly, intersectedPoints, pIndexB);
            List<Point2d> sortedA = DoSortClockwise(poly, intersectedPoints, pIndexA);
            List<Point2d> sortedB = DoSortClockwise(poly, intersectedPoints, pIndexB);
            twoSets.Add(sortedA);
            twoSets.Add(sortedB);
            polyA = new Polygon2d(twoSets[0], 0);
            polyB = new Polygon2d(twoSets[1], 0);


            List<Polygon2d> splittedPoly = new List<Polygon2d>();

            splittedPoly.Add(polyA);
            splittedPoly.Add(polyB);



            return new Dictionary<string, object>
            {
                { "PolyAfterSplit", (splittedPoly) },
                { "SplitLine", (splitLine) },
                { "IntersectedPoints", (intersectedPoints) },
                { "SpansBBox", (spans) },
                { "EachPolyPoint", (twoSets) }
            };

        }



        //checker function - can be discarded ater
        public static List<Point2d> CheckLowest_HighestPoint(Polygon2d poly)
        {
            List<Point2d> returnPts = new List<Point2d>();
            List<Point2d> ptList = poly.Points;
            int highPtInd = GraphicsUtility.ReturnHighestPointFromListNew(ptList);
            int  lowPtInd = GraphicsUtility.ReturnLowestPointFromListNew(ptList);
            returnPts.Add(ptList[lowPtInd]);
            returnPts.Add(ptList[highPtInd]);

            return returnPts;

        }


        // not using now can be discarded
        internal static List<Point2d> organizePointToMakePoly(List<Point2d> poly, List<Point2d> intersectedPoints, List<int> pIndex)
        {
            List<Point2d> sortedPoint = new List<Point2d>();
            List<Point2d> unsortedPt = new List<Point2d>();
            // make two unsorted point lists
            for (int i = 0; i < pIndex.Count; i++)
            {
                unsortedPt.Add(poly[pIndex[i]]);
            }
            unsortedPt.AddRange(intersectedPoints);
            //compute lowest and highest pts
            Point2d lowPt = unsortedPt[GraphicsUtility.ReturnLowestPointFromListNew(unsortedPt)];
            Point2d hiPt = unsortedPt[GraphicsUtility.ReturnHighestPointFromListNew(unsortedPt)];
            //form a line2d between them
            Line2d lineHiLo = new Line2d(lowPt, hiPt);

            //make left and right points based on the line
            List<Point2d> ptOnA = new List<Point2d>();
            List<Point2d> ptOnB = new List<Point2d>();
            for (int i = 0; i < unsortedPt.Count; i++)
            {
                bool check = GraphicsUtility.CheckPointSide(lineHiLo, unsortedPt[i]);
                if (check)
                {
                    //pIndexA.Add(i);
                    ptOnA.Add(unsortedPt[i]);
                }
                else
                {
                    //pIndexB.Add(i);
                    ptOnB.Add(unsortedPt[i]);
                }
            }

            //sort ptOnA and ptOnB individually
            List<Point2d> SortedPtA = GraphicsUtility.SortPointsByDistanceFromPoint(ptOnA,
                         lowPt);
            List<Point2d> SortedPtB = GraphicsUtility.SortPointsByDistanceFromPoint(ptOnB,
                         lowPt);
            SortedPtB.Reverse();
            //add the sorted ptOnA and ptOnB
            sortedPoint.AddRange(SortedPtA);
            sortedPoint.AddRange(SortedPtB);
            return sortedPoint;
        }

        //can be discarded cleans duplicate points and returns updated list
        public static List<Point2d> CleanDuplicatePoint2d(List<Point2d> ptListUnclean)
        {
            List<Point2d> cleanList = new List<Point2d>();
            List<double> dummyList = new List<double>();
            List<bool> isDuplicate = new List<bool>();
            double eps = 1;
            bool duplicate = false;
           


            for (int i = 0; i < ptListUnclean.Count; i++)
            {
                duplicate = false;
                double count = 0;
                for (int j = 0; j < ptListUnclean.Count; j++)
                {
                    
                    if (j == i)
                    {
                        continue;
                    }

                    if(ptListUnclean[i].X - eps < ptListUnclean[j].X  && ptListUnclean[j].X < ptListUnclean[i].X + eps)
                    {
                        if (ptListUnclean[i].Y - eps < ptListUnclean[j].Y  && ptListUnclean[j].Y < ptListUnclean[i].Y + eps)
                        {
                            count += 1;
                        }
                    }

                    if(count > 1)
                    {
                        duplicate = true;
                        
                        //continue;
                    }


                }

                dummyList.Add(count);
                if (!duplicate)
                {
                    //cleanList.Add(ptListUnclean[i]);
                }


              
            }
            for (int i = 0; i < ptListUnclean.Count; i++)
            {
                Trace.WriteLine("count here is : " + dummyList[i]);
                if (dummyList[i] < 2)
                {
                    cleanList.Add(ptListUnclean[i]);
                }

            }
            /*
            List<double> itemX = new List<double>();
            List<double> itemY = new List<double>();
            for (int i = 0; i < ptListUnclean.Count; i++)
            {
                itemX.Add(ptListUnclean[i].X);
                itemX.Add(ptListUnclean[i].Y);
            }

            var duplicateIndexesX = itemX.Select((item, index) => new { item, index })
                        .GroupBy(g => g.item)
                        .Where(g => g.Count() > 1)
                        .SelectMany(g => g.Skip(1), (g, item) => item.index);

            var duplicateIndexesY = itemY.Select((item, index) => new { item, index })
                        .GroupBy(g => g.item)
                        .Where(g => g.Count() > 1)
                        .SelectMany(g => g.Skip(1), (g, item) => item.index);


            itemX = (List<double>)itemX.Where((item, index) => (!duplicateIndexesX.Contains(index) && !duplicateIndexesY.Contains(index)));
            itemY = (List<double>)itemY.Where((item, index) => (!duplicateIndexesX.Contains(index) && !duplicateIndexesY.Contains(index)));

            for (int i = 0; i < itemX.Count; i++)
            {
                cleanList.Add(new Point2d(itemX[i], itemY[i]));
            }
            */
            return cleanList;
        }


        //cleans duplicate points and returns updated list
        public static List<Point2d> CleanDuplicatePoint2dNew(List<Point2d> ptListUnclean)
        {
            List<Point2d> cleanList = new List<Point2d>();
            List<double> exprList = new List<double>();
            double a = 45, b = 65;
            for(int i = 0; i < ptListUnclean.Count; i++)
            {
                double expr = a * ptListUnclean[i].X + b * ptListUnclean[i].Y;
                exprList.Add(expr);
            }

            var dups = exprList.GroupBy(x => x)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToList();

            List<double> distinct = exprList.Distinct().ToList();
            for(int i = 0; i < distinct.Count; i++)
            {
                double dis = distinct[i];
                for(int j = 0; j < exprList.Count; j++)
                {
                    if(dis == exprList[j])
                    {
                        cleanList.Add(ptListUnclean[j]);
                        break;
                    }
                }
            }
            return cleanList;

        }
        //trying new way to sort points clockwise
        internal static List<Point2d> DoSortClockwise(List<Point2d> poly, List<Point2d> intersectedPoints, List<int> pIndex)
        {
            
            if (intersectedPoints.Count > 2)
            {
                
                Trace.WriteLine("Wow found  " + intersectedPoints.Count + " intersection points!!!!!!!!!!!!!!!");
                List<Point2d> cleanedPtList = CleanDuplicatePoint2dNew(intersectedPoints);
                Trace.WriteLine("After Cleaning found  " + cleanedPtList.Count + " intersection points!!!!!!!!!!!!!!!");
                //intersectedPoints = GraphicsUtility.CleanDuplicatePoint2d(intersectedPoints);
                return null;
                
            }
            /*

            if (intersectedPoints.Count < 2)
            {
                Trace.WriteLine("Returning null as less than 1 points");
                //return null;
            }
            List<Point2d> pointList = new List<Point2d>();
            List<Point2d> mergedPoints = new List<Point2d>();
            
            for (int i = 0; i < pIndex.Count; i++)
            {
                mergedPoints.Add(poly[pIndex[i]]);
            }
            reference = GraphicsUtility.CentroidInPointLists(mergedPoints);
            mergedPoints.AddRange(intersectedPoints);

            //mergedPoints.Sort((a, b) => GraphicsUtility.Angle(a, reference).CompareTo(GraphicsUtility.Angle(b, reference)));
            //mergedPoints.Sort(new Comparison<Point2d>(GraphicsUtility.SortCornersClockwise));
            //return mergedPoints;
            */
            return makePolyPointsStraight(poly, intersectedPoints, pIndex);
        }

        internal static List<Point2d> makePolyPointsStraight(List<Point2d> poly, List<Point2d> intersectedPoints, List<int> pIndex)
        {

            if (intersectedPoints.Count < 1)
            {
                return null;
            }

            //List<Point2d> intersectedPoints = GraphicsUtility.PointUniqueChecker(intersectedPointsUnclean);
            //Trace.WriteLine("Intersected Points Length are : " + intersectedPoints.Count);
            //bool isUnique = intersectedPoints.Distinct().Count() == intersectedPoints.Count();
            //Trace.WriteLine("Intersected Point List is unique ?   " + isUnique);
            List<Point2d> pt = new List<Point2d>();
            bool added = false;

            int a = 0;
            int b = 1;
            if (intersectedPoints.Count > 2)
            {
                Trace.WriteLine("Intersected pnts are more than one " + intersectedPoints.Count);
                //a = 0;
                //b = intersectedPoints.Count - 1;
            }

            //Trace.WriteLine("Intersected Points Length are : " + intersectedPoints.Count);
            //Trace.WriteLine("a and b are : " + a + "  ,  " + b );
            //Trace.WriteLine("Index Point length is :  " + pIndex.Count);
            for (int i = 0; i < pIndex.Count - 1; i++)
            {
                pt.Add(poly[pIndex[i]]);
                //enter only when indices difference are more than one and intersected points are not added yet
                if (Math.Abs(pIndex[i] - pIndex[i + 1]) > 1 && added == false)
                {
                    List<Point2d> intersNewList = GraphicsUtility.SortPointsByDistanceFromPoint(intersectedPoints,
                        poly[pIndex[i]]);
                    pt.Add(intersNewList[a]);
                    pt.Add(intersNewList[b]);
                    //Trace.WriteLine("Added Intersect Before for PtA");
                    added = true;
                }

                if (i == (pIndex.Count - 2) && added == false)
                {
                    pt.Add(poly[pIndex[i + 1]]);
                    List<Point2d> intersNewList = GraphicsUtility.SortPointsByDistanceFromPoint(intersectedPoints,
                             poly[pIndex[i + 1]]);
                    pt.Add(intersNewList[a]);
                    pt.Add(intersNewList[b]);
                    added = true;
                }

                else if (i == (pIndex.Count - 2) && added == true)
                {
                    pt.Add(poly[pIndex[i + 1]]);
                }
            }
            //Trace.WriteLine("Point Returned Length : " + pt.Count);
            //Trace.WriteLine("I++++++++++++++++++++++++++++++++++++++++++");
            return pt;
        }


     



    }
}