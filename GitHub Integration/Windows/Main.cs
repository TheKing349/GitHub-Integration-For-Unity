using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;

public class Main : EditorWindow
{
    private GitHubAPIHandler apiHandler;
    
    private Vector2 addedFilesScrollPosition = Vector2.zero;
    private Vector2 modifiedFilesScrollPosition = Vector2.zero;
    private Vector2 deletedFilesScrollPosition = Vector2.zero;

    private List<string> branchOptions = new();

    public string selectedBranch = "";
    
    private string summaryMessage = "Summary (required)";
    private string descriptionMessage = "Description";
    
    public int currentSelectedBranchIndex;
    private int newSelectedBranchIndex;
    
    private bool hasLocalChanges;

    [MenuItem("Window/GitHub Integration for Unity/GitHub Integration")]
    public static void ShowWindow()
    {
        Main window = (Main)GetWindow(typeof(Main));
        window.titleContent = new GUIContent("GitHub Integration");
        window.Show();
    }

    private void OnEnable()
    {
        apiHandler = FindObjectOfType<GitHubAPIHandler>();
        GetBranches();
        GetChanges();
    }

    private void Update()
    {
        if (!apiHandler)
        {
            apiHandler = FindObjectOfType<GitHubAPIHandler>();
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Git Actions", EditorStyles.boldLabel);

        #region Branch Dropdown
        newSelectedBranchIndex = EditorGUILayout.Popup("Select Branch", GetSelectedBranchIndex(), branchOptions.ToArray());
        if (newSelectedBranchIndex != currentSelectedBranchIndex)
        {
            CheckoutBranch();
        }
        #endregion

        summaryMessage = EditorGUILayout.TextField("Summary (required)", summaryMessage);

        #region Description Text Area
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Description");
        descriptionMessage = EditorGUILayout.TextArea(descriptionMessage, GUILayout.Height(180));
        EditorGUILayout.EndHorizontal();
        #endregion
        
        #region Buttons
        if (GUILayout.Button("Commit & Push Changes"))
            CommitAndPushChanges();

        if (GUILayout.Button("Fetch & Pull Changes"))
            FetchAndPullChanges();

        if (GUILayout.Button("Refresh Branches"))
            GetBranches();

        if (GUILayout.Button("Modify Branches"))
            ModifyBranches();

        if (GUILayout.Button("Get Changes"))
            GetChanges();
        #endregion

        GUILayout.Space(20);

        #region Label Fields
        GUILayout.Label("Changes Since Last Commit", EditorStyles.boldLabel);

        // Scroll view for added files
        EditorGUILayout.LabelField("Added Files:", EditorStyles.boldLabel);
        addedFilesScrollPosition = EditorGUILayout.BeginScrollView(addedFilesScrollPosition, GUILayout.Height(100));
        DisplayFileList(apiHandler.addedFiles);
        EditorGUILayout.EndScrollView();

        // Scroll view for modified files
        EditorGUILayout.LabelField("Modified Files:", EditorStyles.boldLabel);
        modifiedFilesScrollPosition = EditorGUILayout.BeginScrollView(modifiedFilesScrollPosition, GUILayout.Height(100));
        DisplayFileList(apiHandler.modifiedFiles);
        EditorGUILayout.EndScrollView();

        // Scroll view for deleted files
        EditorGUILayout.LabelField("Deleted Files:", EditorStyles.boldLabel);
        deletedFilesScrollPosition = EditorGUILayout.BeginScrollView(deletedFilesScrollPosition, GUILayout.Height(100));
        DisplayFileList(apiHandler.deletedFiles);
        EditorGUILayout.EndScrollView();
        #endregion
    }
    
    /// <summary>
    /// Calls GitHub API to get all branches in current repository
    /// </summary>
    private void GetBranches()
    {
        List<string> prevBranchOptions = branchOptions;
        
        if (apiHandler)
        {
            branchOptions = apiHandler.GetBranches();
        }
        else
        {
            Debug.LogError("GithubAPIHandler not found!");
        }
        
        if (selectedBranch == "" && DoListsMatch(prevBranchOptions, branchOptions))
        {
            selectedBranch = branchOptions[0];
            currentSelectedBranchIndex = 0;
        }
    }

    /// <summary>
    /// Gets the selected branch index in reference to the branch dropdown
    /// </summary>
    public int GetSelectedBranchIndex()
    {
        for (int i = 0; i < branchOptions.Count; i++)
        {
            if (branchOptions[i] == selectedBranch)
            {
                return i;
            }
        }
        return 0; //default to first branch if selected branch not found
    }

    /// <summary>
    /// Calls GitHub API to commit and push all local changes to specified branch
    /// </summary>
    private void CommitAndPushChanges()
    {
        if (summaryMessage == "")
        {
            Debug.LogError("Summary Message cannot be empty");
            return;
        }
        else if (!hasLocalChanges)
        {
            Debug.LogError("No Local Changes to Commit");
            return;
        }
        EditorSceneManager.SaveOpenScenes();
        GitHubAPIHandler.CommitAndPushChanges(selectedBranch, summaryMessage, descriptionMessage);
        summaryMessage = "";
        descriptionMessage = "";

        // Set the flag to indicate local changes were made
        hasLocalChanges = false;
    }

    /// <summary>
    /// Calls GitHub API to fetch and pull all incoming changes with a specified branch
    /// </summary>
    private void FetchAndPullChanges()
    {
        EditorSceneManager.SaveOpenScenes();
        GitHubAPIHandler.FetchAndPullChanges(selectedBranch);
    }

    /// <summary>
    /// Calls GitHub API to get all local changes with the current branch
    /// </summary>
    private void GetChanges()
    {
        apiHandler.GetLocalChanges();
        hasLocalChanges = true;
    }

    /// <summary>
    /// Calls GitHub API to switch to a new branch
    /// </summary>
    private void CheckoutBranch()
    {
        if (!GitHubAPIHandler.HasChanges())
        {
            selectedBranch = branchOptions[newSelectedBranchIndex];
            GitHubAPIHandler.CheckoutBranch(selectedBranch);
            currentSelectedBranchIndex = newSelectedBranchIndex;
        }
        else
        {
            SwitchBranchConfirmationWindow();
        }
    }

    /// <summary>
    /// Shows the <see cref="Branches"/> window
    /// </summary>
    private static void ModifyBranches()
    {
        Branches branchesWindow = GetWindow<Branches>();
        branchesWindow.Show();
    }

    /// <summary>
    /// Shows the <see cref="SwitchBranchConfirmation"/> window
    /// </summary>
    private void SwitchBranchConfirmationWindow()
    {
        SwitchBranchConfirmation switchBranchConfirmation = GetWindow<SwitchBranchConfirmation>();
        switchBranchConfirmation.currentBranch = selectedBranch;
        switchBranchConfirmation.switchBranch = branchOptions[newSelectedBranchIndex];
        switchBranchConfirmation.Show();
    }

    /// <summary>
    /// Determines whether two lists match
    /// </summary>
    /// <param name="list1">The first list</param>
    /// <param name="list2">The second list</param>
    /// <returns>Returns a <see cref="bool"/></returns>
    private static bool DoListsMatch(List<string> list1, List<string> list2)
    {
        if (list1.Count != list2.Count) return false;

        list1.Sort();
        list2.Sort();
        
        //for loop, if list1[i] != list2[i], return false, else return true
        return !list1.Where((t, i) => t != list2[i]).Any();
    }
    
    /// <summary>
    /// Displays all files to GUI
    /// </summary>
    /// <param name="fileList"></param>
    private static void DisplayFileList(List<string> fileList)
    {
        foreach (string file in fileList)
        {
            EditorGUILayout.LabelField(file);
        }
    }
}