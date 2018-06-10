using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyController), true)]
public class EnemyEditor : Editor {

    private void OnSceneGUI() {        
		var t = target as EnemyController;

        var distance = t.distanceToCharge;
		Vector3 distPosition = t.transform.TransformPoint(Vector3.forward * distance);

        using (var cc = new EditorGUI.ChangeCheckScope())
        {
			distPosition = Handles.PositionHandle(distPosition, Quaternion.AngleAxis(180, t.transform.up) * t.transform.rotation);
			Handles.Label(distPosition, "Distance " + distance, "button");

            if (cc.changed)
            {
                Undo.RecordObject(t, "Move Handles");
				t.distanceToCharge = (distPosition - t.transform.position).magnitude;
            }
        }

		Handles.DrawDottedLine(t.transform.position, distPosition, 0.2f);
	}
}
