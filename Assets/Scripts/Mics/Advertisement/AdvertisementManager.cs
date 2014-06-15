using UnityEngine;
using System.Collections;

public class AdvertisementManager : MonoBehaviour {

	bool bEnable = false;

	public void HideAds() {	if (bEnable) AdvertisementHandler.HideAds(); }
	public void ShowAds() {	if (bEnable) AdvertisementHandler.ShowAds(); }
	public void EnableAds() {	if (bEnable) AdvertisementHandler.EnableAds(); }
	public void DisableAds() {	if (bEnable) AdvertisementHandler.DisableAds(); }

	// Use this for initialization
	public void Init () {
		if (Application.platform == RuntimePlatform.Android) {
			bEnable = true;
			AdvertisementHandler.Instantiate("a15342d3f28c966", AdvertisementHandler.AdvSize.BANNER, AdvertisementHandler.AdvOrientation.HORIZONTAL, AdvertisementHandler.Position.BOTTOM, AdvertisementHandler.Position.CENTER_HORIZONTAL, false, AdvertisementHandler.AnimationInType.SLIDE_IN_LEFT, AdvertisementHandler.AnimationOutType.FADE_OUT, AdvertisementHandler.LevelOfDebug.LOW);
			EnableAds();
		}
	}

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {	
	}
}
