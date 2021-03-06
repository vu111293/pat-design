﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 4810 $</version>
// </file>

using ICSharpCode.SharpDevelop.Internal.Templates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;
using MSBuild = Microsoft.Build.BuildEngine;

namespace ICSharpCode.SharpDevelop.Project
{
	public enum OutputType {
		[Description("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.Exe}")]
		Exe,
		[Description("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.WinExe}")]
		WinExe,
		[Description("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.Library}")]
		Library,
		[Description("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.Module}")]
		Module
	}
	
	/// <summary>
	/// A compilable project based on MSBuild.
	/// </summary>
	public abstract class CompilableProject : MSBuildBasedProject
	{
		#region Static methods
		/// <summary>
		/// Gets the file extension of the assembly created when building a project
		/// with the specified output type.
		/// Example: OutputType.Exe => ".exe"
		/// </summary>
		public static string GetExtension(OutputType outputType)
		{
			switch (outputType) {
				case OutputType.WinExe:
				case OutputType.Exe:
					return ".exe";
				case OutputType.Module:
					return ".netmodule";
				default:
					return ".dll";
			}
		}
		#endregion
		
		/// <summary>
		/// A list of project properties that cause reparsing of references when they are changed.
		/// </summary>
		protected readonly Set<string> reparseReferencesSensitiveProperties = new Set<string>();
		
		/// <summary>
		/// A list of project properties that cause reparsing of code when they are changed.
		/// </summary>
		protected readonly Set<string> reparseCodeSensitiveProperties = new Set<string>();
		
		protected CompilableProject(IMSBuildEngineProvider engineProvider)
			: base(engineProvider.BuildEngine)
		{
		}
		
        //protected override void Create(ICSharpCode.SharpDevelop.Internal.Templates.ProjectCreateInformation information)
        //{
        //    base.Create(information);
			
        //    this.MSBuildProject.DefaultTargets = "Build";
			
        //    this.OutputType = OutputType.Exe;
        //    this.RootNamespace = information.RootNamespace;
        //    this.AssemblyName = information.ProjectName;
			
        //    if (!string.IsNullOrEmpty(information.TargetFramework)) {
        //        this.TargetFrameworkVersion = information.TargetFramework;
        //        AddOrRemoveExtensions();
        //    }
			
        //    SetProperty("Debug", null, "OutputPath", @"bin\Debug\",
        //                PropertyStorageLocations.ConfigurationSpecific, true);
        //    SetProperty("Release", null, "OutputPath", @"bin\Release\",
        //                PropertyStorageLocations.ConfigurationSpecific, true);
        //    InvalidateConfigurationPlatformNames();
			
        //    SetProperty("Debug", null, "DebugSymbols", "True",
        //                PropertyStorageLocations.ConfigurationSpecific, true);
        //    SetProperty("Release", null, "DebugSymbols", "False",
        //                PropertyStorageLocations.ConfigurationSpecific, true);
			
        //    SetProperty("Debug", null, "DebugType", "Full",
        //                PropertyStorageLocations.ConfigurationSpecific, true);
        //    SetProperty("Release", null, "DebugType", "None",
        //                PropertyStorageLocations.ConfigurationSpecific, true);
			
        //    SetProperty("Debug", null, "Optimize", "False",
        //                PropertyStorageLocations.ConfigurationSpecific, true);
        //    SetProperty("Release", null, "Optimize", "True",
        //                PropertyStorageLocations.ConfigurationSpecific, true);
        //}
		
		/// <summary>
		/// Gets the path where temporary files are written to during compilation.
		/// </summary>
		[Browsable(false)]
		public string IntermediateOutputFullPath {
			get {
				string outputPath = GetEvaluatedProperty("IntermediateOutputPath");
				if (string.IsNullOrEmpty(outputPath)) {
					outputPath = GetEvaluatedProperty("BaseIntermediateOutputPath");
					if (string.IsNullOrEmpty(outputPath)) {
						outputPath = "obj";
					}
					outputPath = Path.Combine(outputPath, this.ActiveConfiguration);
				}
				return Path.Combine(Directory, outputPath);
			}
		}
		
