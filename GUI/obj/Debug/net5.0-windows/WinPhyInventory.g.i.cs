﻿#pragma checksum "..\..\..\WinPhyInventory.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "55F28C4CEF7218283A7BC19C67F172A7B285C781"
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


namespace GUI {
    
    
    /// <summary>
    /// WinPhyInventory
    /// </summary>
    public partial class WinPhyInventory : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 42 "..\..\..\WinPhyInventory.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DockPanel dpTop;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\WinPhyInventory.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblTop;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\WinPhyInventory.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnClose;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\WinPhyInventory.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GroupBox grpSearch;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\WinPhyInventory.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblSearchByKeyword;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\..\WinPhyInventory.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblSearchByCategory;
        
        #line default
        #line hidden
        
        
        #line 65 "..\..\..\WinPhyInventory.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtSearchByKeyword;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\..\WinPhyInventory.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cboSearchByCategory;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\..\WinPhyInventory.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnResetFilters;
        
        #line default
        #line hidden
        
        
        #line 70 "..\..\..\WinPhyInventory.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lvwPhyInventory;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.4.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/GUI;component/winphyinventory.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\WinPhyInventory.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.4.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.dpTop = ((System.Windows.Controls.DockPanel)(target));
            return;
            case 2:
            this.lblTop = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.btnClose = ((System.Windows.Controls.Button)(target));
            
            #line 44 "..\..\..\WinPhyInventory.xaml"
            this.btnClose.Click += new System.Windows.RoutedEventHandler(this.btnClose_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.grpSearch = ((System.Windows.Controls.GroupBox)(target));
            return;
            case 5:
            this.lblSearchByKeyword = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.lblSearchByCategory = ((System.Windows.Controls.Label)(target));
            return;
            case 7:
            this.txtSearchByKeyword = ((System.Windows.Controls.TextBox)(target));
            
            #line 65 "..\..\..\WinPhyInventory.xaml"
            this.txtSearchByKeyword.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.txtSearchByKeyword_TextChanged);
            
            #line default
            #line hidden
            return;
            case 8:
            this.cboSearchByCategory = ((System.Windows.Controls.ComboBox)(target));
            
            #line 66 "..\..\..\WinPhyInventory.xaml"
            this.cboSearchByCategory.Loaded += new System.Windows.RoutedEventHandler(this.cboSearchByCategory_Loaded);
            
            #line default
            #line hidden
            
            #line 66 "..\..\..\WinPhyInventory.xaml"
            this.cboSearchByCategory.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.cboSearchByCategory_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 9:
            this.btnResetFilters = ((System.Windows.Controls.Button)(target));
            
            #line 67 "..\..\..\WinPhyInventory.xaml"
            this.btnResetFilters.Click += new System.Windows.RoutedEventHandler(this.btnResetFilters_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.lvwPhyInventory = ((System.Windows.Controls.ListView)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

