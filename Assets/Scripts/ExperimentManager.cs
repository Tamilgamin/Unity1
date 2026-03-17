using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages chemistry experiments with instructions and expected reactions.
/// Tracks experiment progress and validates chemical combinations.
/// </summary>
public class ExperimentManager : MonoBehaviour
{
    /// <summary>
    /// Data class for an experiment
    /// </summary>
    [System.Serializable]
    public class Experiment
    {
        public string experimentName;
        public string description;
        public List<string> requiredChemicals = new List<string>();
        public string instructions;
        public string expectedReaction;
        public Sprite experimentImage;
        public bool isCompleted = false;

        public override string ToString()
        {
            return $"{experimentName}: {description}";
        }
    }

    /// <summary>
    /// List of available experiments
    /// </summary>
    [SerializeField]
    private List<Experiment> experiments = new List<Experiment>();

    /// <summary>
    /// Index of the currently active experiment
    /// </summary>
    private int currentExperimentIndex = 0;

    /// <summary>
    /// Reference to the beaker for tracking chemicals
    /// </summary>
    private Beaker currentBeaker;

    /// <summary>
    /// Event for experiment started
    /// </summary>
    public delegate void ExperimentStartedDelegate(Experiment experiment);
    public event ExperimentStartedDelegate OnExperimentStarted;

    /// <summary>
    /// Event for experiment completed
    /// </summary>
    public delegate void ExperimentCompletedDelegate(Experiment experiment);
    public event ExperimentCompletedDelegate OnExperimentCompleted;

    /// <summary>
    /// Event for experiment progress
    /// </summary>
    public delegate void ExperimentProgressDelegate(Experiment experiment, float progress);
    public event ExperimentProgressDelegate OnExperimentProgress;

    private void Start()
    {
        // Initialize default experiments if none are set
        if (experiments.Count == 0)
        {
            InitializeDefaultExperiments();
        }

        // Find a beaker in the scene
        currentBeaker = FindObjectOfType<Beaker>();

        if (currentBeaker != null)
        {
            currentBeaker.OnChemicalAdded += HandleChemicalAdded;
        }
    }

    private void OnDestroy()
    {
        if (currentBeaker != null)
        {
            currentBeaker.OnChemicalAdded -= HandleChemicalAdded;
        }
    }

    /// <summary>
    /// Initializes default chemistry experiments
    /// </summary>
    private void InitializeDefaultExperiments()
    {
        // Experiment 1: Neutralization
        Experiment exp1 = new Experiment
        {
            experimentName = "Acid-Base Neutralization",
            description = "Combine an acid (HCl) with a base (NaOH) to create a neutral solution",
            instructions = "1. Add 50ml of HCl\n2. Add 50ml of NaOH\n3. Observe the color change",
            expectedReaction = "Bubbling and color change to neutral gray",
            isCompleted = false
        };
        exp1.requiredChemicals.Add("HCl");
        exp1.requiredChemicals.Add("NaOH");
        experiments.Add(exp1);

        // Experiment 2: Precipitation
        Experiment exp2 = new Experiment
        {
            experimentName = "Precipitation Reaction",
            description = "Combine AgNO3 with NaCl to form a white precipitate",
            instructions = "1. Add 50ml of AgNO3\n2. Add 50ml of NaCl\n3. Observe white precipitate formation",
            expectedReaction = "White precipitate forms and settles",
            isCompleted = false
        };
        exp2.requiredChemicals.Add("AgNO3");
        exp2.requiredChemicals.Add("NaCl");
        experiments.Add(exp2);

        // Experiment 3: Combustion
        Experiment exp3 = new Experiment
        {
            experimentName = "Metal Combustion",
            description = "Observe the combustion of magnesium in oxygen",
            instructions = "1. Add Mg powder\n2. Add O2\n3. Heat with Bunsen burner\n4. Observe bright flame",
            expectedReaction = "Bright white flame and heat generation",
            isCompleted = false
        };
        exp3.requiredChemicals.Add("Mg");
        exp3.requiredChemicals.Add("O2");
        experiments.Add(exp3);
    }

    /// <summary>
    /// Gets the current active experiment
    /// </summary>
    public Experiment GetCurrentExperiment()
    {
        if (currentExperimentIndex >= 0 && currentExperimentIndex < experiments.Count)
        {
            return experiments[currentExperimentIndex];
        }
        return null;
    }

