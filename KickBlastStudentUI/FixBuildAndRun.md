# FixBuildAndRun

If Visual Studio fails to run (debug profile/exe missing), do the following:

1. **Clean + Rebuild**
   - Build > Clean Solution
   - Build > Rebuild Solution

2. **Delete bin/obj**
   - Close Visual Studio
   - Delete `KickBlastStudentUI/bin` and `KickBlastStudentUI/obj`
   - Re-open solution

3. **Set Startup Project**
   - Right click `KickBlastStudentUI` project > Set as Startup Project

4. **Check configuration**
   - Confirm **Debug** and **Any CPU** are selected

5. **Check launchSettings.json**
   - File: `KickBlastStudentUI/Properties/launchSettings.json`
   - Ensure it uses only:
     ```json
     { "commandName": "Project" }
     ```
   - Do not set `executablePath`
