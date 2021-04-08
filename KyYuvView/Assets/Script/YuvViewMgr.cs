using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class YuvViewMgr : MonoBehaviour
{

    enum Type {
        YUV420_YU12 = 0,
        YUV420_YV12 = 1,
        YUV420SP_NV21 = 2,
        YUV420SP_NV12 = 3
    };


    public RawImage rawImage;
    public Text showTex;

    Type globalType;

    public Material yu12Mat;
    public Material yv12Mat;
    public Material nv12Mat;
    public Material nv21Mat;

    string filePath = "";
    int videoW = 1280;
    int videoH = 720;

    byte[] bufY = null;
    byte[] bufU = null;
    byte[] bufV = null;

    protected Texture2D texY = null;
    protected Texture2D texU = null;
    protected Texture2D texV = null;
    protected Texture2D texTest = null;
    Dictionary<string, Material> piexlDict= new Dictionary<string, Material>();

    public Button btnYu;
    public Button btnYv;
    public Button btnNv12;
    public Button btnNv21;

    public InputField widthInput;
    public InputField heightInput;

    // Start is called before the first frame update
    void Start()
    {
        btnYu.onClick.AddListener(clickBtnYu);
        btnYv.onClick.AddListener(clickBtnYv);
        btnNv12.onClick.AddListener(clickNv12);
        btnNv21.onClick.AddListener(clickNv21);

        widthInput.onEndEdit.AddListener(End_ValueW);
        heightInput.onEndEdit.AddListener(End_ValueH);

        piexlDict.Add("yu12",yu12Mat);
        piexlDict.Add("yv12", yv12Mat);
        piexlDict.Add("nv12", nv12Mat);
        piexlDict.Add("nv21", nv21Mat);

     }

    public void End_ValueW(string inp)
    {
        videoW = int.Parse(inp);
        print("文本内容W:" + inp);
    }

    public void End_ValueH(string inp)
    {
        videoH = int.Parse(inp);
        print("文本内容H:" + inp);
    }

    void clearBuffer()
    {
        bufY = null;bufV = null;bufU = null;
        Destroy(texY); Destroy(texU); Destroy(texV);
        texV = texU = texY = null;
    }


    void openFile()
    {
        filePath = Util.GetOpenPath();
    }

    // Update is called once per frame
    void Update()
    {
        if (texY != null && texU != null) {
            showTex.text = globalType.ToString();

            texY.LoadRawTextureData(bufY);
            texY.Apply();

            texU.LoadRawTextureData(bufU);
            texU.Apply();
            if (texV)
            {
                texV.LoadRawTextureData(bufV);
                texV.Apply();
            }
        }

    }

    void clickNv21()
    {
        clearBuffer();
        readBuff(Type.YUV420SP_NV21);
        if (filePath == null)
        {
            return;
        }
        //if (texY == null && texU == null)
        //{
        texY = new Texture2D(videoW, videoH, TextureFormat.Alpha8, false);
            //U分量和V分量分别存放在两张贴图中
            texU = new Texture2D(videoW >> 1, videoH >> 1, TextureFormat.RGBA4444, false);

            rawImage.material = piexlDict["nv21"];

            rawImage.material.SetTexture("_MainTex", texY);
            rawImage.material.SetTexture("_uvTexture", texU);
        //}
        Debug.Log("点击了NV21按钮");

    }

    void clickNv12()
    {
        clearBuffer();
        readBuff(Type.YUV420SP_NV12);
        if (filePath == null)
        {
            return;
        }
        //if (texY == null && texU == null)
        //{
        texY = new Texture2D(videoW, videoH, TextureFormat.Alpha8, false);
            //U分量和V分量分别存放在两张贴图中
            texU = new Texture2D(videoW >> 1, videoH >> 1, TextureFormat.RGBA4444, false);

            rawImage.material = piexlDict["nv12"];

            rawImage.material.SetTexture("_MainTex", texY);
            rawImage.material.SetTexture("_uvTexture", texU);
        //}
        Debug.Log("点击了NV12按钮");
    }


    void clickBtnYv()
    {
        clearBuffer();
        readBuff(Type.YUV420_YV12);
        if (filePath == null)
        {
            return;
        }
        //if (texY == null && texU == null && texV == null)
        //{
        texY = new Texture2D(videoW, videoH, TextureFormat.Alpha8, false);
            //U分量和V分量分别存放在两张贴图中
            texU = new Texture2D(videoW >> 1, videoH >> 1, TextureFormat.Alpha8, false);
            texV = new Texture2D(videoW >> 1, videoH >> 1, TextureFormat.Alpha8, false);

            rawImage.material = piexlDict["yv12"];

            rawImage.material.SetTexture("_MainTex", texY);
            rawImage.material.SetTexture("_uTexture", texU);
            rawImage.material.SetTexture("_vTexture", texV);
        //}
        Debug.Log("点击了YV按钮");


    }

    void clickBtnYu() {
        clearBuffer();
        readBuff(Type.YUV420_YU12);
        if (filePath == null)
        {
            return;
        }
        //if (texY == null && texU == null && texV == null)
        //{
        texY = new Texture2D(videoW, videoH, TextureFormat.Alpha8, false);
            //U分量和V分量分别存放在两张贴图中
        texU = new Texture2D(videoW >> 1, videoH >> 1, TextureFormat.Alpha8, false);
        texV = new Texture2D(videoW >> 1, videoH >> 1, TextureFormat.Alpha8, false);

        rawImage.material = piexlDict["yu12"];

        rawImage.material.SetTexture("_MainTex", texY);
        rawImage.material.SetTexture("_uTexture", texU);
        rawImage.material.SetTexture("_vTexture", texV);
        //}
        Debug.Log("点击了YU按钮");

    }


    void readBuff(Type type)
    {
        globalType = type;
        

        rawImage.rectTransform.sizeDelta =  Screen.width <= Screen.height ?  new Vector2((float)Screen.width, (float)Screen.width * ( (float)videoH / (float)videoW)) : new Vector2((float)Screen.height * ((float)videoW / (float)videoH), (float)Screen.height );

        //if (Directory.Exists(filePath1)
        //{
        //    DirectoryInfo direction = new DirectoryInfo(filePath1);
        //    FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

        //    Debug.Log(files.Length);
        //    ttt += files.Length.ToString();
        //    for (int i = 0; i < files.Length; i++)
        //    {
        //        if (files[i].Name.EndsWith(".meta"))
        //        {
        //            continue;
        //        }
        //        ttt += files[i].Name;
        //        Debug.Log("Name:" + files[i].Name);  //打印出来这个文件架下的所有文件 
        //    }
        //}

        filePath = Util.GetOpenPath();
        print(filePath);
        if (filePath != null)
        {
            using (FileStream fstream = new FileStream(filePath, FileMode.Open))
            {
                try
                {
                    //FileStream fstream = new FileStream(filePath, FileMode.Open);
                    byte[] buff = new byte[fstream.Length];
                    fstream.Read(buff, 0, (int)fstream.Length);
                    bufY = new byte[videoH * videoW];
                    Array.Copy(buff, 0, bufY, 0, videoW * videoH);
                    if (type == Type.YUV420_YU12)
                    {
                        bufU = new byte[videoH * videoW / 4];
                        Array.Copy(buff, videoW * videoH, bufU, 0, videoW * videoH / 4);

                        bufV = new byte[videoH * videoW / 4];
                        Array.Copy(buff, videoW * videoH / 4 * 5, bufV, 0, videoW * videoH / 4);
                    }
                    else if (type == Type.YUV420_YV12)
                    {
                        bufV = new byte[videoH * videoW / 4];
                        Array.Copy(buff, videoW * videoH, bufV, 0, videoW * videoH / 4);

                        bufU = new byte[videoH * videoW / 4];
                        Array.Copy(buff, videoW * videoH / 4 * 5, bufU, 0, videoW * videoH / 4);
                    }
                    else if (type == Type.YUV420SP_NV21)
                    {
                        bufU = new byte[videoH * videoW / 2];
                        Array.Copy(buff, videoW * videoH, bufU, 0, videoW * videoH / 2);
                    }
                    else if (type == Type.YUV420SP_NV12)
                    {
                        bufU = new byte[videoH * videoW / 2];
                        Array.Copy(buff, videoW * videoH, bufU, 0, videoW * videoH / 2);
                    }

                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                    showTex.text = "错误了";
                }
            }
        }
    }






    

    //private void OnGUI()
    //{
    //    if (GUILayout.Button("YUV-YU12", GUILayout.Width(200), GUILayout.Height(100)))
    //    {
    //        type = Type.YUV420_YU12;
    //        rawImage.material = yu12Mat;
    //        yu12Mat.EnableKeyword("_MainTex");
    //        yu12Mat.SetTexture("_MainTex", texY);
    //        LoadYUV_YU12();
    //    }
    //    //if (GUILayout.Button("YUV420-YV12", GUILayout.Width(200), GUILayout.Height(100)))
    //    //{
    //    //    type = Type.YUV420_YV12;
    //    //    LoadYUV_YV12();
    //    //}

    //    if (GUILayout.Button("Render YUV", GUILayout.Width(200), GUILayout.Height(100)))
    //    {
    //        switch (type)
    //        {
    //            case Type.YUV420_YU12:
    //                showTex.text = "YUV420_YU12";
                
    //                //rawImage.material.SetTexture("_uTexture", texU);
    //                //rawImage.material.SetTexture("_vTexture", texV);
    //                break;
    //            case Type.YUV420_YV12:
    //                showTex.text = "YUV420_YV12";
    //                rawImage.material = yu12Mat;
    //                //rawImage.material = yv12Mat;
    //                rawImage.material.SetTexture("_MainTex", texY);
    //                rawImage.material.SetTexture("_uTexture", texU);
    //                rawImage.material.SetTexture("_vTexture", texV);
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //}

    void LoadYUV_YU12()
    {

        string filePath = Application.streamingAssetsPath + "/yuv_intex_2.yuv";
        string filePath1 = Application.streamingAssetsPath;
        string ttt = "";

        if (Directory.Exists(filePath1))
        {
            DirectoryInfo direction = new DirectoryInfo(filePath1);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            Debug.Log(files.Length);
            ttt += files.Length.ToString();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta"))
                {
                    continue;
                }
                ttt += files[i].Name;
                Debug.Log("Name:" + files[i].Name);  //打印出来这个文件架下的所有文件
                //Debug.Log( "FullName:" + files[i].FullName );  
                //Debug.Log( "DirectoryName:" + files[i].DirectoryName );  
            }
        }
        showTex.text = ttt;
        //string filePath = Application.dataPath + "/Resources/" + "yuv_intex_2.yuv";

        FileStream fstream = new FileStream(filePath, FileMode.Open);
        byte[] buff = new byte[fstream.Length];
        fstream.Read(buff, 0, (int)fstream.Length);
    
        bufY = new byte[videoH * videoW];
        Array.Copy(buff, 0, bufY, 0, videoW * videoH);

        bufU = new byte[videoH * videoW / 4];
        Array.Copy(buff, videoW * videoH, bufU, 0, videoW * videoH / 4);

        bufV = new byte[videoH * videoW / 4];
        Array.Copy(buff, videoW * videoH / 4 * 5, bufV, 0, videoW * videoH / 4);

        texY.LoadRawTextureData(bufY);
        texY.Apply();

        texU.LoadRawTextureData(bufU);
        texU.Apply();

        texV.LoadRawTextureData(bufV);
        texV.Apply();

    }

    void LoadYUV_YV12()
    {
        string filePath = Application.streamingAssetsPath + "/" + "yuv_intex_2.yuv";
        
        showTex.text = filePath ;
        FileStream fstream = new FileStream(filePath, FileMode.Open);
        byte[] buff = new byte[fstream.Length];
        fstream.Read(buff, 0, (int)fstream.Length);

        bufY = new byte[videoH * videoW];
        Array.Copy(buff, 0, bufY, 0, videoW * videoH);

        bufV = new byte[videoH * videoW / 4];
        Array.Copy(buff, videoW * videoH, bufV, 0, videoW * videoH / 4);

        bufU = new byte[videoH * videoW / 4];
        Array.Copy(buff, videoW * videoH / 4 * 5, bufU, 0, videoW * videoH / 4);

        texY.LoadRawTextureData(buff);
        texY.Apply();

        texV.LoadRawTextureData(bufV);
        texV.Apply();

        texU.LoadRawTextureData(bufU);
        texU.Apply();

        fstream.Close();
        fstream.Dispose();
    }
}