    /// <summary>
    /// Starts the current experiment
    /// </summary>
    public void StartCurrentExperiment()
    {
        Experiment current = GetCurrentExperiment();
        if (current != null)
        {
            Debug.Log($"Starting experiment: {current.experimentName}");
            OnExperimentStarted?.Invoke(current);
        }
    }

    /// <summary>
    /// Moves to the next experiment in the list
    /// </summary>
    public bool NextExperiment()
    {
        if (currentExperimentIndex < experiments.Count - 1)
        {
            currentExperimentIndex++;
            StartCurrentExperiment();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Moves to the previous experiment in the list
    /// </summary>
    public bool PreviousExperiment()
    {
        if (currentExperimentIndex > 0)
        {
            currentExperimentIndex--;
            StartCurrentExperiment();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Sets the current experiment by index
    /// </summary>
    public void SetExperiment(int index)
    {
        if (index >= 0 && index < experiments.Count)
        {
            currentExperimentIndex = index;
            StartCurrentExperiment();
        }
    }

    /// <summary>
    /// Marks the current experiment as completed
    /// </summary>
    public void CompleteCurrentExperiment()
    {
        Experiment current = GetCurrentExperiment();
        if (current != null)
        {
            current.isCompleted = true;
            Debug.Log($"Experiment completed: {current.experimentName}");
            OnExperimentCompleted?.Invoke(current);
        }
    }

    /// <summary>
    /// Checks if the current experiment is complete
    /// </summary>
    public bool IsExperimentComplete()
    {
        Experiment current = GetCurrentExperiment();
        return current != null && current.isCompleted;
    }

    /// <summary>
    /// Validates if the beaker has the required chemicals
    /// </summary>
    public bool ValidateChemicalRequirements(Beaker beaker)
    {
        Experiment current = GetCurrentExperiment();
        if (current == null)
            return false;

        List<Chemical> chemicalsInBeaker = beaker.GetChemicals();
        List<string> chemicalNames = new List<string>();

        foreach (Chemical chem in chemicalsInBeaker)
        {
            chemicalNames.Add(chem.chemicalName);
        }

        // Check if all required chemicals are present
        foreach (string requiredChem in current.requiredChemicals)
        {
            if (!chemicalNames.Contains(requiredChem))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Gets the progress for the current experiment
    /// </summary>
    public float GetCurrentProgress(Beaker beaker)
    {
        Experiment current = GetCurrentExperiment();
        if (current == null)
            return 0f;

        int chemicalCount = beaker.GetChemicalCount();
        int requiredCount = current.requiredChemicals.Count;

        if (requiredCount == 0)
            return 0f;

        return Mathf.Min(1f, (float)chemicalCount / requiredCount);
    }

    /// <summary>
    /// Gets all experiments
    /// </summary>
    public List<Experiment> GetAllExperiments()
    {
        return new List<Experiment>(experiments);
    }

    /// <summary>
    /// Gets the count of experiments
    /// </summary>
    public int GetExperimentCount()
    {
        return experiments.Count;
    }

    /// <summary>
    /// Gets the count of completed experiments
    /// </summary>
    public int GetCompletedExperimentCount()
    {
        int count = 0;
        foreach (Experiment exp in experiments)
        {
            if (exp.isCompleted)
                count++;
        }
        return count;
    }

    /// <summary>
    /// Adds a new experiment to the list
    /// </summary>
    public void AddExperiment(Experiment experiment)
    {
        if (experiment != null)
        {
            experiments.Add(experiment);
        }
    }

    /// <summary>
    /// Resets all experiments
    /// </summary>
    public void ResetAllExperiments()
    {
        foreach (Experiment exp in experiments)
        {
            exp.isCompleted = false;
        }
        currentExperimentIndex = 0;
        Debug.Log("All experiments reset.");
    }

    /// <summary>
    /// Handles when a chemical is added to a beaker
    /// </summary>
    private void HandleChemicalAdded(Chemical chemical)
    {
        if (currentBeaker == null)
            return;

        float progress = GetCurrentProgress(currentBeaker);
        Experiment current = GetCurrentExperiment();

        if (current != null)
        {
            OnExperimentProgress?.Invoke(current, progress);
        }

        // Auto-complete if all required chemicals are present
        if (ValidateChemicalRequirements(currentBeaker))
        {
            CompleteCurrentExperiment();
        }
    }
}
