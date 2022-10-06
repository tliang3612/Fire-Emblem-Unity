/*using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Levels))]
public class LevelEditor : Editor
{

    public bool showLevels = true;
    Vector2 scrollPos = Vector2.zero;

    public override void OnInspectorGUI()
    {
        Levels levels = (Levels)target;     
        EditorGUILayout.Space();

        showLevels = EditorGUILayout.Foldout(showLevels, "Levels (" + levels.allLevels.Length + ")");

        if (showLevels)
        {
            EditorGUI.indentLevel++;

            for (int i = 0; i < levels.allLevels.Length; i++)
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

                levels.allLevels[i].showBoard = EditorGUILayout.Foldout(levels.allLevels[i].showBoard, "Board");
                if (levels.allLevels[i].showBoard)
                {
                    EditorGUI.indentLevel = 0;

                    GUIStyle tableStyle = new GUIStyle("box");
                    tableStyle.padding = new RectOffset(10, 10, 10, 10);
                    tableStyle.margin.left = 32;

                    GUIStyle headerColumnStyle = new GUIStyle();
                    headerColumnStyle.fixedWidth = 35;

                    GUIStyle columnStyle = new GUIStyle();
                    columnStyle.fixedWidth = 56;

                    GUIStyle rowStyle = new GUIStyle();
                    rowStyle.fixedHeight = 25;

                    GUIStyle rowHeaderStyle = new GUIStyle();
                    rowHeaderStyle.fixedWidth = columnStyle.fixedWidth - 1;

                    GUIStyle columnHeaderStyle = new GUIStyle();
                    columnHeaderStyle.fixedWidth = 30;
                    columnHeaderStyle.fixedHeight = 25f;

                    GUIStyle columnLabelStyle = new GUIStyle();
                    columnLabelStyle.fixedWidth = rowHeaderStyle.fixedWidth - 6;
                    columnLabelStyle.alignment = TextAnchor.MiddleCenter;
                    columnLabelStyle.fontStyle = FontStyle.Bold;

                    GUIStyle cornerLabelStyle = new GUIStyle();
                    cornerLabelStyle.fixedWidth = 42;
                    cornerLabelStyle.alignment = TextAnchor.MiddleRight;
                    cornerLabelStyle.fontStyle = FontStyle.BoldAndItalic;
                    cornerLabelStyle.fontSize = 14;
                    cornerLabelStyle.padding.top = -5;

                    GUIStyle rowLabelStyle = new GUIStyle();
                    rowLabelStyle.fixedWidth = 25;
                    rowLabelStyle.alignment = TextAnchor.MiddleRight;
                    rowLabelStyle.fontStyle = FontStyle.Bold;

                    GUIStyle enumStyle = new GUIStyle("popup");
                    rowStyle.fixedWidth = 53;

                    EditorGUILayout.BeginHorizontal(tableStyle);
                    for (int x = -1; x < levels.allLevels[i].columns; x++)
                    {
                        EditorGUILayout.BeginVertical((x == -1) ? headerColumnStyle : columnStyle);
                        for (int y = -1; y < levels.allLevels[i].rows; y++)
                        {
                            if (x == -1 && y == -1)
                            {
                                EditorGUILayout.BeginVertical(rowHeaderStyle);
                                EditorGUILayout.LabelField("[X,Y]", cornerLabelStyle);
                                EditorGUILayout.EndHorizontal();
                            }
                            else if (x == -1)
                            {
                                EditorGUILayout.BeginVertical(columnHeaderStyle);
                                EditorGUILayout.LabelField(y.ToString(), rowLabelStyle);
                                EditorGUILayout.EndHorizontal();
                            }
                            else if (y == -1)
                            {
                                EditorGUILayout.BeginVertical(rowHeaderStyle);
                                EditorGUILayout.LabelField(x.ToString(), columnLabelStyle);
                                EditorGUILayout.EndHorizontal();
                            }

                            if (x >= 0 && y >= 0)
                            {
                                EditorGUILayout.BeginHorizontal(rowStyle);
                                levels.allLevels[i].board[x, y] = (TileType)EditorGUILayout.EnumPopup(levels.allLevels[i].board[x, y], enumStyle);
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();
                }                         
                GUILayout.EndScrollView();
                
            }        
        }
        
    }
}*/