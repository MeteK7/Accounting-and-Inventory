// Updated by XamlIntelliSenseFileGenerator 1/10/2021 5:33:51 PM
#pragma checksum "..\..\..\WinCategories.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "42A17169E7B7AF54EA4CC9A6267C1DB4B8149EDF"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using GUI;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace GUI
{


    /// <summary>
    /// WinCategories
    /// </summary>
    public partial class WinCategories : System.Windows.Window, System.Windows.Markup.IComponentConnector
    {

        private bool _contentLoaded;

        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.1.0")]
        public void InitializeComponent()
        {
            if (_contentLoaded)
            {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/GUI;V1.0.0.0;component/wincategories.xaml", System.UriKind.Relative);

#line 1 "..\..\..\WinCategories.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);

#line default
#line hidden
        }

        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.1.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
        {
            this._contentLoaded = true;
        }

        internal System.Windows.Controls.DockPanel dpTop;
        internal System.Windows.Controls.Label lblTop;
        internal System.Windows.Controls.Button btnClose;
        internal System.Windows.Controls.Label lblCategoryId;
        internal System.Windows.Controls.TextBox txtCategoryId;
        internal System.Windows.Controls.Label lblTitle;
        internal System.Windows.Controls.TextBox txtTitle;
        internal System.Windows.Controls.Label lblDescription;
        internal System.Windows.Controls.TextBox txtDescription;
        internal System.Windows.Controls.Button btnCategoryAdd;
        internal System.Windows.Controls.Button btnCategoryUpdate;
        internal System.Windows.Controls.Button btnCategoryDelete;
        internal System.Windows.Controls.DataGrid dtgCategories;
        internal System.Windows.Controls.Label lblCategorySearch;
        internal System.Windows.Controls.TextBox txtCategorySearch;
    }
}
