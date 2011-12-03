using System.Reflection;

#if DEBUG
[assembly: AssemblyProduct("Kiwi (Debug)")]
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyProduct("Kiwi (Release)")]
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyDescription("Library used for embedding your GitHub Wiki locally.")]
[assembly: AssemblyCompany("Daniel Wertheim")]
[assembly: AssemblyCopyright("Copyright © Daniel Wertheim")]
[assembly: AssemblyTrademark("")]

[assembly: AssemblyVersion("0.2.1")]
[assembly: AssemblyFileVersion("0.2.1")]


