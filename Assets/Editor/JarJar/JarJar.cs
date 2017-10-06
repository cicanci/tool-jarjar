using System;
using System.IO;
using System.Linq;
using System.Xml;
using Ionic.Zip;
using UnityEditor;
using UnityEngine;

////////////////////////////////////////////////////////////
// JarJar uses UniZip: https://github.com/tsubaki/UnityZip
////////////////////////////////////////////////////////////

namespace Editor.JarJar
{
    public class JarJar : MonoBehaviour
    {
        private const string _manifestName = "AndroidManifest.xml";
        private static string _androidPath;
        private static string _outputPath;

        [MenuItem("Tools/Android/Update AAR with JarJar")]
        public static void UpdateWithJarJar()
        {
#if UNITY_ANDROID
            _androidPath = Application.dataPath + "/Plugins/Android/";
            if(!Directory.Exists(_androidPath))
            {
                Debug.LogWarningFormat("[JarJar] Path not found '{0}'", _androidPath);
            }
            else
            {
                Debug.LogFormat("[JarJar] Android package name '{0}'", Application.identifier);
                _outputPath = Application.dataPath.Replace("/Assets", "/_jarjar");

                try
                {
                    RunJarJarForLibrary();
                    RunJarJarForManifest();
                    Debug.Log("[JarJar] All AAR files were updated!");
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("[JarJar] Error: {0}", e.Message);
                }
                finally
                {
                    if(Directory.Exists(_outputPath))
                    {
                        Directory.Delete(_outputPath, true);
                    }
                    EditorUtility.ClearProgressBar();
                }
            }
#else
            Debug.LogError("[JarJar] JarJar is only support in Android! Change your current platform to Android!");
#endif
        }

        private static void RunJarJarForLibrary()
        {
#if DEBUG_JARJAR
            Debug.LogFormat("[JarJar] Looking for ARR files in '{0}'", androidPath);
#endif
            var files = Directory.GetFiles(_androidPath, "*.aar", SearchOption.AllDirectories).ToList();
            if(files.Count == 0)
            {
                Debug.LogWarningFormat("[JarJar] No AARs found in '{0}'", _androidPath);
            }
            else
            {
                for(int i = 0; i < files.Count; i++)
                {
                    EditorUtility.DisplayProgressBar("JarJar", 
                        string.Format("Processing file {0}", files[i].Replace(_androidPath, string.Empty)), 
                        (float)i / files.Count);

                    ExtractFile(files[i], _outputPath);
                    UpdateIdentifier(files[i], _outputPath);
                    CreateFile(files[i], _outputPath);
                }
            }
        }

        private static void RunJarJarForManifest()
        {
#if DEBUG_JARJAR
            Debug.LogFormat("[JarJar] Looking for AndroidManifest files in '{0}'", androidPath);
#endif
            var files = Directory.GetFiles(_androidPath, _manifestName, SearchOption.AllDirectories).ToList();
            if(files.Count == 0)
            {
                Debug.LogWarningFormat("[JarJar] No AndroidManifest found in '{0}'", _androidPath);
            }
            else
            {
                for(int i = 0; i < files.Count; i++)
                {
                    EditorUtility.DisplayProgressBar("JarJar", 
                        string.Format("Processing file {0}", files[i].Replace(_androidPath, string.Empty)), 
                        (float)i / files.Count);

                    UpdateIdentifier(files[i], files[i].Replace(_manifestName, string.Empty));
                }
            }
        }

        private static void ExtractFile(string filePath, string outputPath)
        {
#if DEBUG_JARJAR
            Debug.LogFormat("[JarJar] Extract file from '{0}' to '{1}'", filePath, outputPath);
#endif
            Unzip(filePath, outputPath);
        }

        private static void UpdateIdentifier(string filePath, string outputPath)
        {
            outputPath += "/" + _manifestName;

            XmlDocument document = new XmlDocument();
            document.Load(outputPath);

            XmlNode root = document.DocumentElement;
            if(root == null || root.Attributes == null)
            {
                Debug.LogErrorFormat("[JarJar] Invalid XML file '{0}'", outputPath);
            }
            else
            {
#if DEBUG_JARJAR
                Debug.LogFormat("[JarJar] Update identifier from '{0}' to '{1}'", root.Attributes["package"].Value, Application.identifier);
#endif
                root.Attributes["package"].Value = Application.identifier;
                document.Save(outputPath);
            }
        }

        private static void CreateFile(string filePath, string outputPath)
        {
            string[] files = Directory.GetFiles(outputPath, "*", SearchOption.AllDirectories).ToArray();
            Zip(filePath, files, outputPath);
        }

        public static void Unzip(string zipFilePath, string location)
        {
            Directory.CreateDirectory(location);
            using(ZipFile zip = ZipFile.Read(zipFilePath)) 
            {
                zip.ExtractAll(location, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        public static void Zip(string zipFileName, string[] files, string outputPath)
        {
#if DEBUG_JARJAR
            Debug.LogFormat("[JarJar] Zipping {0} files at '{1}'", files.Length, zipFileName);
#endif
            using(ZipFile zip = new ZipFile()) 
            {
                for(int i = 0; i < files.Length; i++)
                {
                    string filePath = files[i].Replace(outputPath, string.Empty).Replace(Path.GetFileName(files[i]), string.Empty);
                    zip.AddFile(files[i], filePath);
                }
                zip.Save(zipFileName);
            }
        }
    }
}
