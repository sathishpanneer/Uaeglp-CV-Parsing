using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.Services.WebService
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.7.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace = "http://xmlns.moi.gov.ae/2019/InquiryServices/CitizenVerification", ConfigurationName = "WebService.CitizenVerification_ptt")]
    public interface CitizenVerification_ptt
    {

        // CODEGEN: Generating message contract since the operation CitizenVerification is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action = "CitizenVerification", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        Uaeglp.Services.WebService.CitizenVerificationResponse1 CitizenVerification(Uaeglp.Services.WebService.CitizenVerificationRequest1 request);

        [System.ServiceModel.OperationContractAttribute(Action = "CitizenVerification", ReplyAction = "*")]
        System.Threading.Tasks.Task<Uaeglp.Services.WebService.CitizenVerificationResponse1> CitizenVerificationAsync(Uaeglp.Services.WebService.CitizenVerificationRequest1 request);
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.moi.gov.ae/2019/InquiryServices/CitizenVerificationSchema")]
    public partial class CitizenVerificationRequest : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string emiratesIDNumberField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string EmiratesIDNumber
        {
            get
            {
                return this.emiratesIDNumberField;
            }
            set
            {
                this.emiratesIDNumberField = value;
                this.RaisePropertyChanged("EmiratesIDNumber");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.moi.gov.ae/2019/InquiryServices/CitizenVerificationSchema")]
    public partial class CitizenVerificationResponse : object, System.ComponentModel.INotifyPropertyChanged
    {

        private ResultType verificationResultField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public ResultType VerificationResult
        {
            get
            {
                return this.verificationResultField;
            }
            set
            {
                this.verificationResultField = value;
                this.RaisePropertyChanged("VerificationResult");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.moi.gov.ae/2019/InquiryServices/CitizenVerificationSchema")]
    public enum ResultType
    {

        /// <remarks/>
        Eligible,

        /// <remarks/>
        NotEligible,

        /// <remarks/>
        NoDataFound,
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.7.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
    public partial class CitizenVerificationRequest1
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://xmlns.moi.gov.ae/2019/InquiryServices/CitizenVerificationSchema", Order = 0)]
        public Uaeglp.Services.WebService.CitizenVerificationRequest CitizenVerificationRequest;

        public CitizenVerificationRequest1()
        {
        }

        public CitizenVerificationRequest1(Uaeglp.Services.WebService.CitizenVerificationRequest CitizenVerificationRequest)
        {
            this.CitizenVerificationRequest = CitizenVerificationRequest;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.7.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
    public partial class CitizenVerificationResponse1
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://xmlns.moi.gov.ae/2019/InquiryServices/CitizenVerificationSchema", Order = 0)]
        public Uaeglp.Services.WebService.CitizenVerificationResponse CitizenVerificationResponse;

        public CitizenVerificationResponse1()
        {
        }

        public CitizenVerificationResponse1(Uaeglp.Services.WebService.CitizenVerificationResponse CitizenVerificationResponse)
        {
            this.CitizenVerificationResponse = CitizenVerificationResponse;
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.7.0.0")]
    public interface CitizenVerification_pttChannel : Uaeglp.Services.WebService.CitizenVerification_ptt, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.7.0.0")]
    public partial class CitizenVerification_pttClient : System.ServiceModel.ClientBase<Uaeglp.Services.WebService.CitizenVerification_ptt>, Uaeglp.Services.WebService.CitizenVerification_ptt
    {

        public CitizenVerification_pttClient()
        {
        }

        public CitizenVerification_pttClient(string endpointConfigurationName) :
                base(endpointConfigurationName)
        {
        }

        public CitizenVerification_pttClient(string endpointConfigurationName, string remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public CitizenVerification_pttClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public CitizenVerification_pttClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
        {
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Uaeglp.Services.WebService.CitizenVerificationResponse1 Uaeglp.Services.WebService.CitizenVerification_ptt.CitizenVerification(Uaeglp.Services.WebService.CitizenVerificationRequest1 request)
        {
            return base.Channel.CitizenVerification(request);
        }

        public Uaeglp.Services.WebService.CitizenVerificationResponse CitizenVerification(Uaeglp.Services.WebService.CitizenVerificationRequest CitizenVerificationRequest)
        {
            Uaeglp.Services.WebService.CitizenVerificationRequest1 inValue = new Uaeglp.Services.WebService.CitizenVerificationRequest1();
            inValue.CitizenVerificationRequest = CitizenVerificationRequest;
            Uaeglp.Services.WebService.CitizenVerificationResponse1 retVal = ((Uaeglp.Services.WebService.CitizenVerification_ptt)(this)).CitizenVerification(inValue);
            return retVal.CitizenVerificationResponse;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Uaeglp.Services.WebService.CitizenVerificationResponse1> Uaeglp.Services.WebService.CitizenVerification_ptt.CitizenVerificationAsync(Uaeglp.Services.WebService.CitizenVerificationRequest1 request)
        {
            return base.Channel.CitizenVerificationAsync(request);
        }

        public System.Threading.Tasks.Task<Uaeglp.Services.WebService.CitizenVerificationResponse1> CitizenVerificationAsync(Uaeglp.Services.WebService.CitizenVerificationRequest CitizenVerificationRequest)
        {
            Uaeglp.Services.WebService.CitizenVerificationRequest1 inValue = new Uaeglp.Services.WebService.CitizenVerificationRequest1();
            inValue.CitizenVerificationRequest = CitizenVerificationRequest;
            return ((Uaeglp.Services.WebService.CitizenVerification_ptt)(this)).CitizenVerificationAsync(inValue);
        }
    }
}
