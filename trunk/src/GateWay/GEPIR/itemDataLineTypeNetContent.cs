﻿#region WSCF
//------------------------------------------------------------------------------
// <autogenerated code>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated code>
//------------------------------------------------------------------------------
// data objects suffixed with GWO 
//
// This source code was auto-generated by WsContractFirst, Version=0.7.6319.1
#endregion


namespace Consumentor.ShopGun.Gateway.Gepir
{
    using System.Xml.Serialization;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Diagnostics;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wscf", "0.7.6319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.gepir.org/", TypeName="itemDataLineTypeNetContent")]
    [System.ComponentModel.TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
    public partial class itemDataLineTypeNetContentGWO
    {
        
        /// <remarks/>
        private string uom;
        
        /// <remarks/>
        private string value;
        
        public itemDataLineTypeNetContentGWO()
        {
        }
        
        public itemDataLineTypeNetContentGWO(string uom, string value)
        {
            this.uom = uom;
            this.value = value;
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute(AttributeName="uom")]
        public string Uom
        {
            get
            {
                return this.uom;
            }
            set
            {
                if ((this.uom != value))
                {
                    this.uom = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                if ((this.value != value))
                {
                    this.value = value;
                }
            }
        }
    }
}