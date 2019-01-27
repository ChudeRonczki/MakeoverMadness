using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MenuButton : MonoBehaviour
{
	[SerializeField] private Animator m_animator;

	[SerializeField] private string m_select;
	[SerializeField] private string m_click;
	[SerializeField] private string m_deselect;

	[SerializeField] private UnityEvent m_clickEvent;
	public TextMeshPro m_textMesh;

	public void Select()
	{
		m_animator.ResetTrigger(m_deselect);
		m_animator.SetTrigger(m_select);
	}

	public void Click()
	{
		m_animator.SetTrigger(m_click);
		m_clickEvent?.Invoke();
	}

	public void Deselect()
	{
		m_animator.ResetTrigger(m_select);
		m_animator.SetTrigger(m_deselect);
	}
}