using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    [Header("PAUSE")]
    public GameObject pausePanel;
    public Button resumeButton;
    public Button saveButton;
    public Button menuButton;
    public TextMeshProUGUI slotButtonLabel;

    [Header("CONFIRM")]
    public GameObject confirmPanel;
    public Button backButton;
    public Button saveAndQuitButton;
    public Button menuConfirmButton;

    [Header("SAVE SLOT SELECTION")]
    public GameObject saveSlotSelectionPanel;  // New panel for selecting save slot
    public Button[] saveSlotButtons;  // Buttons for each slot (1-3)
    public TextMeshProUGUI[] saveSlotLabels;  // Labels for each slot (1-3)
    public Button backToPauseButton;  // Back button to return to the pause menu

    public GameManagerSO gameManager;

    void Start()
    {
        // Initialize button listeners
        resumeButton.onClick.AddListener(ResumeGame);
        saveButton.onClick.AddListener(Save);
        menuButton.onClick.AddListener(MenuClick);
        menuConfirmButton.onClick.AddListener(MenuConfirmClick);
        backButton.onClick.AddListener(ConfirmBackClick);
        saveAndQuitButton.onClick.AddListener(SaveAndQuit);

        // Add listeners to each save slot button (1, 2, 3)
        for (int i = 0; i < saveSlotButtons.Length; i++)
        {
            int slot = i + 1;  // Slot numbers are 1, 2, 3
            saveSlotButtons[i].onClick.AddListener(() => OnSaveSlotChosen(slot));
        }

        // Set up the back button for returning to the pause menu
        backToPauseButton.onClick.AddListener(ReturnToPauseMenu);

        // Initially hide the Save Slot Selection panel
        saveSlotSelectionPanel.SetActive(false);

        UpdateSlotButton(false);
    }

    private void UpdateSlotButton(bool isSaved)
    {
        slotButtonLabel.text = isSaved ? "SAVED" : "SAVE";
        saveButton.interactable = !isSaved;
    }

    void Update()
    {
        // Handle pause and resume with the Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausePanel.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void ResumeGame()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        UpdateSlotButton(false);
        pausePanel.SetActive(true);
        gameManager.saveManager.AutoSaveAll();
    }

    private void Save()
    {
        // If the player is in the auto-save slot (slot 0), always prompt for slot selection
        if (gameManager.saveManager.currentSlot == 0)
        {
            saveSlotSelectionPanel.SetActive(true);  // Show the slot selection panel
            pausePanel.SetActive(false);             // Hide the pause panel
            UpdateChooseSlotButtons();              // Update button labels (Override/EMPTY)
        }
        else
        {
            // Save directly to the selected slot (1, 2, or 3) without prompting
            gameManager.saveManager.SaveAllData();
            UpdateSlotButton(true);
        }
    }

    private void OnSaveSlotChosen(int slot)
    {
        // Save the data to the selected slot (1, 2, or 3)
        gameManager.saveManager.currentSlot = slot;
        gameManager.saveManager.SaveAllData();

        // Close the save slot selection panel
        saveSlotSelectionPanel.SetActive(false);

        // Update the save status on the main screen
        UpdateSlotButton(true);
    }

    private void UpdateChooseSlotButtons()
    {
        // Update the slot buttons dynamically based on whether the slots are empty or have saved data
        for (int i = 0; i < saveSlotButtons.Length; i++)
        {
            int slot = i + 1;  // Slot numbers are 1, 2, 3

            if (gameManager.saveManager.IsDataSaved(slot))
            {
                // If the slot contains data, set the label to "Override"
                saveSlotLabels[i].text = "Override";
            }
            else
            {
                // If the slot is empty, set the label to "EMPTY"
                saveSlotLabels[i].text = "EMPTY";
            }

            // Style the text to be size 24, bold, red, and centered
            saveSlotLabels[i].fontSize = 24;
            saveSlotLabels[i].fontStyle = FontStyles.Bold;
            saveSlotLabels[i].color = Color.red;
            saveSlotLabels[i].alignment = TextAlignmentOptions.Center;
        }
    }

    private void ReturnToPauseMenu()
    {
        // Hide the save slot selection panel and return to the pause menu
        saveSlotSelectionPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    private void MenuClick()
    {
        if (saveButton.interactable)
        {
            pausePanel.SetActive(false);
            confirmPanel.SetActive(true);
            return;
        }

        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    private void MenuConfirmClick()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    private void ConfirmBackClick()
    {
        confirmPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    private void SaveAndQuit()
    {
        gameManager.saveManager.SaveAllData();
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }
}