		/// <summary>
		/// Gets the full path to the xml documentation file generated by the project, or
		/// <c>null</c> if no xml documentation is being generated.
		/// </summary>
		[Browsable(false)]
		public string DocumentationFileFullPath {
			get {
				string file = GetEvaluatedProperty("DocumentationFile");
				if (string.IsNullOrEmpty(file))
					return null;
				return Path.Combine(Directory, file);
			}
		}
		
		// Make Language abstract again to ensure backend-binding implementers don't forget
		// to set it.
		public abstract override string Language {
			get;
		}
		
		public abstract override ICSharpCode.SharpDevelop.Dom.LanguageProperties LanguageProperties {
			get;
		}
		
		[Browsable(false)]
		public string TargetFrameworkVersion {
			get { return GetEvaluatedProperty("TargetFrameworkVersion") ?? "v2.0"; }
			set { SetProperty("TargetFrameworkVersion", value); }
		}
		
		public override string AssemblyName {
			get { return GetEvaluatedProperty("AssemblyName") ?? Name; }
			set { SetProperty("AssemblyName", value); }
		}
		
		public override string RootNamespace {
			get { return GetEvaluatedProperty("RootNamespace") ?? ""; }
			set { SetProperty("RootNamespace", value); }
		}
		
		/// <summary>
		/// The full path of the assembly generated by the project.
		/// </summary>
		public override string OutputAssemblyFullPath {
			get {
				string outputPath = GetEvaluatedProperty("OutputPath") ?? "";
				return Path.Combine(Path.Combine(Directory, outputPath), AssemblyName + GetExtension(OutputType));
			}
		}
		
		/// <summary>
		/// The full path of the folder where the project's primary output files go.
		/// </summary>
		public string OutputFullPath {
			get {
				string outputPath = GetEvaluatedProperty("OutputPath");
				// Path.GetFullPath() cleans up any back references.
				// e.g. C:\windows\system32\..\system becomes C:\windows\system
				return Path.GetFullPath(Path.Combine(Directory, outputPath));
			}
		}
		
		[Browsable(false)]
		public OutputType OutputType {
			get {
				try {
					return (OutputType)Enum.Parse(typeof(OutputType), GetEvaluatedProperty("OutputType") ?? "Exe");
				} catch (ArgumentException) {
					return OutputType.Exe;
				}
			}
			set {
				SetProperty("OutputType", value.ToString());
			}
		}
		
		protected override ParseProjectContent CreateProjectContent()
		{
			return ParseProjectContent.CreateUninitalized(this);
		}
		
		#region Starting (debugging)
		public override bool IsStartable {
			get {
				switch (this.StartAction) {
					case StartAction.Project:
						return OutputType == OutputType.Exe || OutputType == OutputType.WinExe;
					case StartAction.Program:
						return this.StartProgram.Length > 0;
					case StartAction.StartURL:
						return this.StartUrl.Length > 0;
					default:
						return false;
				}
			}
		}
		
		static string RemoveQuotes(string text)
		{
			if (text.StartsWith("\"") && text.EndsWith("\""))
				return text.Substring(1, text.Length - 2);
			else
				return text;
		}
		
		[Obsolete("Override CreateStartInfo instead of using Start()")]
		protected void Start(string program, bool withDebugging)
		{
			ProcessStartInfo psi;
			try {
				psi = CreateStartInfo();
			} catch (ProjectStartException ex) {
				MessageService.ShowError(ex.Message);
				return;
			}
            //if (withDebugging) {
            //    DebuggerService.CurrentDebugger.Start(psi);
            //} else {
            //    DebuggerService.CurrentDebugger.StartWithoutDebugging(psi);
            //}
		}
		
