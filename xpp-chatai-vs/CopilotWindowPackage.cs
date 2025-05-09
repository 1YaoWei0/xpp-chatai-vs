﻿using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using xpp_chatai_vs.Command;
using xpp_chatai_vs.Utilities;
using xpp_chatai_vs.View;
using xpp_chatai_vs.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace xpp_chatai_vs
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// Willie Yao - 04/14/2025
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(CopilotWindow))]
    [ProvideToolWindow(typeof(xpp_chatai_vs.View.KnowledgeBaseWindow))]
    [ProvideOptionPage(typeof(OptionsProvider.OptionOptions), "Xpp ChatAI Options", "General", 0, 0, true, SupportsProfiles = true)]
    [Guid(CopilotWindowPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]  
    public sealed class CopilotWindowPackage : AsyncPackage
    {
        /// <summary>
        /// CopilotWindowPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "d33fb880-22a0-442d-8408-545cfa9b5a4b";

        internal static CopilotWindowPackage Instance { get; private set; }

        public CopilotSessionViewModel CopilotSessionViewModel { get; } = new CopilotSessionViewModel();

        public KnowledgeBaseViewModel KnowledgeBaseViewModel { get; } = new KnowledgeBaseViewModel();

        /// <summary>
        /// Initializes a new instance of the <see cref="CopilotWindowPackage"/> class.
        /// Willie Yao - 04/14/2025
        /// </summary>
        public CopilotWindowPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// Willie Yao - 04/14/2025
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            Instance = this;
            await CopilotSessionViewModel.LoadSessionsAsync();
            // KnowledgeBaseViewModel.LoadDataAsync();

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await CopilotWindowCommand.InitializeAsync(this);            
            await KnowledgeBaseWindowCommand.InitializeAsync(this);
        }

        /// <summary>
        /// Release resource after closing visual studio, 
        /// uninstalling or disable extension package
        /// Willie Yao - 04/10/2025
        /// </summary>
        /// <param name="disposing">Disposing</param>
        protected override void Dispose(bool disposing)
        {
            _ = CopilotSessionUtility.SaveSessionsAsync(CopilotSessionViewModel.Sessions);
        }

        public override IVsAsyncToolWindowFactory GetAsyncToolWindowFactory(Guid toolWindowType)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (toolWindowType == typeof(CopilotWindow).GUID)
            {
                return this;
            }

            return base.GetAsyncToolWindowFactory(toolWindowType);
        }

        protected override string GetToolWindowTitle(Type toolWindowType, int id)
        {
            if (toolWindowType == typeof(CopilotWindow))
            {
                return "CopilotWindow loading";
            }

            return base.GetToolWindowTitle(toolWindowType, id);
        }

        #endregion
    }
}
