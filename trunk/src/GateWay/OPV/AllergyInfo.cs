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


namespace Consumentor.ShopGun.Gateway.Opv
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.mediabanken.se/", TypeName="AllergyInfo")]
    [System.ComponentModel.TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
    public partial class AllergyInfoGWO
    {
        
        /// <remarks/>
        private string substanceName;
        
        /// <remarks/>
        private ContainsAllergenic contains;
        
        /// <remarks/>
        private string remark;
        
        public AllergyInfoGWO()
        {
        }
        
        public AllergyInfoGWO(string substanceName, ContainsAllergenic contains, string remark)
        {
            this.substanceName = substanceName;
            this.contains = contains;
            this.remark = remark;
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="SubstanceName")]
        public string SubstanceName
        {
            get
            {
                return this.substanceName;
            }
            set
            {
                if ((this.substanceName != value))
                {
                    this.substanceName = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="Contains")]
        public ContainsAllergenic Contains
        {
            get
            {
                return this.contains;
            }
            set
            {
                if ((this.contains != value))
                {
                    this.contains = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="Remark")]
        public string Remark
        {
            get
            {
                return this.remark;
            }
            set
            {
                if ((this.remark != value))
                {
                    this.remark = value;
                }
            }
        }
    }
}
