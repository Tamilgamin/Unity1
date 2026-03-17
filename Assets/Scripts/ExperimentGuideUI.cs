using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays experiment instructions and guidance messages on the UI canvas.
/// Provides user feedback for AR interactions and chemistry lab operations.
/// </summary>
public class ExperimentGuideUI : MonoBehaviour
{
    /// <summary>
    /// Text element for displaying current instructions
    /// </summary>
    [SerializeField]
    private Text instructionsText;

    /// <summary>
    /// Text element for displaying status messages
    /// </summary>
    [SerializeField]
    private Text statusText;

    /// <summary>
    /// Text element for displaying the experiment title
    /// </summary>
    [SerializeField]
    private Text experimentTitleText;

    /// <summary>
    /// Text element for displaying chemical count
    /// </summary>
    [SerializeField]
    private Text chemicalCountText;

    /// <summary>
    /// Image for progress bar display
    /// </summary>
    [SerializeField]
    private Image progressBar;

    /// <summary>
    /// Panel for experiment guide
    /// </summary>
    [SerializeField]
    private CanvasGroup guidePanel;

    /// <summary>
    /// Button to proceed to next step
    /// </summary>
    [SerializeField]
    private Button nextButton;

    /// <summary>
    /// Button to go back
    /// </summary>
    [SerializeField]
    private Button previousButton;

    /// <summary>
    /// Duration for status messages
    /// </summary>
    [SerializeField]
    private float messageDisplayDuration = 3f;

    /// <summary>
    /// Reference to the experiment manager
    /// </summary>
    private ExperimentManager experimentManager;

    /// <summary>
    /// Reference to the AR object placement manager
    /// </summary>
    private ARObjectPlacement arObjectPlacement;

    /// <summary>
    /// Reference to the beaker
    /// </summary>
    private Beaker currentBeaker;

    /// <summary>
    /// Current state of the app
    /// </summary>
    private enum AppState
    {
        WaitingForTablePlacement,
        TablePlaced,
        ReadyForExperiment,
        ExperimentInProgress,
        ReactionOccurred
    }

    private AppState currentState = AppState.WaitingForTablePlacement;

