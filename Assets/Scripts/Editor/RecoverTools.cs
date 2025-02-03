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
}
