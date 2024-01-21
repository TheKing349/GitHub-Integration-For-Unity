using UnityEditor;
using UnityEngine;

public class SwitchBranchConfirmation : EditorWindow
{
	private GitHubAPIHandler apiHandler;

	public string currentBranch;
	public string switchBranch;

	private void OnEnable()
	{
		apiHandler = FindObjectOfType<GitHubAPIHandler>();
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
		if (GUILayout.Button($"Leave Changes on {currentBranch}"))
			LeaveChanges();

		if (GUILayout.Button($"Bring Changes to {switchBranch}"))
			BringChanges();
	}

	/// <summary>
	/// Calls GitHub API to leave all local changes on the current branch and switch to a new branch
	/// </summary>
	private void LeaveChanges()
	{
		GitHubAPIHandler.LeaveChanges(switchBranch);
		HideWindow();
	}

	/// <summary>
	/// Calls GitHub API to bring all local changes on the current branch to a new branch and switches to the new branch
	/// </summary>
	private void BringChanges()
	{
		GitHubAPIHandler.BringChanges(switchBranch);
		HideWindow();
	}

	/// <summary>
	/// Hides this window
	/// </summary>
	private void HideWindow()
	{
		Main mainWindow = (Main)GetWindow(typeof(Main));
		mainWindow.selectedBranch = switchBranch;
		mainWindow.currentSelectedBranchIndex = mainWindow.GetSelectedBranchIndex();
		
		SwitchBranchConfirmation window = (SwitchBranchConfirmation)GetWindow(typeof(SwitchBranchConfirmation));
		window.Close();
	}
}