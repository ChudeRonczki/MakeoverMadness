using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
	[SerializeField] private MenuButton[] m_buttons;
	
	private int m_currentIndex = 0;

	private bool m_inputVerticalActive = false;
	private bool m_inputHorizontalActive = false;

	[SerializeField] private string m_horizontalAxis = "Horizontal";
	[SerializeField] private string m_verticalAxis = "Vertical";
	[SerializeField] private string m_acceptButton = "FurniturePreview";
	[SerializeField] private string m_backButton = "Lift";

	[SerializeField] private GameObject m_credits;

	[SerializeField] private string[] m_levelsText;
	[SerializeField] private string[] m_levelsNames;

	private int m_levelIndex = 0;

	private void Start()
	{
		m_buttons[0].Select();
		ChangeLevelSelection(0);
		m_credits.SetActive(false);
	}

	private void ChangeLevelSelection(int change)
	{
		m_levelIndex = (m_levelIndex + m_levelsText.Length + change) % m_levelsText.Length;
		m_buttons[0].m_textMesh.text = "< " + m_levelsText[m_levelIndex] + " >";
	}

	public void StartGame()
	{
		SceneManager.LoadScene(m_levelsNames[m_levelIndex]);
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
			if (m_currentIndex == 0)
			{
				float horizontal = Input.GetAxis(m_horizontalAxis);

				if (!m_inputHorizontalActive && !Mathf.Approximately(horizontal, 0))
				{
					m_inputHorizontalActive = true;
					
					if (horizontal < 0)
						ChangeLevelSelection(-1);
					else
						ChangeLevelSelection(1);
				}
				else if (m_inputHorizontalActive && Mathf.Approximately(horizontal, 0))
				{
					m_inputHorizontalActive = false;
				}
			}
			
			float verticalAxis = Input.GetAxis(m_verticalAxis);

			if (!m_inputVerticalActive && !Mathf.Approximately(verticalAxis, 0))
			{
				m_inputVerticalActive = true;
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
			else if (m_inputVerticalActive && Mathf.Approximately(verticalAxis, 0))
			{
				m_inputVerticalActive = false;
			}

			if (Input.GetButtonDown(m_acceptButton))
			{
				m_buttons[m_currentIndex].Click();
			}
		}
	}
}