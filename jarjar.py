import os
import shutil
import sys
import zipfile

##################################################
##### Check if the project path param exists
##################################################

if len(sys.argv) <= 1:
    print("You should provide the Unity project path as a parameter. Check the documentation.")
    sys.exit(0)

projectPath = sys.argv[1]

print("AAR location: " + projectPath)

##################################################
##### Create the output directories
##################################################

tempDir = "_gungan"
outputDir = "_binks"

if os.path.exists(tempDir):
	shutil.rmtree(tempDir)

if os.path.exists(outputDir):
	shutil.rmtree(outputDir)

os.mkdir(tempDir)
os.mkdir(outputDir)

##################################################
##### For each AAR file found in the project path, 
##### extract the JAR and rename it
##################################################

for file in os.listdir(projectPath):
	if file.endswith(".aar"):
		print("Extracting JAR from " + file)
		
		currentFileName = os.path.splitext(file)[0]
		currentPath = tempDir + "/" + currentFileName
		os.mkdir(currentPath)

		zip_ref = zipfile.ZipFile(projectPath + "/" + file)
		zip_ref.extractall(currentPath)
		zip_ref.close()
		
		shutil.copyfile(currentPath + "/classes.jar", outputDir + "/classes.jar")
		os.rename(outputDir + "/classes.jar", outputDir + "/" + currentFileName + ".jar")
