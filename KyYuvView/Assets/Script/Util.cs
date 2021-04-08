using System.Runtime.InteropServices;
using UnityEngine;
using System;

public class Util : MonoBehaviour
{
    /// <summary>
    /// 选择路径，导入文件时使用
    /// </summary>
    /// <param name="fileType"></param>
    /// <returns></returns>
    public static string GetOpenPath(params string[] fileType)
    {
        string path = null;

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        path = Util.GetPathFromMac();       

#else
            // |
            //CSV文件\0*.csv\0文本文件\0*.txt
            string filter = string.Empty;
            for (int i = 0; i < fileType.Length; i++)
            {
                if (i == 0)
                    filter += string.Format("{0}文件\0*.{1}", fileType[i].ToUpper(), fileType[i]);
                else
                {
                    filter += string.Format("\0{0}文件\0*.{1}", fileType[i].ToUpper(), fileType[i]);
                }
            }

            OpenFileName openFileName = Util.OpenSaveFileWindow(filter);
            if (LocalDialog.GetOpenFileName(openFileName))
            {
                //ParseXMLData(openFileName);
                if (openFileName.file == null) return null;
                path = openFileName.file;
                //path = openFileName.file;
            }
#endif
        return path;
    }


    public static string GetPathFromMac()
    {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        return getFilePath();
#else
            return null;
#endif
    }
    
    /// <summary>
    /// Window操作系统打开选择文件窗口
    /// </summary>
    /// <param name="filter">文件格式</param>
    /// <returns></returns>
    public static OpenFileName OpenSaveFileWindow(string filter = null)
    {
        OpenFileName openFileName = new OpenFileName();
        openFileName.structSize = Marshal.SizeOf(openFileName);
        if (filter != null)
            openFileName.filter = filter; // "*JSON文件(*.json)\0*.json";
        openFileName.file = new string(new char[256]);
        openFileName.maxFile = openFileName.file.Length;
        openFileName.fileTitle = new string(new char[64]);
        openFileName.maxFileTitle = openFileName.fileTitle.Length;
        openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\'); //默认路径
        openFileName.title = "窗口标题";
        openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
        return openFileName;
    }
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
    [DllImport("OpenFinderForUnity", CharSet = CharSet.Auto)]
    private static extern string getFilePath();

#endif
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenFileName
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}

public class LocalDialog
{
    //链接指定系统函数       打开文件对话框
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);
    public static bool GetOFN([In, Out] OpenFileName ofn)
    {
        return GetOpenFileName(ofn);
    }

    //链接指定系统函数        另存为对话框
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetSaveFileName([In, Out] OpenFileName ofn);
    public static bool GetSFN([In, Out] OpenFileName ofn)
    {
        return GetSaveFileName(ofn);
    }
}
