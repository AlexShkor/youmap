// Type: StructureMap.Graph.IAssemblyScanner
// Assembly: StructureMap, Version=2.6.3.0, Culture=neutral, PublicKeyToken=e60ad81abae3c223
// Assembly location: D:\install\YouMap\YouMap\packages\structuremap.2.6.3\lib\StructureMap.dll

using StructureMap.Configuration.DSL.Expressions;
using System;
using System.Reflection;

namespace StructureMap.Graph
{
  public interface IAssemblyScanner
  {
    void Assembly(Assembly assembly);

    void Assembly(string assemblyName);

    void TheCallingAssembly();

    void AssemblyContainingType<T>();

    void AssemblyContainingType(Type type);

    void AssembliesFromPath(string path);

    void AssembliesFromPath(string path, Predicate<Assembly> assemblyFilter);

    void AssembliesFromApplicationBaseDirectory();

    void AssembliesFromApplicationBaseDirectory(Predicate<Assembly> assemblyFilter);

    void With(ITypeScanner scanner);

    void With<T>() where T : new(), ITypeScanner;

    void LookForRegistries();

    FindAllTypesFilter AddAllTypesOf<PLUGINTYPE>();

    FindAllTypesFilter AddAllTypesOf(Type pluginType);

    void IgnoreStructureMapAttributes();

    void Exclude(Func<Type, bool> exclude);

    void ExcludeNamespace(string nameSpace);

    void ExcludeNamespaceContainingType<T>();

    void Include(Func<Type, bool> predicate);

    void IncludeNamespace(string nameSpace);

    void IncludeNamespaceContainingType<T>();

    void ExcludeType<T>();

    void Convention<T>() where T : new(), IRegistrationConvention;

    void With(IRegistrationConvention convention);

    void ModifyGraphAfterScan(Action<PluginGraph> modifyGraph);

    ConfigureConventionExpression WithDefaultConventions();

    ConfigureConventionExpression ConnectImplementationsToTypesClosing(Type openGenericType);

    ConfigureConventionExpression RegisterConcreteTypesAgainstTheFirstInterface();

    ConfigureConventionExpression SingleImplementationsOfInterface();
  }
}
