# Extract JAR files from AAR files

Kinda fixes the [multidex problem](https://issuetracker.unity3d.com/issues/android-the-rjava-generated-by-unity-during-android-compilation-is-too-big-which-causes-rjava-refer-to-all-resources) when using a lot of AAR files on Unity.

Kinda not.

![Jar Jar](http://unrealitymag.com/wp-content/uploads/2015/11/Star-Wars-Jar-Jar-Binks-banner.png)

## Usage

```
python jarjar.py PATH_TO_AAR_FILES
```

## Example

```
python jarjar.py /Developer/Projects/TestProject/Assets/Plugins/Android
```

## Output

The script will generate two directories:

`_binks` contains the JAR files, renamed to match each AAR file

`_gungan` all content extracted from each AAR file (useful to get the res directory)


### Log

```
AAR location: /Developer/Projects/TestProject/Assets/Plugins/Android
Extracting JAR from play-services-auth-9.6.1.aar
Extracting JAR from play-services-auth-base-9.6.1.aar
Extracting JAR from play-services-base-9.6.1.aar
Extracting JAR from play-services-basement-9.6.1.aar
Extracting JAR from play-services-drive-9.6.1.aar
Extracting JAR from play-services-games-9.6.1.aar
Extracting JAR from play-services-nearby-9.6.1.aar
Extracting JAR from play-services-tasks-9.6.1.aar
Extracting JAR from support-v4-24.0.0.aar
```
