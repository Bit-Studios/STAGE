using Ksp2Uitk.API;
using UnityEngine;
using UnityEngine.UIElements;
using ShadowUtilityLIB.UI;
using KSP.Messages;
using KSP.Sim.DeltaV;
using KSP.OAB;
using ShadowUtilityLIB;
using ShadowUtilityLIB.logging;
using Logger = ShadowUtilityLIB.logging.Logger;
using KSP.Game;
using System.Globalization;
using System.Collections;
using Shapes;

namespace STAGE.UI
{
    public class stageUI
    {
        private Logger logger = new Logger(StageMod.ModName, StageMod.ModVersion);
        public bool UIRunning = false;
        public bool use3d = false;
        public Dictionary<string, bool[]> StageWindows = new Dictionary<string, bool[]>();
        public stageUI()
        {

        }
        public void Init()
        {
            try
            {
                VisualElement root = Element.Root();
                root.style.width = 400;
                root.style.height = 500;
                root.style.left = 900;
                root.style.top = 300;
                root.style.backgroundColor = new StyleColor(new Color32(7, 9, 13, 255));
                root.style.borderRightColor = new StyleColor(new Color32(7, 9, 13, 255));
                root.style.borderLeftColor = new StyleColor(new Color32(7, 9, 13, 255));
                root.style.borderTopColor = new StyleColor(new Color32(7, 9, 13, 255));
                root.style.borderBottomColor = new StyleColor(new Color32(7, 9, 13, 255));
                root.AddManipulator(new DragManipulator());
                Label TitleElement = Element.Label("Title", "S.T.A.G.E");
                TitleElement.style.fontSize = 30;
                TitleElement.style.unityTextAlign = TextAnchor.MiddleCenter;
                TitleElement.enableRichText = true;
                root.Add(TitleElement);

                Label SelectedVessel = Element.Label("SelectedVessel", "Active Vessel: ");
                root.Add(SelectedVessel);
                Label DVseaTotal = Element.Label("DVseaTotal", "Δv (At Sea Level):");
                root.Add(DVseaTotal);
                Label DVvacTotal = Element.Label("DVvacTotal", "Δv (At Vacuum):");
                root.Add(DVvacTotal);
                Label BTTotal = Element.Label("BTTotal", "Burn Time:");
                root.Add(BTTotal);
                ScrollView StageList = Element.ScrollView("StageList");
                StageList.style.height = 150;
                StageList.style.paddingTop = 5;
                StageList.style.paddingBottom = 5;
                
                root.Add(StageList);
                UIDocument window = Window.CreateFromElement(root);
                window.rootVisualElement.visible = false;
                StageMod.manager.Add("stageUI", window);
            }
            catch (Exception e)
            {
                logger.Error($"{e.Message}\n{e.InnerException}\n{e.Source}\n{e.Data}\n{e.HelpLink}\n{e.HResult}\n{e.StackTrace}\n{e.TargetSite}");
            }
        }
        public void GenerateStageInfoWindow(int stageID, DeltaVStageInfo stageData)
        {
            VisualElement stageInfoWindow = Element.Root();
            stageInfoWindow.style.width = 250;
            stageInfoWindow.style.height = 200;
            stageInfoWindow.style.left = 900;
            stageInfoWindow.style.top = 300;
            stageInfoWindow.style.backgroundColor = new StyleColor(new Color32(7, 9, 13, 255));
            stageInfoWindow.style.borderRightColor = new StyleColor(new Color32(7, 9, 13, 255));
            stageInfoWindow.style.borderLeftColor = new StyleColor(new Color32(7, 9, 13, 255));
            stageInfoWindow.style.borderTopColor = new StyleColor(new Color32(7, 9, 13, 255));
            stageInfoWindow.style.borderBottomColor = new StyleColor(new Color32(7, 9, 13, 255));
            stageInfoWindow.AddManipulator(new DragManipulator());
            Label TitleElement = Element.Label($"Stage{stageID}Title", $"Stage: {stageID} {stageData.SeparationIndex} {stageData.PayloadStage}");
            TitleElement.style.fontSize = 30;
            TitleElement.style.unityTextAlign = TextAnchor.MiddleCenter;
            TitleElement.enableRichText = true;
            stageInfoWindow.Add(TitleElement);

            Label DVsea = Element.Label($"Stage{stageID}DVsea", $"Δv (At Sea Level): {stageData.DeltaVatASL.ToString("F3", CultureInfo.InvariantCulture)} m/s");
            stageInfoWindow.Add(DVsea);
            Label DVvac = Element.Label($"Stage{stageID}DVvac", $"Δv (At Vacuum): {stageData.DeltaVinVac.ToString("F3", CultureInfo.InvariantCulture)} m/s");
            stageInfoWindow.Add(DVvac);
            Label BT = Element.Label($"Stage{stageID}BT", $"Burn Time: {stageData.StageBurnTime.ToString("F3", CultureInfo.InvariantCulture)}s");
            stageInfoWindow.Add(BT);

            SliderInt stageIDlabel = Element.SliderInt($"Stage{stageID}stageIDlabel", stageID - 1, stageID + 1, stageID);
            stageIDlabel.visible = false;
            stageInfoWindow.Add(stageIDlabel);
            UIDocument window = Window.CreateFromElement(stageInfoWindow);
            window.rootVisualElement.visible = true;
            StageMod.manager.Add($"Stage{stageID}window", window);
        }
        public void StageInfoWindow(string StageString)
        {
            VesselDeltaVComponent vesselDeltaVComponent = GameObject.Find("OAB(Clone)").GetComponent<ObjectAssemblyBuilderInstance>().ActivePartTracker.partAssemblies.First().VesselDeltaV;
            int stageID = StageMod.manager.Get(StageString).rootVisualElement.Q<SliderInt>($"{StageString.Split('w')[0]}stageIDlabel").value;
            DeltaVStageInfo deltaVStageInfo = vesselDeltaVComponent.StageInfo[stageID];
            StageMod.manager.Get(StageString).rootVisualElement.Q<Label>($"Stage{stageID}DVsea").text = $"Δv (At Sea Level): {deltaVStageInfo.DeltaVatASL.ToString("F3", CultureInfo.InvariantCulture)} m/s";
            StageMod.manager.Get(StageString).rootVisualElement.Q<Label>($"Stage{stageID}DVvac").text = $"Δv (At Vacuum): {deltaVStageInfo.DeltaVinVac.ToString("F3", CultureInfo.InvariantCulture)} m/s";
            StageMod.manager.Get(StageString).rootVisualElement.Q<Label>($"Stage{stageID}BT").text = $"Burn Time: {deltaVStageInfo.StageBurnTime.ToString("F3", CultureInfo.InvariantCulture)}s";
        }
        public IEnumerator OABStageUpdate()
        {
            while (UIRunning)
            {
                try
                {
                    VesselDeltaVComponent vesselDeltaVComponent = GameObject.Find("OAB(Clone)").GetComponent<ObjectAssemblyBuilderInstance>().ActivePartTracker.partAssemblies.First().VesselDeltaV;
                    logger.Debug("OABStageUpdate - vesselDeltaVComponent");
                    StageMod.manager.Get("stageUI").rootVisualElement.Q<Label>("SelectedVessel").text = $"Active Vessel: {GameObject.Find("OAB(Clone)").GetComponent<ObjectAssemblyBuilderInstance>().Stats.CurrentWorkspaceVehicleDisplayName.GetValue()}";
                    logger.Debug("OABStageUpdate - SelectedVessel");
                    StageMod.manager.Get("stageUI").rootVisualElement.Q<Label>("DVseaTotal").text = $"Δv (At Sea Level): {vesselDeltaVComponent.TotalDeltaVASL.ToString("F3", CultureInfo.InvariantCulture)} m/s";
                    logger.Debug("OABStageUpdate - DVseaTotal");
                    StageMod.manager.Get("stageUI").rootVisualElement.Q<Label>("DVvacTotal").text = $"Δv (At Vacuum): {vesselDeltaVComponent.TotalDeltaVVac.ToString("F3", CultureInfo.InvariantCulture)} m/s";
                    logger.Debug("OABStageUpdate - DVvacTotal");
                    StageMod.manager.Get("stageUI").rootVisualElement.Q<Label>("BTTotal").text = $"Burn Time: {vesselDeltaVComponent.TotalBurnTime.ToString("F3", CultureInfo.InvariantCulture)}s";
                    logger.Debug("OABStageUpdate - BTTotal");
                    List<DeltaVStageInfo> stages = new List<DeltaVStageInfo>();
                    logger.Debug("OABStageUpdate - DeltaVStageInfo");
                    stages = vesselDeltaVComponent.StageInfo;
                    logger.Debug("OABStageUpdate - stages");
                    StageMod.manager.Get("stageUI").rootVisualElement.Q<ScrollView>($"StageList").Clear();
                    logger.Debug("OABStageUpdate - stageUI Clear");
                    StageWindows.ForEach(stageWindowID => {
                        logger.Debug("OABStageUpdate - StageWindows.ForEach");
                        stageWindowID.Value[0] = false;
                    });
                    foreach (var stage in stages.OrderBy(o => o.Stage))
                    {
                        logger.Debug("OABStageUpdate - foreach (var stage in stages.OrderBy(o => o.Stage))");
                        Button StageSelectButton = Element.Button($"Stage{stage.Stage}", $"Stage: {stage.Stage}");
                        logger.Debug("OABStageUpdate - StageSelectButton");
                        StageSelectButton.clickable = new Clickable(() => {
                            logger.Debug("StageSelectButton - clickable");
                            if (StageWindows.ContainsKey($"Stage{stage.Stage}window")){}else
                            {
                                logger.Debug("StageSelectButton - StageWindows.ContainsKey");
                                StageWindows.Add($"Stage{stage.Stage}window", new bool[] { false, true });
                                logger.Debug("StageSelectButton - StageWindows.Add");
                                GenerateStageInfoWindow(stage.Stage, stage);
                                logger.Debug("StageSelectButton - GenerateStageInfoWindow");
                            }
                            
                            
                        });
                        StageMod.manager.Get("stageUI").rootVisualElement.Q<ScrollView>($"StageList").Add(StageSelectButton);
                        if (StageWindows.ContainsKey($"Stage{stage.Stage}window"))
                        {
                            StageWindows[$"Stage{stage.Stage}window"][0] = true;
                        }
                    }
                    StageWindows.ForEach(stageWindowID =>
                    {
                        if (stageWindowID.Value[0])
                        {
                            StageMod.manager.Set(stageWindowID.Key, stageWindowID.Value[1]);
                            if (stageWindowID.Value[1])
                            {
                                StageInfoWindow(stageWindowID.Key);
                            }
                        }
                        else
                        {
                            StageMod.manager.Set(stageWindowID.Key, false);
                        }
                        
                    });
                }
                catch (Exception e)
                {
                    logger.Error($"{e.Message}\n{e.InnerException}\n{e.Source}\n{e.Data}\n{e.HelpLink}\n{e.HResult}\n{e.StackTrace}\n{e.TargetSite}");
                }
                yield return new WaitForSeconds(1);
            }
        }
    }
}


