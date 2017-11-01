# Siteimprove Plugin for Kentico
![Go to Modules Application](https://www.bizstream.com/KENTICO-BIZSTREAM/media/Siteimprove-Git/siteimprove-sample-usage.png)

To get **Siteimprove** to work within your Kentico site, you have **two installation options**.

## Option 1: Toolkit Extension Installation Instructions:
If you have already have **Toolkit for Kentico** installed, you can use this one-click installation option. If you do not have Toolkit for Kentico installed, what are you waiting for? **It's FREE** and allows you to easily install and upgrade our extensions (just like this Siteimprove extension).

<a href="https://www.youtube.com/watch?v=bVL35Jmoivo" target="_blank"><img src="http://img.youtube.com/vi/bVL35Jmoivo/0.jpg" 
alt="IMAGE ALT TEXT HERE" width="360" border="10" /></a>


## Option 2: Manual Installation Instructions:
If you do not have **Toolkit for Kentico** installed, and you do not want to install it, even though it is free :), follow the below steps to get the Siteimprove Plugin loaded into your Kentico site.

*Note: This installation option is provided as-is. Unlike the Toolkit Extension version, it will not have as many features or options. Also, it will not be upgraded with the new features that the Toolkit version will have in the future. You are welcome to change the code and add your own custom functionality, features and options.*


### Create a new **Module**

We need to create a new Module to store a setting. In the **Toolkit for Kentico** version of the Extension, there are many more options and settings, but for this example, we will only go through what is absolutely needed.

1. Go to the **Modules** application

   ![Go to Modules Application](https://www.bizstream.com/KENTICO-BIZSTREAM/media/Siteimprove-Git/GoToModulesApp.PNG)
   <br/><br/>

2. Click the **New module** button

   ![alttext](https://www.bizstream.com/KENTICO-BIZSTREAM/media/Siteimprove-Git/ClickNewModule.PNG)
   <br/><br/>

3. Name the module **Siteimprove**, then click **Save**

   ![alttext](https://www.bizstream.com/KENTICO-BIZSTREAM/media/Siteimprove-Git/NameTheModule.PNG)
   <br/><br/>

4. Select **Settings** from the left sidebar menu

   ![alttext](https://www.bizstream.com/KENTICO-BIZSTREAM/media/Siteimprove-Git/GoToSettingsVerticalTab.PNG)
   <br/><br/>

5. Add a new Category by clicking the **+** (plus) sign. Name the category display name and code name **Siteimprove** and then click the **Save** button

   ![alttext](https://www.bizstream.com/KENTICO-BIZSTREAM/media/Siteimprove-Git/CreateSettingsCategory.PNG)

   *Optional: Move it up or down to position anywhere you want*
   <br/><br/>

6. Now click the horizontal **Settings** tab, then click the **New settings group** button

   ![alttext](https://www.bizstream.com/KENTICO-BIZSTREAM/media/Siteimprove-Git/NewSettingsGroup.PNG)
   <br/><br/>

7. Set the display name to "Siteimprove Settings" and the code name to "SiteimproveSettings" (note no space in the code name), then click the **Save** button

   ![alttext](https://www.bizstream.com/KENTICO-BIZSTREAM/media/Siteimprove-Git/CreateSettingsGroup.PNG)
   <br/><br/>

8. Click the **Settings** tab again, then click **New settings key** button

   ![alttext](https://www.bizstream.com/KENTICO-BIZSTREAM/media/Siteimprove-Git/ClickNewSettingsKey.PNG)
   <br/><br/>

9. Enter "Siteimprove Token" in the description field, "BTKPluginSiteimproveToken" in the code name, then change the type to **text**, and then click the **Save** button

   ![alttext](https://www.bizstream.com/KENTICO-BIZSTREAM/media/Siteimprove-Git/NewSettingsKey.PNG)

   *Important: Code name MUST be **BTKPluginSiteimproveToken***
   <br/><br/>

You now have a new setting created. The code will use this setting to maintain the access token for the Siteimprove plugin. Typically when creating a module, additional configuration is required; but for our purposes, we just needed a spot for the settings.


### Add Code Files
Below are all the required code files for Kentico 10.0. You will need to add these files to your project.

![alttext](https://www.bizstream.com/KENTICO-BIZSTREAM/media/Siteimprove-Git/FileList.PNG)
<br/><br/>

* For a **Website** Project:
  Add these files to your **App_Code** folder under your **CMS** project

* For a **Web Application** Project:
  Add these files to your **Old_App_Code** folder under your **CMSApp_AppCode** project

   [Code for this repo](https://github.com/bizstream-markschmidt/siteimprove-for-kentico/tree/master/App_Code)

