using System.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System;
using SHDocVw;
using mshtml;
using System.Diagnostics;

namespace IEExtension
{
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("FC4801A3-2BA9-11CF-A229-00AA003D7352")]
    public interface IObjectWithSite
    {
        [PreserveSig]
        int SetSite([MarshalAs(UnmanagedType.IUnknown)]object site);
        [PreserveSig]
        int GetSite(ref Guid guid, out IntPtr ppvSite);
    }

    [
            ComVisible(true),
            Guid("2159CB25-EF9A-54C1-B43C-E30D1A4A8277"),
            ClassInterface(ClassInterfaceType.None)
    ]
    public class BHO : IObjectWithSite
    {
        public const string BHO_REGISTRY_KEY_NAME = "Software\\Microsoft\\Windows\\" +"CurrentVersion\\Explorer\\Browser Helper Objects";

        private WebBrowser webBrowser;

        public int SetSite(object site)
        {
            if (site != null)
            {
                webBrowser = (WebBrowser)site;

                webBrowser.DocumentComplete +=
                  new DWebBrowserEvents2_DocumentCompleteEventHandler(
                  OnDocumentComplete);
            }
            else
            {
                webBrowser.DocumentComplete -=
                  new DWebBrowserEvents2_DocumentCompleteEventHandler(OnDocumentComplete);

                webBrowser = null;
            }

            return 0;
        }

        public int GetSite(ref Guid guid, out IntPtr ppvSite)
        {
            IntPtr punk = Marshal.GetIUnknownForObject(webBrowser);

            int hr = Marshal.QueryInterface(punk, ref guid, out ppvSite);

            Marshal.Release(punk);

            return hr;
        }

        public void OnDocumentComplete(object pDisp, ref object URL)
        {
            //bool isInjected = false;

            HTMLDocument document = (HTMLDocument)webBrowser.Document;

            IHTMLElement head = (IHTMLElement)((IHTMLElementCollection)document.all.tags("head")).item(null, 0);

            IHTMLScriptElement scriptObject = (IHTMLScriptElement)document.createElement("script");

            foreach (var htmlNode in ((HTMLHeadElement)head).childNodes)
            {
                var script = htmlNode as IHTMLScriptElement;

                if (script != null && script.src == "https://greasyfork.org/scripts/14666-easy-analysis-extension/code/Easy%20Analysis%20Extension.user.js")
                {
                    //isInjected = true;
                }
            }

            scriptObject.type = @"text/javascript";

            scriptObject.src = "https://greasyfork.org/scripts/14666-easy-analysis-extension/code/Easy%20Analysis%20Extension.user.js";

            ((HTMLHeadElement)head).appendChild((IHTMLDOMNode)scriptObject);
        }

        #region COM Register
        [ComRegisterFunction]
        public static void RegisterBHO(Type type)
        {
            RegistryKey registryKey =
              Registry.LocalMachine.OpenSubKey(BHO_REGISTRY_KEY_NAME, true);

            if (registryKey == null)
                registryKey = Registry.LocalMachine.CreateSubKey(
                                        BHO_REGISTRY_KEY_NAME);

            string guid = type.GUID.ToString("B");
            RegistryKey ourKey = registryKey.OpenSubKey(guid);

            if (ourKey == null)
            {
                ourKey = registryKey.CreateSubKey(guid);
            }

            ourKey.SetValue("NoExplorer", 1, RegistryValueKind.DWord);

            registryKey.Close();
            ourKey.Close();
        }

        [ComUnregisterFunction]
        public static void UnregisterBHO(Type type)
        {
            RegistryKey registryKey =
              Registry.LocalMachine.OpenSubKey(BHO_REGISTRY_KEY_NAME, true);
            string guid = type.GUID.ToString("B");

            if (registryKey != null)
                registryKey.DeleteSubKey(guid, false);
        }

        #endregion
    }
}
