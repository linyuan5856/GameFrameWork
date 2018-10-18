using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Excel;
using System.Reflection;


namespace Pandora
{
    public class ProjectBuild
    {
        private static readonly List<string> Levels = new List<string>();

        [MenuItem("program/Build/CleanCache")]
        private static void CleanBundleCache()
        {
            Caching.ClearCache();
        }

        [MenuItem("program/GameObject/ShowPath", false, 10)]
        private static void ExplodeNow3()
        {
            if (Selection.activeGameObject == null)
                return;
            EditorUtility.DisplayDialog("GameObject", Selection.activeGameObject.GetPath(), "I See!", "CopyPath");
        }

        [MenuItem("program/Build/BuildAPK")]
        public static void BuildForAndroid()
        {
            foreach (var scene in EditorBuildSettings.scenes.Where(scene => scene.enabled))
            {
                Levels.Add(scene.path);
            }
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            if (Environment.GetCommandLineArgs()[4] == "1")
            {
            }
            PlayerSettings.Android.keystoreName = Environment.GetCommandLineArgs()[2];
            PlayerSettings.Android.keystorePass = "HanFeng";
            PlayerSettings.Android.keyaliasName = "Pandora";
            PlayerSettings.Android.keyaliasPass = "HanFeng";
            var rs = BuildPipeline.BuildPlayer(Levels.ToArray(), Environment.GetCommandLineArgs()[1], BuildTarget.Android, BuildOptions.None);
            WriteFile(Environment.GetCommandLineArgs()[3], rs);
            EditorApplication.Exit(0);
        }

        [MenuItem("program/Build/BuildIOS")]
        public static void BuildIOS()
        {
            foreach (var scene in EditorBuildSettings.scenes.Where(scene => scene.enabled))
            {
                Levels.Add(scene.path);
            }
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);

            var build = Environment.GetCommandLineArgs()[1];
            var rs = BuildPipeline.BuildPlayer(Levels.ToArray(), build, BuildTarget.iOS, BuildOptions.None);
            WriteFile(Environment.GetCommandLineArgs()[2], rs);
            EditorApplication.Exit(0);
        }


        private static AssetBundleBuild[] build_configs()
        {
            AssetBundleBuild asset = new AssetBundleBuild();
            asset.assetBundleName = "configs.bundle";
            List<string> say_options = new List<string>();
            DirectoryInfo TheFolder = new DirectoryInfo(ConfigEditor.Path_GenerateExcel);
            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {
                if (NextFile.Name.Contains(".meta"))
                    continue;

                string name = NextFile.Name;
                say_options.Add(ConfigEditor.Path_GenerateExcel + "\\" + name);
            }
            asset.assetNames = say_options.ToArray();

            //build
            AssetBundleBuild[] buildMap = new AssetBundleBuild[1] { asset };

            return buildMap;
        }

         
        [MenuItem("program/Build/打包配置-Window", false, 1)]
        private static void BuildConfig_W64()
        {
            //		EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.StandaloneWindows64);
            var buildMap = build_configs();

            BuildPipeline.BuildAssetBundles(
                Application.dataPath + "/../AssetBundles/Windows/",
                buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
        }

        [MenuItem("program/Build/打包配置-Android", false, 2)]
        private static void BuildConfig_Android()
        {
            //		EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.Android);
            var buildMap = build_configs();

            BuildPipeline.BuildAssetBundles(
                Application.dataPath + "/../AssetBundles/Android/",
                buildMap, BuildAssetBundleOptions.None, BuildTarget.Android);
        }

        [MenuItem("program/Build/打包配置-IOS", false, 3)]
        private static void BuildConfig_IOS()
        {
            //		EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.iOS);
            var buildMap = build_configs();

            BuildPipeline.BuildAssetBundles(
                Application.dataPath + "/../AssetBundles/IOS/",
                buildMap, BuildAssetBundleOptions.None, BuildTarget.iOS);
        }

        [MenuItem("program/Build/打包配置+Android+IOS", false, 4)]
        private static void BuildConfig_ALL()
        {
            //		BuildConfig_W64();
            BuildConfig_Android();
            BuildConfig_IOS();
        }

        //资源
        [MenuItem("program/Build/打包资源-Window", false, 101)]
        private static void BuildAssetsBundle_W64()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
            BuildPipeline.BuildAssetBundles(Application.dataPath + "/../AssetBundles/Windows/", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
        }

        [MenuItem("program/Build/打包资源-Android", false, 102)]
        private static void BuildAssetsBundle_Android()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            BuildPipeline.BuildAssetBundles(Application.dataPath + "/../AssetBundles/Android/", BuildAssetBundleOptions.None, BuildTarget.Android);
        }

        [MenuItem("program/Build/打包资源-IOS", false, 103)]
        private static void BuildAssetsBundle_IOS()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            BuildPipeline.BuildAssetBundles(Application.dataPath + "/../AssetBundles/IOS/", BuildAssetBundleOptions.None, BuildTarget.iOS);
        }

        [MenuItem("program/Build/打包资源-Windows+Android+IOS", false, 104)]
        private static void BuildAssetsBundle_ALL()
        {
            BuildAssetsBundle_W64();
            BuildAssetsBundle_Android();
            BuildAssetsBundle_IOS();
        }

        static string[] _scenePaths = { "Assets/Scenes/name1.unity", "Assets/Scenes/name2.unity" };

        [MenuItem("program/Build/打包场景-Windows", false, 201)]
        public static void BuildScene_W64()
        {
            BuildScene(BuildTarget.StandaloneWindows64, "Windows");
        }

        [MenuItem("program/Build/打包场景-Android", false, 202)]
        public static void BuildScene_Android()
        {
            BuildScene(BuildTarget.Android, "Android");
        }

        [MenuItem("program/Build/打包场景-IOS", false, 203)]
        public static void BuildScene_IOS()
        {
            BuildScene(BuildTarget.iOS, "IOS");
        }

        [MenuItem("program/Build/打包场景-Windows+Android+IOS", false, 204)]
        private static void BuildScene_ALL()
        {
            BuildScene_W64();
            BuildScene_Android();
            BuildScene_IOS();
        }

        private static void BuildScene(BuildTarget target, String dir)
        {
            BuildPipeline.BuildPlayer(_scenePaths,
                Application.dataPath + "/../AssetBundles/" + dir + "/scene.bundle", target,
                BuildOptions.BuildAdditionalStreamedScenes);
        }

        //ALL
        [MenuItem("program/Build/[打包配置+资源+场景]-Windows", false, 301)]
        private static void Build_ALL_Win64()
        {
            BuildConfig_W64();
            BuildAssetsBundle_W64();
            BuildScene_W64();
        }

        [MenuItem("program/Build/打包[配置+资源+场景]-Android", false, 302)]
        private static void Build_ALL_Android()
        {
            BuildConfig_Android();
            BuildAssetsBundle_Android();
            BuildScene_Android();
        }

        [MenuItem("program/Build/打包[配置+资源+场景]-IOS", false, 302)]
        private static void Build_ALL_IOS()
        {
            BuildConfig_IOS();
            BuildAssetsBundle_Android();
            BuildScene_Android();
        }

        //ALL平台
        [MenuItem("program/Build/打包所有[配置+资源+场景]-Windows+Andorid+IOS", false, 401)]
        private static void Build_ALL()
        {
            Build_ALL_Win64();
            Build_ALL_Android();
            Build_ALL_IOS();
        }

        private static void WriteFile(string file, string msg)
        {
            var fileStream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write);
            var sw = new StreamWriter(fileStream);
            sw.Write(msg);
            sw.Close();
            fileStream.Close();
        }

    }
}

