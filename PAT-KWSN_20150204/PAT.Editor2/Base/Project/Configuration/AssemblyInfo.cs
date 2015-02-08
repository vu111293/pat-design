using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: CLSCompliant(false)]
[assembly: StringFreezing()]


[assembly: Dependency("ICSharpCode.TextEditor", LoadHint.Always)]
[assembly: Dependency("System.Core", LoadHint.Always)]
[assembly: Dependency("System.Drawing", LoadHint.Always)]
[assembly: Dependency("System.Xml", LoadHint.Always)]
[assembly: Dependency("System.Windows.Forms", LoadHint.Always)]
[assembly: Dependency("Microsoft.Build.Engine", LoadHint.Always)]

[assembly: AssemblyTitle("PAT.Editor.Core")]
[assembly: AssemblyDescription("The Core functionality of Editor")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
