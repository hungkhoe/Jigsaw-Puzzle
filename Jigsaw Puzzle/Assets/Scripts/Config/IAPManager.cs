using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace JigsawPuzzle.IAP
{
    public class IAPManager : MonoBehaviour, IStoreListener
    {
        private static IAPManager _instance = null;

        private IStoreController m_StoreController = null;          // The Unity Purchasing system.
        private IExtensionProvider m_StoreExtensionProvider = null; // The store-specific Purchasing subsystems.

        public const string kProductIDConsumable = "consumable";
        public const string kProductIDNonConsumable = "nonconsumable";
        public const string kProductIDSubscription = "subscription";

        private Dictionary<string, IAPListener> listenerContainer = new Dictionary<string, IAPListener>();
        private IAPListener _listener = null;
        private string keyListener = null;
        
        private Dictionary<string, object> shopDict = null;

        public class IAPConst
        {
            // Apple
            public const string kProductNameAppleSubscription = "com.unity3d.subscription.new";

            // play store
            public const string kProductNameGooglePlaySubscription = "com.unity3d.subscription.original";


            // list ID
            public const string goldPack1 = "com.bs.jigsaw_v1.iap.gold1";
            public const string goldPack2 = "com.bs.jigsaw_v1.iap.gold2";
            public const string goldPack3 = "com.bs.jigsaw_v1.iap.gold3";           

        }

        public static IAPManager Instance
        {
            get { return _instance; }
        }

        void Awake()
        {
            shopDict = new Dictionary<string, object>();            

            if (_instance != null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;            
                DontDestroyOnLoad(this);
            }

        }

        void Start()
        {
            // If we haven't set up the Unity Purchasing reference
            if (m_StoreController == null)
            {
                // Begin to configure our connection to Purchasing
                InitializePurchasing();
            }
        }

        public void InitializePurchasing()
        {
            if (IsInitialized())
            {
                return;
            }

            // Create a builder, first passing in a suite of Unity provided stores.
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            builder.Configure<IGooglePlayConfiguration>().SetPublicKey("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAleYKGcUX8/Cndwz5RAdZgW3SIHIzMP9elLidMEiSsC2QAx1jjTkrylT9BnHjiKffrnPCDEoVo+Jr5v+wue4EqI7YC2vmym57Ck8VHxxAsce8VW8TPTEitipHXs8LobY/Bj5Sctd7DKZXhay3uGf63QvPnhu1dS6IXuu3OY/HZhAjqpfavDZuLl0ZMI7GawQUxNa72I8Au7YhME5OQBYvxz/n1QFBq/4ESp9+iJqXgEfNRc1DdZrTUi9xu7ZPOG+1HxzkRawK7CpCJqqiRqydrVKHN2YXQSwnebE9uGKxdTS8GdMoXnzm939jyuVJiBzjFmp7TAetMEYVvC+oGTDfbwIDAQAB");





            // Add a product to sell / restore by way of its identifier, associating the general identifier
            // with its store-specific identifiers.
            //builder.AddProduct(kProductIDConsumable, ProductType.Consumable);
            // Continue adding the non-consumable product.
            //builder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable);

            //Pack
            builder.AddProduct(IAPConst.goldPack1, ProductType.Consumable);
            builder.AddProduct(IAPConst.goldPack2, ProductType.Consumable);
            builder.AddProduct(IAPConst.goldPack3, ProductType.Consumable);

            // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
            // if the Product ID was configured differently between Apple and Google stores. Also note that
            // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
            // must only be referenced here. 
            /*builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs(){
                { IAPConst.kProductNameAppleSubscription, AppleAppStore.Name },
                { IAPConst.kProductNameGooglePlaySubscription, GooglePlay.Name },
            });*/

            // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
            // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
            UnityPurchasing.Initialize(this, builder);
        }

        public bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            var prodId = args.purchasedProduct.definition.id;

            processProductPurchased(prodId, args);


            return PurchaseProcessingResult.Complete;
        }

        private void processProductPurchased(string prodId, PurchaseEventArgs args = null)
        {
            IAPListener tmpListener = null;
            checkAdState(prodId);


            Debug.Log(string.Format("ProcessPurchase. Product: '{0}'", prodId));


            if (listenerContainer.TryGetValue(keyListener, out tmpListener) && shopDict.ContainsKey(prodId))
            {
                tmpListener.OnProductIDPurchased(args, shopDict[prodId]);

            }
        }

        private void checkAdState(string prodId)
        {
         
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
            // this reason with the user to guide their troubleshooting actions.
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));

            if (IsInitialized())
            {
                m_StoreController.ConfirmPendingPurchase(product);
                if (failureReason == PurchaseFailureReason.DuplicateTransaction)
                {
                    processProductPurchased(product.definition.id);
                }
            }
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            // Overall Purchasing system, configured with products for this application.
            m_StoreController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            m_StoreExtensionProvider = extensions;

        }

        public void BuyProductID(string productId)
        {
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                // ... look up the Product reference with the general product identifier and the Purchasing 
                // system's products collection.
                Product product = m_StoreController.products.WithID(productId);

                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                    // asynchronously.
                    m_StoreController.InitiatePurchase(product);
                }
                // Otherwise ...
                else
                {
                    // ... report the product look-up failure situation  
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            // Otherwise ...
            else
            {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
                // retrying initiailization.
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }


        // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
        // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
        public void RestorePurchases()
        {
            // If Purchasing has not yet been set up ...
            if (!IsInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            // If we are running on an Apple device ... 
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                // ... begin restoring purchases
                Debug.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
                // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result) =>
                {
                    // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                    // no purchases are available to be restored.
                    Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                Debug.Log("RestorePurchases started for Android platform ...");
                //var androidStore = m_StoreExtensionProvider.GetExtension<IAndroidStoreSelection>();
            }

            // Otherwise ...
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }

        public decimal GetLocalizedPrice(string productId)
        {
            if (IsInitialized())
            {
                Product product = m_StoreController.products.WithID(productId);
                if (product != null)
                {
                    return product.metadata.localizedPrice;
                }
                else
                {
                    return new decimal(1);
                }

            }
            else
            {
                return new decimal(1);
            }
        }

        public string GetLocalizedPriceString(string productId)
        {
            if (IsInitialized())
            {
                Product product = m_StoreController.products.WithID(productId);
                if (product != null)
                {
                    return product.metadata.localizedPriceString;
                }

                return string.Empty;


            }
            else
            {
                return string.Empty;
            }
        }

        public void RegisterShopItem(string key, object item)
        {
            shopDict.Remove(key);
            shopDict.Add(key, item);

        }

        public void RegisterIAPListener(string key, IAPListener mLis)
        {

            unregisterAdmobListener(key);
            this.keyListener = key;
            listenerContainer.Add(key, mLis);
            this._listener = mLis;
        }

        public void unregisterAdmobListener(string key)
        {
            listenerContainer.Remove(key);
            _listener = null;
        }

        public interface IAPListener
        {
            void OnProductIDPurchased(PurchaseEventArgs eventArgs, object item);
        }
    }
}
