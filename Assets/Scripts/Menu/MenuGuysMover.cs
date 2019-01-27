using UnityEngine;

public class MenuGuysMover : MonoBehaviour
{
	[SerializeField] private float m_oscilationValue = 1;

	private Vector3 m_position;
	private float m_timer;
	private void Start()
	{
		m_timer = 0;
		m_position = transform.position;
	}

	private void Update()
	{
		m_timer += Time.deltaTime;
		m_timer = Mathf.Repeat(m_timer, 2 * Mathf.PI);
		transform.position = m_position + Vector3.right * Mathf.Sin(m_timer) * m_oscilationValue;
	}
}