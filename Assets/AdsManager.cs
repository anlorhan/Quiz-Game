using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdsManager : MonoBehaviour
{
    public static AdsManager instance;
    private BannerView bannerView;
    private InterstitialAd interstitial;
    private RewardedAd rewardedAd;
    private string BannerId = "ca-app-pub-8987765162186421/4890665454";
    private string InterstitialId = "ca-app-pub-8987765162186421/7018135344";
    private string RewardAdId = "ca-app-pub-8987765162186421/4742507552";
    void Start()
    {
        MobileAds.Initialize(initStatus => { });

        this.RequestBanner();
    }

    public void RequestBanner()
    {
        //deneme idsi
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";

        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(BannerId, AdSize.IABBanner, AdPosition.Bottom);//adUnitId test id si
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        this.bannerView.LoadAd(request);
    }
    public void RequestInterstitial()
    {
        //deneme idsi
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(InterstitialId);//adUnitId test id si

        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += İnterstitial_OnAdLoaded;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    public void İnterstitial_OnAdLoaded(object sender, System.EventArgs e)
    {
        interstitial.Show();
    }

    public void CreateAndLoadRewardedAd()
    {
        //deneme idsi
        string adUnitId = "ca-app-pub-3940256099942544/5224354917";

        this.rewardedAd = new RewardedAd(RewardAdId);//adUnitId test id si

        this.rewardedAd.OnAdLoaded += RewardedAd_OnAdLoaded; 
        this.rewardedAd.OnUserEarnedReward += RewardedAd_OnUserEarnedReward; 
        this.rewardedAd.OnAdClosed += RewardedAd_OnAdClosed; 

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }

    public void RewardedAd_OnAdClosed(object sender, System.EventArgs e)
    {
        Vector3 vector = new Vector3(1.5f, 1.5f, 1.5f);
        QuizManager.instance.Cointext.transform.LeanScale(vector, 0.3f).setEaseOutBack().setLoopPingPong(1).setOnComplete(FindObjectOfType<QuizManager>().ScaleNormalize);
    }

    public void RewardedAd_OnUserEarnedReward(object sender, Reward e)
    {
        int temp;
        QuizManager.instance.coins += 100;
        temp = QuizManager.instance.coins;
        PlayerPrefs.SetInt("Coins", temp);
    }

    public void RewardedAd_OnAdLoaded(object sender, System.EventArgs e)
    {
        rewardedAd.Show();
    }
}
