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
    [System.Web.Services.WebServiceBindingAttribute(Name="ProductServiceSoap", Namespace="http://www.mediabanken.se/")]
    [System.ComponentModel.TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
    public partial class ProductSearchWebServiceGateway : Consumentor.ShopGun.Gateway.WebServiceBase, IProductSearchWebServiceGateway
    {
        
        private AuthHeaderGWO authHeaderValue;
        
        private System.Threading.SendOrPostCallback TestAuthorizationOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetProductDataOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetProductListDataOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetAllAvailableEANsOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetChangedProductsEANsOperationCompleted;
        
        /// <remarks/>
        public ProductSearchWebServiceGateway()
        {
            
        }
        
        [System.Xml.Serialization.XmlElementAttribute(ElementName="AuthHeaderValue")]
        public AuthHeaderGWO AuthHeaderValue
        {
            get
            {
                return this.authHeaderValue;
            }
            set
            {
                if ((value == null))
                {
                    throw new System.ArgumentNullException("AuthHeaderValue");
                }
                if ((this.authHeaderValue != value))
                {
                    this.authHeaderValue = value;
                }
            }
        }
        
        /// <remarks/>
        public event TestAuthorizationCompletedEventHandler TestAuthorizationCompleted;
        
        /// <remarks/>
        public event GetProductDataCompletedEventHandler GetProductDataCompleted;
        
        /// <remarks/>
        public event GetProductListDataCompletedEventHandler GetProductListDataCompleted;
        
        /// <remarks/>
        public event GetAllAvailableEANsCompletedEventHandler GetAllAvailableEANsCompleted;
        
        /// <remarks/>
        public event GetChangedProductsEANsCompletedEventHandler GetChangedProductsEANsCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapHeaderAttribute("AuthHeaderValue")]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.mediabanken.se/TestAuthorization", RequestNamespace="http://www.mediabanken.se/", ResponseNamespace="http://www.mediabanken.se/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string TestAuthorization()
        {
            object[] results = this.Invoke("TestAuthorization", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void TestAuthorizationAsync()
        {
            this.TestAuthorizationAsync(null);
        }
        
        /// <remarks/>
        public void TestAuthorizationAsync(object userState)
        {
            if ((this.TestAuthorizationOperationCompleted == null))
            {
                this.TestAuthorizationOperationCompleted = new System.Threading.SendOrPostCallback(this.OnTestAuthorizationOperationCompleted);
            }
            this.InvokeAsync("TestAuthorization", new object[0], this.TestAuthorizationOperationCompleted, userState);
        }
        
        private void OnTestAuthorizationOperationCompleted(object arg)
        {
            if ((this.TestAuthorizationCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.TestAuthorizationCompleted(this, new TestAuthorizationCompletedEventArgsGWO(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapHeaderAttribute("AuthHeaderValue")]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.mediabanken.se/GetProductData", RequestNamespace="http://www.mediabanken.se/", ResponseNamespace="http://www.mediabanken.se/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public ProductGWO[] GetProductData([System.Xml.Serialization.XmlArrayAttribute(ElementName="eanVec")] string[] eanVec)
        {
            object[] results = this.Invoke("GetProductData", new object[] {
                        eanVec});
            return ((ProductGWO[])(results[0]));
        }
        
        /// <remarks/>
        public void GetProductDataAsync(string[] eanVec)
        {
            this.GetProductDataAsync(eanVec, null);
        }
        
        /// <remarks/>
        public void GetProductDataAsync(string[] eanVec, object userState)
        {
            if ((this.GetProductDataOperationCompleted == null))
            {
                this.GetProductDataOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetProductDataOperationCompleted);
            }
            this.InvokeAsync("GetProductData", new object[] {
                        eanVec}, this.GetProductDataOperationCompleted, userState);
        }
        
        private void OnGetProductDataOperationCompleted(object arg)
        {
            if ((this.GetProductDataCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetProductDataCompleted(this, new GetProductDataCompletedEventArgsGWO(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapHeaderAttribute("AuthHeaderValue")]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.mediabanken.se/GetProductListData", RequestNamespace="http://www.mediabanken.se/", ResponseNamespace="http://www.mediabanken.se/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public ListProductGWO[] GetProductListData([System.Xml.Serialization.XmlArrayAttribute(ElementName="eanVec")] string[] eanVec)
        {
            object[] results = this.Invoke("GetProductListData", new object[] {
                        eanVec});
            return ((ListProductGWO[])(results[0]));
        }
        
        /// <remarks/>
        public void GetProductListDataAsync(string[] eanVec)
        {
            this.GetProductListDataAsync(eanVec, null);
        }
        
        /// <remarks/>
        public void GetProductListDataAsync(string[] eanVec, object userState)
        {
            if ((this.GetProductListDataOperationCompleted == null))
            {
                this.GetProductListDataOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetProductListDataOperationCompleted);
            }
            this.InvokeAsync("GetProductListData", new object[] {
                        eanVec}, this.GetProductListDataOperationCompleted, userState);
        }
        
        private void OnGetProductListDataOperationCompleted(object arg)
        {
            if ((this.GetProductListDataCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetProductListDataCompleted(this, new GetProductListDataCompletedEventArgsGWO(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapHeaderAttribute("AuthHeaderValue")]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.mediabanken.se/GetAllAvailableEANs", RequestNamespace="http://www.mediabanken.se/", ResponseNamespace="http://www.mediabanken.se/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string[] GetAllAvailableEANs()
        {
            object[] results = this.Invoke("GetAllAvailableEANs", new object[0]);
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public void GetAllAvailableEANsAsync()
        {
            this.GetAllAvailableEANsAsync(null);
        }
        
        /// <remarks/>
        public void GetAllAvailableEANsAsync(object userState)
        {
            if ((this.GetAllAvailableEANsOperationCompleted == null))
            {
                this.GetAllAvailableEANsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetAllAvailableEANsOperationCompleted);
            }
            this.InvokeAsync("GetAllAvailableEANs", new object[0], this.GetAllAvailableEANsOperationCompleted, userState);
        }
        
        private void OnGetAllAvailableEANsOperationCompleted(object arg)
        {
            if ((this.GetAllAvailableEANsCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetAllAvailableEANsCompleted(this, new GetAllAvailableEANsCompletedEventArgsGWO(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapHeaderAttribute("AuthHeaderValue")]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.mediabanken.se/GetChangedProductsEANs", RequestNamespace="http://www.mediabanken.se/", ResponseNamespace="http://www.mediabanken.se/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string[] GetChangedProductsEANs([System.Xml.Serialization.XmlElementAttribute(ElementName="changedAfter")] System.DateTime changedAfter)
        {
            object[] results = this.Invoke("GetChangedProductsEANs", new object[] {
                        changedAfter});
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public void GetChangedProductsEANsAsync(System.DateTime changedAfter)
        {
            this.GetChangedProductsEANsAsync(changedAfter, null);
        }
        
        /// <remarks/>
        public void GetChangedProductsEANsAsync(System.DateTime changedAfter, object userState)
        {
            if ((this.GetChangedProductsEANsOperationCompleted == null))
            {
                this.GetChangedProductsEANsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetChangedProductsEANsOperationCompleted);
            }
            this.InvokeAsync("GetChangedProductsEANs", new object[] {
                        changedAfter}, this.GetChangedProductsEANsOperationCompleted, userState);
        }
        
        private void OnGetChangedProductsEANsOperationCompleted(object arg)
        {
            if ((this.GetChangedProductsEANsCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetChangedProductsEANsCompleted(this, new GetChangedProductsEANsCompletedEventArgsGWO(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }
    }
}