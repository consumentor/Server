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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.gepir.org/", TypeName="GetOwnerOfGLN")]
    [System.ComponentModel.TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
    public partial class GetOwnerOfGLNGWO
    {
        
        /// <remarks/>
        private System.Collections.Generic.List<string> requestedGln;
        
        /// <remarks/>
        private System.Collections.Generic.List<string> requestedLanguages;
        
        /// <remarks/>
        private decimal version;
        
        /// <remarks/>
        private bool versionSpecified;
        
        public GetOwnerOfGLNGWO()
        {
        }
        
        public GetOwnerOfGLNGWO(System.Collections.Generic.List<string> requestedGln, System.Collections.Generic.List<string> requestedLanguages, decimal version, bool versionSpecified)
        {
            this.requestedGln = requestedGln;
            this.requestedLanguages = requestedLanguages;
            this.version = version;
            this.versionSpecified = versionSpecified;
        }
        
        [System.Xml.Serialization.XmlElementAttribute("requestedGln")]
        public System.Collections.Generic.List<string> RequestedGln
        {
            get
            {
                return this.requestedGln;
            }
            set
            {
                if ((this.requestedGln != value))
                {
                    this.requestedGln = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlArrayItemAttribute(DataType="language", IsNullable=false)]
        [System.Xml.Serialization.XmlArrayAttribute(ElementName="requestedLanguages")]
        public System.Collections.Generic.List<string> RequestedLanguages
        {
            get
            {
                return this.requestedLanguages;
            }
            set
            {
                if ((this.requestedLanguages != value))
                {
                    this.requestedLanguages = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute(AttributeName="version")]
        public decimal Version
        {
            get
            {
                return this.version;
            }
            set
            {
                if ((this.version != value))
                {
                    this.version = value;
                    this.versionSpecified = true;
                }
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool VersionSpecified
        {
            get
            {
                return this.versionSpecified;
            }
            set
            {
                if ((this.versionSpecified != value))
                {
                    this.versionSpecified = value;
                }
            }
        }
    }
}
