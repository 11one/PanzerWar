using UnityEditor;
using UnityEngine;

namespace PigeonCoopToolkit.Generic.Editor
{
    public class IntroDialogue : EditorWindow
    {
        public VersionInformation versionInformation;
        public Texture2D banner;
        public string UserGuidePath;


        void OnGUI()
        {
            if(banner == null)
            {
                return;
            }


            GUI.DrawTexture(new Rect(0, 0, banner.width, banner.height), banner);
            GUILayout.Space(banner.height - 18);
            if (versionInformation != null) GUILayout.Label(versionInformation.ToString());
            GUIStyle lessPaddingNotif = new GUIStyle("NotificationText");
            lessPaddingNotif.padding = new RectOffset(10,10,10,10);
            lessPaddingNotif.margin = new RectOffset(10, 10, 10, 10);
            lessPaddingNotif.stretchWidth = true;
            GUILayout.Label("Thanks for your purchase! ♥", lessPaddingNotif);

            GUILayout.BeginHorizontal();
            GUILayout.Space(16);
            GUILayout.BeginVertical();
            GUILayout.Label("We hope you enjoy this tool. Feel free to contact us at our twitter or email - send us feature requests, get some help from us, or just say hi!", "WordWrapLabel");
            GUILayout.Label("Don't forget to rate or review "+versionInformation.Name+" on the asset store once you've had a chance to evaluate it", "WordWrapLabel");
            GUILayout.EndVertical();
            GUILayout.Space(16);
            GUILayout.EndHorizontal(); 
            

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
 
            GUILayout.BeginVertical();
            if (!string.IsNullOrEmpty(UserGuidePath))
            {
                if (GUILayout.Button("Need help? Read the guide!","LargeButton"))
                {
                    Application.OpenURL(UserGuidePath);
                };

            }
            GUILayout.Space(5);
            if (GUILayout.Button("Want to say hello? @PigeonCoopAU", "LargeButton"))
            {
                Application.OpenURL("http://www.twitter.com/PigeonCoopAU");
            };

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.Label("© 2014 Pigeon Coop ", EditorStyles.miniLabel);

        }

        public void Init(Texture2D _banner, VersionInformation _versionInformation, string userGuidePath)
        {
            banner = _banner;
            UserGuidePath = userGuidePath;

            if (System.IO.File.Exists(FileUtil.GetProjectRelativePath(userGuidePath)) == false)
                UserGuidePath = null;

            versionInformation = _versionInformation;
            minSize = maxSize = new Vector2(banner.width, 500);
        }
    }
}