using Mastercam.Database;
using Mastercam.IO;
using Mastercam.Database.Types;
using Mastercam.App.Types;
using Mastercam.GeometryUtility;
using Mastercam.Support;
using Mastercam.Database.Interop;
using Mastercam.Surfaces;

namespace _GeoAssistConvSide
{
    public class GeoAssistConvSide : Mastercam.App.NetHook3App
    {
        public Mastercam.App.Types.MCamReturn GeoAssistConvSideRun(Mastercam.App.Types.MCamReturn notused)
        {
            void offsetCutchain(){
                SelectionManager.UnselectAllGeometry();
                LevelsManager.RefreshLevelsManager();
                GraphicsManager.Repaint(true);
                int mainGeo = 10;
                int cleanOut = 12;
                int roughSurf = 138;
                int finishSurf = 139;
                SurfaceDraftParams roughSurfaceDraftParams = new SurfaceDraftParams {
                    draftMethod=SurfaceDraftParams.DraftMethod.Length,
                    geometryType=SurfaceDraftParams.GeometryType.Surface,   
                    length=-0.100,
                    angle=Mastercam.Math.VectorManager.RadiansToDegrees(15),
                    draftDirection=SurfaceDraftParams.DraftDirection.Defined
                    } ;
                SurfaceDraftParams finishSurfaceDraftParams = new SurfaceDraftParams{
                    draftMethod = SurfaceDraftParams.DraftMethod.Length,
                    geometryType = SurfaceDraftParams.GeometryType.Surface,
                    length = -0.100,
                    angle = Mastercam.Math.VectorManager.RadiansToDegrees(20),
                    draftDirection = SurfaceDraftParams.DraftDirection.Defined
                };
                var selectedCutChain = ChainManager.GetMultipleChains("Select Geometry");
                foreach (var chain in selectedCutChain){
                    var mainGeoSide1 = chain.OffsetChain2D(OffsetSideType.Right, .002, OffsetRollCornerType.All, .5, false, .005, false);
                    var mainGeoResult = SearchManager.GetResultGeometry();
                    foreach (var entity in mainGeoResult){
                        entity.Color = mainGeo;
                        entity.Level = mainGeo;  
                        entity.Selected = false;
                        entity.Commit();
                    }
                    var thisChain10 = ChainManager.ChainGeometry(mainGeoResult);
                    foreach (var draftChain10 in thisChain10) {
                        var draftSurface10 = SurfaceDraftInterop.CreateDrafts(draftChain10, roughSurfaceDraftParams, false, 1);
                        foreach (var surface10 in draftSurface10){
                            if (Geometry.RetrieveEntity(surface10) is Geometry roughDraftSurface10) {
                                roughDraftSurface10.Level = roughSurf;
                                roughDraftSurface10.Commit();
                            }
                        }
                    }
                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                    SelectionManager.UnselectAllGeometry();
                    var cleanOutSide1 = chain.OffsetChain2D(OffsetSideType.Right, .0025, OffsetRollCornerType.All, .5, false, .005, false);
                    var cleanOutResult = SearchManager.GetResultGeometry();
                    foreach (var entity in cleanOutResult){
                        entity.Level = cleanOut;
                        entity.Color = cleanOut;
                        entity.Selected = false;
                        entity.Commit();
                    }
                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                    SelectionManager.UnselectAllGeometry();
                    var finishSurfSide1 = chain.OffsetChain2D(OffsetSideType.Right, .0005, OffsetRollCornerType.All, .5, false, .005, false);
                    var finishSurfResult = SearchManager.GetResultGeometry();
                    foreach (var entity in finishSurfResult){
                        entity.Color = finishSurf;
                        entity.Level = finishSurf;
                        entity.Selected = false;
                        entity.Commit();
                    }
                    var thisChain139 = ChainManager.ChainGeometry(finishSurfResult);
                    foreach (var draftChain139 in thisChain139){
                        var draftSurface139 = SurfaceDraftInterop.CreateDrafts(draftChain139, finishSurfaceDraftParams, false, 1);
                        foreach (var surface139 in draftSurface139){
                            if (Geometry.RetrieveEntity(surface139) is Geometry finishDraftSurface139){
                                finishDraftSurface139.Level = finishSurf;
                                finishDraftSurface139.Commit();
                            }
                        }
                    }
                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                    SelectionManager.UnselectAllGeometry();
                }
            }
            offsetCutchain();
            GraphicsManager.Repaint(true);
            return MCamReturn.NoErrors;
        }
    }
}