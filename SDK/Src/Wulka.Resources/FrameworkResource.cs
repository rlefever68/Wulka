using System;
using System.Windows;

namespace Wulka.Resources
{
    public class FrameworkResource : ResourceBase
    {

        class Const
        {
            public const string Uri = "/Wulka.Resources;Component/Theme/CommonThemes.xaml";
        }

        readonly System.Collections.ObjectModel.Collection<ResourceDictionary> _styleDictionaries = new System.Collections.ObjectModel.Collection<ResourceDictionary>();



        //  private static Dictionary<string, ResourceDictionary> _resourceDictionaries = new Dictionary<string, ResourceDictionary>();

        /// <summary>
        /// Gets the resource dictionary collection.
        /// </summary>
        /// <returns></returns>
        public System.Collections.ObjectModel.Collection<ResourceDictionary> GetResourceDictionaryCollection()
        {
            var stylesDictionary = new ResourceDictionary
            {
                Source =
                    (new Uri(Const.Uri, UriKind.RelativeOrAbsolute))
            };

            _styleDictionaries.Add(stylesDictionary);
            //_resourceDictionaries  = (Dictionary<string, ResourceDictionary>) Application.LoadComponent(new Uri("/Wulka.Resources;component/Theme/XceedGridTheme.xaml",UriKind.Relative));
            return _styleDictionaries;
        }


        /// <summary>
        /// Gets the resource dictionary.
        /// </summary>
        /// <returns></returns>
        public static ResourceDictionary GetResourceDictionary()
        {
            return (ResourceDictionary)Application.LoadComponent(new Uri(Const.Uri, UriKind.Relative));
        }

    }

}
