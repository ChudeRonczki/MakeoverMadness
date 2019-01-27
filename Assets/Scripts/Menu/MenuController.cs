using UnityEngine;

public class MenuController : MonoBehaviour
{
	[SerializeField] private MenuButton[] m_buttons;
	
	private int m_currentIndex = 0;

	private bool m_inputActive = false;

	[SerializeField] private string m_verticalAxis = "Vertical";
	[SerializeField] private string m_acceptButton = "FurniturePreview";
	[SerializeField] private string m_backButton = "Lift";

	[SerializeField] private GameObject m_credits;

	private void Start()
	{
		m_buttons[0].Select();
		m_credits.SetActive(false);
	}

	public void StartGame()
	{
		Debug.Log("Start game");
	}

	public void ShowHighscores()
	{
		Debug.Log("HighScores");
	}

	public void ShowCredits()
	{
		m_credits.SetActive(true);
	}

	public void ExitGame()
	{
		Application.Quit();
	}

	private void Update()
	{
		if (m_credits.activeSelf)
		{
			if (Input.GetButtonDown(m_backButton))
				m_credits.SetActive(false);
		}
		else
		{
			float verticalAxis = Input.GetAxis(m_verticalAxis);

			if (!m_inputActive && !Mathf.Approximately(verticalAxis, 0))
			{
				m_inputActive = true;
				int newIndex = m_currentIndex;

				if (verticalAxis < 0)
				{
					newIndex = Mathf.Min(m_buttons.Length - 1, m_currentIndex + 1);
				}
				else if (verticalAxis > 0)
				{
					newIndex = Mathf.Max(0, m_currentIndex - 1);
				}

				if (newIndex != m_currentIndex)
				{
					m_buttons[m_currentIndex].Deselect();
					m_buttons[newIndex].Select();
					m_currentIndex = newIndex;
				}
			}
			else if (m_inputActive && Mathf.Approximately(verticalAxis, 0))
			{
				m_inputActive = false;
			}

			if (Input.GetButtonDown(m_acceptButton))
			{
				m_buttons[m_currentIndex].Click();
			}
		}
	}
}