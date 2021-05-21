using UnityEngine;
using UnityEngine.Advertisements;

namespace Octamino
{
    public class AdvertisementController : IUnityAdsListener
    {
        private const string AdvertisementId = "4137687";
        private const string AdvertisementType = "video";

        public AdvertisementController()
        {
            Advertisement.AddListener(this);
            Advertisement.Initialize (AdvertisementId, false);
        }

        public void ShowRewardedVideo() 
        {
            if (Advertisement.IsReady(AdvertisementType)) 
            {
                Advertisement.Show(AdvertisementType);
            } 
            else 
            {
                Debug.Log("Rewarded video is not ready at the moment! Please try again later!");
            }
        }

        ~AdvertisementController()
        {
            Advertisement.RemoveListener(this);
        }
        
        public void OnUnityAdsReady(string placementId)
        {
        }

        public void OnUnityAdsDidError(string message)
        {
            Debug.Log(message);
        }

        public void OnUnityAdsDidStart(string placementId)
        {
        }

        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        {
            if (placementId.Equals(AdvertisementType))
            {
                switch (showResult)
                {
                    case ShowResult.Finished:
                        Game.Instance.RollBack();
                        break;
                    case ShowResult.Skipped:
                        Game.Instance.RollBack();
                        break;
                    case ShowResult.Failed:
                        Debug.LogWarning ("The ad did not finish due to an error.");
                        break;
                }
            }
        }
    }
}