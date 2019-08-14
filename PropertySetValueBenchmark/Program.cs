using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PropertySetValueBenchmark
{
    [RPlotExporter, RankColumn]
    internal class Program
    {
        private static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<BenchmarkMethods>();
            Console.Read();
        }
    }

    [MemoryDiagnoser]
    public class BenchmarkMethods
    {
        private readonly Dictionary<string, PropertyInfo> _propiedades = new Dictionary<string, PropertyInfo>();

        [Benchmark]
        public void ObteniendoLaPropiedadSiempre()
        {
            var objeto = new ExampleClass();
            var type = objeto.GetType();
            type.GetProperty("Propiedad1").SetValue(objeto, "test1");
            type.GetProperty("Propiedad2").SetValue(objeto, "test2");
        }

        [Benchmark]
        public void ObteniendoLaPropiedadSiempreConLower()
        {
            var objeto = new ExampleClass();
            var type = objeto.GetType();
            type.GetProperties().Where(x => x.Name.ToLower() == ("Propiedad1").ToLower()).First().SetValue(objeto, "test1");
            type.GetProperties().Where(x => x.Name.ToLower() == ("Propiedad2").ToLower()).First().SetValue(objeto, "test2");
        }

        [Benchmark]
        public void ObteniendoLaDeCache()
        {
            var objeto = new ExampleClass();
            GetPropertyInfoFromReflexionOrCacheSinLower("Propiedad1").SetValue(objeto, "test1");
            GetPropertyInfoFromReflexionOrCacheSinLower("Propiedad2").SetValue(objeto, "test2");
        }

        [Benchmark]
        public void ObteniendoLaDeCacheConLower()
        {
            var objeto = new ExampleClass();
            GetPropertyInfoFromReflexionOrCacheConLower(("Propiedad1").ToLower()).SetValue(objeto, "test1");
            GetPropertyInfoFromReflexionOrCacheConLower(("Propiedad2").ToLower()).SetValue(objeto, "test2");
        }

        private PropertyInfo GetPropertyInfoFromReflexionOrCacheSinLower(string propertyName)
        {
            if (_propiedades.ContainsKey(propertyName))
            {
                return _propiedades[propertyName];
            }
            else
            {
                var type = typeof(ExampleClass);
                var property = type.GetProperty(propertyName);
                _propiedades.Add(propertyName, property);
                return property;
            }
        }

        private PropertyInfo GetPropertyInfoFromReflexionOrCacheConLower(string propertyName)
        {
            if (_propiedades.ContainsKey(propertyName))
            {
                return _propiedades[propertyName];
            }
            else
            {
                var type = typeof(ExampleClass);
                var property = type.GetProperties().Where(x=>x.Name.ToLower() == propertyName).First();
                _propiedades.Add(propertyName, property);
                return property;
            }
        }
    }

    internal class ExampleClass
    {
        public string Propiedad1 { get; set; }
        public string Propiedad2 { get; set; }
    }
}