    private void Start()
    {
        // Get references to manager scripts
        experimentManager = FindObjectOfType<ExperimentManager>();
        arObjectPlacement = FindObjectOfType<ARObjectPlacement>();

        // Setup UI element references if not assigned
        if (instructionsText == null)
        {
            instructionsText = GetComponentInChildren<Text>();
        }

        // Setup button listeners
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnNextButtonClicked);
        }

        if (previousButton != null)
        {
            previousButton.onClick.AddListener(OnPreviousButtonClicked);
        }

        // Subscribe to events
        if (arObjectPlacement != null)
        {
            arObjectPlacement.OnTablePlaced += HandleTablePlaced;
            arObjectPlacement.OnPlacementFailed += HandlePlacementFailed;
        }

        if (experimentManager != null)
        {
            experimentManager.OnExperimentStarted += HandleExperimentStarted;
            experimentManager.OnExperimentCompleted += HandleExperimentCompleted;
            experimentManager.OnExperimentProgress += HandleExperimentProgress;
        }

        // Initialize UI with starting message
        InitializeUI();
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (arObjectPlacement != null)
        {
            arObjectPlacement.OnTablePlaced -= HandleTablePlaced;
            arObjectPlacement.OnPlacementFailed -= HandlePlacementFailed;
        }

        if (experimentManager != null)
        {
            experimentManager.OnExperimentStarted -= HandleExperimentStarted;
            experimentManager.OnExperimentCompleted -= HandleExperimentCompleted;
            experimentManager.OnExperimentProgress -= HandleExperimentProgress;
        }

        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(OnNextButtonClicked);
        }

        if (previousButton != null)
        {
            previousButton.onClick.RemoveListener(OnPreviousButtonClicked);
        }
    }

    /// <summary>
    /// Initializes the UI with startup instructions
    /// </summary>
    private void InitializeUI()
    {
        currentState = AppState.WaitingForTablePlacement;
        UpdateInstructions("Welcome to AR Chemistry Lab\n\nTap on a horizontal surface to place the lab table.");
        UpdateStatus("Waiting for table placement...");
    }

    /// <summary>
    /// Updates the instruction text display
    /// </summary>
    public void UpdateInstructions(string text)
    {
        if (instructionsText != null)
        {
            instructionsText.text = text;
        }
        Debug.Log($"Instructions: {text}");
    }

    /// <summary>
    /// Updates the status message display
    /// </summary>
    public void UpdateStatus(string text)
    {
        if (statusText != null)
        {
            statusText.text = text;
        }
        Debug.Log($"Status: {text}");
    }

    /// <summary>
    /// Updates the experiment title display
    /// </summary>
    private void UpdateExperimentTitle(string title)
    {
        if (experimentTitleText != null)
        {
            experimentTitleText.text = title;
        }
    }

    /// <summary>
    /// Updates the chemical count display
    /// </summary>
    private void UpdateChemicalCount(int count, int total)
    {
        if (chemicalCountText != null)
        {
            chemicalCountText.text = $"Chemicals: {count}/{total}";
        }
    }

    /// <summary>
    /// Updates the progress bar
    /// </summary>
    private void UpdateProgressBar(float progress)
    {
        if (progressBar != null)
        {
            progressBar.fillAmount = Mathf.Clamp01(progress);
        }
    }

    /// <summary>
    /// Shows a temporary status message
    /// </summary>
    public void ShowTemporaryMessage(string message)
    {
        UpdateStatus(message);
        StartCoroutine(ClearStatusAfterDelay(messageDisplayDuration));
    }

    /// <summary>
    /// Coroutine to clear status after delay
    /// </summary>
    private System.Collections.IEnumerator ClearStatusAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UpdateStatus("");
    }

    /// <summary>
    /// Handles table placement event
    /// </summary>
    private void HandleTablePlaced(Vector3 position)
    {
        currentState = AppState.TablePlaced;
        
        UpdateInstructions("Lab table placed successfully!\n\nTap on the table to place laboratory equipment (beaker, test tube, flask, burner)");
        ShowTemporaryMessage("Table placed - Ready to add equipment");

        if (experimentManager != null)
        {
            experimentManager.StartCurrentExperiment();
        }
    }

    /// <summary>
    /// Handles placement failure event
    /// </summary>
    private void HandlePlacementFailed(string reason)
    {
        ShowTemporaryMessage($"Placement failed: {reason}");
        UpdateInstructions("Unable to place table. Try tapping on a different horizontal surface.");
    }

    /// <summary>
    /// Handles experiment started event
    /// </summary>
    private void HandleExperimentStarted(ExperimentManager.Experiment experiment)
    {
        currentState = AppState.ReadyForExperiment;

        UpdateExperimentTitle(experiment.experimentName);
        UpdateInstructions($"Experiment: {experiment.experimentName}\n\n{experiment.instructions}");
        ShowTemporaryMessage($"Started: {experiment.experimentName}");
    }

    /// <summary>
    /// Handles experiment completed event
    /// </summary>
    private void HandleExperimentCompleted(ExperimentManager.Experiment experiment)
    {
        currentState = AppState.ExperimentInProgress;
        
        ShowTemporaryMessage($"Experiment Complete! Expected: {experiment.expectedReaction}");
        UpdateStatus($"Expected reaction: {experiment.expectedReaction}");
    }

    /// <summary>
    /// Handles experiment progress event
    /// </summary>
    private void HandleExperimentProgress(ExperimentManager.Experiment experiment, float progress)
    {
        UpdateProgressBar(progress);

        if (currentBeaker != null)
        {
            UpdateChemicalCount(currentBeaker.GetChemicalCount(), experiment.requiredChemicals.Count);
        }
    }

    /// <summary>
    /// Handles next button click
    /// </summary>
    private void OnNextButtonClicked()
    {
        if (experimentManager != null)
        {
            if (experimentManager.NextExperiment())
            {
                ShowTemporaryMessage("Next experiment");
            }
            else
            {
                ShowTemporaryMessage("No more experiments available");
            }
        }
    }

    /// <summary>
    /// Handles previous button click
    /// </summary>
    private void OnPreviousButtonClicked()
    {
        if (experimentManager != null)
        {
            if (experimentManager.PreviousExperiment())
            {
                ShowTemporaryMessage("Previous experiment");
            }
            else
            {
                ShowTemporaryMessage("Already at first experiment");
            }
        }
    }

    /// <summary>
    /// Displays help message
    /// </summary>
    public void ShowHelp()
    {
        switch (currentState)
        {
            case AppState.WaitingForTablePlacement:
                UpdateInstructions("1. Look around to find a horizontal surface\n2. Tap on the surface to place the lab table\n3. The table must be placed on a detected plane");
                break;

            case AppState.TablePlaced:
                UpdateInstructions("1. Tap on the table to place equipment\n2. Different buttons spawn: Beaker (B), Test Tube (T), Flask (F), Burner (U)\n3. Once placed, add chemicals to experiment");
                break;

            case AppState.ReadyForExperiment:
                UpdateInstructions("1. Place beakers and test tubes on the table\n2. Add the required chemicals listed in the experiment\n3. Watch the reaction effects when all chemicals are mixed");
                break;

            case AppState.ReactionOccurred:
                UpdateInstructions("Reaction observed! The chemicals have combined.\nYou can continue with another experiment or reset.");
                break;
        }
    }

    /// <summary>
    /// Sets the current beaker reference for monitoring
    /// </summary>
    public void SetCurrentBeaker(Beaker beaker)
    {
        currentBeaker = beaker;
    }

    /// <summary>
    /// Toggles the visibility of the guide panel
    /// </summary>
    public void ToggleGuidePanel()
    {
        if (guidePanel != null)
        {
            guidePanel.alpha = (guidePanel.alpha > 0.5f) ? 0f : 1f;
        }
    }
}
