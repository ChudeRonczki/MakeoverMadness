using UnityEngine;

public class WallGenerator : MonoBehaviour
{
	public enum ElenentIndex
	{
		TL = 0,
		TR = 1,
		BL = 2,
		BR = 3
	}

	[SerializeField] public Vector2Int m_size = new Vector2Int(3, 3);
	[SerializeField] public float m_wallHeight = 5f;

	[SerializeField] private GameObject[] m_normalWall;
	[SerializeField] private GameObject[] m_cornerWall;

	[HideInInspector] public bool[] m_verticalConnections;
	[HideInInspector] public bool[] m_horizontalConnections;
	
	[SerializeField] private Material m_mainMaterial;
	[SerializeField] private Material[] m_mainMaterialVariants;

	private void Reset()
	{
		UpdateSize();
	}

	[ContextMenu("Update sizes")]
	public void UpdateSize()
	{
		m_verticalConnections = new bool[(m_size.x + 1) * m_size.y];
		m_horizontalConnections = new bool[m_size.x * (m_size.y + 1)];
	}

	[ContextMenu("Generate")]
	public void Generate()
	{
		ClearWallS();

		for (int x = 0; x <= m_size.x; x++)
		{
			for (int y = 0; y <= m_size.y; y++)
			{
				bool down = y - 1 >= 0 && m_verticalConnections[x + (m_size.x + 1) * (y - 1)];
				bool up = y < m_size.y && m_verticalConnections[x + (m_size.x + 1) * y];

				bool left = x - 1 >= 0 && m_horizontalConnections[x - 1 + m_size.x * y];
				bool right = x < m_size.x && m_horizontalConnections[x + m_size.x*  y];

				/*bool up = m_dir.HasFlag(Dir.U);
				bool down = m_dir.HasFlag(Dir.D);
				
				bool left = m_dir.HasFlag(Dir.L);
				bool right = m_dir.HasFlag(Dir.R);*/

				Vector3 currentPos = transform.position + new Vector3(x, 0, y);

				if (up)
				{
					if (left) // 90 deg bend
					{
						InstantiateWallSegment(m_cornerWall[(int) ElenentIndex.BR], currentPos,
							Quaternion.AngleAxis(-90, Vector3.up), transform);
					}
					else if (right && !down) // 270 deg bend
					{
						InstantiateWallSegment(m_cornerWall[(int) ElenentIndex.TL],  currentPos,
							Quaternion.AngleAxis(90, Vector3.up), transform);
					}
					else // straight
					{
						InstantiateWallSegment(m_normalWall[(int) ElenentIndex.BR],  currentPos,
							Quaternion.AngleAxis(-90, Vector3.up), transform);
					}

					if (right) // 90 deg bend
					{
						InstantiateWallSegment(m_cornerWall[(int) ElenentIndex.BL],  currentPos,
							Quaternion.AngleAxis(90, Vector3.up), transform);
					}
					else if (left && !down) // 270 deg bend
					{
						InstantiateWallSegment(m_cornerWall[(int) ElenentIndex.TR],  currentPos,
							Quaternion.AngleAxis(-90, Vector3.up), transform);
					}
					else // straight
					{
						InstantiateWallSegment(m_normalWall[(int) ElenentIndex.BL],  currentPos,
							Quaternion.AngleAxis(90, Vector3.up), transform);
					}
				}

				if (down)
				{
					if (right)
					{
						InstantiateWallSegment(m_cornerWall[(int) ElenentIndex.BR], currentPos,
							Quaternion.AngleAxis(90, Vector3.up), transform);
					}
					else if (left && !up)
					{
						InstantiateWallSegment(m_cornerWall[(int) ElenentIndex.TL],  currentPos,
							Quaternion.AngleAxis(-90, Vector3.up), transform);
					}
					else
					{
						InstantiateWallSegment(m_normalWall[(int) ElenentIndex.TL],  currentPos,
							Quaternion.AngleAxis(-90, Vector3.up), transform);
					}

					if (left)
					{
						InstantiateWallSegment(m_cornerWall[(int) ElenentIndex.BL],  currentPos,
							Quaternion.AngleAxis(-90, Vector3.up), transform);
					}
					else if (right && !up)
					{
						InstantiateWallSegment(m_cornerWall[(int) ElenentIndex.TR], currentPos,
							Quaternion.AngleAxis(90, Vector3.up), transform);
					}
					else
					{
						InstantiateWallSegment(m_normalWall[(int) ElenentIndex.TR], currentPos,
							Quaternion.AngleAxis(90, Vector3.up), transform);
					}
				}


				if (left)
				{
					if (down)
					{
						InstantiateWallSegment(m_cornerWall[(int) ElenentIndex.BR],  currentPos,
							Quaternion.AngleAxis(180, Vector3.up), transform);
					}
					else if (up && !right)
					{
						InstantiateWallSegment(m_cornerWall[(int) ElenentIndex.TL],  currentPos,
							Quaternion.AngleAxis(0, Vector3.up), transform);
					}
					else
					{
						InstantiateWallSegment(m_normalWall[(int) ElenentIndex.BR],  currentPos,
							Quaternion.AngleAxis(-180, Vector3.up), transform);
					}

					if (up)
					{
						InstantiateWallSegment(m_cornerWall[(int) ElenentIndex.BL],  currentPos,
							Quaternion.AngleAxis(0, Vector3.up), transform);
					}
					else if (down && !right)
					{
						InstantiateWallSegment(m_cornerWall[(int) ElenentIndex.TR],  currentPos,
							Quaternion.AngleAxis(180, Vector3.up), transform);
					}
					else
					{
						InstantiateWallSegment(m_normalWall[(int) ElenentIndex.BL],  currentPos,
							Quaternion.AngleAxis(0, Vector3.up), transform);
					}
				}

				if (right)
				{
					if (up)
					{
						InstantiateWallSegment(m_cornerWall[(int) ElenentIndex.BR],  currentPos,
							Quaternion.AngleAxis(0, Vector3.up), transform);
					}
					else if (down && !left)
					{
						InstantiateWallSegment(m_cornerWall[(int) ElenentIndex.TL],  currentPos,
							Quaternion.AngleAxis(-180, Vector3.up), transform);
					}
					else
					{
						InstantiateWallSegment(m_normalWall[(int) ElenentIndex.TL],  currentPos,
							Quaternion.AngleAxis(-180, Vector3.up), transform);
					}

					if (down)
					{
						InstantiateWallSegment(m_cornerWall[(int) ElenentIndex.BL],  currentPos,
							Quaternion.AngleAxis(-180, Vector3.up), transform);
					}
					else if (up && !left)
					{
						InstantiateWallSegment(m_cornerWall[(int) ElenentIndex.TR],  currentPos,
							Quaternion.AngleAxis(0, Vector3.up), transform);
					}
					else
					{
						InstantiateWallSegment(m_normalWall[(int) ElenentIndex.TR],  currentPos,
							Quaternion.AngleAxis(0, Vector3.up), transform);
					}
				}
			}
		}
	}

	void InstantiateWallSegment(GameObject template, Vector3 position, Quaternion rotation, Transform parent)
	{
		var segment = Instantiate(template, position, rotation, parent);
		var collider = segment.AddComponent<BoxCollider>();
		collider.size = collider.size + new Vector3(0f, m_wallHeight, 0f);
	}

	private void ClearWallS()
	{
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			DestroyImmediate((transform.GetChild(i).gameObject));
		}
	}

	private void Start()
	{
		var meshRenderers = GetComponentsInChildren<MeshRenderer>();
		var mainMaterialVariant = m_mainMaterialVariants[Random.Range(0, m_mainMaterialVariants.Length)];

		foreach (var meshRenderer in meshRenderers)
		{
			var sharedMaterials = meshRenderer.sharedMaterials;

			for (int i = 0; i < sharedMaterials.Length; ++i)
			{
				if (sharedMaterials[i] == m_mainMaterial)
					sharedMaterials[i] = mainMaterialVariant;
			}

			meshRenderer.sharedMaterials = sharedMaterials;
		}
	}
}