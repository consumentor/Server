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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.gepir.org/", TypeName="GetStatistics")]
    [System.ComponentModel.TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
    public partial class GetStatisticsGWO
    {
        
        /// <remarks/>
        private string requestedCountry;
        
        /// <remarks/>
        private System.DateTime periodBegin;
        
        /// <remarks/>
        private System.DateTime periodEnd;
        
        /// <remarks/>
        private decimal version;
        
        /// <remarks/>
        private bool versionSpecified;
        
        public GetStatisticsGWO()
        {
        }
        
        public GetStatisticsGWO(string requestedCountry, System.DateTime periodBegin, System.DateTime periodEnd, decimal version, bool versionSpecified)
        {
            this.requestedCountry = requestedCountry;
            this.periodBegin = periodBegin;
            this.periodEnd = periodEnd;
            this.version = version;
            this.versionSpecified = versionSpecified;
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
        
        [System.Xml.Serialization.XmlElementAttribute(DataType="date", ElementName="periodBegin")]
        public System.DateTime PeriodBegin
        {
            get
            {
                return this.periodBegin;
            }
            set
            {
                if ((this.periodBegin != value))
                {
                    this.periodBegin = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(DataType="date", ElementName="periodEnd")]
        public System.DateTime PeriodEnd
        {
            get
            {
                return this.periodEnd;
            }
            set
            {
                if ((this.periodEnd != value))
                {
                    this.periodEnd = value;
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
