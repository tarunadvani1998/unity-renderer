using System;
using System.Collections;
using System.Reflection;
using DCL;
using DCL.Camera;
using DCL.ECS7;
using DCL.ECSComponents;
using DCL.ECSRuntime;
using DCL.Interface;
using DCL.Models;
using UnityEngine;
using Environment = DCL.Environment;

public class ECSTestScene : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadScene(SceneScript));
    }

    private static void SceneScript(string sceneId, IECSComponentWriter componentWriter)
    {
        componentWriter.PutComponent(sceneId, 1, 1,
            new ECSTransform() { position = new UnityEngine.Vector3(8, 1, 8) });
        AddAvatarShapeComponent(sceneId, componentWriter);
        AddBoxComponent(sceneId, componentWriter);
    }

    private static void AddAvatarShapeComponent(string sceneId, IECSComponentWriter componentWriter)
    {
        PBAvatarShape avatarShape = new PBAvatarShape();
        avatarShape.Id = "0xe7dd153081b0526e0a8c582497cbcee7cd44e464";
        avatarShape.Name = "TestName#2354";
        avatarShape.BodyShape = "urn:decentraland:off-chain:base-avatars:BaseFemale";
        avatarShape.ExpressionTriggerId = "Idle";
        
        avatarShape.EyeColor = new Color3();
        avatarShape.EyeColor.R = 0.223f;
        avatarShape.EyeColor.G = 0.484f;
        avatarShape.EyeColor.B = 0.691f;   
        
        avatarShape.HairColor = new Color3();
        avatarShape.HairColor.R = 0.223f;
        avatarShape.HairColor.G = 0.484f;
        avatarShape.HairColor.B = 0.691f;
        
        avatarShape.SkinColor = new Color3();
        avatarShape.SkinColor.R = 0.223f;
        avatarShape.SkinColor.G = 0.484f;
        avatarShape.SkinColor.B = 0.691f;
        
        avatarShape.Wearables.Add("urn:decentraland:off-chain:base-avatars:f_eyebrows_07");
        avatarShape.Wearables.Add("urn:decentraland:off-chain:base-avatars:eyes_02");
        avatarShape.Wearables.Add("urn:decentraland:off-chain:base-avatars:f_mouth_03");
        avatarShape.Wearables.Add("urn:decentraland:off-chain:base-avatars:f_school_skirt");
        avatarShape.Wearables.Add("urn:decentraland:off-chain:base-avatars:SchoolShoes");
        avatarShape.Wearables.Add("urn:decentraland:matic:collections-v2:0x177535a421e7867ec52f2cc19b7dfed4f289a2bb:0");
        avatarShape.Wearables.Add("urn:decentraland:matic:collections-v2:0xd89efd0be036410d4ff194cd6ecece4ef8851d86:1");
        avatarShape.Wearables.Add("urn:decentraland:matic:collections-v2:0x1df3011a14ea736314df6cdab4fff824c5d46ec1:0");
        avatarShape.Wearables.Add("urn:decentraland:matic:collections-v2:0xbada8a315e84e4d78e3b6914003647226d9b4001:1");
        avatarShape.Wearables.Add("urn:decentraland:matic:collections-v2:0x1df3011a14ea736314df6cdab4fff824c5d46ec1:5");
        avatarShape.Wearables.Add("urn:decentraland:matic:collections-v2:0xd89efd0be036410d4ff194cd6ecece4ef8851d86:0");
        
        componentWriter.PutComponent(sceneId, 4, ComponentID.AVATAR_SHAPE,
            avatarShape);
    }

    private static void AddGLTFShapeComponent(string sceneId, IECSComponentWriter componentWriter)
    {
        Environment.i.world.state.scenesSortedByDistance[0].contentProvider.baseUrl = "https://peer.decentraland.org/content/contents/";
        Environment.i.world.state.scenesSortedByDistance[0].contentProvider.fileToHash.Add("models/SCENE.glb".ToLower(), "QmQgQtuAg9qsdrmLwnFiLRAYZ6Du4Dp7Yh7bw7ELn7AqkD");
            
        componentWriter.PutComponent(sceneId, 2, ComponentID.GLTF_SHAPE,
            new PBGLTFShape() { Src = "models/SCENE.glb", Visible = true});
    }

    private static void AddBoxComponent(string sceneId, IECSComponentWriter componentWriter)
    {
        PBBoxShape model = new PBBoxShape();
        model.Visible = true;
        model.WithCollisions = true;
        componentWriter.PutComponent(sceneId,2,ComponentID.BOX_SHAPE,
            model );
    }
    
    private static void AddNFTComponent(string sceneId, IECSComponentWriter componentWriter)
    {
        PBNFTShape model = new PBNFTShape();
        model.Src = "ethereum://0x06012c8cf97bead5deae237070f9587f8e7a266d/1540722";
        model.Visible = true;
        model.WithCollisions = true;
        model.Color = new Color3();
        model.Style = 6;
        model.Color.R = 0.5f;
        model.Color.G = 0.5f;
        model.Color.B = 1f;
        componentWriter.PutComponent(sceneId,0,ComponentID.NFT_SHAPE,
            model );
    }

    private static IEnumerator LoadScene(Action<string, IECSComponentWriter> sceneScript)
    {
        LoadParcelScenesMessage.UnityParcelScene scene = new LoadParcelScenesMessage.UnityParcelScene()
        {
            basePosition = new Vector2Int(0, 0),
            parcels = new Vector2Int[] { new Vector2Int(0, 0) },
            id = "temptation"
        };

        bool started = false;

        WebInterface.OnMessageFromEngine += (type, s1) =>
        {
            if (type == "SystemInfoReport")
            {
                started = true;
            }
        };

        yield return new WaitUntil(() => started);
        yield return new WaitForSeconds(2);

        var mainGO = GameObject.Find("Main");
        mainGO.SendMessage("SetDebug");

        Environment.i.world.sceneController.LoadParcelScenes(JsonUtility.ToJson(scene));
        var playerAvatarController = FindObjectOfType<PlayerAvatarController>();
        CommonScriptableObjects.rendererState.RemoveLock(playerAvatarController);

        yield return new WaitForSeconds(1);

        ECS7Plugin ecs7Plugin = new ECS7Plugin();
        IECSComponentWriter componentWriter = typeof(ECS7Plugin)
                                              .GetField("componentWriter", BindingFlags.NonPublic | BindingFlags.Instance)
                                              .GetValue(ecs7Plugin) as IECSComponentWriter;

        sceneScript.Invoke(scene.id, componentWriter);

        mainGO.SendMessage("ActivateRendering");

        var message = new QueuedSceneMessage_Scene
        {
            sceneId = scene.id,
            tag = "",
            type = QueuedSceneMessage.Type.SCENE_MESSAGE,
            method = MessagingTypes.INIT_DONE,
            payload = new Protocol.SceneReady()
        };
        Environment.i.world.sceneController.EnqueueSceneMessage(message);

        var cameraConfig = new CameraController.SetRotationPayload()
        {
            x = 0,
            y = 0,
            z = 0,
            cameraTarget = new UnityEngine.Vector3(0, 0, 1)
        };
        CommonScriptableObjects.cameraMode.Set(CameraMode.ModeId.FirstPerson);
        var cameraController = GameObject.Find("CameraController");
        cameraController.SendMessage("SetRotation", JsonUtility.ToJson(cameraConfig));
    }
}