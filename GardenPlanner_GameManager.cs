// Import necessary namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EasyBuildSystem;
using EasyBuildSystem.Features.Runtime.Buildings.Manager;
using EasyBuildSystem.Features.Runtime.Buildings.Part;
using EasyBuildSystem.Features.Runtime.Buildings.Placer;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using static EasyBuildSystem.Features.Runtime.Buildings.Placer.BuildingPlacer;
using UnityEditor.Experimental.GraphView;
using com.ootii;
using com.ootii.Cameras;
using UnityEngine.InputSystem;

// Define GardenPlanner_GameManager class that inherits from MonoBehaviour
public class GardenPlanner_GameManager : MonoBehaviour
{
    // Define public GameObjects for UI elements
    public GameObject Plant_itemSlot; //Gameobject made up of a parent gameobject with 4 child Buttons 1- PlantName_Button 2- PlantCount_Button 3- Spacing_Button 4- PlantSchedule_Button
    public GameObject Table_Panel;
    public GameObject GardenSettingsPanel;
    public GameObject Location_Panel, Weather_Panel, Environment_Panel, Time_Panel;

    // Reference to the Planner_Database script
    public Planner_Database plannerDatabase;

    // Input actions for player controls
    public InputActionAsset inputActionAsset;
    private InputAction scrollAction, changeCameraView_Action;

    // Variables for UI elements and components
    public TMP_Text windowHeaderTitle,
        GridToggle_Text;
    public GameObject itemSlot,
        RadsPanel,
        contentPanel,
        mainContent_Plants_Panel,
        optionsMenu_Panel,
        Game_Main_Panel,
        GridButton_on,
        GridButton_off,
        Instructions_Panel;
    public BuildingManager BuildingManager;
    public Camera mainCamera;
    public GameObject _Player;
    public CameraController mainCameraController;

    // Reference to the BuildingPlacer component
    public BuildingPlacer buildingPlacer;
    bool isTop_View,
        isAxisAligned;

    // Called before the first frame update
    void Start()
    {
        // Initialize input actions for camera controls
        changeCameraView_Action = inputActionAsset.FindAction("Player/Camera Aim");
        if (changeCameraView_Action == null)
        {
            // Handle the case where the action is not found
        }
        scrollAction = inputActionAsset.FindAction("Player/Camera Zoom");
        if (scrollAction == null)
        {
            // Handle the case where the action is not found
        }
        // Hide the RadsPanel at the start of the game
        RadsPanel.SetActive(false);
    }

    // Called once per frame
    void Update()
    {
        // Check the camera mode in each frame
        CameraModeCheck();
        // Apply scroller effect for top view camera
        CameraController_TopView_ScrollerEffect();
    }

