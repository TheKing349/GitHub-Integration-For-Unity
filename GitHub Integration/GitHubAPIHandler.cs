using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

public class GitHubAPIHandler : MonoBehaviour
{
    private const string USER = GitHubInfo.USER;
    private const string TOKEN = GitHubInfo.TOKEN;
    private const string REPO = GitHubInfo.REPO;

    [HideInInspector] public List<string> addedFiles = new();
    [HideInInspector] public List<string> modifiedFiles = new();
    [HideInInspector] public List<string> deletedFiles = new();
    
    private List<string> branchOptions = new();
    
    #region branches
    /// <summary>
    /// Retrieves a list of branches in the current repository
    /// </summary>
    /// <returns>Returns a <see cref="List"/> of type string of branches</returns>
    public List<string> GetBranches()
    {
        StartCoroutine(GetBranchesCoroutine());
        return branchOptions;
    }
    
    /// <summary>
    /// Passes the response from a GitHub API request to <see cref="FindAllBranchNames"/>
    /// </summary>
    /// <returns>Returns a <see cref="Void"/></returns>
    private IEnumerator GetBranchesCoroutine()
    {
        string apiUrl = $"https://api.github.com/repos/{USER}/{REPO}/branches";
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        request.SetRequestHeader("Authorization", $"token {TOKEN}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseFromServer = request.downloadHandler.text;
            FindAllBranchNames(responseFromServer);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }
    
    /// <summary>
    /// Adds all branch names to a <see cref="List"/> of type string
    /// </summary>
    /// <param name="response">The response .json</param>
    /// <returns> Returns a <see cref="Void"/></returns>
    private void FindAllBranchNames(string response)
    {
        branchOptions.Clear();
        branchOptions = FindAllOccurencesOf(response, "\"name\":", 9, 2);
    }

    /// <summary>
    /// Switches to the specified branch using git command
    /// </summary>
    /// <param name="newBranch">The new branch to switch to</param>
    /// <returns> Returns <see cref="Void"/></returns>
    public static void CheckoutBranch(string newBranch)
    {
        string checkoutCommand = $"git checkout {newBranch}";
        
        ProcessStartInfo processInfo = new("cmd", $"/c \"{checkoutCommand}\"")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        Process process = Process.Start(processInfo);
        process.WaitForExit();

        string output = process.StandardOutput.ReadToEnd();

        Debug.Log("Output: " + output);
    }
    #endregion

    #region commit/push
    /// <summary>
    /// Commits and pushes all local changes using git commands
    /// </summary>
    /// <param name="branch">The branch to commit and push to</param>
    /// <param name="summaryMessage">The brief overview of the changes to attach to the commit</param>
    /// <param name="commitMessage">The detailed message of the changes to attach to the commit</param>
    /// <returns> Returns a <see cref="Void"/></returns>
    public static void CommitAndPushChanges(string branch, string summaryMessage, string commitMessage)
    {
        string commitCommand = $"git commit -m \"{summaryMessage}\"";
        List<string> commitMessageList = commitMessage.Split("\n").ToList();
        
        //foreach (string message in commitMessageList), commitCommand += $" -m \"{message}\""
        commitCommand = commitMessageList.Aggregate(commitCommand, (current, message) => current + $" -m \"{message}\"");

        string addCommand = $"git add .";
        string pushCommand = $"git push origin {branch}";

        string combinedCommands = $"{addCommand} & {commitCommand} & {pushCommand}";

        ProcessStartInfo processInfo = new("cmd", $"/c \"{combinedCommands}\"")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

        Process process = Process.Start(processInfo);
        process.WaitForExit();

        string output = process.StandardOutput.ReadToEnd();

        Debug.Log("Output: " + output);
    }
    #endregion

    #region fetch/pull
    /// <summary>
    /// Fetches and pulls all incoming changes using git commands
    /// </summary>
    /// <param name="branch">The branch to fetch and pull from</param>
    /// <returns> Returns a <see cref="Void"/></returns>
    public static void FetchAndPullChanges(string branch)
    {
        string fetchCommand = $"git fetch origin {branch}";
        string pullCommand = $"git pull origin {branch}";

        string combinedCommands = $"{fetchCommand} & {pullCommand}";

        ProcessStartInfo processInfo = new("cmd", $"/c \"{combinedCommands}\"")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

        Process process = Process.Start(processInfo);
        process.WaitForExit();

        string output = process.StandardOutput.ReadToEnd();

        Debug.Log("Output: " + output);
    }
    #endregion

    #region modify-branches
    /// <summary>
    /// Adds a specified branch using git commands
    /// </summary>
    /// <param name="branch">The branch to add</param>
    /// <returns> Returns a <see cref="Void"/></returns>
    public static void AddBranch(string branch)
    {
        string addBranchCommand = $"git branch {branch}";
        string publishBranchCommand = $"git push origin {branch}";

        string combinedCommands = $"{addBranchCommand} & {publishBranchCommand}";

        ProcessStartInfo processInfo = new("cmd", $"/c \"{combinedCommands}\"")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

        Process process = Process.Start(processInfo);
        process.WaitForExit();

        string output = process.StandardOutput.ReadToEnd();
        Debug.Log("Output: " + output);
    }

    /// <summary>
    /// Removes a specified branch using git commands
    /// </summary>
    /// <param name="branch">The branch to remove</param>
    /// <returns> Returns a <see cref="Void"/></returns>
    public static void RemoveBranch(string branch)
    {
        string removeBranchCommand = $"git branch -d {branch}";
        string deleteBranchCommand = $"git push origin --delete {branch}";

        string combinedCommands = $"{removeBranchCommand} && {deleteBranchCommand}";

        ProcessStartInfo processInfo = new("cmd", $"/c \"{combinedCommands}\"")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        Process process = Process.Start(processInfo);
        process.WaitForExit();

        string output = process.StandardOutput.ReadToEnd();
        Debug.Log("Output: " + output);
    }
    #endregion

    #region get-local-changes
    /// <summary>
    /// Gets all local changes with git commands and adds them to <see cref="List"/> of type strings
    /// </summary>
    /// <returns> Returns a <see cref="Void"/></returns>
    public void GetLocalChanges()
    {
        string addCommand = "git add .";
        string diffCommand = $"git --no-pager diff --cached --no-renames --name-status";

        string combinedCommands = $"{addCommand} & {diffCommand}";
        ProcessStartInfo processInfo = new("cmd", $"/c \"{combinedCommands}\"")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        Process process = Process.Start(processInfo);
        process.WaitForExit();

        string diff = process.StandardOutput.ReadToEnd();
        
        List<string> outputList = diff.Split("\n").ToList();
        
        deletedFiles.Clear();
        addedFiles.Clear();
        modifiedFiles.Clear();
        
        foreach (string output in outputList.Where(output => output != ""))
        {
            switch (output[0])
            {
                case 'D':
                    deletedFiles.Add(output.Remove(0, 1));
                    break;
                case 'A':
                    addedFiles.Add(output.Remove(0, 1));
                    break;
                case 'M':
                    modifiedFiles.Add(output.Remove(0, 1));
                    break;
            }
        }
    }
    #endregion

    #region bring-leave-changes
    /// <summary>
    /// Brings all local changes to a specified branch using git commands
    /// </summary>
    /// <param name="newBranch">The branch to switch to</param>
    /// <returns> Returns a <see cref="Void"/></returns>
    public static void BringChanges(string newBranch)
    {
        string stashCommand = "git stash";
        string checkoutCommand = $"git checkout {newBranch}";
        string applyStashCommand = "git stash apply";

        string combinedCommands = $"{stashCommand} & {checkoutCommand} & {applyStashCommand}";

        ProcessStartInfo processInfo = new("cmd", $"/c \"{combinedCommands}\"")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        Process process = Process.Start(processInfo);
        process.WaitForExit();

        string output = process.StandardOutput.ReadToEnd();
        Debug.Log("Output: " + output);
    }

    /// <summary>
    /// UNFINISHED
    /// Leaves all local changes to the current branch and switches to a specified branch
    /// </summary>
    /// <param name="newBranch">The branch to switch to</param>
    /// <returns> Returns a <see cref="Void"/></returns>
    public static void LeaveChanges(string newBranch)
    {
        Debug.Log("TODO: LEAVE CHANGES LOGIC");
    }
    
    #endregion
    
    /// <summary>
    /// Finds all ocurrences of a specified string
    /// </summary>
    /// <param name="response">the response .json</param>
    /// <param name="searchTerm">the term to find and return</param>
    /// <param name="startTrimIndex">the index to start excluding from the return</param>
    /// <param name="endTrimIndex">the index to end excluding from the return</param>
    /// <returns>Returns a <see cref="List"/> of type string</returns>
    private static List<string> FindAllOccurencesOf(string response, string searchTerm, int startTrimIndex, int endTrimIndex)
    {
        List<string> foundAllText = new();
        int startIndex = 0; while (startIndex < response.Length)
        {
            int foundIndex = response.IndexOf(searchTerm, startIndex);

            if (foundIndex == -1)
            {
                break;
            }

            int nextLineBreak = response.IndexOf('\n', foundIndex);
            if (nextLineBreak != -1)
            {
                string foundText = response.Substring(foundIndex, nextLineBreak - foundIndex);
                foundAllText.Add(foundText[startTrimIndex..^endTrimIndex]);
            }

            startIndex = foundIndex + searchTerm.Length;
        }
        return foundAllText;
    }
    
    /// <summary>
    /// Determines whether there are local changes using git commands
    /// </summary>
    /// <returns>Returns a <see cref="bool"/></returns>
    public static bool HasChanges()
    {
        string diffCachedCommand = "git --no-pager diff --cached --no-renames --name-status";
        string diffUncachedCommand = "git --no-pager diff --no-renames --name-status";

        string combinedCommands = $"{diffCachedCommand} & {diffUncachedCommand}";
        ProcessStartInfo processInfo = new("cmd", $"/c \"{combinedCommands}\"")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        Process process = Process.Start(processInfo);
        process.WaitForExit();

        string output = process.StandardOutput.ReadToEnd();

        return output != "";
    }
}