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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.gepir.org/", TypeName="itemDataLineType")]
    [System.ComponentModel.TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
    public partial class itemDataLineTypeGWO
    {
        
        /// <remarks/>
        private string gtin;
        
        /// <remarks/>
        private string informationProviderGln;
        
        /// <remarks/>
        private string manufacturerGln;
        
        /// <remarks/>
        private string itemName;
        
        /// <remarks/>
        private string brandName;
        
        /// <remarks/>
        private itemDataLineTypeTradeItemUnitDescriptor tradeItemUnitDescriptor;
        
        /// <remarks/>
        private string descriptiveSize;
        
        /// <remarks/>
        private itemDataLineTypeNetContentGWO netContent;
        
        /// <remarks/>
        private System.Collections.Generic.List<itemDataLineTypeLinkUriGWO> linkUri;
        
        /// <remarks/>
        private System.Collections.Generic.List<classificationCodeTypeGWO> classificationCode;
        
        /// <remarks/>
        private System.DateTime lastChangeDateTime;
        
        /// <remarks/>
        private bool lastChangeDateTimeSpecified;
        
        /// <remarks/>
        private System.Collections.Generic.List<itemDataLineTypeChildItemsChildItemGWO> childItems;
        
        /// <remarks/>
        private string lang;
        
        public itemDataLineTypeGWO()
        {
        }
        
        public itemDataLineTypeGWO(string gtin, string informationProviderGln, string manufacturerGln, string itemName, string brandName, itemDataLineTypeTradeItemUnitDescriptor tradeItemUnitDescriptor, string descriptiveSize, itemDataLineTypeNetContentGWO netContent, System.Collections.Generic.List<itemDataLineTypeLinkUriGWO> linkUri, System.Collections.Generic.List<classificationCodeTypeGWO> classificationCode, System.DateTime lastChangeDateTime, bool lastChangeDateTimeSpecified, System.Collections.Generic.List<itemDataLineTypeChildItemsChildItemGWO> childItems, string lang)
        {
            this.gtin = gtin;
            this.informationProviderGln = informationProviderGln;
            this.manufacturerGln = manufacturerGln;
            this.itemName = itemName;
            this.brandName = brandName;
            this.tradeItemUnitDescriptor = tradeItemUnitDescriptor;
            this.descriptiveSize = descriptiveSize;
            this.netContent = netContent;
            this.linkUri = linkUri;
            this.classificationCode = classificationCode;
            this.lastChangeDateTime = lastChangeDateTime;
            this.lastChangeDateTimeSpecified = lastChangeDateTimeSpecified;
            this.childItems = childItems;
            this.lang = lang;
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="gtin")]
        public string Gtin
        {
            get
            {
                return this.gtin;
            }
            set
            {
                if ((this.gtin != value))
                {
                    this.gtin = value;
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
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="manufacturerGln")]
        public string ManufacturerGln
        {
            get
            {
                return this.manufacturerGln;
            }
            set
            {
                if ((this.manufacturerGln != value))
                {
                    this.manufacturerGln = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="itemName")]
        public string ItemName
        {
            get
            {
                return this.itemName;
            }
            set
            {
                if ((this.itemName != value))
                {
                    this.itemName = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="brandName")]
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
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="tradeItemUnitDescriptor")]
        public itemDataLineTypeTradeItemUnitDescriptor TradeItemUnitDescriptor
        {
            get
            {
                return this.tradeItemUnitDescriptor;
            }
            set
            {
                if ((this.tradeItemUnitDescriptor != value))
                {
                    this.tradeItemUnitDescriptor = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="descriptiveSize")]
        public string DescriptiveSize
        {
            get
            {
                return this.descriptiveSize;
            }
            set
            {
                if ((this.descriptiveSize != value))
                {
                    this.descriptiveSize = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="netContent")]
        public itemDataLineTypeNetContentGWO NetContent
        {
            get
            {
                return this.netContent;
            }
            set
            {
                if ((value == null))
                {
                    throw new System.ArgumentNullException("NetContent");
                }
                if ((this.netContent != value))
                {
                    this.netContent = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute("linkUri")]
        public System.Collections.Generic.List<itemDataLineTypeLinkUriGWO> LinkUri
        {
            get
            {
                return this.linkUri;
            }
            set
            {
                if ((this.linkUri != value))
                {
                    this.linkUri = value;
                }
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute("classificationCode")]
        public System.Collections.Generic.List<classificationCodeTypeGWO> ClassificationCode
        {
            get
            {
                return this.classificationCode;
            }
            set
            {
                if ((this.classificationCode != value))
                {
                    this.classificationCode = value;
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
        
        [System.Xml.Serialization.XmlArrayItemAttribute("childItem", IsNullable=false)]
        [System.Xml.Serialization.XmlArrayAttribute(ElementName="childItems")]
        public System.Collections.Generic.List<itemDataLineTypeChildItemsChildItemGWO> ChildItems
        {
            get
            {
                return this.childItems;
            }
            set
            {
                if ((this.childItems != value))
                {
                    this.childItems = value;
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
