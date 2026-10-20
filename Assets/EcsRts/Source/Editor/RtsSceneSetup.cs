using EcsRts.Presentation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EcsRts.EditorTools
{
    /// <summary>
    /// 一键生成场景骨架（文档 §3 场景规划）。生成后按 Console 输出的步骤手工补齐 SubScene 引用与 Authoring。
    /// </summary>
    internal static class RtsSceneSetup
    {
        private const string ScenesDir = "Assets/EcsRts/Scenes";

        [MenuItem("RTS/创建场景骨架 (Boot + Gameplay)")]
        public static void CreateScenes()
        {
            if (!AssetDatabase.IsValidFolder(ScenesDir))
                AssetDatabase.CreateFolder("Assets/EcsRts", "Scenes");

            // 1) Gameplay：SubScene 内容载体（空场景；单位 prefab/刷兵器/障碍/设置后续拖入）
            var gameplay = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(gameplay, $"{ScenesDir}/Gameplay.unity");

            // 2) Boot：默认相机+灯光，挂相机桥并摆 RTS 俯视角
            var boot = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            var camera = Object.FindObjectOfType<Camera>();
            if (camera != null)
            {
                camera.gameObject.AddComponent<RtsCameraRig>();
                camera.transform.position = new Vector3(0f, 60f, -60f);
                camera.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
            }
            EditorSceneManager.SaveScene(boot, $"{ScenesDir}/Boot.unity");

            Debug.Log(
                "[RTS] 场景骨架已创建。后续步骤：\n" +
                "1. 打开 Boot：菜单 GameObject > Entities > SubScene 新建 SubScene，把 Gameplay 拖入其 Scene 引用（自动加载）；\n" +
                "2. Build Settings：加入 Boot（Gameplay 作为 SubScene 不直接进列表），并移除已失效的 EcsCrowdDemo 场景引用；\n" +
                "3. 在 SubScene 中编辑 Gameplay：建单位 prefab（图元+UnitAuthoring）→ 挂 UnitPrefabLibraryAuthoring、SpawnerAuthoring×2 阵营、GameSettingsAuthoring、ObstacleAuthoring；\n" +
                "4. Boot 中建 Canvas（含 TMP 文本）并挂 HudController，文本绑定 M1 补齐（文档 §7.9）。");
        }
    }
}