/*
 * OLD stuff
 * 
    private void FillWindow(int windowID)
    {
        GameStateConfiguration gameStateConfiguration = GameManager.Instance.Game.GlobalGameState.GetGameState();
        VesselDeltaVComponent vesselDeltaVComponent= new VesselDeltaVComponent();
        if (gameStateConfiguration.IsObjectAssembly)
        {
            var oab = GameObject.Find("OAB(Clone)");
            ObjectAssemblyBuilderInstance oabInstance = oab.GetComponent<ObjectAssemblyBuilderInstance>();
            vesselDeltaVComponent = oabInstance.ActivePartTracker.partAssemblies.First().VesselDeltaV;
        }
        else if (gameStateConfiguration.IsFlightMode)
        {
            vesselDeltaVComponent = GameManager.Instance.Game.ViewController.GetActiveSimVessel().VesselDeltaV;
        }
        else
        {

        }
        boxStyle = GUI.skin.GetStyle("Box");
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Vessel Totals");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label($"Burn Time: {vesselDeltaVComponent.TotalBurnTime}");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label($"Δv (At Sea Level): {vesselDeltaVComponent.TotalDeltaVASL}");
        GUILayout.Label($"Δv (At Vacuum): {vesselDeltaVComponent.TotalDeltaVVac}");
        GUILayout.EndHorizontal();
        List<DeltaVStageInfo> stages = new List<DeltaVStageInfo>();
        stages = vesselDeltaVComponent.StageInfo;
        foreach ( var stage in stages.OrderBy(o => o.Stage))
        {
            if (StageExpanded.Count >= vesselDeltaVComponent.StageInfo.Count) { }
            else
            {
                for (var i = StageExpanded.Count; i < vesselDeltaVComponent.StageInfo.Count; i++)
                {
                    StageExpanded.Add(false);
                }
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"Stage {stage.SeparationIndex}"))
            {
                StageExpanded[stage.Stage] = !StageExpanded[stage.Stage];
            }
            GUILayout.EndHorizontal();
            if (StageExpanded[stage.Stage])
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Burn Time: {stage.StageBurnTime}");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Δv (At Sea Level): {stage.DeltaVatASL}");
                GUILayout.Label($"Δv (At Vacuum): {stage.DeltaVinVac}");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label($"TWR (At Sea Level): {stage.TWRASL}");
                GUILayout.Label($"TWR (At Vacuum): {stage.TWRVac}");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label($"ISP (At Sea Level): {stage.IspASL}");
                GUILayout.Label($"ISP (At Vacuum): {stage.IspVac}");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Stage Mass: {stage.StageMass}");
                GUILayout.Label($"Fuel Mass: {stage.FuelMass}");
                GUILayout.Label($"Dry Mass: {stage.DryMass}");
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndVertical();
        GUI.DragWindow(new Rect(0, 0, windowWidth, 700));
    }
    */