		/// <summary>
		/// Creates a <see cref="ProcessStartInfo"/> for the specified program, using
		/// arguments and working directory from the project options.
		/// </summary>
		protected ProcessStartInfo CreateStartInfo(string program)
		{
			program = RemoveQuotes(program);
			if (!FileUtility.IsValidPath(program)) {
				throw new ProjectStartException(program + " is not a valid path; the process cannot be started.");
			}
			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = Path.Combine(Directory, program);
			string workingDir = StringParser.Parse(this.StartWorkingDirectory);
			
			if (workingDir.Length == 0) {
				psi.WorkingDirectory = Path.GetDirectoryName(psi.FileName);
			} else {
				workingDir = RemoveQuotes(workingDir);
				
				if (!FileUtility.IsValidPath(workingDir)) {
					throw new ProjectStartException("Working directory '" + workingDir + "' is invalid; the process cannot be started. You can specify the working directory in the project options.");
				}
				psi.WorkingDirectory = Path.Combine(Directory, workingDir);
			}
			psi.Arguments = StringParser.Parse(this.StartArguments);
			
			if (!File.Exists(psi.FileName)) {
				throw new ProjectStartException(psi.FileName + " does not exist and cannot be started.");
			}
			if (!System.IO.Directory.Exists(psi.WorkingDirectory)) {
				throw new ProjectStartException("Working directory " + psi.WorkingDirectory + " does not exist; the process cannot be started. You can specify the working directory in the project options.");
			}
			return psi;
		}
		
		public override ProcessStartInfo CreateStartInfo()
		{
			switch (this.StartAction) {
				case StartAction.Project:
					return CreateStartInfo(this.OutputAssemblyFullPath);
				case StartAction.Program:
					return CreateStartInfo(this.StartProgram);
				case StartAction.StartURL:
					string url = this.StartUrl;
					if (!FileUtility.IsUrl(url))
						url = "http://" + url;
					return new ProcessStartInfo(url);
				default:
					throw new System.ComponentModel.InvalidEnumArgumentException("StartAction", (int)this.StartAction, typeof(StartAction));
			}
		}
		
		[Browsable(false)]
		public string StartProgram {
			get {
				return GetEvaluatedProperty("StartProgram") ?? "";
			}
			set {
				SetProperty("StartProgram", string.IsNullOrEmpty(value) ? null : value);
			}
		}
		
		[Browsable(false)]
		public string StartUrl {
			get {
				return GetEvaluatedProperty("StartURL") ?? "";
			}
			set {
				SetProperty("StartURL", string.IsNullOrEmpty(value) ? null : value);
			}
		}
		
		[Browsable(false)]
		public StartAction StartAction {
			get {
				try {
					return (StartAction)Enum.Parse(typeof(StartAction), GetEvaluatedProperty("StartAction") ?? "Project");
				} catch (ArgumentException) {
					return StartAction.Project;
				}
			}
			set {
				SetProperty("StartAction", value.ToString());
			}
		}
		
		[Browsable(false)]
		public string StartArguments {
			get {
				return GetEvaluatedProperty("StartArguments") ?? "";
			}
			set {
				SetProperty("StartArguments", string.IsNullOrEmpty(value) ? null : value);
			}
		}
		
		[Browsable(false)]
		public string StartWorkingDirectory {
			get {
				return GetEvaluatedProperty("StartWorkingDirectory") ?? "";
			}
			set {
				SetProperty("StartWorkingDirectory", string.IsNullOrEmpty(value) ? null : value);
			}
		}
		#endregion
		
		protected override void OnActiveConfigurationChanged(EventArgs e)
		{
			base.OnActiveConfigurationChanged(e);
			if (!isLoading) {
				ParserService.Reparse(this, true, true);
			}
		}
		
		protected override void OnActivePlatformChanged(EventArgs e)
		{
			base.OnActivePlatformChanged(e);
			if (!isLoading) {
				ParserService.Reparse(this, true, true);
			}
		}
		
		protected override void OnPropertyChanged(ProjectPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (!isLoading) {
				if (reparseReferencesSensitiveProperties.Contains(e.PropertyName)) {
					ParserService.Reparse(this, true, false);
				}
				if (reparseCodeSensitiveProperties.Contains(e.PropertyName)) {
					ParserService.Reparse(this, false, true);
				}
			}
		}
		
		[Browsable(false)]
		public override string TypeGuid {
			get {
				return LanguageBindingService.GetCodonPerLanguageName(Language).Guid;
			}
			set {
				throw new NotSupportedException();
			}
		}
		
