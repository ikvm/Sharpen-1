﻿using System;
using System.ComponentModel.Design;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Sharpen.VisualStudioExtension.ToolWindows;

namespace Sharpen.VisualStudioExtension.Commands
{
    internal abstract class BaseSharpenCommand<TSharpenCommand> where TSharpenCommand : BaseSharpenCommand<TSharpenCommand>
    {
        protected Package Package { get; }
        protected IServiceProvider ServiceProvider { get; }

        protected BaseSharpenCommand(Package package, int commandId, Guid commandSet)
        {
            Package = package ?? throw new ArgumentNullException(nameof(package));
            ServiceProvider = package;

            if (!(ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)) return;

            var menuCommandId = new CommandID(commandSet, commandId);
            var menuItem = new MenuCommand((sender, e) => OnExecute(), menuCommandId);
            commandService.AddCommand(menuItem);
        }

        protected void ShowSharpenResultsToolWindow()
        {
            ToolWindowPane window = Package.FindToolWindow(typeof(SharpenResultsToolWindow), 0, true);
            if (window?.Frame == null)
                throw new NotSupportedException($"Cannot create the '{typeof(SharpenResultsToolWindow)}' tool window.");

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        protected void ShowInformation(string message)
        {
            MessageBox.Show(message, "Sharpen", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        protected abstract void OnExecute();

        public static TSharpenCommand Instance { get; protected set; }
    }
}