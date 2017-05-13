using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BaseStarShot.Controls.ImageHelper
{
    public class AnonymousAsyncTask<TParam, TProgress, TResult> : AsyncTask<TParam, TProgress, TResult>
    {
        public AnonymousAsyncTask(Func<TParam[], TResult> runInBackgroundFunc, Action<TResult> postExecuteAction)
        {
            this.RunInBackgroundFunc = runInBackgroundFunc;
            this.PostExecuteAction = postExecuteAction;
        }

        public Func<TParam[], TResult> RunInBackgroundFunc;
        public Action<TResult> PostExecuteAction;

        protected override TResult RunInBackground(params TParam[] @params)
        {
            return this.RunInBackgroundFunc(@params);
        }

        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] native_parms)
        {
            return base.DoInBackground(native_parms);
        }

        protected override void OnPostExecute(TResult result)
        {
            this.PostExecuteAction(result);
        }
    }
}