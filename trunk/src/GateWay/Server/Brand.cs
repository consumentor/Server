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


namespace Consumentor.ShopGun.Gateway.Server
{
    using System.Diagnostics;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wscf", "0.7.6319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://consumentor.com/ShopGunDomain", TypeName="Brand")]
    [System.ComponentModel.TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
    public partial class BrandGWO
    {
        
        /// <remarks/>
        private string brandName;
        
        public BrandGWO()
        {
        }
        
        public BrandGWO(string brandName)
        {
            this.brandName = brandName;
        }
        
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, ElementName="BrandName")]
        public string BrandName
        {
            get
            {
                return this.brandName;
            }
            set
            {
                if ((this.brandName != value))
                {
                    this.brandName = value;
                }
            }
        }
    }
}
