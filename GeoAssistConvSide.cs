using Mastercam.Database;
using Mastercam.IO;
using Mastercam.Database.Types;
using Mastercam.App.Types;
using Mastercam.GeometryUtility;
using Mastercam.Support;
using Mastercam.Database.Interop;
using Mastercam.Curves;
using Mastercam.Math;
using System.Collections.Generic;

namespace _GeoAssistConvSide
{
    public class GeoAssistConvSide : Mastercam.App.NetHook3App
    {

        public Mastercam.App.Types.MCamReturn GeoAssistConvSideRun(Mastercam.App.Types.MCamReturn notused)
        {

            void offsetCutchain()
            {
                ArcGeometry filletCheck(ArcGeometry filletArc, Geometry geo)
                {
                    var tempAngle = 0.0;
                    if (geo is LineGeometry line)
                    {
                        if (VectorManager.Distance(filletArc.EndPoint1, line.EndPoint1) < 0.0001)
                        {
                            tempAngle = filletArc.Data.StartAngleDegrees;
                            filletArc.Data.StartAngleDegrees = filletArc.Data.EndAngleDegrees;
                            filletArc.Data.EndAngleDegrees = tempAngle;
                            filletArc.Commit();
                        }
                        if (VectorManager.Distance(filletArc.EndPoint2, line.EndPoint2) < 0.0001)
                        {
                            tempAngle = filletArc.Data.StartAngleDegrees;
                            filletArc.Data.StartAngleDegrees = filletArc.Data.EndAngleDegrees;
                            filletArc.Data.EndAngleDegrees = tempAngle;
                            filletArc.Commit();
                        }
                    }
                    if (geo is ArcGeometry arc)
                    {
                        if (VectorManager.Distance(filletArc.EndPoint1, arc.EndPoint1) < 0.0001)
                        {
                            tempAngle = filletArc.Data.StartAngleDegrees;
                            filletArc.Data.StartAngleDegrees = filletArc.Data.EndAngleDegrees;
                            filletArc.Data.EndAngleDegrees = tempAngle;
                            filletArc.Commit();
                        }
                        if (VectorManager.Distance(filletArc.EndPoint2, arc.EndPoint2) < 0.0001)
                        {
                            tempAngle = filletArc.Data.StartAngleDegrees;
                            filletArc.Data.StartAngleDegrees = filletArc.Data.EndAngleDegrees;
                            filletArc.Data.EndAngleDegrees = tempAngle;
                            filletArc.Commit();
                        }
                    }
                    return filletArc;
                }
                var levelTenList1 = new List<Geometry>();
                var level139List1 = new List<Geometry>();
                var levelTenList2 = new List<Geometry>();
                var level139List2 = new List<Geometry>();
                var chainList1 = new List<Chain>();
                var chainList2 = new List<Chain>();
                var chainList3 = new List<Chain>();
                var chainList4 = new List<Chain>();
                var chainList5 = new List<Chain>();
                var chainList6 = new List<Chain>();

                SelectionManager.UnselectAllGeometry();
                LevelsManager.RefreshLevelsManager();
                GraphicsManager.Repaint(true);
                int mainGeo = 10;
                int cleanOut = 12;
                int roughSurf = 138;
                int finishSurf = 139;
                var depth = -0.100;
                var roughAngle = 15.0;
                var finishAngle = 20.0;
                var selectedCutChain = ChainManager.GetMultipleChains("Select Geometry");
                if (selectedCutChain == null)
                {
                    return;
                }
                var depthDialog = DialogManager.AskForNumber("Enter Depth", ref depth);
                if (depthDialog == 0)
                {
                    return;
                }
                var roughAngleDialog = DialogManager.AskForAngle("Enter Rough Angle", ref roughAngle);
                if (roughAngleDialog == 0)
                {
                    return;
                }
                var finishAngleDialog = DialogManager.AskForAngle("Enter Finish Angle", ref finishAngle);
                if (finishAngleDialog == 0)
                {
                    return;
                }
                SurfaceDraftParams roughSurfaceDraftParams1 = new SurfaceDraftParams
                {
                    draftMethod = SurfaceDraftParams.DraftMethod.Length,
                    geometryType = SurfaceDraftParams.GeometryType.Surface,
                    length = depth,
                    angle = Mastercam.Math.VectorManager.RadiansToDegrees(-roughAngle),
                    draftDirection = SurfaceDraftParams.DraftDirection.Defined
                };
                SurfaceDraftParams finishSurfaceDraftParams1 = new SurfaceDraftParams
                {
                    draftMethod = SurfaceDraftParams.DraftMethod.Length,
                    geometryType = SurfaceDraftParams.GeometryType.Surface,
                    length = depth,
                    angle = Mastercam.Math.VectorManager.RadiansToDegrees(-finishAngle),
                    draftDirection = SurfaceDraftParams.DraftDirection.Defined
                };
                SurfaceDraftParams roughSurfaceDraftParams2 = new SurfaceDraftParams
                {
                    draftMethod = SurfaceDraftParams.DraftMethod.Length,
                    geometryType = SurfaceDraftParams.GeometryType.Surface,
                    length = depth,
                    angle = Mastercam.Math.VectorManager.RadiansToDegrees(roughAngle),
                    draftDirection = SurfaceDraftParams.DraftDirection.Defined
                };
                SurfaceDraftParams finishSurfaceDraftParams2 = new SurfaceDraftParams
                {
                    draftMethod = SurfaceDraftParams.DraftMethod.Length,
                    geometryType = SurfaceDraftParams.GeometryType.Surface,
                    length = depth,
                    angle = Mastercam.Math.VectorManager.RadiansToDegrees(finishAngle),
                    draftDirection = SurfaceDraftParams.DraftDirection.Defined
                };
                ChainManager.ChainTolerance = 0.0005;
                foreach (var chain in selectedCutChain)
                {
                    if (chain.IsClosed) { continue; }
                    else
                    {
                        DialogManager.Error("The chain is not closed, try again", "Chain Not Closed Error");
                        return;
                    }
                }

                foreach (var chain in selectedCutChain)
                {
                    var chainGeos = ChainManager.GetGeometryInChain(chain);
                    foreach (var entity in chainGeos)
                    {
                        if (entity is SplineGeometry || entity is NURBSCurveGeometry)
                        {
                            Mastercam.IO.DialogManager.Error("Spline Found", "Fix Splines and try again");
                            return;
                        }
                    }
                }
                foreach (var chain in selectedCutChain)
                {
                    chainList1.Add(chain);
                    chainList3.Add(chain);
                    chainList5.Add(chain);
                    chainList2.Add(chain);
                    chainList4.Add(chain);
                    chainList6.Add(chain);
                    SelectionManager.UnselectAllGeometry();
                }
                //Thread thread1 = new Thread(new ThreadStart(chain10Side1));
                //Thread thread2 = new Thread(new ThreadStart(chain10Side2));
                //Thread thread3 = new Thread(new ThreadStart(chain12Side1));
                //Thread thread4 = new Thread(new ThreadStart(chain12Side2));
                //Thread thread5 = new Thread(new ThreadStart(chain139Side1));
                //Thread thread6 = new Thread(new ThreadStart(chain139Side2));
                //thread1.Start();
                //thread2.Start();
                //thread3.Start();
                //thread4.Start();
                //thread5.Start();
                //thread6.Start();
                chain10Side1();
                clearLists();
                //chain10Side2();
                //clearLists();
                chain12Side1();
                clearLists();
                //chain12Side2();
                //clearLists();
                chain139Side1();
                clearLists();
                //chain139Side2();
                //clearLists();



                void chain10Side1()
                {
                    foreach (var chain in chainList2)
                    {
                        var mainGeoSide2 = chain.OffsetChain2D(OffsetSideType.Right, .002, OffsetRollCornerType.All, .5, false, .005, false);
                        var resultChainGeo = SearchManager.GetResultGeometry();
                        foreach (var entity in resultChainGeo)
                        {
                            entity.Color = mainGeo;
                            entity.Level = mainGeo;
                            entity.Selected = false;
                            levelTenList2.Add(entity);
                            entity.Commit();
                        }
                        foreach (var entity1 in resultChainGeo)
                        {
                            if (entity1 is LineGeometry line1)
                            {
                                foreach (var entity2 in resultChainGeo)
                                {
                                    if (entity2 is LineGeometry line2 && entity1.GetEntityID() != entity2.GetEntityID())
                                    {
                                        if ((VectorManager.Distance(line1.EndPoint1, line2.EndPoint1) <= 0.0001)
                                            ||
                                            (VectorManager.Distance(line1.EndPoint1, line2.EndPoint2) <= 0.0001)
                                            ||
                                            (VectorManager.Distance(line1.EndPoint2, line2.EndPoint2) <= 0.0001)
                                            ||
                                            (VectorManager.Distance(line1.EndPoint2, line2.EndPoint1) <= 0.0001))
                                        {
                                            var newFillet = GeometryManipulationManager.FilletTwoCurves(line1, line2, 0.0020, mainGeo, mainGeo, true);
                                            if (newFillet != null)
                                            {
                                                line2.Retrieve();
                                                newFillet = filletCheck(newFillet, entity2);
                                                newFillet.Retrieve();
                                                newFillet.Commit();
                                                levelTenList2.Add(newFillet);
                                            }
                                            line1.Retrieve();
                                            line1.Commit();
                                            line2.Retrieve();
                                            line2.Commit();
                                        }
                                    }
                                    if (entity2 is ArcGeometry arc2 && entity1.GetEntityID() != entity2.GetEntityID())
                                    {
                                        if (arc2.Data.Radius > 0.002)
                                        {
                                            if ((VectorManager.Distance(line1.EndPoint1, arc2.EndPoint1) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(line1.EndPoint1, arc2.EndPoint2) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(line1.EndPoint2, arc2.EndPoint2) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(line1.EndPoint2, arc2.EndPoint1) <= 0.0001))
                                            {
                                                var newFillet = GeometryManipulationManager.FilletTwoCurves(line1, arc2, 0.0020, mainGeo, mainGeo, true);
                                                if (newFillet != null)
                                                {
                                                    arc2.Retrieve();
                                                    newFillet = filletCheck(newFillet, entity2);
                                                    newFillet.Retrieve();
                                                    newFillet.Commit();
                                                    levelTenList2.Add(newFillet);
                                                }
                                                line1.Retrieve();
                                                line1.Commit();
                                                arc2.Retrieve();
                                                arc2.Commit();
                                            }
                                        }
                                    }
                                }
                            }
                            if (entity1 is ArcGeometry arc1)
                            {
                                if (arc1.Data.Radius > 0.002)
                                {
                                    foreach (var entity2 in resultChainGeo)
                                    {
                                        if (entity2 is LineGeometry line2 && entity1.GetEntityID() != entity2.GetEntityID())
                                        {
                                            if ((VectorManager.Distance(arc1.EndPoint1, line2.EndPoint1) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(arc1.EndPoint1, line2.EndPoint2) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(arc1.EndPoint2, line2.EndPoint2) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(arc1.EndPoint2, line2.EndPoint1) <= 0.0001))
                                            {
                                                var newFillet = GeometryManipulationManager.FilletTwoCurves(arc1, line2, 0.0020, mainGeo, mainGeo, true);
                                                if (newFillet != null)
                                                {
                                                    line2.Retrieve();
                                                    newFillet = filletCheck(newFillet, entity2);
                                                    newFillet.Retrieve();
                                                    newFillet.Commit();
                                                    levelTenList2.Add(newFillet);
                                                }
                                                arc1.Retrieve();
                                                arc1.Commit();
                                                line2.Retrieve();
                                                line2.Commit();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        var thisChain102 = ChainManager.ChainGeometry(levelTenList2.ToArray());
                        var roughDraftParams = roughSurfaceDraftParams1;
                        if (chain.Direction == ChainDirectionType.CounterClockwise) { roughDraftParams = roughSurfaceDraftParams2; }
                        foreach (var draftChain10 in thisChain102)
                        {


                            var draftSurface10 = SurfaceDraftInterop.CreateDrafts(draftChain10, roughDraftParams, false, 1);
                            foreach (var surface10 in draftSurface10)
                            {
                                if (Geometry.RetrieveEntity(surface10) is Geometry roughDraftSurface10)
                                {
                                    roughDraftSurface10.Level = roughSurf;
                                    roughDraftSurface10.Commit();
                                }
                            }
                        }

                    }
                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                    SelectionManager.UnselectAllGeometry();
                }
                void chain10Side2()
                {
                    foreach (var chain in chainList2)
                    {
                        var mainGeoSide2 = chain.OffsetChain2D(OffsetSideType.Left, .002, OffsetRollCornerType.All, .5, false, .005, false);
                        var resultChainGeo = SearchManager.GetResultGeometry();
                        foreach (var entity in resultChainGeo)
                        {
                            entity.Color = mainGeo;
                            entity.Level = mainGeo;
                            entity.Selected = false;
                            levelTenList2.Add(entity);
                            entity.Commit();
                        }
                        foreach (var entity1 in resultChainGeo)
                        {
                            if (entity1 is LineGeometry line1)
                            {
                                foreach (var entity2 in resultChainGeo)
                                {
                                    if (entity2 is LineGeometry line2 && entity1.GetEntityID() != entity2.GetEntityID())
                                    {
                                        if ((VectorManager.Distance(line1.EndPoint1, line2.EndPoint1) <= 0.0001)
                                            ||
                                            (VectorManager.Distance(line1.EndPoint1, line2.EndPoint2) <= 0.0001)
                                            ||
                                            (VectorManager.Distance(line1.EndPoint2, line2.EndPoint2) <= 0.0001)
                                            ||
                                            (VectorManager.Distance(line1.EndPoint2, line2.EndPoint1) <= 0.0001))
                                        {
                                            var newFillet = GeometryManipulationManager.FilletTwoCurves(line1, line2, 0.0020, mainGeo, mainGeo, true);
                                            if (newFillet != null)
                                            {
                                                line2.Retrieve();
                                                newFillet = filletCheck(newFillet, entity2);
                                                newFillet.Retrieve();
                                                newFillet.Commit();
                                                levelTenList2.Add(newFillet);
                                            }
                                            line1.Retrieve();
                                            line1.Commit();
                                            line2.Retrieve();
                                            line2.Commit();
                                        }
                                    }
                                    if (entity2 is ArcGeometry arc2 && entity1.GetEntityID() != entity2.GetEntityID())
                                    {
                                        if (arc2.Data.Radius > 0.002)
                                        {
                                            if ((VectorManager.Distance(line1.EndPoint1, arc2.EndPoint1) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(line1.EndPoint1, arc2.EndPoint2) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(line1.EndPoint2, arc2.EndPoint2) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(line1.EndPoint2, arc2.EndPoint1) <= 0.0001))
                                            {
                                                var newFillet = GeometryManipulationManager.FilletTwoCurves(line1, arc2, 0.0020, mainGeo, mainGeo, true);
                                                if (newFillet != null)
                                                {
                                                    arc2.Retrieve();
                                                    newFillet = filletCheck(newFillet, entity2);
                                                    newFillet.Retrieve();
                                                    newFillet.Commit();
                                                    levelTenList2.Add(newFillet);
                                                }
                                                line1.Retrieve();
                                                line1.Commit();
                                                arc2.Retrieve();
                                                arc2.Commit();
                                            }
                                        }
                                    }
                                }
                            }
                            if (entity1 is ArcGeometry arc1)
                            {
                                if (arc1.Data.Radius > 0.002)
                                {
                                    foreach (var entity2 in resultChainGeo)
                                    {
                                        if (entity2 is LineGeometry line2 && entity1.GetEntityID() != entity2.GetEntityID())
                                        {
                                            if ((VectorManager.Distance(arc1.EndPoint1, line2.EndPoint1) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(arc1.EndPoint1, line2.EndPoint2) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(arc1.EndPoint2, line2.EndPoint2) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(arc1.EndPoint2, line2.EndPoint1) <= 0.0001))
                                            {
                                                var newFillet = GeometryManipulationManager.FilletTwoCurves(arc1, line2, 0.0020, mainGeo, mainGeo, true);
                                                if (newFillet != null)
                                                {
                                                    line2.Retrieve();
                                                    newFillet = filletCheck(newFillet, entity2);
                                                    newFillet.Retrieve();
                                                    newFillet.Commit();
                                                    levelTenList2.Add(newFillet);
                                                }
                                                arc1.Retrieve();
                                                arc1.Commit();
                                                line2.Retrieve();
                                                line2.Commit();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        var thisChain102 = ChainManager.ChainGeometry(levelTenList2.ToArray());
                        var roughDraftParams = roughSurfaceDraftParams2;
                        if (chain.Direction == ChainDirectionType.CounterClockwise) { roughDraftParams = roughSurfaceDraftParams1; }
                        foreach (var draftChain10 in thisChain102)
                        {
                            var draftSurface10 = SurfaceDraftInterop.CreateDrafts(draftChain10, roughDraftParams, false, 1);
                            foreach (var surface10 in draftSurface10)
                            {
                                if (Geometry.RetrieveEntity(surface10) is Geometry roughDraftSurface10)
                                {
                                    roughDraftSurface10.Level = roughSurf;
                                    roughDraftSurface10.Commit();
                                }
                            }
                        }
                    }
                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                    SelectionManager.UnselectAllGeometry();
                }
                void chain12Side1()
                {
                    foreach (var chain in chainList3)
                    {
                        var cleanOutSide1 = chain.OffsetChain2D(OffsetSideType.Right, .0025, OffsetRollCornerType.All, .5, false, .005, false);
                        var resultChainGeo = SearchManager.GetResultGeometry();
                        foreach (var entity in resultChainGeo)
                        {
                            entity.Color = cleanOut;
                            entity.Level = cleanOut;
                            entity.Selected = false;
                            entity.Commit();
                        }
                        foreach (var entity1 in resultChainGeo)
                        {
                            if (entity1 is LineGeometry line1)
                            {
                                foreach (var entity2 in resultChainGeo)
                                {
                                    if (entity2 is LineGeometry line2 && entity1.GetEntityID() != entity2.GetEntityID())
                                    {
                                        if ((VectorManager.Distance(line1.EndPoint1, line2.EndPoint1) <= 0.0001)
                                            ||
                                            (VectorManager.Distance(line1.EndPoint1, line2.EndPoint2) <= 0.0001)
                                            ||
                                            (VectorManager.Distance(line1.EndPoint2, line2.EndPoint2) <= 0.0001)
                                            ||
                                            (VectorManager.Distance(line1.EndPoint2, line2.EndPoint1) <= 0.0001))
                                        {
                                            var newFillet = GeometryManipulationManager.FilletTwoCurves(line1, line2, 0.0025, cleanOut, cleanOut, true);
                                            if (newFillet != null)
                                            {
                                                line2.Retrieve();
                                                newFillet = filletCheck(newFillet, entity2);
                                                newFillet.Retrieve();
                                                newFillet.Commit();
                                            }
                                            line1.Retrieve();
                                            line1.Commit();
                                            line2.Retrieve();
                                            line2.Commit();
                                        }
                                    }
                                    if (entity2 is ArcGeometry arc2 && entity1.GetEntityID() != entity2.GetEntityID())
                                    {
                                        if (arc2.Data.Radius > 0.0025)
                                        {

                                            if ((VectorManager.Distance(line1.EndPoint1, arc2.EndPoint1) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(line1.EndPoint1, arc2.EndPoint2) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(line1.EndPoint2, arc2.EndPoint2) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(line1.EndPoint2, arc2.EndPoint1) <= 0.0001))
                                            {
                                                var newFillet = GeometryManipulationManager.FilletTwoCurves(line1, arc2, 0.0025, cleanOut, cleanOut, true);
                                                if (newFillet != null)
                                                {
                                                    arc2.Retrieve();
                                                    newFillet = filletCheck(newFillet, entity2);
                                                    newFillet.Retrieve();
                                                    newFillet.Commit();
                                                }
                                                line1.Retrieve();
                                                line1.Commit();
                                                arc2.Retrieve();
                                                arc2.Commit();
                                            }
                                        }
                                    }
                                }
                            }
                            if (entity1 is ArcGeometry arc1)
                            {
                                if (arc1.Data.Radius > 0.0025)
                                {

                                    foreach (var entity2 in resultChainGeo)
                                    {
                                        if (entity2 is LineGeometry line2 && entity1.GetEntityID() != entity2.GetEntityID())
                                        {
                                            if ((VectorManager.Distance(arc1.EndPoint1, line2.EndPoint1) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(arc1.EndPoint1, line2.EndPoint2) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(arc1.EndPoint2, line2.EndPoint2) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(arc1.EndPoint2, line2.EndPoint1) <= 0.0001))
                                            {
                                                var newFillet = GeometryManipulationManager.FilletTwoCurves(arc1, line2, 0.0025, cleanOut, cleanOut, true);
                                                if (newFillet != null)
                                                {
                                                    line2.Retrieve();
                                                    newFillet = filletCheck(newFillet, entity2);
                                                    newFillet.Retrieve();
                                                    newFillet.Commit();
                                                }
                                                arc1.Retrieve();
                                                arc1.Commit();
                                                line2.Retrieve();
                                                line2.Commit();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                    SelectionManager.UnselectAllGeometry();
                }
                void chain12Side2()
                {
                    foreach (var chain in chainList4)
                    {
                        var cleanOutSide2 = chain.OffsetChain2D(OffsetSideType.Left, .0025, OffsetRollCornerType.All, .5, false, .005, false);
                        var resultChainGeo = SearchManager.GetResultGeometry();
                        foreach (var entity in resultChainGeo)
                        {
                            entity.Color = cleanOut;
                            entity.Level = cleanOut;
                            entity.Selected = false;
                            entity.Commit();
                        }
                        foreach (var entity1 in resultChainGeo)
                        {
                            if (entity1 is LineGeometry line1)
                            {
                                foreach (var entity2 in resultChainGeo)
                                {
                                    if (entity2 is LineGeometry line2 && entity1.GetEntityID() != entity2.GetEntityID())
                                    {
                                        if ((VectorManager.Distance(line1.EndPoint1, line2.EndPoint1) <= 0.0001)
                                            ||
                                            (VectorManager.Distance(line1.EndPoint1, line2.EndPoint2) <= 0.0001)
                                            ||
                                            (VectorManager.Distance(line1.EndPoint2, line2.EndPoint2) <= 0.0001)
                                            ||
                                            (VectorManager.Distance(line1.EndPoint2, line2.EndPoint1) <= 0.0001))
                                        {
                                            var newFillet = GeometryManipulationManager.FilletTwoCurves(line1, line2, 0.0025, cleanOut, cleanOut, true);
                                            if (newFillet != null)
                                            {
                                                line2.Retrieve();
                                                newFillet = filletCheck(newFillet, entity2);
                                                newFillet.Retrieve();
                                                newFillet.Commit();
                                            }
                                            line1.Retrieve();
                                            line1.Commit();
                                            line2.Retrieve();
                                            line2.Commit();
                                        }
                                    }
                                    if (entity2 is ArcGeometry arc2 && entity1.GetEntityID() != entity2.GetEntityID())
                                    {
                                        if (arc2.Data.Radius > 0.0025)
                                        {

                                            if ((VectorManager.Distance(line1.EndPoint1, arc2.EndPoint1) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(line1.EndPoint1, arc2.EndPoint2) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(line1.EndPoint2, arc2.EndPoint2) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(line1.EndPoint2, arc2.EndPoint1) <= 0.0001))
                                            {
                                                var newFillet = GeometryManipulationManager.FilletTwoCurves(line1, arc2, 0.0025, cleanOut, cleanOut, true);
                                                if (newFillet != null)
                                                {
                                                    arc2.Retrieve();
                                                    newFillet = filletCheck(newFillet, entity2);
                                                    newFillet.Retrieve();
                                                    newFillet.Commit();
                                                }
                                                line1.Retrieve();
                                                line1.Commit();
                                                arc2.Retrieve();
                                                arc2.Commit();
                                            }
                                        }
                                    }
                                }
                            }
                            if (entity1 is ArcGeometry arc1)
                            {
                                if (arc1.Data.Radius > 0.0025)
                                {

                                    foreach (var entity2 in resultChainGeo)
                                    {
                                        if (entity2 is LineGeometry line2 && entity1.GetEntityID() != entity2.GetEntityID())
                                        {
                                            if ((VectorManager.Distance(arc1.EndPoint1, line2.EndPoint1) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(arc1.EndPoint1, line2.EndPoint2) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(arc1.EndPoint2, line2.EndPoint2) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(arc1.EndPoint2, line2.EndPoint1) <= 0.0001))
                                            {
                                                var newFillet = GeometryManipulationManager.FilletTwoCurves(arc1, line2, 0.0025, cleanOut, cleanOut, true);
                                                if (newFillet != null)
                                                {
                                                    line2.Retrieve();
                                                    newFillet = filletCheck(newFillet, entity2);
                                                    newFillet.Retrieve();
                                                    newFillet.Commit();
                                                }
                                                arc1.Retrieve();
                                                arc1.Commit();
                                                line2.Retrieve();
                                                line2.Commit();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                    SelectionManager.UnselectAllGeometry();
                }
                void chain139Side1()
                {
                    foreach (var chain in chainList6)
                    {
                        var finishSurfSide1 = chain.OffsetChain2D(OffsetSideType.Right, .0005, OffsetRollCornerType.All, .5, false, .0005, false);
                        var resultChainGeo = SearchManager.GetResultGeometry();
                        foreach (var entity in resultChainGeo)
                        {
                            entity.Color = finishSurf;
                            entity.Level = finishSurf;
                            entity.Selected = true;
                            level139List2.Add(entity);
                            entity.Commit();
                        }
                        foreach (var entity1 in resultChainGeo)
                        {
                            if (entity1 is LineGeometry line1)
                            {
                                foreach (var entity2 in resultChainGeo)
                                {
                                    if (entity2 is LineGeometry line2 && entity1.GetEntityID() != entity2.GetEntityID())
                                    {
                                        if ((VectorManager.Distance(line1.EndPoint1, line2.EndPoint1) <= 0.00001)
                                            ||
                                            (VectorManager.Distance(line1.EndPoint1, line2.EndPoint2) <= 0.00001)
                                            ||
                                            (VectorManager.Distance(line1.EndPoint2, line2.EndPoint2) <= 0.00001)
                                            ||
                                            (VectorManager.Distance(line1.EndPoint2, line2.EndPoint1) <= 0.00001))
                                        {
                                            var newFillet = GeometryManipulationManager.FilletTwoCurves(line1, line2, 0.0005, finishSurf, finishSurf, true);
                                            if (newFillet != null)
                                            {
                                                line2.Retrieve();
                                                newFillet = filletCheck(newFillet, entity2);
                                                newFillet.Retrieve();
                                                newFillet.Commit();
                                                level139List2.Add(newFillet);
                                            }
                                            line1.Retrieve();
                                            line1.Commit();
                                            line2.Retrieve();
                                            line2.Commit();
                                        }
                                    }
                                    if (entity2 is ArcGeometry arc2 && entity1.GetEntityID() != entity2.GetEntityID())
                                    {
                                        if (arc2.Data.Radius > 0.0005)
                                        {
                                            if ((VectorManager.Distance(line1.EndPoint1, arc2.EndPoint1) <= 0.00001)
                                                ||
                                                (VectorManager.Distance(line1.EndPoint1, arc2.EndPoint2) <= 0.00001)
                                                ||
                                                (VectorManager.Distance(line1.EndPoint2, arc2.EndPoint2) <= 0.00001)
                                                ||
                                                (VectorManager.Distance(line1.EndPoint2, arc2.EndPoint1) <= 0.00001))
                                            {
                                                var newFillet = GeometryManipulationManager.FilletTwoCurves(line1, arc2, 0.0005, finishSurf, finishSurf, true);
                                                if (newFillet != null)
                                                {
                                                    arc2.Retrieve();
                                                    newFillet = filletCheck(newFillet, entity2);
                                                    newFillet.Retrieve();
                                                    newFillet.Commit();
                                                    level139List2.Add(newFillet);
                                                }
                                                line1.Retrieve();
                                                line1.Commit();
                                                arc2.Retrieve();
                                                arc2.Commit();
                                            }
                                        }
                                    }
                                }

                            }
                            if (entity1 is ArcGeometry arc1)
                            {
                                if (arc1.Data.Radius > 0.0005)
                                {
                                    foreach (var entity2 in resultChainGeo)
                                    {
                                        if (entity2 is LineGeometry line2 && entity1.GetEntityID() != entity2.GetEntityID())
                                        {
                                            if ((VectorManager.Distance(arc1.EndPoint1, line2.EndPoint1) <= 0.00001)
                                                ||
                                                (VectorManager.Distance(arc1.EndPoint1, line2.EndPoint2) <= 0.00001)
                                                ||
                                                (VectorManager.Distance(arc1.EndPoint2, line2.EndPoint2) <= 0.00001)
                                                ||
                                                (VectorManager.Distance(arc1.EndPoint2, line2.EndPoint1) <= 0.00001))
                                            {
                                                var newFillet = GeometryManipulationManager.FilletTwoCurves(arc1, line2, 0.0005, finishSurf, finishSurf, true);
                                                if (newFillet != null)
                                                {
                                                    line2.Retrieve();
                                                    newFillet = filletCheck(newFillet, entity2);
                                                    newFillet.Retrieve();
                                                    newFillet.Commit();
                                                    level139List2.Add(newFillet);
                                                }
                                                arc1.Retrieve();
                                                arc1.Commit();
                                                line2.Retrieve();
                                                line2.Commit();
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        var thisChain1392 = ChainManager.ChainGeometry(level139List2.ToArray());
                        var roughDraftParams = finishSurfaceDraftParams1;
                        if (chain.Direction == ChainDirectionType.CounterClockwise) { roughDraftParams = finishSurfaceDraftParams2; }
                        foreach (var draftChain139 in thisChain1392)
                        {
                            var draftSurface139 = SurfaceDraftInterop.CreateDrafts(draftChain139, roughDraftParams, false, 1);
                            foreach (var surface139 in draftSurface139)
                            {
                                if (Geometry.RetrieveEntity(surface139) is Geometry finishDraftSurface139)
                                {
                                    finishDraftSurface139.Level = finishSurf;
                                    finishDraftSurface139.Commit();
                                }
                            }
                        }
                    }
                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                    SelectionManager.UnselectAllGeometry();
                }
                void chain139Side2()
                {
                    foreach (var chain in chainList6)
                    {
                        var finishSurfSide2 = chain.OffsetChain2D(OffsetSideType.Left, .0005, OffsetRollCornerType.All, .5, false, .005, false);
                        var resultChainGeo = SearchManager.GetResultGeometry();
                        foreach (var entity in resultChainGeo)
                        {
                            entity.Color = finishSurf;
                            entity.Level = finishSurf;
                            entity.Selected = true;
                            level139List2.Add(entity);
                            entity.Commit();
                        }
                        foreach (var entity1 in resultChainGeo)
                        {
                            if (entity1 is LineGeometry line1)
                            {
                                foreach (var entity2 in resultChainGeo)
                                {
                                    if (entity2 is LineGeometry line2 && entity1.GetEntityID() != entity2.GetEntityID())
                                    {
                                        if ((VectorManager.Distance(line1.EndPoint1, line2.EndPoint1) <= 0.0001)
                                            ||
                                            (VectorManager.Distance(line1.EndPoint1, line2.EndPoint2) <= 0.0001)
                                            ||
                                            (VectorManager.Distance(line1.EndPoint2, line2.EndPoint2) <= 0.0001)
                                            ||
                                            (VectorManager.Distance(line1.EndPoint2, line2.EndPoint1) <= 0.0001))
                                        {
                                            var newFillet = GeometryManipulationManager.FilletTwoCurves(line1, line2, 0.0005, finishSurf, finishSurf, true);
                                            if (newFillet != null)
                                            {
                                                line2.Retrieve();
                                                newFillet = filletCheck(newFillet, entity2);
                                                newFillet.Retrieve();
                                                newFillet.Commit();
                                                level139List2.Add(newFillet);
                                            }
                                            line1.Retrieve();
                                            line1.Commit();
                                            line2.Retrieve();
                                            line2.Commit();
                                        }
                                    }
                                    if (entity2 is ArcGeometry arc2 && entity1.GetEntityID() != entity2.GetEntityID())
                                    {
                                        if (arc2.Data.Radius > 0.0005)
                                        {
                                            if ((VectorManager.Distance(line1.EndPoint1, arc2.EndPoint1) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(line1.EndPoint1, arc2.EndPoint2) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(line1.EndPoint2, arc2.EndPoint2) <= 0.0001)
                                                ||
                                                (VectorManager.Distance(line1.EndPoint2, arc2.EndPoint1) <= 0.0001))
                                            {
                                                var newFillet = GeometryManipulationManager.FilletTwoCurves(line1, arc2, 0.0005, finishSurf, finishSurf, true);
                                                if (newFillet != null)
                                                {
                                                    arc2.Retrieve();
                                                    newFillet = filletCheck(newFillet, entity2);
                                                    newFillet.Retrieve();
                                                    newFillet.Commit();
                                                    level139List2.Add(newFillet);
                                                }
                                                line1.Retrieve();
                                                line1.Commit();
                                                arc2.Retrieve();
                                                arc2.Commit();
                                            }
                                        }
                                    }
                                }

                            }
                            if (entity1 is ArcGeometry arc1)
                            {
                                if (arc1.Data.Radius > 0.0005)
                                {
                                    foreach (var entity2 in resultChainGeo)
                                    {
                                        if (entity2 is LineGeometry line2 && entity1.GetEntityID() != entity2.GetEntityID())
                                        {
                                            if ((VectorManager.Distance(arc1.EndPoint1, line2.EndPoint1) <= 0.001)
                                                ||
                                                (VectorManager.Distance(arc1.EndPoint1, line2.EndPoint2) <= 0.001)
                                                ||
                                                (VectorManager.Distance(arc1.EndPoint2, line2.EndPoint2) <= 0.001)
                                                ||
                                                (VectorManager.Distance(arc1.EndPoint2, line2.EndPoint1) <= 0.001))
                                            {
                                                var newFillet = GeometryManipulationManager.FilletTwoCurves(arc1, line2, 0.0005, finishSurf, finishSurf, true);
                                                if (newFillet != null)
                                                {
                                                    line2.Retrieve();
                                                    newFillet = filletCheck(newFillet, entity2);
                                                    newFillet.Retrieve();
                                                    newFillet.Commit();
                                                    level139List2.Add(newFillet);
                                                }
                                                arc1.Retrieve();
                                                arc1.Commit();
                                                line2.Retrieve();
                                                line2.Commit();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        var thisChain1392 = ChainManager.ChainGeometry(level139List2.ToArray());
                        var roughDraftParams = finishSurfaceDraftParams2;
                        if (chain.Direction == ChainDirectionType.CounterClockwise) { roughDraftParams = finishSurfaceDraftParams1; }
                        foreach (var draftChain139 in thisChain1392)
                        {
                            var draftSurface139 = SurfaceDraftInterop.CreateDrafts(draftChain139, roughDraftParams, false, 1);
                            foreach (var surface139 in draftSurface139)
                            {
                                if (Geometry.RetrieveEntity(surface139) is Geometry finishDraftSurface139)
                                {
                                    finishDraftSurface139.Level = finishSurf;
                                    finishDraftSurface139.Commit();
                                }
                            }
                        }
                    }
                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                    SelectionManager.UnselectAllGeometry();
                }
                void clearLists()
                {
                    levelTenList1.Clear();
                    level139List1.Clear();
                    levelTenList2.Clear();
                    level139List2.Clear();
                }
            }
            void deSelect()
            {
                var selectedGeo = SearchManager.GetGeometry();
                foreach (var entity in selectedGeo)
                {
                    entity.Retrieve();
                    entity.Selected = false;
                    entity.Commit();
                }
            }
            deSelect();
            offsetCutchain();
            deSelect();
            GraphicsManager.Repaint(true);
            return MCamReturn.NoErrors;
        }
    }
}