using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;

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
//                for (int i = 0; i < files.Count; i++)
//                {
//                    ExtractFile(files[i]);
//                    UpdateIdentifier(files[i]);
//                    ZipFile(files[i]);
//                }
                
                DirectoryInfo directorySelected = new DirectoryInfo(path);
                foreach (FileInfo fileToDecompress in directorySelected.GetFiles("*.aar"))
                {
                    Decompress(fileToDecompress);
                }
            }
        }
    }

    private static void ExtractFile(string file)
    {
        Debug.LogFormat("ExtractFile: {0}", file);
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

    private static void ZipFile(string file)
    {
        Debug.LogFormat("ZipFile: {0}", file);
    }
    
    private static void Decompress(FileInfo fileToDecompress)
    {
        using (FileStream originalFileStream = fileToDecompress.OpenRead())
        {
            string currentFileName = fileToDecompress.FullName;
            string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

            using (FileStream decompressedFileStream = File.Create(newFileName))
            {
                using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                {
                    CopyTo(decompressionStream, decompressedFileStream);
                    Console.WriteLine("Decompressed: {0}", fileToDecompress.Name);
                }
            }
        }
    }

    private static void CopyTo(Stream input, Stream output)
    {
        byte[] buffer = new byte[16 * 1024];
        int bytesRead;
        while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
        {
            output.Write(buffer, 0, bytesRead);
        }
    }
}
