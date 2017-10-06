# Extract JAR files from AAR files

Kinda fixes the [multidex problem](https://issuetracker.unity3d.com/issues/android-the-rjava-generated-by-unity-during-android-compilation-is-too-big-which-causes-rjava-refer-to-all-resources) when using a lot of AAR files on Unity.

Kinda not.

![Jar Jar](http://unrealitymag.com/wp-content/uploads/2015/11/Star-Wars-Jar-Jar-Binks-banner.png)

JarJar replaces the package name in the `AndroidManiest.xml` files that he finds inside all AAR files in the Plugins/Android folder. He also loos for `AndroidManifest.xml` files and update them. The new package name is the one set in the Player Settings. 

## Usage

Import the JarJar plugin in Unity (Assets/Import Package/Custom Packages...), you can find the latest version [here](https://github.com/cicanci/tool-jarjar/releases).

After the plugin import is done, an option will appear in the Unity Editor menu: Tools/Android/Update AAR and AndroidManifest with JarJar, just click and wait. Android should be the selected platform before run it.

### Project

JarJar uses the amazing [UniZip](https://github.com/tsubaki/UnityZip). The [Google Play Games Plugin for Unity](https://github.com/playgameservices/play-games-plugin-for-unity) is in this project only for test purpose, it is not part of the plugin.

### Log

You can enable a debug log by adding `DEBUG_JARJAR` in the Scripting Define Symbols section located at the Player Settings.

### Results
