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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.gepir.org/", TypeName="itemDataLineTypeLinkUri")]
    [System.ComponentModel.TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
    public partial class itemDataLineTypeLinkUriGWO
    {
        
        /// <remarks/>
        private itemDataLineTypeLinkUriFormat format;
        
        /// <remarks/>
        private string value;
        
        public itemDataLineTypeLinkUriGWO()
        {
        }
        
        public itemDataLineTypeLinkUriGWO(itemDataLineTypeLinkUriFormat format, string value)
        {
            this.format = format;
            this.value = value;
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute(AttributeName="format")]
        public itemDataLineTypeLinkUriFormat Format
        {
            get
            {
                return this.format;
            }
            set
            {
                if ((this.format != value))
                {
                    this.format = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlTextAttribute(DataType="anyURI")]
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