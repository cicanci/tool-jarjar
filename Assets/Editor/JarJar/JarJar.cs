using System;
using System.IO;
using System.Linq;
using System.Xml;
using Ionic.Zip;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;

namespace Editor.JarJar
{
    public class JarJar : MonoBehaviour 
    {
        [MenuItem("Tools/Android/Update AAR with JarJar")]
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
                        CreateFile(files[i], outputPath);
                    }

                    Directory.Delete(outputPath, true);
                }
            }
        }

        private static void ExtractFile(string filePath, string outputPath)
        {
            Debug.LogFormat("Extract file from '{0}' to '{1}'", filePath, outputPath);
            Unzip(filePath, outputPath);
        }

        private static void UpdateIdentifier(string filePath, string outputPath)
        {
            outputPath += "/AndroidManifest.xml";

            XmlDocument document = new XmlDocument();
            document.Load(outputPath);

            XmlNode root = document.DocumentElement;
            if (root == null || root.Attributes == null)
            {
                Debug.LogErrorFormat("Invalid XML file '{0}'", outputPath);
            }
            else
            {
                Debug.LogFormat("Update identifier from '{0}' to '{1}'", root.Attributes["package"].Value, Application.identifier);
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
            using (ZipFile zip = ZipFile.Read(zipFilePath)) 
            {
                zip.ExtractAll(location, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        public static void Zip(string zipFileName, string[] files, string outputPath)
        {
            Debug.LogFormat("Zipping {0} files at '{1}'", files.Length, zipFileName);
            using (ZipFile zip = new ZipFile()) 
            {
                for (int i = 0; i < files.Length; i++)
                {
                    string filePath = files[i].Replace(outputPath, string.Empty).Replace(Path.GetFileName(files[i]), string.Empty);
                    zip.AddFile(files[i], filePath);
                }
                zip.Save(zipFileName);
            }
        }
    }
}
