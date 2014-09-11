****************
** README.TXT **
****************
This file is a rolling notebook of daily practice and experience. The purpose is to serve a a draft sketchbook for development and Publishment.
You may find loose notes in here, but also more extensive draft descriptions.


*************************
** Creating a S: drive **
*************************
Execute substS_MobiGuider.cmd  (or similar cmd according to your needs and specific folder structure)


******************
** Strong names **
******************
Before compiling any projects, please execute sn_Wulka.cmd from  a .NET Command Box
If prompted for password use: prodata


*************************
** Wulka.pfx password **
*************************
Password for Wulka.pfx = K1llB1ll2
Sign all Clickonce Manifests using Wulka.pfx

****************************
** MobiFxSDK.pfx password **
****************************
Import the certificate, password: toto
Visual Studio command prompt: sn -i mobiFxSDK.pfx VS_KEY_0E3F782D3AAEC946

****************************
** LOG4NET LOGGING POLICY **
****************************
1) Define a System Environment Variable called Wulka_LOGS and assign it <your prefered logging folder>
2) in your logging configuration, replace the litteral folder with ${Wulka_LOGS} i.e. ${Wulka_LOGS}\Wulka.MobiFx.Service.log
3) Restart your machine!




**********************************************
** SOA Initial Setup and Installation Notes **
**********************************************
1) Run IIS7.5 + AppFabric
2) Install Server Certificate MobiFx.pfx:
     - Right click on cer-file -> Install Certificate
     - In IIS: server -> Server Certificates -> Import... -> browse to MobiFx.pfx and enter password Pr0d@t@
3) Run in command prompt with administrator rights: "C:\Program Files\Windows Resource Kits\Tools\winhttpcertcfg" -g -c LOCAL_MACHINE\My -s MobiFx -a NetworkService
	 Download winhttpcertcfg if needed @ http://www.microsoft.com/downloads/en/details.aspx?familyid=c42e27ac-3409-40e9-8667-c748e422833f&displaylang=en
	 On x64 systems use "C:\Program Files (x86)\Windows Resource Kits\Tools\winhttpcertcfg" -g -c LOCAL_MACHINE\My -s MobiFx -a NetworkService
	 This tool is also available on the deployment server! in the forlder Where all the tools and goodies are (WCFTools?)

4) Create Application pool defaults
     (    -> server -> Configuration Editor -> system.applicationHost/applicationPools -> applicationPoolDefaults
       OR in IIS -> Application Pools -> Set Application Pool Defaults )

   AutoStart=true, 
   StartMode = AlwaysRunning, 
   Identity=NetworkService, 
   IdleTimeout=00:00:00, 
   NO Recycling

5) Create v application pools

  - DiscoProxy
  - Application
  - Business
  - Infrastructure
  - Repository
  
 
6) Create 2 Websites:

Step 0) Suggested Folder Structure:

     <root>
        +- MobiGuider
        +- DiscoProxy  

Step 1) Site "MobiGuider" on port 80 => bindings: http, https (certificate "MobiFX", port 443), net.pipe => App Pool: MobiGuider
Step 2) Site "DiscoProxy" on port 88 => bindings: http, net.pipe => App Pool: DiscoProxy
Step 3) Make sure that the "NETWORK SERVICE" Account has full access to the root folder of these websites
Step 4) In appFabric, set "Enable metadata over HTTP" to true for both websites
Step 5) In "Advanced Settings" set "enabled protocols" to http,net.pipe


**********************************
** SOA Core Publishing sequence **
**********************************
Step 0) Open DebugView from SysInternals and set filter to "DiscoveryProxy"

--> All these core services can be found in the Wulka.MobiFxCoreSetup.root solution !

Step 1) 1.0 Publish the [DiscoveryProxy] service to    http://localhost:88/discoproxy/ 
        1.1 Convert the resuling directory to an application, select "DiscoProxy" as application pool
        1.2 Advanced settings => "Enabled Protocols" (for that application) set to http,net.pipe
        1.3 In Appfabric, set application to "Enable automatic startup" 

Step 2) 2.0 Publish the [Disco] service to             http://localhost/services/infrastructure/disco
        2.1 Convert the resuling directory to an application, select "Infrastructure" as application pool
        2.2 Advanced settings => "Enabled Protocols" (for that application) set to http,net.pipe
        2.3 In Appfabric, set application to "Enable automatic startup" 

Step 3) 3.0 Publish the [MobiGuider] repository to     http://localhost/services/repository/mobiguider 
        3.1 Convert the resuling directory to an application, select "Repository" as application pool
        3.2 Advanced settings => "Enabled Protocols" (for that application) set to http,net.pipe
        3.3 In Appfabric, set application to "Enable automatic startup" 

Step 4) 4.0 Publish the [MobiFx] service to      http://localhost/services/infrastructure/mobifx 
        4.1 Convert the resuling directory to an application, select "Application" as application pool
        4.2 Advanced settings => "Enabled Protocols" (for that application) set to http,net.pipe
        4.3 In Appfabric, set application to "Enable automatic startup" 

Step 5) 5.0 Publish the [SessionProxy] service to      http://localhost/services/infrastructure/sessionproxy 
        5.1 Convert the resuling directory to an application, select "Infrastructure" as application pool
        5.2 Advanced settings => "Enabled Protocols" (for that application) set to http,net.pipe
        5.3 In Appfabric, set application to "Enable automatic startup" 

Step 6) 6.0 Publish the [Authentication] service to    http://localhost/services/infrastructure/auth 
        6.1 Convert the resuling directory to an application, select "Infrastructure" as application pool
        6.2 Advanced settings => "Enabled Protocols" (for that application) set to http,net.pipe
        6.3 In Appfabric, set application to "Enable automatic startup" 

Step 7) 7.0 Publish the [WinAuthentication] service to http://localhost/services/infrastructure/winauth 
        7.1 Convert the resuling directory to an application, select "Infrastructure" as application pool
        7.2 Advanced settings => "Enabled Protocols" (for that application) set to http,net.pipe
        7.3 In Appfabric, set application to "Enable automatic startup" 
        7.4 For this application, open the "Authentication" Options and set "Windows Authentication" to ENABLED  

Step 8) 8.0 Publish the [MonitorDisco] service to      http://localhost/services/application/monitordisco 
        8.1 Convert the resuling directory to an application, select "Application" as application pool
        8.2 Advanced settings => "Enabled Protocols" (for that application) set to http,net.pipe
        8.3 In Appfabric, set application to "Enable automatic startup" 


Step 10) Compile the MonitorDisco.UI applet and run it, you should see the published services in the list

*******************************
** Publishing applets to IIS **
*******************************
Publishing applets to the IIS server is done using the classic "Publish" functionality for clickonce applications.
1) Sign the ClickOnce Manifest of an applet using the Wulka.pfx key (select file)
2) Publish the applet to the following location: http://localhost/clickonce/Wulka.<appletname>.ui
3) once in a while, clean up older versions on the server - they can be found in subfolders


















