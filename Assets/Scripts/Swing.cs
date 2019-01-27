using UnityEngine;

public class Swing : MonoBehaviour
{
	[SerializeField] private float m_swingSpeed = 10;
	[SerializeField] private float m_swingAmount = 10;

	private Quaternion m_rotation;
	private float m_timer = 0;
	
	private void Start()
	{
		m_rotation = transform.rotation;
	}

	private void Update()
	{
		m_timer += Time.deltaTime*m_swingSpeed;
		m_timer = Mathf.Repeat(m_timer, 2 * Mathf.PI);

		transform.rotation = m_rotation * Quaternion.AngleAxis(Mathf.Sin(m_timer) * m_swingAmount, Vector3.forward);
	}
}