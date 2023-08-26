using UnityEngine;
using UnityEngine.UI;

public class GrowSpace : MonoBehaviour
{
    public Vector3 size; // public variable for the size (length, width, height) of the grow space
    public float volume; // public variable for the volume of the grow space

    public GameObject LWH_inputPanelPrefab; // Reference to a prefab for a panel containing three TextMeshPro input fields
    public Button buildButton; // Reference to the button that will apply the changes

    private GameObject inputPanel; // Instance of the input panel

    // Start is called before the first frame update
    void Start()
    {
        // delete this to run this scripot. PromptForSize();
    }

    public void PromptForSize()
    {
        // Instantiate the input panel from the prefab
        inputPanel = Instantiate(LWH_inputPanelPrefab, Vector3.zero, Quaternion.identity);
        inputPanel.transform.SetParent(GameObject.Find("Canvas").transform, false);

        // Find the input fields on the panel and add listeners to the End Edit event
        InputField[] inputFields = inputPanel.GetComponentsInChildren<InputField>();
        if (inputFields.Length != 3)
        {
            Debug.LogError("Input panel should have exactly three InputFields for length, width, and height.");
            return;
        }

        inputFields[0].onEndEdit.AddListener(OnLengthEntered);
        inputFields[1].onEndEdit.AddListener(OnWidthEntered);
        inputFields[2].onEndEdit.AddListener(OnHeightEntered);

        // Add listener to the Apply button
        buildButton.onClick.AddListener(ApplySize);
    }

    public void OnLengthEntered(string lengthString)
    {
        ParseAndSetDimension(ref size.x, lengthString);
    }

    public void OnWidthEntered(string widthString)
    {
        ParseAndSetDimension(ref size.y, widthString);
    }

    public void OnHeightEntered(string heightString)
    {
        ParseAndSetDimension(ref size.z, heightString);
    }

    private void ParseAndSetDimension(ref float dimension, string dimensionString)
    {
        // Parse the string input to a float
        float enteredDimension;
        if (float.TryParse(dimensionString, out enteredDimension))
        {
            // Set the dimension of the grow space
            dimension = enteredDimension;
        }
    }

    private void ApplySize()
    {
        // Recalculate volume
        volume = size.x * size.y * size.z;

        // Set the scale of the GameObject to match the entered size
        transform.localScale = size;

        // Check if all dimensions have been set, and if so, remove the input panel
        if (size.x > 0 && size.y > 0 && size.z > 0)
        {
            Destroy(inputPanel);
        }
    }
}
