using UnityEditor;
using UnityEngine;
using System.Collections;
using PigeonCoopToolkit.Generic.Editor;

namespace PigeonCoopToolkit.Effects.Trails.Editor
{
    [InitializeOnLoad]
    public class PCTKEffectsIntroDialogue
    {
        static PCTKEffectsIntroDialogue()
        {
            EditorApplication.update+=Update;
        }

        static void Update()
        {
            if (EditorPrefs.GetBool("PCTK/Effects/Trails/ShownIntroDialogue") == false)  
            {
                IntroDialogue dialogue = EditorWindow.GetWindow<IntroDialogue>(true, "Thanks for your purchase!");
                dialogue.Init(Resources.Load("PCTK/Effects/Trails/banner") as Texture2D, new Generic.VersionInformation("Better Trails", 1, 3, 0), Application.dataPath + "/PigeonCoopToolkit/__Effects (Trails) Examples/Pigeon Coop Toolkit - Effects (Trails).pdf");
                EditorPrefs.SetBool("PCTK/Effects/Trails/ShownIntroDialogue",true);
            } 
        }
    }

}
 