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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.gepir.org/", TypeName="partyDataLineType")]
    [System.ComponentModel.TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
    public partial class partyDataLineTypeGWO
    {
        
        /// <remarks/>
        private string gln;
        
        /// <remarks/>
        private string returnCode;
        
        /// <remarks/>
        private string informationProviderGln;
        
        /// <remarks/>
        private System.DateTime lastChangeDateTime;
        
        /// <remarks/>
        private bool lastChangeDateTimeSpecified;
        
        /// <remarks/>
        private string gcp;
        
        /// <remarks/>
        private string additionalPartyId;
        
        /// <remarks/>
        private string partyName;
        
        /// <remarks/>
        private System.Collections.Generic.List<string> streetAddress;
        
        /// <remarks/>
        private string pOBoxNumber;
        
        /// <remarks/>
        private string subDivision;
        
        /// <remarks/>
        private string postalCode;
        
        /// <remarks/>
        private string city;
        
        /// <remarks/>
        private string countryISOCode;
        
        /// <remarks/>
        private System.Collections.Generic.List<partyContactTypeGWO> contact;
        
        /// <remarks/>
        private partyRoleListType partyRole;
        
        /// <remarks/>
        private bool partyRoleSpecified;
        
        /// <remarks/>
        private System.Collections.Generic.List<partyChildTypeGWO> partyContainment;
        
        /// <remarks/>
        private string lang;
        
        public partyDataLineTypeGWO()
        {
        }
        
        public partyDataLineTypeGWO(
                    string gln, 
                    string returnCode, 
                    string informationProviderGln, 
                    System.DateTime lastChangeDateTime, 
                    bool lastChangeDateTimeSpecified, 
                    string gcp, 
                    string additionalPartyId, 
                    string partyName, 
                    System.Collections.Generic.List<string> streetAddress, 
                    string pOBoxNumber, 
                    string subDivision, 
                    string postalCode, 
                    string city, 
                    string countryISOCode, 
                    System.Collections.Generic.List<partyContactTypeGWO> contact, 
                    partyRoleListType partyRole, 
                    bool partyRoleSpecified, 
                    System.Collections.Generic.List<partyChildTypeGWO> partyContainment, 
                    string lang)
        {
            this.gln = gln;
            this.returnCode = returnCode;
            this.informationProviderGln = informationProviderGln;
            this.lastChangeDateTime = lastChangeDateTime;
            this.lastChangeDateTimeSpecified = lastChangeDateTimeSpecified;
            this.gcp = gcp;
            this.additionalPartyId = additionalPartyId;
            this.partyName = partyName;
            this.streetAddress = streetAddress;
            this.pOBoxNumber = pOBoxNumber;
            this.subDivision = subDivision;
            this.postalCode = postalCode;
            this.city = city;
            this.countryISOCode = countryISOCode;
            this.contact = contact;
            this.partyRole = partyRole;
            this.partyRoleSpecified = partyRoleSpecified;
            this.partyContainment = partyContainment;
            this.lang = lang;
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="gln")]
        public string Gln
        {
            get
            {
                return this.gln;
            }
            set
            {
                if ((this.gln != value))
                {
                    this.gln = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(DataType="nonNegativeInteger", ElementName="returnCode")]
        public string ReturnCode
        {
            get
            {
                return this.returnCode;
            }
            set
            {
                if ((this.returnCode != value))
                {
                    this.returnCode = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="informationProviderGln")]
        public string InformationProviderGln
        {
            get
            {
                return this.informationProviderGln;
            }
            set
            {
                if ((this.informationProviderGln != value))
                {
                    this.informationProviderGln = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="lastChangeDateTime")]
        public System.DateTime LastChangeDateTime
        {
            get
            {
                return this.lastChangeDateTime;
            }
            set
            {
                if ((this.lastChangeDateTime != value))
                {
                    this.lastChangeDateTime = value;
                    this.lastChangeDateTimeSpecified = true;
                }
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LastChangeDateTimeSpecified
        {
            get
            {
                return this.lastChangeDateTimeSpecified;
            }
            set
            {
                if ((this.lastChangeDateTimeSpecified != value))
                {
                    this.lastChangeDateTimeSpecified = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="gcp")]
        public string Gcp
        {
            get
            {
                return this.gcp;
            }
            set
            {
                if ((this.gcp != value))
                {
                    this.gcp = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="additionalPartyId")]
        public string AdditionalPartyId
        {
            get
            {
                return this.additionalPartyId;
            }
            set
            {
                if ((this.additionalPartyId != value))
                {
                    this.additionalPartyId = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="partyName")]
        public string PartyName
        {
            get
            {
                return this.partyName;
            }
            set
            {
                if ((this.partyName != value))
                {
                    this.partyName = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute("streetAddress")]
        public System.Collections.Generic.List<string> StreetAddress
        {
            get
            {
                return this.streetAddress;
            }
            set
            {
                if ((this.streetAddress != value))
                {
                    this.streetAddress = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="pOBoxNumber")]
        public string POBoxNumber
        {
            get
            {
                return this.pOBoxNumber;
            }
            set
            {
                if ((this.pOBoxNumber != value))
                {
                    this.pOBoxNumber = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="subDivision")]
        public string SubDivision
        {
            get
            {
                return this.subDivision;
            }
            set
            {
                if ((this.subDivision != value))
                {
                    this.subDivision = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="postalCode")]
        public string PostalCode
        {
            get
            {
                return this.postalCode;
            }
            set
            {
                if ((this.postalCode != value))
                {
                    this.postalCode = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="city")]
        public string City
        {
            get
            {
                return this.city;
            }
            set
            {
                if ((this.city != value))
                {
                    this.city = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="countryISOCode")]
        public string CountryISOCode
        {
            get
            {
                return this.countryISOCode;
            }
            set
            {
                if ((this.countryISOCode != value))
                {
                    this.countryISOCode = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute("contact")]
        public System.Collections.Generic.List<partyContactTypeGWO> Contact
        {
            get
            {
                return this.contact;
            }
            set
            {
                if ((this.contact != value))
                {
                    this.contact = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="partyRole")]
        public partyRoleListType PartyRole
        {
            get
            {
                return this.partyRole;
            }
            set
            {
                if ((this.partyRole != value))
                {
                    this.partyRole = value;
                    this.partyRoleSpecified = true;
                }
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PartyRoleSpecified
        {
            get
            {
                return this.partyRoleSpecified;
            }
            set
            {
                if ((this.partyRoleSpecified != value))
                {
                    this.partyRoleSpecified = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlArrayItemAttribute("partyChild", IsNullable=false)]
        [System.Xml.Serialization.XmlArrayAttribute(ElementName="partyContainment")]
        public System.Collections.Generic.List<partyChildTypeGWO> PartyContainment
        {
            get
            {
                return this.partyContainment;
            }
            set
            {
                if ((this.partyContainment != value))
                {
                    this.partyContainment = value;
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
