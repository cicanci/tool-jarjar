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

            string path = Application.dataPath + "/Plugins/Android/";
            if (!Directory.Exists(path))
            {
                Debug.LogWarningFormat("Path not found '{0}'", path);
            }
            else
            {
                Debug.LogFormat("Looking for ARR files in '{0}'", path);

                var files = Directory.GetFiles(path, "*.aar", SearchOption.AllDirectories).ToList();
                if (files.Count == 0)
                {
                    Debug.LogWarningFormat("No AARs found in '{0}'", path);
                }
                else
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        ExtractFile(files[i]);
                        UpdateIdentifier(files[i]);
                    }
                }
            }
        }

        private static void ExtractFile(string file)
        {
            string path = Application.dataPath.Replace("/Assets", "/_jarjar");
            Debug.LogFormat("ExtractFile: {0} to {1}", file, path);
            Unzip(file, path);
        }

        private static void UpdateIdentifier(string file)
        {
            string path = "C:/Projects/tool-jarjar/Assets/Plugins/Android/AndroidManifest.xml";
            Debug.LogFormat("UpdateIdentifier: {0}", path);

            XmlDocument document = new XmlDocument();
            document.Load(path);

            XmlNode root = document.DocumentElement;
            if (root == null || root.Attributes == null)
            {
                Debug.LogErrorFormat("Invalid XML file '{0}'", path);
            }
            else
            {
                root.Attributes["package"].Value = Application.identifier;
                document.Save(path);
            }
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
