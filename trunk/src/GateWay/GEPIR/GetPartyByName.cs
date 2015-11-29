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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.gepir.org/", TypeName="GetPartyByName")]
    [System.ComponentModel.TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
    public partial class GetPartyByNameGWO
    {
        
        /// <remarks/>
        private string requestedPartyName;
        
        /// <remarks/>
        private string requestedStreetAddress;
        
        /// <remarks/>
        private string requestedPostalCode;
        
        /// <remarks/>
        private string requestedCity;
        
        /// <remarks/>
        private string requestedCountry;
        
        /// <remarks/>
        private System.Collections.Generic.List<string> requestedKeywords;
        
        /// <remarks/>
        private System.Collections.Generic.List<string> requestedLanguages;
        
        /// <remarks/>
        private decimal version;
        
        /// <remarks/>
        private bool versionSpecified;
        
        /// <remarks/>
        private string lang;
        
        public GetPartyByNameGWO()
        {
        }
        
        public GetPartyByNameGWO(string requestedPartyName, string requestedStreetAddress, string requestedPostalCode, string requestedCity, string requestedCountry, System.Collections.Generic.List<string> requestedKeywords, System.Collections.Generic.List<string> requestedLanguages, decimal version, bool versionSpecified, string lang)
        {
            this.requestedPartyName = requestedPartyName;
            this.requestedStreetAddress = requestedStreetAddress;
            this.requestedPostalCode = requestedPostalCode;
            this.requestedCity = requestedCity;
            this.requestedCountry = requestedCountry;
            this.requestedKeywords = requestedKeywords;
            this.requestedLanguages = requestedLanguages;
            this.version = version;
            this.versionSpecified = versionSpecified;
            this.lang = lang;
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="requestedPartyName")]
        public string RequestedPartyName
        {
            get
            {
                return this.requestedPartyName;
            }
            set
            {
                if ((this.requestedPartyName != value))
                {
                    this.requestedPartyName = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="requestedStreetAddress")]
        public string RequestedStreetAddress
        {
            get
            {
                return this.requestedStreetAddress;
            }
            set
            {
                if ((this.requestedStreetAddress != value))
                {
                    this.requestedStreetAddress = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="requestedPostalCode")]
        public string RequestedPostalCode
        {
            get
            {
                return this.requestedPostalCode;
            }
            set
            {
                if ((this.requestedPostalCode != value))
                {
                    this.requestedPostalCode = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="requestedCity")]
        public string RequestedCity
        {
            get
            {
                return this.requestedCity;
            }
            set
            {
                if ((this.requestedCity != value))
                {
                    this.requestedCity = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="requestedCountry")]
        public string RequestedCountry
        {
            get
            {
                return this.requestedCountry;
            }
            set
            {
                if ((this.requestedCountry != value))
                {
                    this.requestedCountry = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlArrayItemAttribute("keyword", IsNullable=false)]
        [System.Xml.Serialization.XmlArrayAttribute(ElementName="requestedKeywords")]
        public System.Collections.Generic.List<string> RequestedKeywords
        {
            get
            {
                return this.requestedKeywords;
            }
            set
            {
                if ((this.requestedKeywords != value))
                {
                    this.requestedKeywords = value;
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
        
        [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified, Namespace="http://www.w3.org/XML/1998/namespace", AttributeName="lang")]
        public string Lang
        {
            get
            {
                return this.lang;
            }
            set
            {
                if ((this.lang != value))
                {
                    this.lang = value;
                }
            }
        }
    }
}
