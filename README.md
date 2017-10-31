# Siteimprove Plugin for Kentico
## Toolkit Installation Instructions:

<a href="https://www.youtube.com/watch?v=bVL35Jmoivo" target="_blank"><img src="http://img.youtube.com/vi/bVL35Jmoivo/0.jpg" 
alt="IMAGE ALT TEXT HERE" width="360" border="10" /></a>


## Manual Installation Instructions:
### Create a new **Module**

We need to create a new Module to store a setting. In BizStream’s version of the Extension, there are many more options and settings, but for this example, we will only go through what is absolutely needed.

1. Go to the **Modules** application

   ![Go to Modules Application](https://www.bizstream.com/KENTICO-BIZSTREAM/media/Siteimprove-Git/GoToModulesApp.PNG)
   <br/><br/>

2. Click the **New module** button

   ![alttext](https://www.bizstream.com/KENTICO-BIZSTREAM/media/Siteimprove-Git/ClickNewModule.PNG)
   <br/><br/>

3. Name the module **Siteimprove**

   ![alttext](https://www.bizstream.com/KENTICO-BIZSTREAM/media/Siteimprove-Git/NameTheModule.PNG)
   <br/><br/>

4. Go to the vertical **Settings** tab

   ![alttext](https://www.bizstream.com/KENTICO-BIZSTREAM/media/Siteimprove-Git/GoToSettingsVerticalTab.PNG)
   <br/><br/>

5. Add a new Category by click the **+** (plus) sign. Then name the category **Siteimprove** and click **Save** button

   ![alttext](https://www.bizstream.com/KENTICO-BIZSTREAM/media/Siteimprove-Git/CreateSettingsCategory.PNG)

   *Optional: Move it up or down to position anywhere you want*
   <br/><br/>

6. Now click the horizontal **Settings** tab, then click the **New settings group** button

   ![alttext](https://www.bizstream.com/KENTICO-BIZSTREAM/media/Siteimprove-Git/NewSettingsGroup.PNG)
   <br/><br/>

7. Name the settings group as follows:

   ![alttext](https://www.bizstream.com/KENTICO-BIZSTREAM/media/Siteimprove-Git/CreateSettingsGroup.PNG)
   <br/><br/>

8. Click the **Settings** tab again, then click **New settings key** button

   ![alttext](https://www.bizstream.com/KENTICO-BIZSTREAM/media/Siteimprove-Git/ClickNewSettingsKey.PNG)
   <br/><br/>

9. Create the settings key as follows:

   ![alttext](https://www.bizstream.com/KENTICO-BIZSTREAM/media/Siteimprove-Git/NewSettingsKey.PNG)

   *Important: Code name MUST be **BTKPluginSiteimproveToken***
   <br/><br/>

At this point, you now have a new setting created. The code will use this setting to maintain the access token for the Siteimprove plugin. Typically when creating a module, you need to do configure a lot more. But for our purposes, we just needed a spot for the settings.


### Add Code Files
Here are some code files for Kentico 10.0. Add these to your App_Code folder for a Web site, or Add them to a new DLL project (or existing Old_App_Code.dll project)

   ![alttext](https://www.bizstream.com/KENTICO-BIZSTREAM/media/Siteimprove-Git/FileList.PNG)
   <br/><br/>

