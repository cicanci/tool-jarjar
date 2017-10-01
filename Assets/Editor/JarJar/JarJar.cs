using System.IO;
using System.Linq;
using System.Xml;
using Ionic.Zip;
using UnityEditor;
using UnityEngine;

namespace Editor.JarJar
{
    public class JarJar : MonoBehaviour 
    {
        [MenuItem("Tools/Update AAR with JarJar")]
        public static void UpdateWithJarJar()
        {
#if UNITY_ANDROID
            StartJarJar();
#else
        Debug.LogError("JarJar is only support in Android! Change your current platform to Android!");
#endif
        }

        private static void StartJarJar()
        {
            Debug.LogFormat("Android package name '{0}'", Application.identifier);

            string androidPath = Application.dataPath + "/Plugins/Android/";
            if (!Directory.Exists(androidPath))
            {
                Debug.LogWarningFormat("Path not found '{0}'", androidPath);
            }
            else
            {
                Debug.LogFormat("Looking for ARR files in '{0}'", androidPath);

                var files = Directory.GetFiles(androidPath, "*.aar", SearchOption.AllDirectories).ToList();
                if (files.Count == 0)
                {
                    Debug.LogWarningFormat("No AARs found in '{0}'", androidPath);
                }
                else
                {
                    string outputPath = Application.dataPath.Replace("/Assets", "/_jarjar");

                    for (int i = 0; i < files.Count; i++)
                    {
                        ExtractFile(files[i], outputPath);
                        UpdateIdentifier(files[i], outputPath);
                        CreateFile(files[i], outputPath, androidPath);
                    }

                    Directory.Delete(outputPath, true);
                }
            }
        }

        private static void ExtractFile(string filePath, string outputPath)
        {
            Debug.LogFormat("ExtractFile: {0} to {1}", filePath, outputPath);
            Unzip(filePath, outputPath);
        }

        private static void UpdateIdentifier(string filePath, string outputPath)
        {
            outputPath += "/AndroidManifest.xml";
            Debug.LogFormat("UpdateIdentifier: {0}", outputPath);

            XmlDocument document = new XmlDocument();
            document.Load(outputPath);

            XmlNode root = document.DocumentElement;
            if (root == null || root.Attributes == null)
            {
                Debug.LogErrorFormat("Invalid XML file '{0}'", outputPath);
            }
            else
            {
                root.Attributes["package"].Value = Application.identifier;
                document.Save(outputPath);
            }
        }

        private static void CreateFile(string filePath, string outputPath, string androidPath)
        {
            
        }

        public static void Unzip(string zipFilePath, string location)
        {
            Directory.CreateDirectory(location);

            using (ZipFile zip = ZipFile.Read(zipFilePath)) 
            {
                zip.ExtractAll(location, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        public static void Zip(string zipFileName, params string[] files)
        {
            string path = Path.GetDirectoryName(zipFileName);
            Directory.CreateDirectory(path);

            using (ZipFile zip = new ZipFile()) 
            {
                for (int i = 0; i < files.Length; i++)
                {
                    zip.AddFile(files[i], "");
                }
                zip.Save(zipFileName);
            }
        }
    }
}