    private void CameraModeCheck()
    {
        // Check if the camera is in top view mode and then if the camera and cameraController are not rotated to off of the proper top view rotations.
        if (isTop_View && changeCameraView_Action.IsPressed())
        {
            // reset the mainCamera local rotation to (0,0,0).
            mainCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);

            isTop_View = false;
            mainCamera.orthographic = false;
        }
        if (!isAxisAligned)
        {
            mainCamera.orthographic = false;
        }
    }

    // Toggle RadsPanel for displaying objects
    public void Objects_SidebarToggle()
    {
        // Check if RadsPanel is not active
        if (!RadsPanel.activeInHierarchy)
        {
            RadsPanel.SetActive(true);
            windowHeaderTitle.text = "Objects";
            ClearItemsPanel();
            List<BuildingPart> objectParts = GetBuildingPartsByType("Object");
            ShowBuildingParts(objectParts);
        }
        else if (RadsPanel.activeInHierarchy)
        {
            RadsPanel.SetActive(false);
        }
    }

    // Toggle RadsPanel for displaying plants
    public void Plants_SidebarToggle()
    {
        // Check if RadsPanel is not active
        if (!RadsPanel.activeInHierarchy)
        {
            RadsPanel.SetActive(true);
            windowHeaderTitle.text = "Plants";
            ClearItemsPanel();
            List<BuildingPart> objectParts = GetBuildingPartsByType("Plant");
            ShowBuildingParts(objectParts);
        }
        else if (RadsPanel.activeInHierarchy)
        {
            RadsPanel.SetActive(false);
        }
    }

    // Toggle options menu panel
    public void OptionMenuToggle()
    {
        // Check if optionsMenu_Panel is not active
        if (!optionsMenu_Panel.activeInHierarchy)
        {
            Game_Main_Panel.SetActive(false);
            optionsMenu_Panel.SetActive(true);
        }
        else if (optionsMenu_Panel.activeInHierarchy)
        {
            Game_Main_Panel.SetActive(true);
            optionsMenu_Panel.SetActive(false);
        }
    }

    // Hide the mainContent_Plants_Panel
    public void View_GardenPlanner()
    {
        if (mainContent_Plants_Panel.activeInHierarchy)
        {
            mainContent_Plants_Panel.SetActive(false);
        }
    }

    // Toggle mainContent_Plants_Panel
    public void View_Plants()
    {
        // Check if mainContent_Plants_Panel is not active
        if (!mainContent_Plants_Panel.activeInHierarchy)
        {
            mainContent_Plants_Panel.SetActive(true);
            plannerDatabase.CategorizeBuildingParts(); // Call CategorizeBuildingParts() here

            // Clear the table panel before populating it
            ClearTablePanel();

            // Get the plant counts from the database
            Dictionary<string, int> plantCounts = plannerDatabase.GetPlantCounts();

            // Iterate through each unique plant
            foreach (var plantName in plantCounts.Keys)
            {
                // Instantiate a new Plant_itemSlot
                GameObject newPlantItem = Instantiate(Plant_itemSlot, Table_Panel.transform);

                // Find a building part with this plant name
                BuildingPart matchingPart = plannerDatabase.categorizedParts["Plants"].Find(part =>
                {
                    PlantData plantData = part.GetComponent<PlantData>();
                    return plantData != null && plantData.plantName == plantName;
                });

                if (matchingPart != null)
                {
                    // Get the PlantData component attached to the plant
                    PlantData plantData = matchingPart.GetComponent<PlantData>();

                    // Get the buttons
                    Button plantNameButton = newPlantItem.transform.Find("PlantName_Button").GetComponent<Button>();
                    Button plantCountButton = newPlantItem.transform.Find("PlantCount_Button").GetComponent<Button>();
                    Button spacingButton = newPlantItem.transform.Find("Spacing_Button").GetComponent<Button>();
                    Button plantScheduleButton = newPlantItem.transform.Find("PlantSchedule_Button").GetComponent<Button>();

                    // Edit the button text
                    plantNameButton.GetComponentInChildren<TMP_Text>().text = plantData.plantName;
                    plantCountButton.GetComponentInChildren<TMP_Text>().text = plantCounts[plantName].ToString();
                    spacingButton.GetComponentInChildren<TMP_Text>().text = plantData.plantSpacing.ToString();
                    plantScheduleButton.GetComponentInChildren<TMP_Text>().text = plantData.plantSchedule;
                }
            }
        }
        else if (mainContent_Plants_Panel.activeInHierarchy)
        {
            mainContent_Plants_Panel.SetActive(false);
        }
    }

    // Clear the Table_Panel
    public void ClearTablePanel()
    {


        foreach (Transform child in Table_Panel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    // Sort plant items by name
    public void SortByName()
    {
        List<GameObject> items = GetSortedPlantItems("PlantName_Button", true);
        UpdatePlantItems(items);
    }

    // Sort plant items by count
    public void SortByCount()
    {
        List<GameObject> items = GetSortedPlantItems("PlantCount_Button", false);
        UpdatePlantItems(items);
    }

    // Sort plant items by spacing
    public void SortBySpacing()
    {
        List<GameObject> items = GetSortedPlantItems("Spacing_Button", false);
        UpdatePlantItems(items);
    }

    // Sort plant items by schedule
    public void SortBySchedule()
    {
        List<GameObject> items = GetSortedPlantItems("PlantSchedule_Button", true);
        UpdatePlantItems(items);
    }

    // Get sorted plant items based on the button name and type
    private List<GameObject> GetSortedPlantItems(string buttonName, bool isString)
    {
        List<GameObject> items = new List<GameObject>();

        for (int i = 0; i < Table_Panel.transform.childCount; i++)
        {
            items.Add(Table_Panel.transform.GetChild(i).gameObject);
        }

        if (isString)
        {
            items.Sort((item1, item2) =>
                string.Compare(
                    item1.transform.Find(buttonName).GetComponentInChildren<TMP_Text>().text,
                    item2.transform.Find(buttonName).GetComponentInChildren<TMP_Text>().text
                )
            );
        }
        else
        {
            items.Sort((item1, item2) =>
            {
                float.TryParse(item1.transform.Find(buttonName).GetComponentInChildren<TMP_Text>().text, out float val1);
                float.TryParse(item2.transform.Find(buttonName).GetComponentInChildren<TMP_Text>().text, out float val2);
                return val1.CompareTo(val2);
            });
        }

        return items;
    }

    // Update the plant items in the Table_Panel
    private void UpdatePlantItems(List<GameObject> items)
    {
        // Remove the existing game objects in the table
        foreach (Transform child in Table_Panel.transform)
        {
            Destroy(child.gameObject);
        }

        // Instantiate the sorted plant items and activate the components
        foreach (var item in items)
        {
            GameObject newPlantItem = Instantiate(item, Table_Panel.transform);

            // Activate the image and button components
            Image image = newPlantItem.GetComponent<Image>();
            if (image != null)
            {
                image.enabled = true;
            }

            Button button = newPlantItem.GetComponent<Button>();
            if (button != null)
            {
                button.enabled = true;
            }
        }
    }

    // Toggle RadsPanel for displaying parts list
    public void View_Parts_List()
    {
        if (!RadsPanel.activeInHierarchy)
        {
            RadsPanel.SetActive(true);
            windowHeaderTitle.text = "Parts List";
        }
        else if (RadsPanel.activeInHierarchy)
        {
            RadsPanel.SetActive(false);
        }
    }

    // Toggle RadsPanel for displaying notes
    public void View_Notes()
    {
        if (!RadsPanel.activeInHierarchy)
        {
            RadsPanel.SetActive(true);
            windowHeaderTitle.text = "Notes";
        }
        else if (RadsPanel.activeInHierarchy)
        {
            RadsPanel.SetActive(false);
        }
    }

    public void View_WeatherSettings()
    {
        if (GardenSettingsPanel.activeInHierarchy)
        {
            if(!Weather_Panel.activeInHierarchy)
            {
                Weather_Panel.SetActive(true);
                Environment_Panel.SetActive(false);
                Location_Panel.SetActive(false);
                Time_Panel.SetActive(false);
            }
        }
    }

    public void View_EnvironmentSettings()
    {
        if (GardenSettingsPanel.activeInHierarchy)
        {
            if (!Environment_Panel.activeInHierarchy)
            {
                Environment_Panel.SetActive(true);
                Weather_Panel.SetActive(false);
                Location_Panel.SetActive(false); 
                Time_Panel.SetActive(false);
            }
        }
    }

    public void View_LocationSettings()
    {
        if (GardenSettingsPanel.activeInHierarchy)
        {
            if (!Location_Panel.activeInHierarchy)
            {
                Location_Panel.SetActive(true);
                Environment_Panel.SetActive(false);
                Weather_Panel.SetActive(false);
                Time_Panel.SetActive(false);
            }
        }
    }

    public void View_TimeSettings()
    {
        if (GardenSettingsPanel.activeInHierarchy)
        {
            if (!Time_Panel.activeInHierarchy)
            {
                Time_Panel.SetActive (true);
                Location_Panel.SetActive(false);
                Environment_Panel.SetActive(false);
                Weather_Panel.SetActive(false);
            }
        }
    }

    // Empty settings function
    public void Settings()
    {
        if(GardenSettingsPanel.activeInHierarchy)
        {
            GardenSettingsPanel.SetActive(false);
        } else if(!GardenSettingsPanel.activeInHierarchy)
        {
            GardenSettingsPanel.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        if (GardenSettingsPanel.activeInHierarchy)
        {
            GardenSettingsPanel.SetActive(false);
        }
    }

    // Function to clear content panel
    private void ClearItemsPanel()
    {
        foreach (Transform child in contentPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    // Function to get building parts by type
    private List<BuildingPart> GetBuildingPartsByType(string type)
    {
        List<BuildingPart> buildingParts = new List<BuildingPart>();
        List<BuildingPart> allBuildingParts = BuildingManager.BuildingPartReferences;

        foreach (BuildingPart part in allBuildingParts)
        {
            if (part.GetGeneralSettings.Type == type)
            {
                buildingParts.Add(part);
                Debug.Log("Part added!");
            }
            else
            {
                Debug.Log("make sure you added the part to the Building Manager - rad");
            }
        }

        return buildingParts;
    }

    // Function to create sprite from texture
    private Sprite CreateSpriteFromTexture(Texture2D texture)
    {
        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );
    }

    // Function to show building parts
    private void ShowBuildingParts(List<BuildingPart> buildingParts)
    {
        foreach (BuildingPart part in buildingParts)
        {
            GameObject item = Instantiate(itemSlot, contentPanel.transform, false);
            TextMeshProUGUI textComponent = item.GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = part.name;
            Sprite thumbnailSprite = CreateSpriteFromTexture(part.GetGeneralSettings.Thumbnail);

            if (thumbnailSprite == null)
            {
                Debug.LogError("Thumbnail sprite is null for building part: " + part.name);
            }
            else
            {
                Debug.Log("Loaded thumbnail sprite for building part: " + part.name);
                Image imageComponent = item.transform.Find("Button").GetComponent<Image>();
                if (imageComponent == null)
                {
                    Debug.LogError(
                        "Image component not found on item for building part: " + part.name
                    );
                }
                else
                {
                    imageComponent.sprite = thumbnailSprite;
                }

                Button buttonComponent = item.GetComponentInChildren<Button>();
                if (buttonComponent == null)
                {
                    Debug.LogError(
                        "Button component not found on item for building part: " + part.name
                    );
                }
                else
                {
                    buttonComponent.onClick.AddListener(() => OnitemSlotClick(part));
                }
            }
        }
    }

    // Method called when a itemSlot is clicked
    public void OnitemSlotClick(BuildingPart partToPlace)
    {
        // Check if the buildingPlacer reference is set
        if (buildingPlacer != null)
        {
            // Check if the instructions panel is active and set the thumbnail sprite.
            if (!Instructions_Panel.activeInHierarchy)
            {
                // Show the instructions panel.
                Instructions_Panel.SetActive(true);

                // Set the instructions text.
                TMP_Text Instruction = Instructions_Panel.transform
                    .Find("Instruction_Text (TMP)")
                    .GetComponent<TMP_Text>();
                Instruction.text = "Press 'F' To Place";

                // Set the thumbnail sprite.
                Sprite thumbnailSprite = CreateSpriteFromTexture(
                    partToPlace.GetGeneralSettings.Thumbnail
                );

                if (thumbnailSprite == null)
                {
                    Debug.LogError(
                        "Thumbnail sprite is null for building part: " + partToPlace.name
                    );
                }
                else
                {
                    // Set the thumbnail sprite.
                    Image imageComponent = Instructions_Panel.transform
                        .Find("Button")
                        .GetComponent<Image>();

                    if (imageComponent == null)
                    {
                        Debug.LogError(
                            "Image component not found on item for building part: "
                                + partToPlace.name
                        );
                    }
                    else
                    {
                        // Set the thumbnail sprite.
                        imageComponent.sprite = thumbnailSprite;
                    }
                }


            }

            // Select the building part to place
            buildingPlacer.SelectBuildingPart(partToPlace);
            // Change the build mode to placement mode
            buildingPlacer.ChangeBuildMode(BuildMode.PLACE);
        }
        else
        {
            // Log an error if the buildingPlacer reference is not set
            Debug.LogError("BuildingPlacer reference not set in GardenPlanner_GameManager.");
        }
    }

    // Change the camera view to orthographic
    public void ChangeCameraViewType_Ortho()
    {
        if (!mainCamera.orthographic && isAxisAligned)
        {
            mainCamera.orthographic = true;
        }
    }

    // Change the camera view to perspective
    public void ChangeCameraViewType_Perspective()
    {
        if (mainCamera.orthographic)
        {
            mainCamera.orthographic = false;
        }
    }

    // Change the camera view to 2D
    public void ChangeTo_2D()
    {
        ChangeCameraViewType_Ortho();
        ChangeCameraView("Top");
    }

    // Change the camera view to 3D
    public void ChangeTo_3D()
    {
        ChangeCameraViewType_Perspective();
    }

    // Change the camera view based on the given view type
    public void ChangeCameraView(string view)
    {

        switch (view)
        {
            case "Top":

                isTop_View = true;
                isAxisAligned = true;

                mainCameraController.transform.rotation = Quaternion.Euler(0, 0, 0);
                mainCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);

                // Apply the rotation in the local space of mainCamera
                mainCamera.transform.localRotation = Quaternion.Euler(90, 0, 0);

                break;

            case "Left":
                isTop_View = false;
                isAxisAligned = true;

                mainCameraController.transform.rotation = Quaternion.Euler(0, 90, 0);

                // Restore the rotation of mainCamera to 0,0,0.
                if (mainCamera.transform.localRotation != Quaternion.Euler(0, 0, 0))
                    mainCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);

                break;

            case "Right":
                isTop_View = false;
                isAxisAligned = true;

                mainCameraController.transform.rotation = Quaternion.Euler(0, -90, 0);

                // Restore the rotation of mainCamera to 0,0,0.
                if (mainCamera.transform.localRotation != Quaternion.Euler(0, 0, 0))
                    mainCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;

            case "Front":
                isTop_View = false;
                isAxisAligned = true;

                mainCameraController.transform.rotation = Quaternion.Euler(0, 0, 0);

                // Restore the rotation of mainCamera to 0,0,0.
                if (mainCamera.transform.localRotation != Quaternion.Euler(0, 0, 0))
                    mainCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);

                break;

            case "Back":
                isTop_View = false;
                isAxisAligned = true;

                mainCameraController.transform.rotation = Quaternion.Euler(0, 180, 0);

                // Restore the rotation of mainCamera to 0,0,0.
                if (mainCamera.transform.localRotation != Quaternion.Euler(0, 0, 0))
                    mainCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);


                break;

            default:
                Debug.LogError("Invalid view: " + view);
                break;
        }
    }

    // Apply scrolling effect when in top view
    private void CameraController_TopView_ScrollerEffect()
    {
        if (isTop_View == true && isAxisAligned)
        {
            Vector2 scrollDelta = scrollAction.ReadValue<Vector2>(); // Get the Scroll Delta from your input system

            if (scrollDelta.y > 0)
            {
                mainCameraController.IsZoomEnabled = false;
                mainCameraController.transform.position += Vector3.down; // Moves the camera up by 1 unit
            }
            else if (scrollDelta.y < 0)
            {
                mainCameraController.IsZoomEnabled = false;
                mainCameraController.transform.position -= Vector3.down; // Moves the camera down by 1 unit
            }
        }
    }

    // Change the camera view to left
    public void ChangeView_Left()
    {
        ChangeCameraView("Left");
    }

    // Change the camera view to right
    public void ChangeView_Right()
    {
        ChangeCameraView("Right");
    }

    // Change the camera view to front
    public void ChangeView_Front()
    {
        ChangeCameraView("Front");
    }

    // Change the camera view to back
    public void ChangeView_Back()
    {
        ChangeCameraView("Back");
    }

    // Toggle the top view for the camera
    public void ChangeView_Top()
    {
        if (isTop_View == true)
        {
            isTop_View = false;
        }
        else if (isTop_View == false)
        {
            isTop_View = true;
        }

        if (isTop_View == true)
        {
            ChangeCameraView("Top");
        }
    }

    // Zoom out the camera
    public void ZoomOut_ButtonPress()
    {
        if (mainCamera.orthographic && isAxisAligned)
        {
            mainCameraController.Camera.orthographicSize += 10;
        }
    }

    // Zoom in the camera
    public void ZoomIn_ButtonPress()
    {
        if (mainCamera.orthographic && isAxisAligned)
        {
            mainCameraController.Camera.orthographicSize -= 10;
        }
    }

    // Load another scene
    public void ExitToMain()
    {
        SceneManager.LoadScene(2);
    }

    // Close the options panel
    public void Close_OptionsPanel()
    {
        optionsMenu_Panel.SetActive(false);
    }

    // Close the RadsPanel
    public void Close_RadsPanel()
    {
        RadsPanel.SetActive(false);
    }

    // Toggle the grid for building placement
    public void GridToggle()
    {
        // Check if the Grid Button On is active and Grid Button Off is not active
        if (GridButton_on.activeInHierarchy && !GridButton_off.activeInHierarchy)
        {
            GridButton_on.SetActive(false);
            GridButton_off.SetActive(true);

            // Disable snapping feature when the Grid Button On was active and now Grid Button Off is Active
            buildingPlacer.GetPreviewSettings.Type = BuildingPlacer
                .PreviewSettings
                .MovementType
                .SMOOTH;
        }
        else if (!GridButton_on.activeInHierarchy && GridButton_off.activeInHierarchy) // Check if the Grid Button Off is active and Grid Button On is not active
        {
            GridButton_on.SetActive(true);
            GridButton_off.SetActive(false);

            // Enable snapping feature when the Grid Button Off was active and is now grid Button On is Active
            buildingPlacer.GetPreviewSettings.Type = BuildingPlacer
                .PreviewSettings
                .MovementType
                .GRID;
        }
    }
}