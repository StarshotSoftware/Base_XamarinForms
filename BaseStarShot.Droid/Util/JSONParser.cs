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
using Java.IO;
using Org.Json;
using Java.Net;

namespace BaseStarShot.Controls
{
    public class JSONParser
    {
         System.IO.Stream inputStream = null;  
         public JSONObject jObj = null;
          public string json = string.Empty;


          public string GetJSONFromUrl(string url)
          {
              try
              {
                  var urlnew = new URL(url);
                  var urlConnection = (HttpURLConnection)urlnew.OpenConnection();
                  inputStream = urlConnection.InputStream;
              }
              catch (Exception e)
              {
                  System.Console.WriteLine("Error:" + e);
              }

              try
              {
                  var reader = new BufferedReader(new InputStreamReader(inputStream, "iso-8859-1"), 8);
                  var sb = new StringBuilder();
                  string line = null;

                  while ((line = reader.ReadLine()) != null)
                  {
                      sb.Append(line + "\n");
                  }

                  json = sb.ToString();
                  inputStream.Close();
              }
              catch (Exception e)
              {
                  System.Console.WriteLine("Error:" + e);
              }
              return json;
          }

    }
}