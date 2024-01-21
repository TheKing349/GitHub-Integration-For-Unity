using UnityEditor;
using UnityEngine;

public class Branches : EditorWindow
{
    private GitHubAPIHandler apiHandler;

    private string branchName = "";

    private void OnEnable()
    {
        apiHandler = FindObjectOfType<GitHubAPIHandler>();
    }
    
    private void Update()
    {
        if (apiHandler == null)
        {
            apiHandler = FindObjectOfType<GitHubAPIHandler>();
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Modify Branches");
        branchName = EditorGUILayout.TextField("Branch Name", branchName);

        if (GUILayout.Button("Add Branch"))
            AddBranch();

        if (GUILayout.Button("Remove Branch"))
            RemoveBranch();
    }

    /// <summary>
    /// Calls GitHub API to add a new branch with a specified name
    /// </summary>
    private void AddBranch()
    {
        GitHubAPIHandler.AddBranch(branchName);
        apiHandler.GetBranches();
        branchName = "";
    }

    /// <summary>
    /// Calls GitHub API to remove a branch with the specified name
    /// </summary>
    private void RemoveBranch()
    {
        GitHubAPIHandler.RemoveBranch(branchName);
        apiHandler.GetBranches();
        branchName = "";
    }
}