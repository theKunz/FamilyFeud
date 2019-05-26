﻿#pragma checksum "..\..\..\Controls\GameBuilder.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "224090EDEB05EE65485B6D04F78948F511E29E39"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using FamilyFeud.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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


namespace FamilyFeud.Controls {
    
    
    /// <summary>
    /// GameBuilder
    /// </summary>
    public partial class GameBuilder : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 44 "..\..\..\Controls\GameBuilder.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lvSelectableRounds;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\..\Controls\GameBuilder.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lvSelectedRounds;
        
        #line default
        #line hidden
        
        
        #line 76 "..\..\..\Controls\GameBuilder.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbBonusSelectorHeader;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\..\Controls\GameBuilder.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton rbMiddle;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\..\Controls\GameBuilder.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton rbEnd;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\..\Controls\GameBuilder.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton rbNone;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\..\Controls\GameBuilder.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lvSelectableBonusQuestions;
        
        #line default
        #line hidden
        
        
        #line 119 "..\..\..\Controls\GameBuilder.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lvSelectedBonusQuestions;
        
        #line default
        #line hidden
        
        
        #line 128 "..\..\..\Controls\GameBuilder.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnDone;
        
        #line default
        #line hidden
        
        
        #line 129 "..\..\..\Controls\GameBuilder.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCancel;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/FamilyFeud;component/controls/gamebuilder.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Controls\GameBuilder.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.lvSelectableRounds = ((System.Windows.Controls.ListView)(target));
            return;
            case 2:
            
            #line 54 "..\..\..\Controls\GameBuilder.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ChooseRound_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 59 "..\..\..\Controls\GameBuilder.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.UnChooseRound_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.lvSelectedRounds = ((System.Windows.Controls.ListView)(target));
            return;
            case 5:
            this.tbBonusSelectorHeader = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.rbMiddle = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 7:
            this.rbEnd = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 8:
            this.rbNone = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 9:
            this.lvSelectableBonusQuestions = ((System.Windows.Controls.ListView)(target));
            return;
            case 10:
            
            #line 104 "..\..\..\Controls\GameBuilder.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ChooseBonusQuestion_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            
            #line 109 "..\..\..\Controls\GameBuilder.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.UnChooseBonusQuestion_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.lvSelectedBonusQuestions = ((System.Windows.Controls.ListView)(target));
            return;
            case 13:
            this.btnDone = ((System.Windows.Controls.Button)(target));
            
            #line 128 "..\..\..\Controls\GameBuilder.xaml"
            this.btnDone.Click += new System.Windows.RoutedEventHandler(this.btnDone_Click);
            
            #line default
            #line hidden
            return;
            case 14:
            this.btnCancel = ((System.Windows.Controls.Button)(target));
            
            #line 129 "..\..\..\Controls\GameBuilder.xaml"
            this.btnCancel.Click += new System.Windows.RoutedEventHandler(this.btnCancel_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

