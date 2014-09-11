using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using NLog;
using Wulka.Configuration;
using Wulka.Exceptions;

namespace Wulka.Utils
{
    public class CompositionHelper
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        public static void ComposeParts(object bag, Type sourceType)
        {
            if (ConfigurationHelper.LogComposition)
                Logger.Info("Composing Parts...");
            var catalog = new AggregateCatalog();
            var assembly = sourceType.Assembly;
            if (ConfigurationHelper.LogComposition)
                Logger.Info("Checking assembly [{0}]", assembly.FullName);
            catalog.Catalogs.Add(new AssemblyCatalog(assembly));
            var composer = new CompositionContainer(catalog);
            try
            {
                composer.ComposeParts(bag);
            }
            catch (CompositionException exception)
            {
                Logger.Error("---> Error Composing Parts <----");
                Logger.Error(exception.GetCombinedMessages());
            }
        }
    }
}
