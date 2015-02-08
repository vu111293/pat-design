using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if ACADEMIC
[assembly: AssemblyTitle("Process Analysis Toolkit (PAT)")]
[assembly: AssemblyDescription("PAT stands for Process Analysis Toolkit, which is an enhanced simulator and model checker for concurrent systems, real-time systems and more. The main features of PAT include fast verification of systems with fairness constraints, failure and divergence refinement and various optimizations techniques.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("PAT - An Enhanced Simulation and Model Checking Tool")]
#else
[assembly: AssemblyTitle("Process Analysis Toolkit Pro (PATPro)")]
[assembly: AssemblyDescription("PATPro stands for Process Analysis Toolkit Pro, which is an enhanced simulator and model checker for concurrent systems, real-time systems and more. The main features of PAT include fast verification of systems with fairness constraints, failure and divergence refinement and various optimizations techniques.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("PATPro - An Enhanced Simulation and Model Checking Tool")]
#endif

[assembly: AssemblyCompany("Semantic Engineering Pte. Ltd.")]
[assembly: AssemblyCopyright("Copyright © Semantic Engineering Pte. Ltd. 2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]


// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("826032d0-18a4-4950-8699-abb824c7dbe1")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:

[assembly: AssemblyVersion("3.5.0.21913")]
[assembly: AssemblyFileVersion("3.5.0.21913")]