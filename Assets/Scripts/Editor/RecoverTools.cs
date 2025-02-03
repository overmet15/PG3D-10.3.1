using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class RecoverTools : EditorWindow
{
    public LightProbeGroup lightProbeComponent;
    public LightProbes selectedProbes;

    [MenuItem("RecoverTools/Tools")]

    static void GetMe()
    {
        GetWindow<RecoverTools>();
    }

    void OnGUI()
    {
        CreditsDraw();
        ProbeDraw();
        LightingData();
    }

    void CreditsDraw()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Made By overmet15 (click here)", EditorStyles.centeredGreyMiniLabel)) Application.OpenURL("https://github.com/overmet15");

        EditorGUILayout.EndHorizontal();
    }

    void ProbeDraw()
    {
        lightProbeComponent = (LightProbeGroup)EditorGUILayout.ObjectField(lightProbeComponent, typeof(LightProbeGroup), true);
        selectedProbes = (LightProbes)EditorGUILayout.ObjectField(selectedProbes, typeof(LightProbes), false);

        if (lightProbeComponent == null || selectedProbes == null) return;

        if (GUILayout.Button("Patch Light Probes"))
        {
            if (lightProbeComponent == null || selectedProbes == null) return; // double check cuz ive seen unity not updating UI
            lightProbeComponent.probePositions = selectedProbes.positions;

            EditorSceneManager.MarkAllScenesDirty();
        }
    }

    void LightingData()
    {
        GUILayout.Label(EditorSceneManager.GetActiveScene().name);

        if (GUILayout.Button("Recover Lightmaps"))
        {
            string path = ResPath.Combine(ResPath.Combine("Lightmap", "High"), EditorSceneManager.GetActiveScene().name);
            Texture2D[] array = Resources.LoadAll<Texture2D>(path);
            if (array != null && array.Length > 0)
            {
                List<Texture2D> list = new List<Texture2D>();
                Texture2D[] array2 = array;
                foreach (Texture2D item in array2)
                {
                    list.Add(item);
                }
                list.Sort((Texture2D lightmap1, Texture2D lightmap2) => lightmap1.name.CompareTo(lightmap2.name));
                LightmapData lightmapData = new LightmapData();
                lightmapData.lightmapColor = list[0];
                List<LightmapData> list2 = new List<LightmapData>();
                list2.Add(lightmapData);
                LightmapSettings.lightmaps = list2.ToArray();
            }
        }
    }
}
