using UnityEngine;

public class BGElementWrapAround : MonoBehaviour
{
	[SerializeField] private float m_minX;
	[SerializeField] private float m_maxX;

	[SerializeField] private float m_moveSpeed = -1;

	private void Start()
	{
		WarpPosition();
	}

	private void Update()
	{
		WarpPosition();
	}

	private void WarpPosition()
	{
		var pos = transform.position + Vector3.right * m_moveSpeed * Time.deltaTime;

		if (m_minX > pos.x)
			pos.x = m_maxX - m_minX + pos.x;

		if (m_maxX < pos.x)
			pos.x = m_minX + m_maxX - pos.x;

		transform.position = pos;
	}

	private void OnDrawGizmosSelected()
	{
		var pos = transform.position;
		pos.x = 0;

		Gizmos.DrawLine(pos + Vector3.right * m_minX, pos + Vector3.right * m_maxX);
	}
}