		public override ItemType GetDefaultItemType(string fileName)
		{
			string extension = Path.GetExtension(fileName);
			if (".resx".Equals(extension, StringComparison.OrdinalIgnoreCase)
			    || ".resources".Equals(extension, StringComparison.OrdinalIgnoreCase))
			{
				return ItemType.EmbeddedResource;
			} else if (".xaml".Equals(extension, StringComparison.OrdinalIgnoreCase)) {
				return ItemType.Page;
			} else {
				return base.GetDefaultItemType(fileName);
			}
		}
		
        //public override void ConvertToMSBuild35(bool changeTargetFrameworkToNet35)
        //{
        //    lock (SyncRoot) {
        //        base.ConvertToMSBuild35(changeTargetFrameworkToNet35);
				
        //        var winFxImport = MSBuildProject.Imports.Cast<Microsoft.Build.BuildEngine.Import>()
        //            .Where(import => !import.IsImported)
        //            .FirstOrDefault(import => string.Equals(import.ProjectPath, "$(MSBuildBinPath)\\Microsoft.WinFX.targets", StringComparison.OrdinalIgnoreCase));
        //        if (winFxImport != null) {
        //            MSBuildProject.Imports.RemoveImport(winFxImport);
        //        }
        //        if (changeTargetFrameworkToNet35) {
        //            bool isDotNet35 = TargetFrameworkVersion == "v3.5";
        //            SetProperty(null, null, "TargetFrameworkVersion", "v3.5", PropertyStorageLocations.Base, true);
					
        //            if (!isDotNet35) {
        //                AddDotnet35References();
        //            }
        //        } else {
        //            foreach (string config in ConfigurationNames) {
        //                foreach (string platform in PlatformNames) {
        //                    PropertyStorageLocations loc;
        //                    string targetFrameworkVersion = GetProperty(config, platform, "TargetFrameworkVersion", out loc);
        //                    if (string.IsNullOrEmpty(targetFrameworkVersion))
        //                        targetFrameworkVersion = "v2.0";
        //                    switch (targetFrameworkVersion) {
        //                        case "CF 1.0":
        //                            targetFrameworkVersion = "CF 2.0";
        //                            break;
        //                        case "v1.0":
        //                        case "v1.1":
        //                            targetFrameworkVersion = "v2.0";
        //                            break;
        //                    }
        //                    if (targetFrameworkVersion == "v2.0" && winFxImport != null)
        //                        targetFrameworkVersion = "v3.0";
        //                    SetProperty(config, platform, "TargetFrameworkVersion", targetFrameworkVersion, loc, true);
        //                }
        //            }
        //        }
        //    }
        //    AddOrRemoveExtensions();
        //}
		
		protected internal virtual void AddDotnet35References()
		{
			ReferenceProjectItem rpi = new ReferenceProjectItem(this, "System.Core");
			rpi.SetMetadata("RequiredTargetFramework", "3.5");
			ProjectService.AddProjectItem(this, rpi);
			
			if (GetItemsOfType(ItemType.Reference).Any(r => r.Include == "System.Data")) {
				rpi = new ReferenceProjectItem(this, "System.Data.DataSetExtensions");
				rpi.SetMetadata("RequiredTargetFramework", "3.5");
				ProjectService.AddProjectItem(this, rpi);
			}
			if (GetItemsOfType(ItemType.Reference).Any(r => r.Include == "System.Xml")) {
				rpi = new ReferenceProjectItem(this, "System.Xml.Linq");
				rpi.SetMetadata("RequiredTargetFramework", "3.5");
				ProjectService.AddProjectItem(this, rpi);
			}
		}
		
		protected internal virtual void AddOrRemoveExtensions()
		{
		}
	}
	
	namespace Commands
	{
		public class AddDotNet35ReferencesIfTargetFrameworkIs35Command : AbstractCommand
		{
			public override void Run()
			{
				CompilableProject project = (CompilableProject)Owner;
				if (project.TargetFrameworkVersion == "v3.5") {
					project.AddDotnet35References();
				}
			}
		}
	}
}
