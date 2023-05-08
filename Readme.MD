# CastleMiner Z Decompilation
A quick and nasty decompilation of CastleMinerZ created using the dark powers of ILSpy and Binwalk.


## Building
You will need:
Visual Studio 2010 Professional,
The Microsoft Xbox 360 SDK (Its out there.),
The Microsoft XNA Framework version 3.1,
An original copy of CastleMiner Z that you legally purchased,
Horizion by Daring Development, or another tool to extract the xbox game files.

Step 1. Extract the original game files to an empty folder, this will be where we'll keep a fresh copy.

Step 2. Copy the 'Content' Folder from the original game to the root of this repo. These are all the game assets.

Step 3. Open the DNA.Common Solution in Visual Studio 2010. Select 'build'

Step 4. If all goes well, you should see a file called 'DNA.Common.dll' in the 'bin/debug' folder. copy this to the root of the repo.

Step 5. Open the CastleMiner Z Solution in Visual Studio.

Step 6. Right click on the project in the solution explorer and click 'Add', 'Reference' and select browse. Select the dll we just created.

Step 7. Click 'build' if all goes well you should have an exe in the 'bin/debug' folder.

Step 8. (cant be too descriptive here, you can find tuts for this.) Use the Xbox 360 XDKs publishing tools to convert the base exe to an xex file.

Step 9. Either transfer the root of this repo to a usb flash drive and use an RGH xbox 360 to play this, or use Xenia (use the xex we just made)
