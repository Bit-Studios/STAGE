using UnityEngine;
using KSP.Game;
using SpaceWarp.API.Mods;
using BepInEx;
using KSP.Sim.DeltaV;
using ShadowUtilityLIB;
using Logger = ShadowUtilityLIB.logging.Logger;
using SpaceWarp.API.Assets;
using SpaceWarp;
using ShadowUtilityLIB.UI;
using KSP.Messages;
using STAGE.UI;

namespace STAGE;
[BepInPlugin("com.shadowdev.stage", "S.T.A.G.E", "0.0.1")]
[BepInDependency(ShadowUtilityLIBMod.ModId, ShadowUtilityLIBMod.ModVersion)]
[BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
public class StageMod : BaseSpaceWarpPlugin
{
    public static string ModId = "com.shadowdev.stage";
    public static string ModName = "S.T.A.G.E";
    public static string ModVersion = "0.0.1";
    private static Logger logger = new Logger(ModName, ModVersion);
    public static stageUI stageUI;
    public static Manager manager;
    private static bool IsDev = true;
    public static float updateSpeed = 1.0f;
    public static int colorChange = 13;
    public static int[] rgbOffset = new int[] { 0, 22, 59 };
    public override void OnInitialized()
    {
        AppBar.Add(AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/Flightbutton.png"), AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/OABbutton.png"), "S.T.A.G.E", "BTN-STAGE", ToggleButtonFlight, ToggleButtonOAB, new bool[]{true,true});
        logger.Log("Registered AppBar");
        manager = new Manager();
        stageUI = new stageUI();
        stageUI.Init();
        if (IsDev)
        {

        }
        /*
         * Tried using message system but not reliable as we dont yet know what all the messages are that we need
         * 
        GameManager.Instance.Game.Messages.Subscribe<PartStageChangedMessage>(stageUI.OABStageUpdate);
        GameManager.Instance.Game.Messages.Subscribe<PartPlacedMessage>(stageUI.OABStageUpdate);
        GameManager.Instance.Game.Messages.Subscribe<PartManipulationCompletedMessage>(stageUI.OABStageUpdate);
        GameManager.Instance.Game.Messages.Subscribe<PartRemovedFromMainAssemblyMessage>(stageUI.OABStageUpdate);
        GameManager.Instance.Game.Messages.Subscribe<GameStateChangedMessage>(stageUI.OABStageUpdate);
        GameManager.Instance.Game.Messages.Subscribe<OABMainAssemblyChanged>(stageUI.OABStageUpdate);
        GameManager.Instance.Game.Messages.Subscribe<OABNewAssemblyMessage>(stageUI.OABStageUpdate);
        GameManager.Instance.Game.Messages.Subscribe<OABNewAssemblyMessage>(stageUI.OABStageUpdate);
        */
        GameManager.Instance.Game.Messages.Subscribe<GameStateChangedMessage>(StateChange);
        logger.Log("Initialized");
    }
    void Awake()
    {
        if (IsDev)
        {
            ShadowUtilityLIBMod.EnableDebugMode();
        }
    }
    void ToggleButtonOAB(bool toggle)
    {
        try
        {
            AppBar.SetValue("BTN-STAGE", toggle);
            manager.Set("stageUI", toggle);
            logger.Debug("Pressed toggle in OAB");
        }
        catch (Exception e)
        {
            logger.Error($"{e.Message}\n{e.InnerException}\n{e.Source}\n{e.Data}\n{e.HelpLink}\n{e.HResult}\n{e.StackTrace}\n{e.TargetSite}");
        }
    }
    void ToggleButtonFlight(bool toggle)
    {
        try
        {
            AppBar.SetValue("BTN-STAGE", toggle);
            manager.Set("stageUI", toggle);
            logger.Debug("Pressed toggle in Flight");
        }
        catch (Exception e)
        {
            logger.Error($"{e.Message}\n{e.InnerException}\n{e.Source}\n{e.Data}\n{e.HelpLink}\n{e.HResult}\n{e.StackTrace}\n{e.TargetSite}");
        }
    }
    public static void StateChange(MessageCenterMessage messageCenterMessage)
    {
        try
        {
            GameStateChangedMessage gameStateChangedMessage = messageCenterMessage as GameStateChangedMessage;
            stageUI.UIRunning = false;
            if (gameStateChangedMessage.CurrentState == GameState.VehicleAssemblyBuilder)
            {
                stageUI.UIRunning = true;
                ShadowUtilityLIBMod.RunCr(stageUI.OABStageUpdate());
            }
            if (gameStateChangedMessage.CurrentState == GameState.FlightView)
            {
                //stageUI.UIRunning = true;
                //ShadowUtilityLIBMod.RunCr(stageUI.OABStageUpdate());
            }
        }
        catch (Exception e)
        {
            logger.Error($"{e.Message}\n{e.InnerException}\n{e.Source}\n{e.Data}\n{e.HelpLink}\n{e.HResult}\n{e.StackTrace}\n{e.TargetSite}");
        }
    }
}