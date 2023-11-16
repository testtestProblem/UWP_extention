# Why need UWP extention
Universal Windows Platform (UWP). It has delicate UI by XAML. But it can not support a lot of WIN32 function. That is why we need UWP extention to unlimited it ability. 

# Project describe
* Paltform: visual studio 2019  
* Function describe: UWP through Windows Desktop Extensions for the UWP tool side loading winform.  
* Compile sample: UWP can not run at any CPU, and target should set Windows Application Packaging Project.  
![image](https://github.com/testtestProblem/UWP_extention/assets/107662393/b60b14bc-4c7e-4bba-9e72-c146665b3147)

# UWP extention

---------------
For just sideload other app

* This UWP_extention can side loading winform by using ```FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync()```.  
 
* Modify WapProj/Package.appxmanifest by adding extensions in Windows Application Packaging Project.
```XML
<Package
  xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10"
  xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10"
  xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities"
  
  xmlns:mp="http://schemas.microsoft.com/appx/2014/phone/manifest"
  xmlns:desktop="http://schemas.microsoft.com/appx/manifest/desktop/windows10"
  IgnorableNamespaces="uap mp desktop rescap">
```

```XML
 <Applications>
    <Application Id="App"
      Executable="$targetnametoken$.exe"
      EntryPoint="$targetentrypoint$">
      <uap:VisualElements
        DisplayName="WapProj_HotTab_Win10"
        Description="WapProj_HotTab_Win10"
        BackgroundColor="transparent"
        Square150x150Logo="Images\Square150x150Logo.png"
        Square44x44Logo="Images\Square44x44Logo.png">
        <uap:DefaultTile Wide310x150Logo="Images\Wide310x150Logo.png" />
        <uap:SplashScreen Image="Images\SplashScreen.png" />
      </uap:VisualElements>
		
	 <Extensions>
		 <uap:Extension Category="windows.appService">
		 	<uap:AppService Name="SampleInteropService" />
		 </uap:Extension>
		 <desktop:Extension Category="windows.fullTrustProcess" Executable="CollectDataAP\CollectDataAP.exe" />
	 </Extensions>
		
    </Application>
  </Applications>
```

In WapProj/Dependencies 
![image](https://github.com/testtestProblem/UWP_extention/assets/107662393/8e778761-9ca0-47d2-8d8f-a4fe259f3b55)

Add UWP extension in UWP/Reference 
![image](https://github.com/testtestProblem/UWP_extention/assets/107662393/c3a48b03-6178-4231-be48-334583b6ae3d)
 
Add and cover this in UWP/App.xmal.cs
```C#
sealed partial class App : Application
    {
        public static BackgroundTaskDeferral AppServiceDeferral = null;
        public static AppServiceConnection Connection = null;
        public static bool IsForeground = false;
        public static event EventHandler<AppServiceTriggerDetails> AppServiceConnected;
        public static event EventHandler AppServiceDisconnected;



        private void App_LeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            IsForeground = true;
        }

        private void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            IsForeground = false;
        }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.EnteredBackground += App_EnteredBackground;
            this.LeavingBackground += App_LeavingBackground;
        }

        /// <summary>
        /// Handles connection requests to the app service
        /// </summary>
        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);

            if (args.TaskInstance.TriggerDetails is AppServiceTriggerDetails details)
            {
                // only accept connections from callers in the same package
                if (details.CallerPackageFamilyName == Package.Current.Id.FamilyName)
                {
                    // connection established from the fulltrust process
                    AppServiceDeferral = args.TaskInstance.GetDeferral();
                    args.TaskInstance.Canceled += OnTaskCanceled;

                    Connection = details.AppServiceConnection;
                    AppServiceConnected?.Invoke(this, args.TaskInstance.TriggerDetails as AppServiceTriggerDetails);
                }
            }
        }

        /// <summary>
        /// Task canceled here means the app service client is gone
        /// </summary>
        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            AppServiceDeferral?.Complete();
            AppServiceDeferral = null;
            Connection = null;
            AppServiceDisconnected?.Invoke(this, null);
        }

....

```
And and cover in UWP/MainPage.xmal.cs
```C#
 public MainPage()
        {
            this.InitializeComponent();

           // ApplicationView.PreferredLaunchViewSize = new Size(200, 200);
           // ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0))
            {
                //for connect or disconnect event
                //App.AppServiceConnected += MainPage_AppServiceConnected;
                //App.AppServiceDisconnected += MainPage_AppServiceDisconnected;

                //for sideload app
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            }
        }

        /// <summary>
        /// When the desktop process is disconnected, reconnect if needed
        /// </summary>
        private async void MainPage_AppServiceDisconnected(object sender, EventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            { 
                Reconnect();
            });
        }

        /// <summary>
        /// Ask user if they want to reconnect to the desktop process
        /// </summary>
        private async void Reconnect()
        {
            if (App.IsForeground)
            {
                MessageDialog dlg = new MessageDialog("Connection to desktop process lost. Reconnect?");
                UICommand yesCommand = new UICommand("Yes", async (r) =>
                {
                    await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
                });
                dlg.Commands.Add(yesCommand);
                UICommand noCommand = new UICommand("No", (r) => { });
                dlg.Commands.Add(noCommand);
                await dlg.ShowAsync();
            }
        }

.......

```


if can not find entrypoint, can see this for more information  
https://stackoverflow.com/questions/77065216/dep0700-registration-of-the-app-failed-while-uwp-extension-launch-consult-app/77493220#77493220

---------------------------------------------------------



Go to BelaunchedApp/reference and add those file 
![image](https://github.com/testtestProblem/UWP_extention/assets/107662393/9f198e02-c419-4764-a2b8-77e8e318e154)





-----------------------------------------------





* And should generate self-signed certification.  
![image](https://github.com/testtestProblem/UWP_extention/assets/107662393/0aca610e-2e1f-483e-be29-ae0ea8851638)

* Add references Windows Desktop Extensions for the UWP in UWP project.  
![image](https://github.com/testtestProblem/UWP_extention/assets/107662393/9c2721f2-0fc4-4a04-8162-bc3da3b69213)

# Reference
* https://stefanwick.com/2018/04/16/uwp-with-desktop-extension-part-3/
* https://www.cnblogs.com/manupstairs/p/14178931.html
* https://github.com/StefanWickDev/UWP-FullTrust/tree/master/UWP_FullTrust_3
* https://freetion26.medium.com/uwp%E9%80%8F%E9%81%8Eappserviceconnection%E5%92%8Cwin32-process%E6%BA%9D%E9%80%9A%E5%8F%96%E5%BE%97%E7%B3%BB%E7%B5%B1%E8%B7%AF%E5%BE%91%E4%B8%8B%E6%AA%94%E6%A1%88%E5%88%97%E8%A1%A8-981dd0765e52
