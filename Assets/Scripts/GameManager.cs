using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public enum TypeScene
    {
        Start,
        Game
    }

    public TypeScene m_typeScene;

    public static GameManager m_instance;
    private GameObject m_pauseMenuUI;
    private GameObject m_gameOverUI;
    public bool m_isPaused = false;
    public int m_passLevel;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Confined;
        m_isPaused = false;

        if (m_typeScene == TypeScene.Game)
        {
            if (m_pauseMenuUI == null)
            {
                m_pauseMenuUI = GameObject.FindGameObjectWithTag("Pause");
                m_pauseMenuUI.SetActive(false);
            }

            if (m_gameOverUI == null)
            {
                m_gameOverUI = GameObject.FindGameObjectWithTag("GameOver");
                m_gameOverUI.SetActive(false);
            }
            m_passLevel = GuillemGroupManager.m_instance.m_numInstances;
        }
    }

    private void Update()
    {

        if (m_typeScene == TypeScene.Game)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (m_isPaused)
                    ResumeGame();
                else
                    PauseGame();
            }

            if (m_passLevel <= 0)
            {
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }

    }

    public void StartGame(string levelname)
    {
        ResumeGame();
        SceneManager.LoadScene(levelname);
    }

    public void NextLevel(string levelname)
    {
        SceneManager.LoadSceneAsync(levelname);
    }

    public void PreviusLevel(string levelname)
    {
        SceneManager.LoadSceneAsync(levelname);
    }

    public void PauseGame()
    {
        m_isPaused = true;
        Time.timeScale = 0;
        if (m_pauseMenuUI != null) m_pauseMenuUI.SetActive(true);
    }

    public void ResumeGame()
    {
        m_isPaused = false;
        Time.timeScale = 1;
        if (m_pauseMenuUI != null) m_pauseMenuUI.SetActive(false);
    }

    public void GameOver()
    {
        m_isPaused = true;
        m_gameOverUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    public void Animation(Animator animator)
    {
        animator.SetBool("In", !animator.GetBool("In"));
    }

    public void RestartLevel()
    {
        if (Time.timeScale == 0f) Time.timeScale = 1;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

}
