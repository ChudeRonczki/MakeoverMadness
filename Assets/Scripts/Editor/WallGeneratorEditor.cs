using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WallGenerator))]
public class WallGeneratorEditor : Editor
{
	private void OnSceneGUI()
	{
		var wallGen = target as WallGenerator;
		Vector3 pos = wallGen.transform.position;
		bool rebuild = false;
		pos.y = 0;

		for (int x = 0; x < wallGen.m_size.x; x++)
		{
			for (int y = 0; y <  wallGen.m_size.y; y++)
			{
				Handles.DrawLine(pos + new Vector3(x, 0, y), pos + new Vector3(x, 0, y + 1));
				Handles.DrawLine(pos + new Vector3(x, 0, y), pos + new Vector3(x + 1, 0, y));
			}
		}

		var vertical = wallGen.m_verticalConnections;

		for (int x = 0; x < wallGen.m_size.x + 1; x++)
		{
			for (int y = 0; y < wallGen.m_size.y; y++)
			{
				int index = x + (wallGen.m_size.x + 1) * y;
				Handles.color = vertical[index] ? Color.green : Color.red;
				
				if (Handles.Button(pos + new Vector3(x,0,  y + 0.5f), Quaternion.identity, 0.1f, 0.3f, Handles.DotHandleCap))
				{
					Undo.RecordObject(target, "change wall");
					wallGen.m_verticalConnections[index] = !wallGen.m_verticalConnections[index];
					rebuild = true;
				}
			}
		}
		
		var horizontal = wallGen.m_horizontalConnections;

		for (int x = 0; x < wallGen.m_size.x; x++)
		{
			for (int y = 0; y < wallGen.m_size.y + 1; y++)
			{
				int index = x + wallGen.m_size.x * y;
				Handles.color = horizontal[index] ? Color.green : Color.red;
				
				if (Handles.Button(pos + new Vector3(x + 0.5f, 0, y), Quaternion.identity, 0.1f, 0.3f, Handles.DotHandleCap))
				{
					Undo.RecordObject(target, "change wall");
					wallGen.m_horizontalConnections[index] = !wallGen.m_horizontalConnections[index];
					rebuild = true;
				}
			}
		}

		if (rebuild)
			wallGen.Generate();
	}
}