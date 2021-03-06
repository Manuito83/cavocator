﻿//
// CAVOKATOR APP
// Website: https://github.com/Manuito83/Cavokator
// License GNU General Public License v3.0
// Manuel Ortega, 2018
//

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using System;
using System.Collections.Generic;
using SupportFragment = Android.Support.V4.App.Fragment;


namespace Cavokator
{
    [Activity(Label = "Cavokator", MainLauncher = true, Icon = "@drawable/ic_appicon",
     ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]

    public class ActivityWxMain : AppCompatActivity, DrawerLayout.IDrawerListener
    {
        //TODO [RELEASE]: Did we create a changelog for this version?
        public static bool versionWithChangelog = true;
        
        // Set "true" only for testing!
        bool overrideShowChangelog = false; 

        DrawerLayout drawerLayout;

        private SupportFragment fragmentToShowWhenDrawerClosed;

        private SupportFragment mCurrentFragment;
        private WxMetarFragment mWxMetarFragment;
        private NotamFragment mNotamFragment;
        private ConditionFragment mConditionFragment;
        private SettingsFragment mSettingsFragment;
        private AboutFragment mAboutFragment;
        private Stack<SupportFragment> mStackFragment;
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.drawer_layout);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawerLayout.AddDrawerListener(this);

            mWxMetarFragment = new WxMetarFragment();
            mNotamFragment = new NotamFragment();
            mConditionFragment = new ConditionFragment();
            mSettingsFragment = new SettingsFragment();
            mAboutFragment = new AboutFragment();

            mStackFragment = new Stack<SupportFragment>();

            // Initialize Toolbar
            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            // Attach item selected handler to navigation view
            var navigationView = FindViewById<NavigationView>(Resource.Id.my_navigation_view);
            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            // Change this depending on what the APP launches in
            var fragmentToLaunch = mWxMetarFragment;

            // Add fragments to container (FrameLayout)
            var ft = SupportFragmentManager.BeginTransaction(); 
            ft.Add(Resource.Id.flContent, fragmentToLaunch);
            ft.Commit();

            mCurrentFragment = fragmentToLaunch;


            //Did we change version number and are showing changelog ?
            if (versionWithChangelog)
            {
                ShowChangelog();
            }
        }

        private void ShowChangelog()
        {
            try
            {
                // Get current version code first
                PackageInfo pInfo = PackageManager.GetPackageInfo(PackageName, 0);
                int currentVersionCode = pInfo.VersionCode;

                Console.WriteLine("VERSION: " + currentVersionCode);

                // Get current preferences
                ISharedPreferences mVersionCodePrefs = Application.Context.GetSharedPreferences("AppVersion_Preferences", FileCreationMode.Private);
                int savedVersion = mVersionCodePrefs.GetInt("appVersionPREF", 0);
                
                // Show dialog if version is old
                if ((savedVersion < currentVersionCode) || (overrideShowChangelog))
                {
                    // Pull up dialog
                    var transaction = SupportFragmentManager.BeginTransaction();
                    var changelogDialog = new ChangelogDialog();
                    changelogDialog.Show(transaction, "changelog_dialog");
                }                    
                
                // Update version code and preferences
                mVersionCodePrefs.Edit().PutInt("appVersionPREF", currentVersionCode).Apply();
            }
            catch
            {
                // If something is wrong, dialog won't show up
            }
        }


        // Assess which button was pressed in toolbar
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                // Hamburger button (open drawer)
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer((int)GravityFlags.Start);
                    break;
                // Pass and let the fragment handle the event
                case Resource.Id.menu_share_icon:
                    return false;
            }

            return true;
        }

        void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            // Close drawer
            drawerLayout.CloseDrawers();

            switch (e.MenuItem.ItemId)
            {
                case Resource.Id.action_fragment_metar:
                    fragmentToShowWhenDrawerClosed = mWxMetarFragment;
                    //ReplaceFragment(mWxMetarFragment);
                    break;
                case Resource.Id.action_fragment_notam:
                    fragmentToShowWhenDrawerClosed = mNotamFragment;
                    //ReplaceFragment(mNotamFragment);
                    break;
                case Resource.Id.action_fragment_condition:
                    fragmentToShowWhenDrawerClosed = mConditionFragment;
                    //ReplaceFragment(mConditionFragment);
                    break;
                case Resource.Id.action_fragment_settings:
                    fragmentToShowWhenDrawerClosed = mSettingsFragment;
                    //ReplaceFragment(mSettingsFragment);
                    break;
                case Resource.Id.action_fragment_about:
                    fragmentToShowWhenDrawerClosed = mAboutFragment;
                    //ReplaceFragment(mAboutFragment);
                    break;
            }

        }

        private void ReplaceFragment (SupportFragment fragment)
        {
            if (fragment.IsVisible)
            {
                return;
            }

            var ft = SupportFragmentManager.BeginTransaction();

            ft.SetCustomAnimations(Resource.Animation.fragment_in, Resource.Animation.slide_right);
            ft.Replace(Resource.Id.flContent, fragment);
            ft.Commit();
            //ft.AddToBackStack(null);

            mCurrentFragment = fragment;
        }

        public override void OnBackPressed()
        {
            if (drawerLayout.IsDrawerOpen((int)GravityFlags.Start))
            {
                drawerLayout.CloseDrawers();
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public void OnDrawerClosed(View drawerView)
        {
            ReplaceFragment(fragmentToShowWhenDrawerClosed);
        }

        public void OnDrawerOpened(View drawerView)
        {
            // Do nothing
        }

        public void OnDrawerSlide(View drawerView, float slideOffset)
        {
            // Do nothing
        }

        public void OnDrawerStateChanged(int newState)
        {
            // Do nothing
        }
    }

}