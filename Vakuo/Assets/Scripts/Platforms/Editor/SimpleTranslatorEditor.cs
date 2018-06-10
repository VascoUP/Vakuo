using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlatformTranslator), true)]
public class SimpleTranslatorEditor : Editor {

    private void OnSceneGUI() {
        var t = target as PlatformTranslator;

        var start = t.startPosition;
        var end = t.endPosition;

        using (var cc = new EditorGUI.ChangeCheckScope())
        {
			start = Handles.PositionHandle(start, Quaternion.AngleAxis(180, t.transform.up) * t.transform.rotation);
			Handles.Label(start, "Start", "button");
			end = Handles.PositionHandle(end, Quaternion.AngleAxis(180, t.transform.up) * t.transform.rotation);
			Handles.Label(end, "End", "button");

            if (cc.changed)
            {
                Undo.RecordObject(t, "Move Handles");
				t.startPosition = start;
				t.endPosition = end;
            }
        }

		Handles.DrawDottedLine(start, end, 0.2f);
	}
}
