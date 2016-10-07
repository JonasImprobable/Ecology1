using Assets.Gamelogic.Visualizers.Player;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerControlsManager))]
public class PlayerControlsManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PlayerControlsManager PlayerControlsManager = (PlayerControlsManager) target;
        if (GUILayout.Button("Update Map"))
        {
            PlayerControlsManager.UpdateMap();
        }
        if (GUILayout.Button("Update Terrain"))
        {
            PlayerControlsManager.RefreshTerrain();
        }
        if (GUILayout.Button("Spawn Animals"))
        {
            PlayerControlsManager.SpawnAnimals();
        }
        if (GUILayout.Button("Set Animal Steering"))
        {
            PlayerControlsManager.SetAnimalSteeringParameters();
        }
    }
}
