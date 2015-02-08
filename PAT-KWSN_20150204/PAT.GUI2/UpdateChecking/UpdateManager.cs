using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;


namespace PAT.GUI.UpdateChecking
{
    public class UpdateManager
    {

        static public Manifest DeploymentManifest;
        public static Version LocalVersion;





        #region Static Public Methods


        /// <summary>
        /// Determines if an update is available by downloading the
        /// deployment manifest for the application and comparing
        /// the current version vs. the most recent deployment
        /// version found in the deployment manifest.
        /// </summary>
        /// <returns>True if an update is available, false otherwise.</returns>        
        static public bool IsUpdateAvailable(string uri) { return IsUpdateAvailable(new Uri(uri), null, null); }
        static public bool IsUpdateAvailable(Uri uri, string username, string password)
        {
            try
            {
                // Append the deployment manifest name to the end of the
                // update uri, if it isn't already provided
                if (!uri.AbsoluteUri.EndsWith(".application", true, CultureInfo.CurrentCulture))
                {
                    string applicationName = Path.GetFileNameWithoutExtension(
                                                 Assembly.GetEntryAssembly().ManifestModule.Name) + ".application";

                    UriBuilder uriBuilder = new UriBuilder(uri.AbsoluteUri);
                    uriBuilder.Path = Path.Combine(uriBuilder.Path, applicationName);
                    uri = uriBuilder.Uri;
                }
                
               
                // Get the local deployment manifest
                //LocalDeploymentManifest = GetLocalDeploymentManifest();

                
                string s = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                LocalVersion = new Version(s);

                
                //LocalDeploymentManifest.CurrentLocalVersion = localVersion;

                //log.Debug("Comparing versions...");
                // Determine if the local version is less than the server version
                Version serverVersion = new Version(1, 0);
                
                // Try to download a deployment manifest
                DeploymentManifest = new Manifest(uri, username, password);

                if (DeploymentManifest != null)
                    serverVersion = DeploymentManifest.CurrentPublishedVersion;

                if (LocalVersion < serverVersion)
                {
                    //log.Debug("An update is available!");

                    // If the server version is newer than our local version,
                    // then an update is available!
                    return true;
                }
                //log.Debug("No update is necessary.");
            }
            catch (WebException ex)
            {
                string s = ex.Message;
            }
            catch (Exception)
            {
                // FIXME: log information on catch
            }
            
            return false;
        }



               
        #endregion

  
    }
}