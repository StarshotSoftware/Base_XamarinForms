


using Android.App;
using Android.Views.InputMethods;
namespace BaseStarShot.Services
{
    public class PlatformService : IPlatformService
    {
        readonly Activity context;

        public PlatformService(Activity context)
        {
            this.context = context;
        }

        void IPlatformService.CloseSoftKeyoard()
        {
            if (context.CurrentFocus != null)
            {
                InputMethodManager inputMethodManager = (InputMethodManager)context.GetSystemService(Activity.InputMethodService);
                inputMethodManager.HideSoftInputFromWindow(context.CurrentFocus.WindowToken, 0);
            }

        }
    }
}