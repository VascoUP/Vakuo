using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

[CustomEditor(typeof(BezierGenerator))]
public class BezierEditor : Editor
{
    private enum JoinType { FirstURow, FirstVRow, LastURow, LastVRow };
    private JoinType this_JoinType = JoinType.FirstURow;
    private JoinType oth_JoinType = JoinType.LastURow;

    private bool _pushURow = false;
    private bool _pushVRow = false;
    private bool _translateAllVertices = false;
    private List<Vector3> m_controlPoints;
    private BezierGenerator _join = null;
    private AnimBool _showJoinOptions = new AnimBool();
    private AnimBool _showAddVRow = new AnimBool();
    private int _addVAtIndex = 0;
    private AnimBool _showAddURow = new AnimBool();
    private int _addUAtIndex = 0;

    private void OnSceneGUI()
    {
        var t = target as BezierGenerator;
        m_controlPoints = new List<Vector3>(t.controlPoints);
        var uv = new Vector2(t.uOrder, t.vOrder);
        
        for (int i = 0; i < m_controlPoints.Count; i++) 
        {
            EditorGUI.BeginChangeCheck();
            m_controlPoints[i] = 
                Handles.PositionHandle(t.transform.TransformPoint(m_controlPoints[i]), Quaternion.AngleAxis(180, t.transform.up) * t.transform.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                DisplaceVertices(t, m_controlPoints, i);
            }
            
            Vector2 coord = BezierGenerator.IndexToDoubleCoordinates(i, t.uOrder + 1);
            Handles.Label(m_controlPoints[i], "(" + (int)coord.x + "," + (int)coord.y + ")", "button");
        }

        Handles.color = Color.yellow;
        for(var v = 0; v < uv.y + 1; v++)
        {
            for (var u = 0; u < uv.x + 1; u++)
            {
                if (u > 0)
                {
                    Handles.DrawDottedLine(
                        m_controlPoints[BezierGenerator.DoubleCoordinatesToIndex(u, v, (int)uv.x + 1)],
                        m_controlPoints[BezierGenerator.DoubleCoordinatesToIndex(u - 1, v, (int)uv.x + 1)], 0.2f);
                }
                if(v > 0)
                {
                    Handles.DrawDottedLine(
                        m_controlPoints[BezierGenerator.DoubleCoordinatesToIndex(u, v, (int)uv.x + 1)],
                        m_controlPoints[BezierGenerator.DoubleCoordinatesToIndex(u, v - 1, (int)uv.x + 1)], 0.2f);
                }
            }
        }

        if(GUI.changed)
        {
            t.reDraw = true;

        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var t = target as BezierGenerator;

        _pushURow = EditorGUILayout.Toggle("Push U Row", _pushURow);
        if (_pushURow)
        {
            if (_pushVRow)
            {
                _pushVRow = false;
            }
        }

        _pushVRow = EditorGUILayout.Toggle("Push V Row", _pushVRow);
        if (_pushVRow)
        {
            if (_pushURow)
            {
                _pushURow = false;
            }
        }

        t.resolution = EditorGUILayout.IntSlider("Resolution", t.resolution, 2, 100);

        _showAddVRow.target = EditorGUILayout.ToggleLeft("Show add V row", _showAddVRow.target);
        using (var addVRow = new EditorGUILayout.FadeGroupScope(_showAddVRow.faded))
        {
            if(addVRow.visible)
            {
                _addVAtIndex = EditorGUILayout.IntSlider(_addVAtIndex, 0, t.vOrder + 1);
                if (GUILayout.Button("Add V Row"))
                {
                    CreateVRow(t, _addVAtIndex);
                }
            }
        }

        _showAddURow.target = EditorGUILayout.ToggleLeft("Show add U row", _showAddURow.target);
        using (var addURow = new EditorGUILayout.FadeGroupScope(_showAddURow.faded))
        {
            if (addURow.visible)
            {
                _addUAtIndex = EditorGUILayout.IntSlider(_addUAtIndex, 0, t.uOrder + 1);
                if (GUILayout.Button("Add U Row"))
                {
                    CreateURow(t, _addUAtIndex);
                }
            }
        }

        _showJoinOptions.target = EditorGUILayout.ToggleLeft("Show join options", _showJoinOptions.target);
        using (var joinGroup = new EditorGUILayout.FadeGroupScope(_showJoinOptions.faded))
        {
            if(joinGroup.visible)
            {
                _translateAllVertices = EditorGUILayout.Toggle("Translate all vertices", _translateAllVertices);
                this_JoinType = (JoinType)EditorGUILayout.EnumPopup("This mesh join side", this_JoinType);
                oth_JoinType = (JoinType)EditorGUILayout.EnumPopup("Other mesh join side", oth_JoinType);

                EditorGUI.BeginChangeCheck();
                _join = EditorGUILayout.ObjectField("Join to:", _join, typeof(BezierGenerator), true) as BezierGenerator;
                if (EditorGUI.EndChangeCheck())
                {
                    if (IsValidJoin(t, _join, this_JoinType, oth_JoinType))
                    {
                        JoinMeshes(t, _join, this_JoinType, oth_JoinType, _translateAllVertices);
                    }
                    _join = null;
                }
            }
        }
    }


    private void CreateVRow(BezierGenerator target, int addIndex)
    {
        if(addIndex > target.vOrder + 1)
        {
            return;
        }

        for(int i = 0; i <= target.uOrder; i++)
        {
            Vector3 position;
            if(addIndex < target.vOrder + 1)
            {
                position = target.controlPoints[2 * i + (target.uOrder + 1) * addIndex];
            }
            target.controlPoints.Insert(i + (target.uOrder + 1) * addIndex, Vector3.zero);
        }

        target.vOrder++;
    }

    private void CreateURow(BezierGenerator target, int addIndex)
    {
        if (addIndex > target.uOrder + 1)
        {
            return;
        }

        for (int i = 0; i <= target.vOrder; i++)
        {
            int index = BezierGenerator.DoubleCoordinatesToIndex(addIndex, i, target.uOrder + 2);
            target.controlPoints.Insert(index, Vector3.zero);
        }

        target.uOrder++;
    }


    private void DisplaceVertices(BezierGenerator target, List<Vector3> cp, int cpIndex)
    {
        Vector3 delta = cp[cpIndex] - target.transform.TransformPoint(target.controlPoints[cpIndex]);
        if (_pushURow || _pushVRow)
        {
            Vector2 uvCoords = BezierGenerator.IndexToDoubleCoordinates(cpIndex, target.uOrder + 1);
            for (int j = 0; j <= (_pushURow ? target.uOrder : target.vOrder); j++)
            {
                int index = 0;

                if (_pushURow)
                {
                    index = BezierGenerator.DoubleCoordinatesToIndex(j, (int)uvCoords.y, target.uOrder + 1);
                }
                else
                {
                    index = BezierGenerator.DoubleCoordinatesToIndex((int)uvCoords.x, j, target.uOrder + 1);
                }

                if (index != cpIndex)
                {
                    if (index < cpIndex)
                    {
                        cp[index] += delta;
                    }
                    else
                    {
                        cp[index] = target.transform.TransformPoint(cp[index]);
                        cp[index] += delta;
                    }

                    target.controlPoints[index] = target.transform.InverseTransformPoint(cp[index]);
                }
            }
        }

        target.controlPoints[cpIndex] = target.transform.InverseTransformPoint(cp[cpIndex]);
    }


    private bool IsValidJoin(BezierGenerator curr, BezierGenerator join, JoinType currGeneratorType, JoinType targetType)
    {
        int currOrder = 0;
        int joinOrder = 0;

        if(currGeneratorType == JoinType.LastURow || currGeneratorType == JoinType.FirstURow)
        {
            currOrder = curr.vOrder;
        }
        else
        {
            currOrder = curr.uOrder;
        }

        if (targetType == JoinType.LastURow || targetType == JoinType.FirstURow)
        {
            joinOrder = join.vOrder;
        }
        else
        {
            joinOrder = join.uOrder;
        }

        return join != null && currOrder == joinOrder && join.IsValid() && curr != join;
    }

    private void JoinMeshes(BezierGenerator curr, BezierGenerator join, JoinType currGeneratorType, JoinType targetType, bool translateAllVertices)
    {
        int maxI = (currGeneratorType == JoinType.LastURow || currGeneratorType == JoinType.FirstURow) ? curr.vOrder : curr.uOrder;
        int maxJ = (currGeneratorType == JoinType.LastURow || currGeneratorType == JoinType.FirstURow) ? curr.uOrder : curr.vOrder;
        for (int i = 0; i <= maxI; i++)
        {
            int index = NextIndex(i, 0, curr.uOrder, curr.vOrder, curr.uOrder + 1, currGeneratorType);
            int joinIndex = NextIndex(i, 0, join.uOrder, join.vOrder, join.uOrder + 1, targetType);

            Vector3 delta =
                curr.controlPoints[index] -
                    curr.transform.InverseTransformPoint(
                        join.transform.TransformPoint(
                            join.controlPoints[joinIndex]));

        
            curr.controlPoints[index] -= delta;
        
            if(translateAllVertices)
            {
                for (int j = 1; j <= maxJ; j++)
                {
                    index = NextIndex(i, j, curr.uOrder, curr.vOrder, curr.uOrder + 1, currGeneratorType);
                    curr.controlPoints[index] -= delta;
                }
            }
        }
    }

    private int NextIndex(int i, int j, int uOrder, int vOrder, int width, JoinType joinType)
    {
        int u = i, v = 0;
        switch(joinType)
        {
            case JoinType.FirstVRow:
                u = i;
                v = j;
                break;
            case JoinType.LastVRow:
                u = i;
                v = vOrder - j;
                break;
            case JoinType.FirstURow:
                u = j;
                v = i;
                break;
            case JoinType.LastURow:
                u = uOrder - j;
                v = i;
                break;
        }
        return BezierGenerator.DoubleCoordinatesToIndex(u, v, width);
    }
}
