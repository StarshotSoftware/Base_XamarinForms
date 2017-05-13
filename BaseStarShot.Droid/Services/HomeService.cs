using Android.App;
using Android.Views.InputMethods;
using System;
namespace BaseStarShot.Services
{
    public class HomeService : IHomeService
    {

        public event EventHandler<bool> HomeCalled = delegate
        {
        };

        public bool GoHome()
        {
          
            return true;
        }

    }
}