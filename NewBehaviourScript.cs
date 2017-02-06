using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using System.IO;
using System.Diagnostics;

namespace Nspace
{
    public class NewBehaviourScript : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        static void DoExtract()
        {
            string FolderPath = ""; //Path to folder containing assetbundles

            //Optional; Modify accordingly
            foreach (FileInfo f in new DirectoryInfo(FolderPath).GetFiles("*.unity3d.lz4"))
            {
                Process p = new Process();
                p.StartInfo.FileName = ""; //Path to lz4er-win.exe
                p.StartInfo.Arguments = f.FullName;
                p.Start();
                p.WaitForExit();
                File.Move(f.FullName + ".extracted", (f.FullName + ".extracted").Replace(".unity3d.lz4.extracted", ".unity3d"));
            }
            //

            foreach (FileInfo f in new DirectoryInfo(FolderPath).GetFiles("*.unity3d"))
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(f.FullName);
                Object[] assets1 = bundle.LoadAllAssets();
                Stack<Texture2D> textures = new Stack<Texture2D>();
                Stack<TextAsset> textassets = new Stack<TextAsset>();
                foreach (Object ob1 in assets1)
                {
                    if (ob1.GetType().ToString() == "UnityEngine.Texture2D")
                    {
                        textures.Push((Texture2D)ob1);
                    }
                    else if (ob1.GetType().ToString() == "UnityEngine.TextAsset")
                    {
                        textassets.Push((TextAsset)ob1);
                    }
                }
                foreach (Texture2D tx1 in textures)
                {
                    try
                    {
                        byte[] b1 = tx1.GetRawTextureData();
                        Texture2D tx2 = new Texture2D(tx1.width, tx1.height, tx1.format, false);
                        tx2.LoadRawTextureData(b1);
                        Color32[] colorArray = tx2.GetPixels32(0);
                        Texture2D tx3 = new Texture2D(tx2.width, tx2.height, TextureFormat.ARGB32, false);
                        tx3.SetPixels32(colorArray);
                        tx3.Apply();
                        try
                        {
                            Directory.CreateDirectory(FolderPath + "\\Texture2D");
                        }
                        catch { }
                        File.WriteAllBytes(FolderPath + "\\Texture2D\\" + tx1.name + ".png", tx3.EncodeToPNG());
                    }
                    catch { }
                }
                foreach (TextAsset text1 in textassets)
                {
                    try
                    {
                        byte[] b1 = text1.bytes;
                        try
                        {
                            Directory.CreateDirectory(FolderPath + "\\TextAsset");
                        }
                        catch { }
                        File.WriteAllBytes(FolderPath + "\\TextAsset\\" + text1.name + ".txt", b1);
                    }
                    catch { }
                }
            }
            //EditorApplication.Exit(0);
        }
    }
}