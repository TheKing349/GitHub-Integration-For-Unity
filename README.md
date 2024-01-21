# GitHub Integration For Unity
This is a proof of concept project to integrate the GitHub API and git commands to Unity.
Think of this as a prototype GitHub Desktop built in straight to Unity.
It is not as good as GitHub Desktop, as I have a little experience with development.
Expect bugs, issues, and errors while using this.
This project is not, in any way, affiliated with GitHub or Unity.

# Getting Started
### Getting Your Private Token
(Video below if confused)
Click on your profile in GitHub and go to the "Settings" page. Scroll down and click on "Developer settings".
Click on "Personal access tokens" and choose "Tokens (classic)". Click "Generate new token" and choose classic. 
Make the name anything and choose the Expiration Date. Personally I use no expiration date so I don't have to change
the token in Unity.
Checkbox everything in the repo scope, then click "Generate Token". Copy the token and save it somewhere safe until we
use it.

### Importing the Package
Go to the [Releases Page](https://github.com/TheKing349/Github-Integration-For-Unity/releases/latest) 
and download the "GitHub Integration For Unity.unitypackage". Then open your Unity project and hover over "Assets".
Click on "Import Package", then "Custom Package". Find the "GitHub Integration For Unity.unitypackage" and double click.
Add the "GitHubAPIHandlerGO" prefab into your current scene.

### Linking GitHub Repo
Create a new repository, if you haven't already, and get your Unity project set up with GitHub.
You should have a .gitignore file in your project somewhere. Place it in the root directory of the
Unity Project, not in the root directory of the repository. 
Add the following lines to the end of the .gitignore file: 
```
# GitHub Integration For Unity
/[Aa]ssets/GitHub Integration.meta
/[Aa]ssets/GitHub Integration

/[Aa]ssets/GitHubAPIHandlerGO.prefab.meta
/[Aa]ssets/GitHubAPIHandlerGO.prefab
```
This will ensure your private token is not exposed when you commit a change.
<br><br>
Note: if you change the names of any of the GitHub Integration For Unity files, you will have to update the
.gitignore

### Using the Package
Go into the "GitHubInfo" script, and type in your GitHub information. It will as you for your GitHub username,
your GitHub repository, and your private token you created earlier.
Then, in your Unity project scene, hover over "Window" in the upper left, and hover over "GitHub Integration For Unity".
Then click on "GitHub Integration". This will show the GitHub Integration For Unity window for you to use. 
If you get ```Error: HTTP/1.1 401 Unauthorized```, you have either not created the repository, or you have not typed in your GitHub information correctly.

### Error Handling
If you get ```Error: HTTP/1.1 403 Forbidden```, you are likely being Rate Limited. 
This happens when you have too many GitHub API calls, and have to time out for a little bit. 
The only API call this project actually uses is the ```GetBranch()``` method in the "GitHubAPIHandler" script.
This method gets all the branches in the current repository, and if you use it a lot, you may be rate limited, and receive this error.
<br><br>
Any other errors you encounter please first look for the error on the [Issues](https://github.com/TheKing349/Github-Integration-For-Unity/issues) page.
If you do not find it, [Create an Issue](https://github.com/TheKing349/Github-Integration-For-Unity/issues/new) and let me know about the error, and the steps you did to encounter the error.

# Contributions
I do accept contributions and if you would like to contribute, please either fork or clone this repository and modify what you want to.
Once you get the revision to a stable point, please submit a [Pull Request](https://github.com/TheKing349/Github-Integration-For-Unity/pulls), so I can review it.
Please leave a well written description of what you have added or modified. Do not expect immediate feedback, as this is more of a proof of concept, not a full blown passion project, and I have a life. 