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

        static string UnixTime()
        {
            int unixTimestamp = (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp.ToString();
        }

        // https://stackoverflow.com/a/44734346/10536842
        // Thank you!
        static Texture2D duplicateTexture(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
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
                try
                {
                    AssetBundle bundle = AssetBundle.LoadFromFile(f.FullName);
                    Object[] assets1 = bundle.LoadAllAssets();
                    foreach (Object ob1 in assets1)
                    {
                        if (ob1.GetType().ToString() == "UnityEngine.Texture2D")
                        {
                            Texture2D tx1 = (Texture2D)ob1;
                            try
                            {
                                Texture2D tx2 = duplicateTexture(tx1);
                                Color32[] colorArray = tx2.GetPixels32(0);
                                Texture2D tx3 = new Texture2D(tx2.width, tx2.height, TextureFormat.ARGB32, false);
                                tx3.SetPixels32(colorArray);
                                tx3.Apply();
                                try
                                {
                                    Directory.CreateDirectory(FolderPath + "\\Texture2D");
                                }
                                catch { }
                                if (!File.Exists(FolderPath + "\\Texture2D\\" + tx1.name + ".png"))
                                {
                                    File.WriteAllBytes(FolderPath + "\\Texture2D\\" + tx1.name + ".png", tx3.EncodeToPNG());
                                }
                                else
                                {
                                    File.WriteAllBytes(FolderPath + "\\Texture2D\\" + tx1.name + UnixTime() + ".png", tx3.EncodeToPNG());
                                }
                            }
                            catch { }

                        }
                        else if (ob1.GetType().ToString() == "UnityEngine.TextAsset")
                        {
                            TextAsset text1 = (TextAsset)ob1;
                            try
                            {
                                byte[] b1 = text1.bytes;
                                try
                                {
                                    Directory.CreateDirectory(FolderPath + "\\TextAsset");
                                }
                                catch { }
                                if (!File.Exists(FolderPath + "\\TextAsset\\" + text1.name + ".txt"))
                                {
                                    File.WriteAllBytes(FolderPath + "\\TextAsset\\" + text1.name + ".txt", b1);
                                }
                                else
                                {
                                    File.WriteAllBytes(FolderPath + "\\TextAsset\\" + text1.name + UnixTime() + ".txt", b1);
                                }
                            }
                            catch { }

                        }
                    }
                }
                catch { }
            }
            EditorApplication.Exit(0);
        }
    }
}
