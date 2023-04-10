using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

public class Encrypt
{
    //預設金鑰向量
    private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };



    /// <summary>
    /// DES加密字串
    /// </summary>
    /// <param name="encryptString">待加密的字串</param>
    /// <param name="encryptKey">加密金鑰,要求為8位</param>
    /// <returns>加密成功返回加密後的字串，失敗返回源串</returns>
    public static string EncryptDES(string source)
    {
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        byte[] key = Encoding.ASCII.GetBytes("12345678");
        byte[] iv = Encoding.ASCII.GetBytes("87654321");
        byte[] dataByteArray = Encoding.UTF8.GetBytes(source);

        des.Key = key;
        des.IV = iv;
        string encrypt = "";
        using (MemoryStream ms = new MemoryStream())
        using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
        {
            cs.Write(dataByteArray, 0, dataByteArray.Length);
            cs.FlushFinalBlock();
            encrypt = Convert.ToBase64String(ms.ToArray());
        }
        return encrypt;
    }


    /// <summary>
    /// DES解密字串
    /// </summary>
    /// <param name="decryptString">待解密的字串</param>
    /// <param name="decryptKey">解密金鑰,要求為8位,和加密金鑰相同</param>
    /// <returns>解密成功返回解密後的字串，失敗返源串</returns>
    public static string DecryptDES(string encrypt)
    {
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        byte[] key = Encoding.ASCII.GetBytes("12345678");
        byte[] iv = Encoding.ASCII.GetBytes("87654321");
        des.Key = key;
        des.IV = iv;

        byte[] dataByteArray = Convert.FromBase64String(encrypt);
        using (MemoryStream ms = new MemoryStream())
        {
            using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(dataByteArray, 0, dataByteArray.Length);
                cs.FlushFinalBlock();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}

public class SettingFile
{
    public int m_intLanguages;
    public String m_StrSMTPServer;
    public int m_intSMTPPort;
    public String m_StrSMTPName;
    public String m_StrSMTPEmail;
    public String m_StrSMTPID;
    public String m_StrSMTPPW;
    public String m_StrSMTPTitle;
    public String m_StrSMTPContent;
    public Boolean m_blnSMTPCheck;
    public Boolean m_blnSMTPSSL;
    public String m_StrTest;
    public SettingFile()
    {
        m_intLanguages = 1;
        m_StrSMTPServer = "";
        m_intSMTPPort = -1;
        m_StrSMTPName = "";
        m_StrSMTPEmail = "";
        m_StrSMTPID = "";
        m_StrSMTPPW = "";
        m_StrSMTPTitle = "";
        m_StrSMTPContent = "";
        m_blnSMTPCheck = false;
        m_blnSMTPSSL = false;
        m_StrTest = "";
    }
    public void saveSettingXML()
    {
        //XmlTextWriter XTW = new XmlTextWriter(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\W_B_Setting.xml", Encoding.UTF8);
        XmlTextWriter XTW = new XmlTextWriter("ConsoleXML.xml", Encoding.UTF8); 
        XTW.WriteStartDocument();

        XTW.WriteStartElement("Setting");

        XTW.WriteElementString("Languages", "" + m_intLanguages);
        XTW.WriteElementString("SMTPServer", m_StrSMTPServer);
        XTW.WriteElementString("SMTPPort", "" + m_intSMTPPort);
        XTW.WriteElementString("SMTPName", m_StrSMTPName);
        XTW.WriteElementString("SMTPEmail", m_StrSMTPEmail);
        XTW.WriteElementString("SMTPID", m_StrSMTPID);
        XTW.WriteElementString("SMTPPW", Encrypt.EncryptDES(m_StrSMTPPW));
        XTW.WriteElementString("SMTPTitle", m_StrSMTPTitle);
        XTW.WriteElementString("SMTPContent", m_StrSMTPContent);
        XTW.WriteElementString("SMTPCheck", m_blnSMTPCheck.ToString());
        XTW.WriteElementString("SMTPSSL", m_blnSMTPSSL.ToString());
        XTW.WriteElementString("SMTPTest", m_StrTest);

        XTW.Flush();
        XTW.Close();
    }
    public void readSettingXML()
    {
        try
        {
            XmlDocument xd = new XmlDocument();

            //xd.Load(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\W_B_Setting.xml");
            xd.Load("ConsoleXML.xml");

            XmlNode root = xd.SelectSingleNode("//Setting");
            int i = 0;
            foreach (XmlElement elm in root.ChildNodes)
            {
                switch (i)
                {
                    case 00:
                        m_intLanguages = Convert.ToInt32(elm.InnerText.Trim(), 10);
                        break;
                    case 01:
                        m_StrSMTPServer = elm.InnerText.Trim();
                        break;
                    case 02:
                        m_intSMTPPort = Convert.ToInt32(elm.InnerText.Trim(), 10);
                        break;
                    case 03:
                        m_StrSMTPName = elm.InnerText.Trim();
                        break;
                    case 04:
                        m_StrSMTPEmail = elm.InnerText.Trim();
                        break;
                    case 05:
                        m_StrSMTPID = elm.InnerText.Trim();
                        break;
                    case 06:
                        m_StrSMTPPW = Encrypt.DecryptDES(elm.InnerText.Trim());
                        break;
                    case 07:
                        m_StrSMTPTitle = elm.InnerText.Trim();
                        break;
                    case 08:
                        m_StrSMTPContent = elm.InnerText.Trim();
                        break;
                    case 09:
                        m_blnSMTPCheck = Convert.ToBoolean(elm.InnerText.Trim());
                        break;
                    case 10:
                        m_blnSMTPSSL = Convert.ToBoolean(elm.InnerText.Trim());
                        break;
                    case 11:
                        m_StrTest = elm.InnerText.Trim();
                        break;
                }
                i++;
                Console.WriteLine(elm.Name.Trim() + ":" + elm.InnerText.Trim());
            }      
        }
        catch
        {
            m_intLanguages = 1;
            m_StrSMTPServer = "";
            m_intSMTPPort = -1;
            m_StrSMTPName = "";
            m_StrSMTPEmail = "";
            m_StrSMTPID = "";
            m_StrSMTPPW = "";
            m_StrSMTPTitle = "";
            m_StrSMTPContent = "";
            m_blnSMTPCheck = false;
            m_blnSMTPSSL = false;
            m_StrTest = "";
        }
    }
}
public class Program
{
    [DllImport("POS_ECM.dll",CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Init_Module")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool Init_Module(out IntPtr pOut);

    static void Pause()
    {
        Console.Write("Press any key to continue...");
        Console.ReadKey(true);
    }
    public static void Main()
    {
        /*
        SettingFile SettingFileBuf=new SettingFile();
        SettingFileBuf.saveSettingXML();
        SettingFileBuf.readSettingXML();
        */
        IntPtr pOut01;
        string msg;
        if (Init_Module(out pOut01))
        {
            msg = Marshal.PtrToStringAuto(pOut01);
        }
        else
        {
            msg = Marshal.PtrToStringAuto(pOut01);
            Console.WriteLine(msg);
        }        
        Pause();
    }
}
