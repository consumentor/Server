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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.gepir.org/", TypeName="partyContactType")]
    [System.ComponentModel.TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
    public partial class partyContactTypeGWO
    {
        
        /// <remarks/>
        private string contactName;
        
        /// <remarks/>
        private System.Collections.Generic.List<CommunicationChannelTypeGWO> communicationChannel;
        
        public partyContactTypeGWO()
        {
        }
        
        public partyContactTypeGWO(string contactName, System.Collections.Generic.List<CommunicationChannelTypeGWO> communicationChannel)
        {
            this.contactName = contactName;
            this.communicationChannel = communicationChannel;
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="contactName")]
        public string ContactName
        {
            get
            {
                return this.contactName;
            }
            set
            {
                if ((this.contactName != value))
                {
                    this.contactName = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute("communicationChannel")]
        public System.Collections.Generic.List<CommunicationChannelTypeGWO> CommunicationChannel
        {
            get
            {
                return this.communicationChannel;
            }
            set
            {
                if ((this.communicationChannel != value))
                {
                    this.communicationChannel = value;
                }
            }
        }
    }
}
