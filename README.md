# Project setup Hero's Crucible
1. Install Unity version 2020.3.22f1, available through the Unity Hub. The Unity hub can be downloaded at unity.com/download. Version 2020.3.26f1 LTS most likely works just fine as well.

2. Clone the project from Gitlab into git client of choice. We recommend Sourcetree, available at sourcetreeapp.com.

3. Remove Photon folder.
If your Unity project has a folder with the path Assets/Photon (do not confuse this with Assets/Dependencies/Photon), remove it. You should not get the message that you have not yet run the Photon setup wizard anymore whenever you run the game.

4. To run the game, make sure you are in either the Menu Scene or the Game Scene (the Game Scene contains an Auto scene switcher).

5. To work on the project, create new branches starting from the development branch. You can not push directly into the Development branch.

6. To create a build through CI/CD, merge Development into Release, and add a tag (e.g. "V0.0.4"). Make sure you Container application is running. When the building is done, you can find your build in the Pipelines tab of CI/CD in Gitlab. 
You can also still build through the Unity Editor, and send Zip files around.
