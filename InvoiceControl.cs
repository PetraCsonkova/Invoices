using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using TietoEnator.WebFW;
using TietoEnator.WebFW.CM;
using TietoEnator.WebFW.CM.MessageHandling;



namespace TietoEnator.WebFW.CM.Controls
{
    // 20100311 LKo: Enumeration for the bank account formats
    public enum BankAccountFormat
    {
        Invalid,
        National,
        IBAN,
        Both
    }

    public enum eTempAddTypes
    {
        InvoiceAddress = 1,
        ReminderAddress = 2,
        DebtCollectionAddress = 3
    }

    // 20100226 LKo: Data class for the invoice configuration fetch parameters
    public class InvoiceConfigurationInputVO : ControlBase
    {
        // Invoice configuration ident
        private int? m_iInvConfId;
        public int? InvConfId { get { return m_iInvConfId; } set { m_iInvConfId = value; } }

        // Customer code
        private string m_sCustomerCode;
        public string CustomerCode { get { return m_sCustomerCode; } set { m_sCustomerCode = value; } }

        // Valid configuration date (???)
        private DateTime? m_dtValidConfigurationDate;
        public DateTime? ValidConfigurationDate { get { return m_dtValidConfigurationDate; } set { m_dtValidConfigurationDate = value; } }
    }

    // 20100226 LKo: Data class for the invoice configuration fetch results
    public class InvoiceConfigurationOutputVO : ControlBase
    {
        // Invoice configuration number
        private int? m_iInvConfNo;
        public int? InvConfNo { get { return m_iInvConfNo; } set { m_iInvConfNo = value; } }

        // PayTermId
        private int? m_iPayTermId;
        public int? PayTermId { get { return m_iPayTermId; } set { m_iPayTermId = value; } }

        // PayTermName
        private string m_sPayTermName;
        public string PayTermName { get { return m_sPayTermName; } set { m_sPayTermName = value; } }

        // PeriodTypeCode
        private string m_sPeriodTypeCode;
        public string PeriodTypeCode { get { return m_sPeriodTypeCode; } set { m_sPeriodTypeCode = value; } }

        // PeriodTypeDesc
        private string m_sPeriodTypeDesc;
        public string PeriodTypeDesc { get { return m_sPeriodTypeDesc; } set { m_sPeriodTypeDesc = value; } }

        // InvoiceGroupId
        private int? m_iInvoiceGroupId;
        public int? InvoiceGroupId { get { return m_iInvoiceGroupId; } set { m_iInvoiceGroupId = value; } }

        // InvoiceGroupName
        private string m_sInvoiceGroupName;
        public string InvoiceGroupName { get { return m_sInvoiceGroupName; } set { m_sInvoiceGroupName = value; } }

        // InvoicePeriodRuleId
        private int? m_iInvoicePeriodRuleId;
        public int? InvoicePeriodRuleId { get { return m_iInvoicePeriodRuleId; } set { m_iInvoicePeriodRuleId = value; } }

        // InvoicePeriodRuleName
        private string m_sInvoicePeriodRuleName;
        public string InvoicePeriodRuleName { get { return m_sInvoicePeriodRuleName; } set { m_sInvoicePeriodRuleName = value; } }

        // InvoiceSendMethod
        private int? m_iInvoiceSendMethod;
        public int? InvoiceSendMethod { get { return m_iInvoiceSendMethod; } set { m_iInvoiceSendMethod = value; } }

        // IsAdvanceNoticeCalc
        private int? m_iIsAdvanceNoticeCalc;
        public int? IsAdvanceNoticeCalc { get { return m_iIsAdvanceNoticeCalc; } set { m_iIsAdvanceNoticeCalc = value; } }

        // Invoicing address
        private InvConfAddressVO m_oInvoicingAddress;
        public InvConfAddressVO InvoicingAddress { get { return m_oInvoicingAddress; } set { m_oInvoicingAddress = value; } }

        // Reminder address
        private InvConfAddressVO m_oReminderAddress;
        public InvConfAddressVO ReminderAddress { get { return m_oReminderAddress; } set { m_oReminderAddress = value; } }

        // Debt collection address
        private InvConfAddressVO m_oDebtCollectionAddress;
        public InvConfAddressVO DebtCollectionAddress { get { return m_oDebtCollectionAddress; } set { m_oDebtCollectionAddress = value; } }

        // List for temporary addresses
        private ControlList<TemporaryAddressVO> m_lTemporaryAddressList = null;
        public ControlList<TemporaryAddressVO> TemporaryAddressList { get { return m_lTemporaryAddressList; } set { m_lTemporaryAddressList = value; } }

        // SuggestedConfiguration
        private int? m_iSuggestedConfiguration;
        public int? SuggestedConfiguration { get { return m_iSuggestedConfiguration; } set { m_iSuggestedConfiguration = value; } }

        public static explicit operator UpdateInvoiceConfigurationDataVO(InvoiceConfigurationOutputVO conf)
        {
            UpdateInvoiceConfigurationDataVO result = new UpdateInvoiceConfigurationDataVO();

            result.InvConfNo = conf.InvConfNo;
            result.PayTermId = conf.PayTermId;
            result.InvoicePeriodId = conf.InvoicePeriodRuleId;
            result.InvoiceGroupId = conf.InvoiceGroupId;
            result.InvoiceSendMethod = conf.InvoiceSendMethod;
            result.IsAdvanceNoticeCalc = conf.IsAdvanceNoticeCalc;
            result.InvoicingAddress = conf.InvoicingAddress;
            result.ReminderAddress = conf.ReminderAddress;
            result.DebtCollectionAddress = conf.DebtCollectionAddress;
            result.TemporaryAddressList = conf.TemporaryAddressList;

            return result;
        }
    }

    // 20100225 LKo: Data class for the temporary addresses
    public class TemporaryAddressVO : ControlBase
    {
        // Address ident
        private int? m_iId;
        public int? Id { get { return m_iId; } set { m_iId = value; } }

        // Address type ident
        private int m_iAddressTypeId;
        public int AddressTypeId { get { return m_iAddressTypeId; } set { m_iAddressTypeId = value; } }

        // Address type name
        private string m_sAddressTypeName;
        public string AddressTypeName { get { return m_sAddressTypeName; } set { m_sAddressTypeName = value; } }

        // Start date for the address
        private DateTime? m_dtStartDate;
        public DateTime? StartDate { get { return m_dtStartDate; } set { m_dtStartDate = value; } }
        public string StartDateShortString { get { return ShortDate(m_dtStartDate); } }

        // End date for the address
        private DateTime? m_dtEndDate;
        public DateTime? EndDate { get { return m_dtEndDate; } set { m_dtEndDate = value; } }
        public string EndDateShortString { get { return ShortDate(m_dtEndDate); } }

        // Title
        private string m_sTitle;
        public string Title { get { return m_sTitle; } set { m_sTitle = value; } }

        // Description
        private string m_sDescription;
        public string Description { get { return m_sDescription; } set { m_sDescription = value; } }

        // And the actual address
        private InvConfAddressVO m_oAddress;
        public InvConfAddressVO Address { get { return m_oAddress; } set { m_oAddress = value; } }

        // Address string for the grid
        public string AddressString { get { if (this.Address != null) return this.Address.AddressString; else return ""; } }

        // Override the constructor
        public TemporaryAddressVO() : base()
        {
            // Create the address class
            this.Address = new InvConfAddressVO();
        }
    }
    
    // 20100225 LKo: Input data for the UpdateInvoicingConfiguration
    public class UpdateInvoiceConfigurationDataVO : ControlBase
    {
        // Invoice configuration number
        private int? m_iInvConfNo;
        public int? InvConfNo { get { return m_iInvConfNo; } set { m_iInvConfNo = value; } }
        
        // Payterm ident
        private int? m_iPayTermId;
        public int? PayTermId { get { return m_iPayTermId; } set { m_iPayTermId = value; } }

        // Payterm start date
        private DateTime? m_iPayTermStartDate;
        public DateTime? PayTermStartDate { get { return m_iPayTermStartDate; } set { m_iPayTermStartDate = value; } }

        // Invoice period ident
        private int? m_iInvoicePeriodId;
        public int? InvoicePeriodId { get { return m_iInvoicePeriodId; } set { m_iInvoicePeriodId = value; } }

        // Invoice group Id
        private int? m_iInvoiceGroupId;
        public int? InvoiceGroupId { get { return m_iInvoiceGroupId; } set { m_iInvoiceGroupId = value; } }

        // Invoice send method (values?)
        private int? m_iInvoiceSendMethod;
        public int? InvoiceSendMethod { get { return m_iInvoiceSendMethod; } set { m_iInvoiceSendMethod = value; } }

        // Advance notice calculation flag
        private int? m_iIsAdvanceNoticeCalc;
        public int? IsAdvanceNoticeCalc { get { return m_iIsAdvanceNoticeCalc; } set { m_iIsAdvanceNoticeCalc = value; } }

        // Invoicing address
        private AddressVO m_oInvoicingAddress;
        public AddressVO InvoicingAddress { get { return m_oInvoicingAddress; } set { m_oInvoicingAddress = value; } }
        
        // Reminder address
        private AddressVO m_oReminderAddress;
        public AddressVO ReminderAddress { get { return m_oReminderAddress; } set { m_oReminderAddress = value; } }
        
        // Debt collection address
        private AddressVO m_oDebtCollectionAddress;
        public AddressVO DebtCollectionAddress { get { return m_oDebtCollectionAddress; } set { m_oDebtCollectionAddress = value; } }

        // List for temporary addresses
        private ControlList<TemporaryAddressVO> m_lTemporaryAddressList = null;
        public ControlList<TemporaryAddressVO> TemporaryAddressList { get { return m_lTemporaryAddressList; } set { m_lTemporaryAddressList = value; } }
    }
    
    public class InvoiceDataVO : ControlBase
    {
        private string customerCode;
        public string CustomerCode { get { return customerCode; } set { customerCode = value; } }
        private int? invoiceNo;
        public int? InvoiceNo { get { return invoiceNo; } set { invoiceNo = value; } }
        private DateTime? sentDate;
        public DateTime? SentDate { get { return sentDate; } set { sentDate = value; } }
        public string SSentDate { get { return ShortDate(sentDate); } }
        private DateTime? dueDate;
        public DateTime? DueDate { get { return dueDate; } set { dueDate = value; } }
        public string SDueDate { get { return ShortDate(dueDate); } }
        private double? dueamount;
        public double? Dueamount { get { return dueamount; } set { dueamount = value; } }
        private double? amount;
        public double? Amount { get { return amount; } set { amount = value; } }        
        private double? vat;
        public double? Vat { get { return vat; } set { vat = value; } }
        private string fromCurrencyCode;
        public string FromCurrencyCode { get { return fromCurrencyCode; } set { fromCurrencyCode = value; } }
        private double? balance;
        public double? Balance { get { return balance; } set { balance = value; } }
        private DateTime? balanceDate;
        public DateTime? BalanceDate { get { return balanceDate; } set { balanceDate = value; } }
        public string SBalanceDate { get { return ShortDate(balanceDate); } }
        private DateTime? lastUpdateTime;
        public DateTime? LastUpdateTime { get { return lastUpdateTime; } set { lastUpdateTime = value; } }
        private int? invoiceType;
        public int? InvoiceType { get { return invoiceType; } set { invoiceType = value; } }
        private int? invoiceState;
        public int? InvoiceState { get { return invoiceState; } set { invoiceState = value; } }
        private int? systemStatusType;
        public int? SystemStatusType { get { return systemStatusType; } set { systemStatusType = value; } }
        public int? InvoicePaymentStatus { get { return systemStatusType; } }
        private string kID;
        public string KID { get { return kID; } set { kID = value; } }
        private long? annualStatement;
        public long? AnnualStatement { get { return annualStatement; } set { annualStatement = value; } }
        private int? invCalcType;
        public int? InvCalcType { get { return invCalcType; } set { invCalcType = value; } }
        private string code;
        public string Code { set { code = value; } get { return code; } }
        public string InvoicePaymentStatusCode { get { return code; } }
        private string name;
        public string Name { set { name = value; } get { return name; } }
        public string InvoicePaymentStatusName { get { return name; } }
        private string contractXML;
        public string ContractXML { get { return contractXML; } set { contractXML = value; } }
        private DateTime? invoiceDate;
        public DateTime? InvoiceDate { get { return invoiceDate; } set { invoiceDate = value; } }
        public string SInvoiceDate { get { return ShortDate(invoiceDate); } }
        private DateTime? approveTime;
        public DateTime? ApproveTime { get { return approveTime; } set { approveTime = value; } }
        public string SApproveTime { get { return ShortDate(approveTime); } }
        private long? invConfId;
        public long? InvConfId { get { return invConfId; } set { invConfId = value; } }
        private string perTypeCode;
        public string PerTypeCode { get { return perTypeCode; } set { perTypeCode = value; } }
        private string periodNo;
        public string PeriodNo { get { return periodNo; } set { periodNo = value; } }
        private string periodName;
        public string PeriodName { get { return periodName; } set { periodName = value; } }
        private DateTime? firstDay;
        public DateTime? FirstDay { get { return firstDay; } set { firstDay = value; } }
        public string SFirstDay { get { return ShortDate(firstDay); } }
        private DateTime? lastDay;
        public DateTime? LastDay { get { return lastDay; } set { lastDay = value; } }
        public string SLastDay { get { return ShortDate(lastDay); } }
        private string accountNo;
        public string AccountNo { get { return accountNo; } set { accountNo = value; } }
        private DateTime? newDueDate;
        public DateTime? NewDueDate { get { return newDueDate; } set { newDueDate = value; } }
        public string SNewDueDate { get { return ShortDate(newDueDate); } }
        private int? debitType;
        public int? DebitType { get { return debitType; } set { debitType = value; } }
        private DateTime? paymentStatusDate;
        public DateTime? PaymentStatusDate { get { return paymentStatusDate; } set { paymentStatusDate = value; } }
        public string SPaymentStatusDate { get { return ShortDate(paymentStatusDate); } }
        private string ptValue;
        public string PtValue { get { return ptValue; } set { ptValue = value; } }
        private string targetName;
        public string TargetName { get { return targetName; } set { targetName = value; } }

        public long? CreditInvoiceNo { get; set; }
        public int? InvoiceType2 { get; set; }
        public string InvoiceNote { get; set; }

        // marttmik 20101028 
        // JIRA CMONDEV-779 / Req #212680
        private string dueamountwithdecimals;
        public string DueamountWithDecimals { get { return CMCommonUtils.SetDecimals(this.dueamount); } set { dueamountwithdecimals = value; } }
        private string amountwithdecimals;
        public string AmountWithDecimals { get { return CMCommonUtils.SetDecimals(this.amount); } set { amountwithdecimals = value; } }
        private string balancewithdecimals;
        public string BalanceWithDecimals { get { return CMCommonUtils.SetDecimals(this.balance); } set { balancewithdecimals = value; } }

        public string InvoiceStateText
        {
            get
            {
                if (invoiceState.HasValue)
                {
                    switch (invoiceState)
                    {
                        case 0:
                            return GGR("ContactManager", "InvoiceControl_NONE");
                        case 1:
                            return GGR("ContactManager", "InvoiceControl_CLR");
                        case -1:
                            return GGR("ContactManager", "InvoiceControl_CLRERR");
                        case 4:
                            return GGR("ContactManager", "InvoiceControl_RRDYW");
                        case 7:
                            return GGR("ContactManager", "InvoiceControl_RRDY");
                        case 10:
                            return GGR("ContactManager", "InvoiceControl_PRC");
                        case -11:
                            return GGR("ContactManager", "InvoiceControl_PRCNO");
                        case 20:
                            return GGR("ContactManager", "InvoiceControl_BLD");
                        case -20:
                            return GGR("ContactManager", "InvoiceControl_BLDERR");
                        case 25:
                            return GGR("ContactManager", "InvoiceControl_CLC");
                        case -25:
                            return GGR("ContactManager", "InvoiceControl_CLCERR");
                        case 30:
                            return GGR("ContactManager", "InvoiceControl_RDY");
                        case -30:
                            return GGR("ContactManager", "InvoiceControl_RDYERR");
                        case 40:
                            return GGR("ContactManager", "InvoiceControl_VER");
                        case -41:
                            return GGR("ContactManager", "InvoiceControl_VERNO");
                        case 50:
                            return GGR("ContactManager", "InvoiceControl_APR");
                        case -50:
                            return GGR("ContactManager", "InvoiceControl_APRERR");
                        case 60:
                            return GGR("ContactManager", "InvoiceControl_SPR");
                        case -60:
                            return GGR("ContactManager", "InvoiceControl_SPRERR");
                        case 70:
                            return GGR("ContactManager", "InvoiceControl_SNC");
                        case -70:
                            return GGR("ContactManager", "InvoiceControl_SNCERR");
                        case 80:
                            return GGR("ContactManager", "InvoiceControl_SND");
                        case -80:
                            return GGR("ContactManager", "InvoiceControl_SNDERR");
                        case 85:
                            return GGR("ContactManager", "InvoiceControl_PrintedNotExported");
                        case 86:
                            return GGR("ContactManager", "InvoiceControl_CNDSX");
                        case 90:
                            return GGR("ContactManager", "InvoiceControl_CLS");
                        case -90:
                            return GGR("ContactManager", "InvoiceControl_CLSERR");
                        case 99:
                            return GGR("ContactManager", "InvoiceControl_END");
                        default:
                            return invoiceState.ToString();
                    }
                }
                return GGR("ContactManager", "Unknown");
            }
        }

        public string DebitTypeText
        {
            get
            {
                if (debitType.HasValue)
                {
                    switch (debitType.Value)
                    {
                        case 1:
                            return GGR("ContactManager", "InvoiceControl_Normal");
                        case 2:
                            return GGR("ContactManager", "InvoiceControl_Transferred");
                        case 3:
                            return GGR("ContactManager", "InvoiceControl_Loss");
                        default:
                            return debitType.ToString();
                    }
                }
                return GGR("ContactManager", "Unknown");
            }
        }

        public string InvoiceTypeText
        {
            get
            {
                if (invoiceType.HasValue)
                {
                    switch (invoiceType.Value)
                    {
                        case 0:
                            // 0-faktura generert iforbindelse med Kun Kredittering
                            return GGR("ContactManager", "InvoiceType_0");
                        case 1:
                            // Periodisk faktura
                            return GGR("ContactManager", "InvoiceType_1");
                        case 2:
                            // Sluttfaktura
                            return GGR("ContactManager", "InvoiceType_2");
                        default:
                            return invoiceType.ToString();
                    }
                }
                return GGR("ContactManager", "InvoiceType_NULL");
            }
        }

        public string InvCalcTypeText
        {
            get
            {
                if (InvoiceType2.HasValue)
                {
                    if (InvoiceType2 == -1)
                    {
                        return GGR("ContactManager", "InvoiceType2_minus1");
                    }
                }

                if (invCalcType.HasValue)
                {
                    switch (invCalcType.Value)
                    {
                        case 1:
                            // prelim
                            return GGR("ContactManager", "InvCalcType_0");
                        case 2:
                            // oppgjør
                            return GGR("ContactManager", "InvCalcType_1");
                        default:
                            return invoiceType.ToString();
                    }
                }
                // Old invoices don't have this information, not an error...
                return String.Empty;
            }
        }
    }


    public class Invoice_ArchiveVO : ControlBase
    {
        private string invoiceTotalSum;
        private DateTime? invoiceDate;
        private DateTime? dueDate;
        private int? invoiceNumber;
        private string kptunnus;
        private string asnimi;
        private string invoiceType;
        private string kposoite;
        private string mtunnus;
        private string asnimi2;
        private string kpnimi;
        private int? asnro;
        private string documentreference;
        private int? pg_num;
        private string file_id;

        [ControlCopyName("Summa")]
        public string InvoiceTotalSum { get { return invoiceTotalSum; } set { invoiceTotalSum = value; } }
        public double DInvoiceTotalSum { get { return CMCommonUtils.GetDouble(invoiceTotalSum); } }
        [ControlCopyName("Laskupvm")]
        public DateTime? InvoiceDate { get { return invoiceDate; } set { invoiceDate = value; } }
        public string SInvoiceDate { get { return ShortDate(invoiceDate); } }
        [ControlCopyName("Eräpvm")]
        public DateTime? DueDate { get { return dueDate; } set { dueDate = value; } }
        public string SDueDate { get { return ShortDate(dueDate); } }
        [ControlCopyName("Laskunro")]
        public int? InvoiceNumber { get { return invoiceNumber; } set { invoiceNumber = value; } }
        public string Kptunnus { get { return kptunnus; } set { kptunnus = value; } }
        public string Asnimi { get { return asnimi; } set { asnimi = value; } }
        [ControlCopyName("Tyyppi")]
        public string InvoiceType { get { return invoiceType; } set { invoiceType = value; } }
        public string Kposoite { get { return kposoite; } set { kposoite = value; } }
        public string Mtunnus { get { return mtunnus; } set { mtunnus = value; } }
        public string Asnimi2 { get { return asnimi2; } set { asnimi2 = value; } }
        public string Kpnimi { get { return kpnimi; } set { kpnimi = value; } }
        public int? Asnro { get { return asnro; } set { asnro = value; } }
        public string Documentreference { get { return documentreference; } set { documentreference = value; } }
        public int? Pg_num { get { return pg_num; } set { pg_num = value; } }
        public string File_id { get { return file_id; } set { file_id = value; } }
    }


    public class InvoiceConfiguration : ControlBase
    {
        private int? invoiceConfigID;
        public int? InvoiceConfigID { get { return invoiceConfigID; } set { invoiceConfigID = value; } }
        private DateTime? invConfStartDate;
        public DateTime? InvConfStartDate { get { return invConfStartDate; } set { invConfStartDate = value; } }
        public string SInvConfStartDate { get { return ShortDate(invConfStartDate); } }
        private DateTime? invConfEndDate;
        public DateTime? InvConfEndDate { get { return invConfEndDate; } set { invConfEndDate = value; } }
        public string SInvConfEndDate { get { return ShortDate(invConfEndDate); } }
        private string invStreetName;
        public string InvStreetName { get { return invStreetName; } set { invStreetName = value; } }
        private string invStreetNo;
        public string InvStreetNo { get { return invStreetNo; } set { invStreetNo = value; } }
        private string invCoAddress;
        public string InvCoAddress { get { return invCoAddress; } set { invCoAddress = value; } }
        private string invAttention;
        public string InvAttention { get { return invAttention; } set { invAttention = value; } }
        private string invFloorNo;
        public string InvFloorNo { get { return invFloorNo; } set { invFloorNo = value; } }
        private string invFlatNo;
        public string InvFlatNo { get { return invFlatNo; } set { invFlatNo = value; } }
        private string invAttCoAddress;
        public string InvAttCoAddress { get { return invAttCoAddress; } set { invAttCoAddress = value; } }
        private string invPostCode;
        public string InvPostCode { get { return invPostCode; } set { invPostCode = value; } }
        private string invPostPlace;
        public string InvPostPlace { get { return invPostPlace; } set { invPostPlace = value; } }
        private string invCountryCode;
        public string InvCountryCode { get { return invCountryCode; } set { invCountryCode = value; } }

        public string InvAddressString
        {
            get
            {
                string addr = string.Empty;
                addr = this.invStreetName + " " + this.invStreetNo + " " + this.invFlatNo + " " + this.invFloorNo;
                addr = addr.Trim() + ", " + this.invPostCode + " " + this.invPostPlace;
                return addr;
            }
        }
    }

    public class InvoicePostponementVO
    {
        public long? InvoiceNo;
        public DateTime? DueDate;
        public string NoteType;
        public int? InterestMethod;
        public string Reason;
    }

    public class InvoiceQueryVO
    {
        public string CustomerCode;
        public string DeliverySiteCode;
        public DateTime? InvoiceDateMin;
        public bool FetchOnlyUnpaid;
        public int? InvoiceConfiguration;
        public string InvoiceNo;
    }

    public class UpdateInvoiceConfAddress
    {        
        private bool onlyMatchingAddress;
        public bool OnlyMatchingAddress { get { return onlyMatchingAddress; } set { onlyMatchingAddress = value; } }
        private bool updReminderAndDebt;
        public bool UpdReminderAndDebt { get { return updReminderAndDebt; } set { updReminderAndDebt = value; } }
        private AddressVO newAddress;
        public AddressVO NewAddress { get { return newAddress; } set { newAddress = value; } }
        private AddressVO prevCustAddress;
        public AddressVO PrevCustAddress { get { return prevCustAddress; } set { prevCustAddress = value; } }
    }

    public class InvoiceControl : CMBaseControl
    {
        public static ControlList<InvoiceDataVO> FetchInvoices(InvoiceQueryVO input, CConnect connection)
        {
            StringBuilder query = new StringBuilder("<InvoiceSearch>");

            // Fetch invoices by customer
            if (!String.IsNullOrEmpty(input.CustomerCode))
                query.AppendFormat("<CustomerCode>{0}</CustomerCode>", input.CustomerCode);
            // Fetch delivery site's invoices?
            if (!String.IsNullOrEmpty(input.DeliverySiteCode))
                query.AppendFormat("<DeliverySiteCode>{0}</DeliverySiteCode>", input.DeliverySiteCode);
            // Fetch invoices beginnning from certain date
            if (input.InvoiceDateMin.HasValue)
                query.AppendFormat("<InvoiceDateMin>{0}</InvoiceDateMin>", input.InvoiceDateMin.Value.ToShortDateString());
            // Fetch only unpaid invoices?
            if (input.FetchOnlyUnpaid)
                query.Append("<PayMentState>1</PayMentState>");
            // Fetch invoices from certain invoiceconfiguration
            if (input.InvoiceConfiguration.HasValue)
                query.AppendFormat("<InvConfigId>{0}</InvConfigId>", input.InvoiceConfiguration.Value.ToString());
            // Fetch invoice from it's invoice number
            if (!string.IsNullOrEmpty(input.InvoiceNo))
                query.AppendFormat("<InvoiceNo>{0}</InvoiceNo>", input.InvoiceNo);

            query.Append("</InvoiceSearch>");
            string invoicesXML = ComCXMLCall("SearchInvoice", query.ToString(), connection);
            ControlList<InvoiceDataVO> invoices = new ControlList<InvoiceDataVO>();
            ControlUtil.ParseXmlPathToObject(invoicesXML, "/INVOICESEARCHRESPONSE/INVOICEDATA", invoices);

            return invoices;
        }

        // 20110120 bastirob - CMONDEV-1189 
        public static ControlList<Invoice_ArchiveVO> FetchInvoiceArchive(string customerID, System.Web.SessionState.HttpSessionState aSession)
        {
            ControlList<Invoice_ArchiveVO> invoices = new ControlList<Invoice_ArchiveVO>();
            if (!String.IsNullOrEmpty(customerID))
            {
                string invoicesXML = GetInvoiceArchive(customerID,aSession);
                ControlUtil.ParseXmlPathToObject(invoicesXML, "/QUERYRESULT/HIT", invoices, true);
            }
            return invoices;
        }

        // 20110120 bastirob - CMONDEV-1189 
        // 20110211 dyrdamar - CMONDEV-1329 - removed adding of "?customerCode=" string to url
        private static string GetInvoiceArchive(string customerID, System.Web.SessionState.HttpSessionState aSession)
        {
            SteeringInfoData invoiceArchiveLink = SteeringInfoControl.FetchSteeringInfoValues(CMConstants.STEER_COMMON_SETTINGS, null, "InvoiceArchive_ddCold", Convert.ToInt32(aSession["LanguageID"]), true, true);             
            StringBuilder url = new StringBuilder();
            url.Append(invoiceArchiveLink.SteeringInfo[0].Steeringvalue);
            url.Append(customerID);

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url.ToString());
            request.Timeout = CMConstants.INVOICE_ARCHIVE_DDCOLD_TIMEOUT;             
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            StreamReader loResponseStream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            return loResponseStream.ReadToEnd().Replace("&", "&amp;");
        }

    

        public static bool InvoiceChanged(InvoiceDataVO invoice, CConnect connection)
        {
            if (invoice == null)
                throw new NullReferenceException("Agument 'invoice' cannot be null.");
            InvoiceQueryVO input = new InvoiceQueryVO();
            input.InvoiceNo = invoice.InvoiceNo.ToString();
            input.CustomerCode = invoice.CustomerCode;
            ControlList<InvoiceDataVO> foundInvoices = FetchInvoices(input, connection);
            foreach (InvoiceDataVO foundInvoice in foundInvoices)
            {
                if (foundInvoice.SentDate == invoice.SentDate &&
                    foundInvoice.DueDate == invoice.DueDate &&
                    foundInvoice.Dueamount == invoice.Dueamount &&
                    foundInvoice.Amount == invoice.Amount &&
                    foundInvoice.Vat == invoice.Vat &&
                    foundInvoice.FromCurrencyCode == invoice.FromCurrencyCode &&
                    foundInvoice.Balance == invoice.Balance &&
                    foundInvoice.BalanceDate == invoice.BalanceDate &&
                    foundInvoice.LastUpdateTime == invoice.LastUpdateTime &&
                    foundInvoice.InvoiceType == invoice.InvoiceType &&
                    foundInvoice.InvoiceState == invoice.InvoiceState &&
                    foundInvoice.SystemStatusType == invoice.SystemStatusType &&
                    foundInvoice.KID == invoice.KID &&
                    foundInvoice.AnnualStatement == invoice.AnnualStatement &&
                    foundInvoice.InvCalcType == invoice.InvCalcType &&
                    foundInvoice.Code == invoice.Code &&
                    foundInvoice.Name == invoice.Name &&
                    foundInvoice.ContractXML == invoice.ContractXML &&
                    foundInvoice.InvoiceDate == invoice.InvoiceDate &&
                    foundInvoice.ApproveTime == invoice.ApproveTime &&
                    foundInvoice.CreditInvoiceNo == invoice.CreditInvoiceNo &&
                    foundInvoice.InvConfId == invoice.InvConfId &&
                    foundInvoice.PerTypeCode == invoice.PerTypeCode &&
                    foundInvoice.PeriodNo == invoice.PeriodNo &&
                    foundInvoice.PeriodName == invoice.PeriodName &&
                    foundInvoice.FirstDay == invoice.FirstDay &&
                    foundInvoice.LastDay == invoice.LastDay &&
                    foundInvoice.AccountNo == invoice.AccountNo &&
                    foundInvoice.NewDueDate == invoice.NewDueDate &&
                    foundInvoice.DebitType == invoice.DebitType &&
                    foundInvoice.PaymentStatusDate == invoice.PaymentStatusDate &&
                    foundInvoice.InvoiceType2 == invoice.InvoiceType2 &&
                    foundInvoice.InvoiceNote == invoice.InvoiceNote &&
                    foundInvoice.TargetName == invoice.TargetName &&
                    foundInvoice.PtValue == invoice.PtValue)
                    return false;
            }
            return true;
        }

        public static ControlList<InvoiceConfiguration> FetchCustomerInvoiceConfigurations(CustomerVO customer, CConnect connection)
        {
            string query = String.Empty;
            if (customer.CustomerCode != "")
            {
                query = "<CustomerCode>" + customer.CustomerCode + "</CustomerCode>";
            }
            string contractsXML = ComCXMLCall("GetContractInfo", query, connection);
            ControlList<InvoiceConfiguration> invoiceConfigurationsTmp = new ControlList<InvoiceConfiguration>();
            ControlUtil.ParseXmlPathToObject(contractsXML, "/CONTRACTDATARESPONSE/CONTRACTDATA", invoiceConfigurationsTmp);

            // Remove duplicates
            int? invConfigId = null;
            ControlList<InvoiceConfiguration> invoiceConfigurations = new ControlList<InvoiceConfiguration>();
            foreach (InvoiceConfiguration invoiceConfigurationTmp in invoiceConfigurationsTmp)
            {
                if ((!invConfigId.HasValue) || (invConfigId.Value != invoiceConfigurationTmp.InvoiceConfigID.Value))
                {
                    invConfigId = invoiceConfigurationTmp.InvoiceConfigID;
                    invoiceConfigurations.Add(invoiceConfigurationTmp);
                }
            }

            return invoiceConfigurations;
        }

        public static void PostponeInvoice(InvoicePostponementVO input, CConnect connection)
        {
            if (!input.InvoiceNo.HasValue)
                throw new Exception("InvoiceNo missing");
            if (!input.DueDate.HasValue)
                throw new Exception("DueDate missing");
            if (String.IsNullOrEmpty(input.NoteType))
                throw new Exception("NoteType missing");
            if (!input.InterestMethod.HasValue)
                throw new Exception("InterestMethod missing");
            
            string query = "";
            query += "<InvoiceNo>" + input.InvoiceNo.ToString() + "</InvoiceNo>";
            query += "<DueDate>" + input.DueDate.Value.ToShortDateString() + "</DueDate>";
            query += "<NoteType>" + input.NoteType + "</NoteType>";
            query += "<InterestMethod>" + input.InterestMethod.Value.ToString() + "</InterestMethod>";
            if (!String.IsNullOrEmpty(input.Reason))
                query += "<Reason>" + input.Reason + "</Reason>";
            string postponeXML = ComCXMLCall("SetNewPostponement", query, connection);
        }

        // 20100225 LKo: Update invoice configuration
        public static bool SaveTemporaryAddresses(List<InvoiceConfiguration> aNewConfigurations, List<InvoiceConfiguration> aOldConfigurations, List<ContractInputVO> aContracts, Hashtable aTemporaryAddressHashTable, CConnect aConnection)
        {
            bool result = false;
            // Check that the parameters are Ok
            if ((aConnection != null) && (aOldConfigurations != null) && (aNewConfigurations != null) && (aTemporaryAddressHashTable != null))
            {
                // Check that the invoice configurations have data
                if ((aOldConfigurations.Count > 0) && (aNewConfigurations.Count > 0))
                {
                    // Variables for the configurations
                    InvoiceConfiguration icNew;
                    InvoiceConfiguration icOld;

                    // Variable for the invoice configuration data search
                    InvoiceConfigurationInputVO iciInput;
                    
                    // String builder for the XML
                    StringBuilder sbXML = new StringBuilder();
                    
                    // Variable for the temporary address data
                    TemporaryAddressVO taNewTempAddress;

                    // Variable for the temporary address list
                    ControlList<TemporaryAddressVO> clTemporaryAddressList;

                    // Variable for the contract
                    ContractInputVO ciContract;

                    // Loop through the configurations
                    for (int i = 0; i < aOldConfigurations.Count; i++)
                    {
                        // Get the old invoice config
                        icOld = aOldConfigurations[i];

                        // Check that the item is Ok
                        if (icOld != null)
                        {
                            // Get the corresponding new item
                            icNew = aNewConfigurations.Find(delegate(InvoiceConfiguration ic) { return ic.InvoiceConfigID == icOld.InvoiceConfigID; });

                            if (icNew != null)
                            {
                                // Try to get the linked contract
                                ciContract = aContracts.Find(delegate(ContractInputVO ci) { return ci.InvoiceConfigId == icOld.InvoiceConfigID; });

                                // Check that the contract is Ok
                                if (ciContract != null)
                                {
                                    // Compare the addresses from the old and new configurations
                                    if ((String.Compare(icNew.InvStreetName, icOld.InvStreetName) != 0) ||
                                        (String.Compare(icNew.InvStreetNo, icOld.InvStreetNo) != 0) ||
                                        (String.Compare(icNew.InvFlatNo, icOld.InvFlatNo) != 0) ||
                                        (String.Compare(icNew.InvFloorNo, icOld.InvFloorNo) != 0) ||
                                        (String.Compare(icNew.InvPostCode, icOld.InvPostCode) != 0) ||
                                        (String.Compare(icNew.InvPostPlace, icOld.InvPostPlace) != 0) ||
                                        (String.Compare(icNew.InvCoAddress, icOld.InvCoAddress) != 0) ||
                                        (String.Compare(icNew.InvAttention, icOld.InvAttention) != 0) ||
                                        (String.Compare(icNew.InvCountryCode, icOld.InvCountryCode) != 0))
                                    {
                                        // Clear the possible previous XML
                                        if (sbXML.Length > 0)
                                            sbXML.Remove(0, sbXML.Length);

                                        // Start the transport object 
                                        sbXML.Append("<TransportObject>");
                                        sbXML.Append("<RequestData>");

                                        // Set the invoice configuration number (the id)
                                        sbXML.Append("<InvConfNo>" + icOld.InvoiceConfigID.ToString() + "</InvConfNo>");
                                                
                                        // Get the temporary address list from the hashtable
                                        clTemporaryAddressList = aTemporaryAddressHashTable[icOld.InvoiceConfigID] as ControlList<TemporaryAddressVO>;

                                        // Create a new list if necessary
                                        if (clTemporaryAddressList == null)
                                            clTemporaryAddressList = new ControlList<TemporaryAddressVO>();
                                        
                                        // Add the old address into the temporary addresses
                                        taNewTempAddress = CreateTemporaryAddress(icOld, ciContract);

                                        // Add the address into the temporary address list
                                        if (taNewTempAddress != null)
                                            clTemporaryAddressList.Add(taNewTempAddress);

                                        // Check if temporary addresses are provided
                                        if (clTemporaryAddressList.Count > 0)
                                            sbXML.Append(AddInvoiceConfigurationTemporaryAddressData(clTemporaryAddressList));
                                    }

                                    // Close the transport object
                                    sbXML.Append("</RequestData>");
                                    sbXML.Append("</TransportObject>");

                                    // Call the WCF-service
                                    try
                                    {
                                        ComCWCFCall("UpdateInvoiceConfiguration", sbXML.ToString(), aConnection, ComCWCFSite.Invoicing);
                                        result =  true;
                                    }
                                    catch (CABException ce)
                                    {
                                        if (ce.Message == "") result = true;
                                        else result = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Return false by default
            return result;
        }

        //Dyrdamar 20100514: Update invoice configuration - without compareing addresses and adding new - only deleting if needed
        public static bool SaveTemporaryAddresses(List<InvoiceConfiguration> aInvoiceConfigurations, Hashtable aTemporaryAddressHashTable, CConnect aConnection)
        {
            bool result = false;
            // Check that the parameters are Ok
            if ((aConnection != null) && (aInvoiceConfigurations != null) && (aTemporaryAddressHashTable != null))
            {
                // Check that the invoice configurations have data
                if (aInvoiceConfigurations.Count > 0)
                {
                    // Variables for the configurations
                    InvoiceConfiguration icNew;                    

                    // String builder for the XML
                    StringBuilder sbXML = new StringBuilder();                    

                    // Variable for the temporary address list
                    ControlList<TemporaryAddressVO> clTemporaryAddressList;

                    // Loop through the configurations
                    for (int i = 0; i < aInvoiceConfigurations.Count; i++)
                    {
                        // Get the old invoice config
                        icNew = aInvoiceConfigurations[i];

                        if (icNew != null)
                        {
                            if (sbXML.Length > 0)
                                sbXML.Remove(0, sbXML.Length);

                            // Start the transport object 
                            sbXML.Append("<TransportObject>");
                            sbXML.Append("<RequestData>");

                            // Set the invoice configuration number (the id)
                            sbXML.Append("<InvConfNo>" + icNew.InvoiceConfigID.ToString() + "</InvConfNo>");

                            clTemporaryAddressList = null;

                            // Get the temporary address list from the hashtable
                            if (aTemporaryAddressHashTable.ContainsKey(icNew.InvoiceConfigID))
                                clTemporaryAddressList = aTemporaryAddressHashTable[icNew.InvoiceConfigID] as ControlList<TemporaryAddressVO>;

                            // Create a new list if necessary
                            if (clTemporaryAddressList == null)
                                clTemporaryAddressList = new ControlList<TemporaryAddressVO>();

                            // Check if temporary addresses are provided
                            if (clTemporaryAddressList.Count > 0)
                                sbXML.Append(AddInvoiceConfigurationTemporaryAddressData(clTemporaryAddressList));

                            // Close the transport object
                            sbXML.Append("</RequestData>");
                            sbXML.Append("</TransportObject>");

                            // Call the WCF-service
                            try
                            {
                                ComCWCFCall("UpdateInvoiceConfiguration", sbXML.ToString(), aConnection, ComCWCFSite.Invoicing);
                                result = true;
                            }
                            catch (CABException ce)
                            {
                                if (ce.Message == "") result = true;
                                else result = false;
                            }
                        }                        
                    }
                }
            }

            // Return false by default
            return result;
        }

        //Dyrdamar 20100514: Update invoice configuration - only for one invoice configuration without adding new address - only deleting if needed
        public static bool SaveTemporaryAddresses(InvoiceConfiguration aInvoiceConfiguration, Hashtable aTemporaryAddressHashTable, CConnect aConnection)
        {
            bool result = false;
            // Check that the parameters are Ok
            if ((aConnection != null) && (aInvoiceConfiguration != null) && (aTemporaryAddressHashTable != null))
            {
                // String builder for the XML
                StringBuilder sbXML = new StringBuilder();

                // Variable for the temporary address list
                ControlList<TemporaryAddressVO> clTemporaryAddressList;

                if (sbXML.Length > 0)
                    sbXML.Remove(0, sbXML.Length);

                // Start the transport object 
                sbXML.Append("<TransportObject>");
                sbXML.Append("<RequestData>");

                // Set the invoice configuration number (the id)
                sbXML.Append("<InvConfNo>" + aInvoiceConfiguration.InvoiceConfigID.ToString() + "</InvConfNo>");

                clTemporaryAddressList = null;

                // Get the temporary address list from the hashtable
                if (aTemporaryAddressHashTable.ContainsKey(aInvoiceConfiguration.InvoiceConfigID))
                    clTemporaryAddressList = aTemporaryAddressHashTable[aInvoiceConfiguration.InvoiceConfigID] as ControlList<TemporaryAddressVO>;

                // Create a new list if necessary
                if (clTemporaryAddressList == null)
                    clTemporaryAddressList = new ControlList<TemporaryAddressVO>();

                // Check if temporary addresses are provided
                if (clTemporaryAddressList.Count > 0)
                    sbXML.Append(AddInvoiceConfigurationTemporaryAddressData(clTemporaryAddressList));

                // Close the transport object
                sbXML.Append("</RequestData>");
                sbXML.Append("</TransportObject>");

                // Call the WCF-service
                try
                {
                    ComCWCFCall("UpdateInvoiceConfiguration", sbXML.ToString(), aConnection, ComCWCFSite.Invoicing);
                    result = true;
                }
                catch (CABException ce)
                {
                    if (ce.Message == "") result = true;
                    else result = false;
                }
            }            

            // Return false by default
            return result;
        }

        private static string AddInvoiceConfigurationAddressInnerData(AddressVO aAddress)
        {
            // Create a string builder for the XML
            StringBuilder sbXML = new StringBuilder();

            // Add the StreetName
            sbXML.Append("<StreetName>");
            sbXML.Append(aAddress.StreetName);
            sbXML.Append("</StreetName>");

            // Add the StreetNumber
            sbXML.Append("<StreetNumber>");
            sbXML.Append(aAddress.StreetNumber);
            sbXML.Append("</StreetNumber>");

            // Add the FlatId
            sbXML.Append("<FlatId>");
            sbXML.Append(aAddress.FlatId);
            sbXML.Append("</FlatId>");

            // Add the Floor
            sbXML.Append("<Floor>");
            sbXML.Append(aAddress.Floor);
            sbXML.Append("</Floor>");

            // Add the COAddress
            sbXML.Append("<COAddress>");
            sbXML.Append(aAddress.CoAddress);
            sbXML.Append("</COAddress>");

            // Add the Attention
            sbXML.Append("<Attention>");
            sbXML.Append(aAddress.Attention);
            sbXML.Append("</Attention>");

            // Add the AttentionCOAddress
            sbXML.Append("<AttentionCOAddress>");
            sbXML.Append(aAddress.AttCoAddress);
            sbXML.Append("</AttentionCOAddress>");

            // Add the PostCode
            sbXML.Append("<PostCode>");
            sbXML.Append(aAddress.PostCode);
            sbXML.Append("</PostCode>");

            // Add the PostPlace
            sbXML.Append("<PostPlace>");
            sbXML.Append(aAddress.PostalPlace);
            sbXML.Append("</PostPlace>");

            // Add the CountryCode
            sbXML.Append("<CountryCode>");
            sbXML.Append(aAddress.CountryCode);
            sbXML.Append("</CountryCode>");

            return sbXML.ToString();
        }

        private static string AddInvoiceConfigurationAddressData(AddressVO aAddress, string aXMLSectionName)
        {
            // Create a string builder for the XML
            StringBuilder sbXML = new StringBuilder();

            // Add the section name
            sbXML.AppendFormat("<{0}>", aXMLSectionName);

            sbXML.Append(AddInvoiceConfigurationAddressInnerData(aAddress));

            // Close the section
            sbXML.AppendFormat("</{0}>", aXMLSectionName);

            // Return the created XML
            return sbXML.ToString();
        }

        private static string AddInvoiceConfigurationTemporaryAddressData(ControlList<TemporaryAddressVO> aTemporaryAddressList)
        {
            // Check that the list is set
            if (aTemporaryAddressList != null)
            {
                // Variable for a single temporary address
                TemporaryAddressVO taTempAddress = null;

                // Create stringbuilder for the xml
                StringBuilder sbXML = new StringBuilder();

                // Start the temporary address "array" section
                sbXML.Append("<TemporaryAddress>");

                // Loop through the items in the list
                for (int i = 0; i < aTemporaryAddressList.Count; i++)
                {
                    // Get the address
                    taTempAddress = (TemporaryAddressVO)aTemporaryAddressList[i];

                    if (taTempAddress != null)
                    {
                        // Create a section for the temporary address
                        sbXML.Append("<TemporaryAddress>");

                        // Start the Id
                        sbXML.Append("<Id>");

                        // Check if the Id is provided (null = Insert)
                        if (taTempAddress.Id.HasValue)
                            sbXML.Append(taTempAddress.Id.Value.ToString());

                        // Close the Id
                        sbXML.Append("</Id>");

                        // Set the address type id
                        sbXML.Append("<AddressTypeId>");
                        sbXML.Append(taTempAddress.AddressTypeId);
                        sbXML.Append("</AddressTypeId>");

                        // Set the address type name
                        sbXML.Append("<AddressTypeName>");
                        sbXML.Append(taTempAddress.AddressTypeName);
                        sbXML.Append("</AddressTypeName>");

                        // Set the StartDate
                        sbXML.Append("<StartDate>");

                        if (taTempAddress.StartDate.HasValue)
                            sbXML.Append(taTempAddress.StartDate.ToString());

                        sbXML.Append("</StartDate>");

                        // Set the EndDate
                        sbXML.Append("<EndDate>");

                        if (taTempAddress.EndDate.HasValue)
                            sbXML.Append(taTempAddress.EndDate);

                        sbXML.Append("</EndDate>");

                        // Set the Title
                        sbXML.Append("<Title>");
                        sbXML.Append(taTempAddress.Title);
                        sbXML.Append("</Title>");

                        // Set the Description
                        sbXML.Append("<Description>");
                        sbXML.Append(taTempAddress.Description);
                        sbXML.Append("</Description>");

                        // Add the actual address
                        sbXML.Append(AddInvoiceConfigurationAddressData(taTempAddress.Address, "Address"));

                        // Close the section for temporary address
                        sbXML.Append("</TemporaryAddress>");
                    }
                }

                // Close the temporary address "array" section
                sbXML.Append("</TemporaryAddress>");

                // Return the created XML
                return sbXML.ToString();
            }

            // Return empty by default
            return "";
        }

        public static ControlList<InvoiceConfigurationOutputVO> GetInvoiceConfiguration(InvoiceConfigurationInputVO aFetchparameters, CConnect aConnection)
        {
            // Check that the parameter is Ok
            if (aFetchparameters != null)
            {
                // Create a string builder
                StringBuilder sbXML = new StringBuilder();
                
                // Start the transport object 
                sbXML.Append("<TransportObject>");

                sbXML.Append("<RequestData>");

                // Set the invoice configuration id
                sbXML.Append("<InvConfId>");
                
                if (aFetchparameters.InvConfId.HasValue)
                    sbXML.Append(aFetchparameters.InvConfId.Value.ToString());
                
                sbXML.Append("</InvConfId>");

                // Set the customer code
                sbXML.Append("<CustomerCode>");
                sbXML.Append(aFetchparameters.CustomerCode);
                sbXML.Append("</CustomerCode>");
                
                // Set the valid configuration date (What's this for?)
                sbXML.Append("<ValidConfigurationDate>");

                if (aFetchparameters.ValidConfigurationDate.HasValue)
                    sbXML.Append(aFetchparameters.ValidConfigurationDate.Value.ToString());

                sbXML.Append("</ValidConfigurationDate>");

                sbXML.Append("</RequestData>");
                
                // Close the transport object
                sbXML.Append("</TransportObject>");

                // Call the WCF-service
                string sResultXML = ComCWCFCall("GetInvoiceConfiguration", sbXML.ToString(), aConnection, ComCWCFSite.Invoicing);

                // Create a result list
                ControlList<InvoiceConfigurationOutputVO> icoOutputList = new ControlList<InvoiceConfigurationOutputVO>();

                ControlList<TemporaryAddressVO> icoTempAddresses = new ControlList<TemporaryAddressVO>();

                ControlUtil.ParseXmlPathToObjectAndSubObject(sResultXML, "TRANSPORTOBJECT/GETINVOICECONFIGURATIONOUTPUTLIST/GETINVOICECONFIGURATIONOUTPUT",
                    icoOutputList, "TEMPORARYADDRESSLIST/TEMPORARYADDRESS", icoTempAddresses);

                if (icoOutputList.Count > 0)
                {
                    // Return the list
                    return icoOutputList;
                }
            }

            // Return null by default
            return null;
        }

        // 20100312 LKo: Method for getting the bank account format steering value
        public static BankAccountFormat FetchBankAccountFormatSteeringValue(System.Web.SessionState.HttpSessionState aSession)
        {
            // Result variable (use invalid by default)
            BankAccountFormat bafResult = BankAccountFormat.Invalid;

            // Fetch the bank account format steering group information
            SteeringInfoData sidBankAccountFormat = SteeringInfoControl.FetchSteeringInfoValues(CMConstants.STEER_COMMON_SETTINGS, null, "BankAccountFormat", Convert.ToInt32(aSession["LanguageID"]), true, false);

            // Variable for the format
            int iFormat = 0;

            // Create the result list for the info
            if ((sidBankAccountFormat != null) && (sidBankAccountFormat.SteeringInfo != null))
            {
                try
                {
                    // Check that the number value is not null
                    if (sidBankAccountFormat.SteeringInfo[0].Numericvalue.HasValue)
                        iFormat = Convert.ToInt32(sidBankAccountFormat.SteeringInfo[0].Numericvalue.Value);
                    else if (!String.IsNullOrEmpty(sidBankAccountFormat.SteeringInfo[0].Steeringvalue))
                        iFormat = Convert.ToInt32(sidBankAccountFormat.SteeringInfo[0].Steeringvalue);
                }
                catch (Exception e)
                {
                    throw new FormatException("BankAccountFormat steering value must be numeric", e);
                }
            }
            else
            {
                iFormat = (int)BankAccountFormat.National;
            }

            // Check that the value is legal
            if ((iFormat > (int)BankAccountFormat.Invalid) && (iFormat <= (int)BankAccountFormat.Both))
                bafResult = (BankAccountFormat)iFormat;
            else
                throw new FormatException("BankAccountFormat value is invalid");

            // Return the result list
            return bafResult;
        }

        // 20100317 LKo: Method for IBAN formatting (CAB requires fixed format)
        public static string FormatIBAN(string aIBANAccount)
        {
            // Trim the spaces from the beginning and the end
            aIBANAccount = aIBANAccount.Trim();
            
            // Make the string upper case
            aIBANAccount = aIBANAccount.ToUpper();

            // Check if the string starts with IBAN
            if (aIBANAccount.StartsWith("IBAN"))
                aIBANAccount = aIBANAccount.Remove(0, 4); // Remove the IBAN

            // Remove the spaces
            if (aIBANAccount.IndexOf(' ') > -1)
                aIBANAccount = aIBANAccount.Replace(" ", "");

            // Return the formatted IBAN
            return aIBANAccount;
        }

        // 20100324 LKo: Method for creating a temporary address data object from the invoice configuration
        private static TemporaryAddressVO CreateTemporaryAddress(InvoiceConfiguration aOldIncoiceConfig, ContractInputVO aContract)
        {
            // Check that the parameters are Ok
            if ((aOldIncoiceConfig != null) && (aContract != null))
            {
                // Create the result object
                TemporaryAddressVO taResult = new TemporaryAddressVO();

                // Set the ID to null so a new address will be added
                taResult.Id = null;

                // Copy the address information from the contract (invoice config doesn't have it)
                taResult.Address.StreetName = aOldIncoiceConfig.InvStreetName;
                taResult.Address.StreetNumber = aOldIncoiceConfig.InvStreetNo;
                taResult.Address.FlatId = aOldIncoiceConfig.InvFlatNo;
                taResult.Address.Floor = aOldIncoiceConfig.InvFloorNo;
                taResult.Address.PostCode = aOldIncoiceConfig.InvPostCode;
                taResult.Address.PostPlace = aOldIncoiceConfig.InvPostPlace;
                taResult.Address.CountryCode = aContract.CountryCode; // aOldIncoiceConfig.InvCountryCode;                
                taResult.Address.AddressType = "TEMP";
                // 20110105 dyrdamar : Added C/O and attention address fields
                taResult.Address.Attention = aOldIncoiceConfig.InvAttention;
                taResult.Address.CoAddress = aOldIncoiceConfig.InvCoAddress;

                // Set the start and end dates
                taResult.StartDate = DateTime.Today;
                taResult.EndDate = aContract.Enddate;
            
                // Set the address type to main address
                taResult.AddressTypeId = 1;
                taResult.Title = "CMMO";
                taResult.Description = "CM MoveOut";
                
                // Return the created temporary address
                return taResult;
            }

            // Return null by default
            return null;
        }

        /// <summary>
        /// Function for creation of temporary address.
        /// </summary>
        /// <param name="sortedTmpAddresses">Sorted list of invoice configuration's temporary addresses.</param>
        /// <param name="address">New address, which will used as base for creation of temporary address.</param>
        /// <param name="addrTypeId">Temporary address type id.</param>
        /// <param name="title">Title for new temporary address.</param>
        /// <param name="contract">Customer contract related to the invoice configuration.</param>
        /// <returns>New temporary address.</returns>
        private static TemporaryAddressVO CreateTemporaryAddress(List<TemporaryAddressVO> sortedTmpAddresses, AddressVO address, int addrTypeId,
            string title, ContractDataVO contract)
        {
            DateTime? startDate = null;
            // Let's find last address from list of sorted temporary addresses, which has the same AddressTypeId as addrTypeId.
            // We will use this address for calculation of new temporary address StartDate.
            TemporaryAddressVO lastAddr = sortedTmpAddresses.FindLast(x => x.AddressTypeId == addrTypeId);
            if (lastAddr != null)
            {
                // New temp address start date will be last temp address end date plus 1 day.
                if (lastAddr.EndDate.HasValue)
                    startDate = lastAddr.EndDate.Value.AddDays(1);
            }
            if (!startDate.HasValue)
            {
                // If last temp address end date wasn't found, we will use ContractStart as StartDate.
                if (contract != null)
                    startDate = contract.ContractStart;
            }
            if (startDate.HasValue && (startDate.Value > DateTime.Today))
                startDate = DateTime.Today;

            TemporaryAddressVO tmpAddress = new TemporaryAddressVO();
            tmpAddress.Id = null;
            tmpAddress.AddressTypeId = addrTypeId;
            tmpAddress.StartDate = startDate.HasValue ? startDate.Value : DateTime.MinValue;
            tmpAddress.EndDate = DateTime.Today;
            tmpAddress.Title = title;
            tmpAddress.Address = address;
            return tmpAddress;
        }

        private static List<int> GetInvoiceConfigurationsIDs(CustomerVO customer, CConnect connection)
        {                        
            if (customer != null)
            {
                if (customer.Contracts == null)
                {                    
                    try
                    {
                        ContractControl.FetchCustomerContracts(customer, connection);
                    }
                    catch
                    {
                        customer.Contracts = null;
                    }
                }
            }
                        
            List<int> invConf = new List<int>();
            if (customer.Contracts != null)
            {
                foreach (ContractDataVO contract in customer.Contracts)
                {
                    if (contract.InvoiceConfigID != null)
                    {
                        if (!invConf.Contains((int)contract.InvoiceConfigID))
                            invConf.Add((int)contract.InvoiceConfigID);
                    }
                }
            }

            return invConf;
        }

        /// <summary>
        /// This method goes though customer's invoice configurations and updates their addresses.
        /// </summary>
        /// <param name="customer">CustomerVO object, which is used for searching configurations.</param>
        /// <param name="contracts">List of customer's contracts, it's used for calculation of new temp addresses start dates. It can be null, if updateTempAddresses = false.</param>
        /// <param name="newAddress">New address, which will be used for updating invoice conf addresses.</param>
        /// <param name="prevCustAddress">Address, which will be used for searching invoice conf addresses, which should be updated. It can be null, if onlyMatchingAddress = false.</param>
        /// <param name="onlyMatchingAddress">If true, only addresses, which are the same as prevCustAddress, will be replaced by newAddress. Otherwise it will replace all addresses.</param>
        /// <param name="updReminderAndDebt">If true, it will update also reminder and debt collection addresses. If false, only invoicing addresses will be updated.</param>
        /// <param name="updateTempAddresses">If true, old addresses, which are replaced by newAddress, will be moved to list of temp addresses.</param>
        /// <param name="tmpAddrTitle">Title for newly created temporary addresses.</param>
        /// <param name="connection">CConnect object.</param>
        public static void UpdateInvoiceConfAddresses(CustomerVO customer, List<ContractDataVO> contracts, AddressVO newAddress, AddressVO prevCustAddress,
            bool onlyMatchingAddress, bool updReminderAndDebt, bool updateTempAddresses, string tmpAddrTitle, CConnect connection)
        {
            if (customer != null && customer.CustomerCode != null && newAddress != null && newAddress.Valid && (!onlyMatchingAddress || prevCustAddress != null)) // 20101231 dyrdamar : Added check if newAddress is valid (CMONDEV-1104)
            {
                // Let's find customer's invoice configurations.
                InvoiceConfigurationInputVO getConfInput = new InvoiceConfigurationInputVO() { CustomerCode = customer.CustomerCode };
                ControlList<InvoiceConfigurationOutputVO> invoiceConfs = null;
                try
                {
                    invoiceConfs = GetInvoiceConfiguration(getConfInput, connection);
                }
                catch
                {                    
                    List<int> invConfIDs = GetInvoiceConfigurationsIDs(customer, connection);
                    invoiceConfs = new ControlList<InvoiceConfigurationOutputVO>();
                    foreach (int invConfID in invConfIDs)
                    {
                        getConfInput.InvConfId = invConfID;
                        ControlList<InvoiceConfigurationOutputVO> invoiceConfsTemp = GetInvoiceConfiguration(getConfInput, connection);
                        if (invoiceConfsTemp != null) 
                            invoiceConfs.AddRange(invoiceConfsTemp);
                    }
                }

                if (invoiceConfs != null)
                {
                    // Let's prepare XML part for new address data. Probably it will be used more than once.
                    string newAddressXml = AddInvoiceConfigurationAddressInnerData(newAddress);

                    // Now we will create dictionary of contracts, where key id invoice config id.
                    Dictionary<int, ContractDataVO> dictInvIdContract = null;
                    if (updateTempAddresses)
                    {
                        dictInvIdContract = new Dictionary<int, ContractDataVO>();
                        if (contracts != null)
                        {
                            foreach (ContractDataVO contract in contracts)
                            {
                                if (contract.InvoiceConfigID.HasValue)
                                {
                                    ContractDataVO tmpContract;
                                    if (dictInvIdContract.TryGetValue(contract.InvoiceConfigID.Value, out tmpContract))
                                    {
                                        if (contract.ContractStart.HasValue && (!tmpContract.ContractStart.HasValue ||
                                            (contract.ContractStart.Value < tmpContract.ContractStart.Value)))
                                        {
                                            dictInvIdContract[contract.InvoiceConfigID.Value] = contract;
                                        }
                                    }
                                    else
                                        dictInvIdContract[contract.InvoiceConfigID.Value] = contract;
                                }
                            }
                        }
                    }

                    // Let's loop through all invoice configurations.
                    foreach (InvoiceConfigurationOutputVO conf in invoiceConfs)
                    {
                        if (conf != null)
                        {
                            bool updateInvAddress = true;
                            // 20101229 dyrdamar : Not needed - we will use updReminderAndDebt parameter
                            //bool updateReminderAddress = updReminderAndDebt;
                            //bool updateDebtAddress = updReminderAndDebt;

                            if (onlyMatchingAddress)
                            {
                                // If onlyMatchingAddress = true, we will update InvoicingAddress, only if it's the same as prevCustAddress.
                                if ((conf.InvoicingAddress != null) && !conf.InvoicingAddress.CompareNormalized(prevCustAddress))
                                    updateInvAddress = false;
                                // 20101229 dyrdamar : Reminder and Debt Collection addresses should be updated if Invoicing Address is the same as prevCustAddress
                                // We will update reminder and debt collection addresses, only if updReminderAndDebt = true and these addresses are the same
                                // as prevCustAddress (because onlyMatchingAddress = true).
                                //if (updReminderAndDebt)
                                //{
                                //    if ((conf.ReminderAddress != null) && !conf.ReminderAddress.CompareNormalized(prevCustAddress))
                                //    {
                                //        updateReminderAddress = false;
                                //    }
                                //    if ((conf.DebtCollectionAddress != null) && !conf.DebtCollectionAddress.CompareNormalized(prevCustAddress))
                                //    {
                                //        updateDebtAddress = false;
                                //    }
                                //}
                            }

                            // We will update this invoice configuration, only if at least one of addresses (invoice, reminder, or debt) is going to be updated.
                            if (updateInvAddress)
                            {
                                if (conf.TemporaryAddressList == null)
                                    conf.TemporaryAddressList = new ControlList<TemporaryAddressVO>();

                                ContractDataVO contract = null;
                                if (updateTempAddresses)
                                {
                                    // If we want to update also temporary addresses, we will have to find contract related to invoice configuration.
                                    // It will be simple fetched from prearranged dictionary.
                                    if (conf.InvConfNo.HasValue)
                                        dictInvIdContract.TryGetValue(conf.InvConfNo.Value, out contract);

                                    // Let's sort existing tmp addresses by StartDate, beacuse it's necessary for CreateTemporaryAddress function, which uses this
                                    // ordination for finding of last temp address.
                                    conf.TemporaryAddressList.Sort(delegate(TemporaryAddressVO a1, TemporaryAddressVO a2)
                                    {
                                        if (a1.StartDate.HasValue)
                                        {
                                            if (a2.StartDate.HasValue)
                                                return a1.StartDate.Value.CompareTo(a2.StartDate.Value);
                                            else
                                                return 1;
                                        }
                                        else if (a2.StartDate.HasValue)
                                            return -1;
                                        else
                                            return 0;
                                    });
                                }

                                StringBuilder sbXml = new StringBuilder();
                                sbXml.Append("<TransportObject>");
                                sbXml.Append("<RequestData>");

                                sbXml.AppendFormat("<InvConfNo>{0}</InvConfNo>", conf.InvConfNo);

                                if (updateInvAddress)
                                {
                                    // Invoice address is going to be updated.
                                    sbXml.AppendFormat("<InvoicingAddress>{0}</InvoicingAddress>", newAddressXml);
                                    // If updateTempAddresses = true and previous invoice address exists, it will be stored to the temporary address list.
                                    if (updateTempAddresses && conf.InvoicingAddress != null)
                                        conf.TemporaryAddressList.Add(CreateTemporaryAddress(conf.TemporaryAddressList, conf.InvoicingAddress, (int)eTempAddTypes.InvoiceAddress, tmpAddrTitle, contract));
                                }
                                if (updReminderAndDebt)
                                {
                                    // Reminder address is going to be updated.
                                    sbXml.AppendFormat("<ReminderAddress>{0}</ReminderAddress>", newAddressXml);
                                    // If updateTempAddresses = true and previous reminder address exists, it will be stored to the temporary address list.
                                    if (updateTempAddresses && conf.ReminderAddress != null)
                                        conf.TemporaryAddressList.Add(CreateTemporaryAddress(conf.TemporaryAddressList, conf.ReminderAddress, (int)eTempAddTypes.ReminderAddress, tmpAddrTitle, contract));
                                
                                    // Debt Collection address is going to be updated.
                                    sbXml.AppendFormat("<DebtCollectionAddress>{0}</DebtCollectionAddress>", newAddressXml);
                                    // If updateTempAddresses = true and previous debt collection address exists, it will be stored to the temporary address list.
                                    if (updateTempAddresses && conf.DebtCollectionAddress != null)
                                        conf.TemporaryAddressList.Add(CreateTemporaryAddress(conf.TemporaryAddressList, conf.DebtCollectionAddress, (int)eTempAddTypes.DebtCollectionAddress, tmpAddrTitle, contract));
                                }

                                // Save temporary address list.
                                sbXml.Append(AddInvoiceConfigurationTemporaryAddressData(conf.TemporaryAddressList));

                                sbXml.Append("</RequestData>");
                                sbXml.Append("</TransportObject>");

                                ComCWCFCall("UpdateInvoiceConfiguration", sbXml.ToString(), connection, ComCWCFSite.Invoicing);
                            }
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Method for updating of invoice configuration.
        /// </summary>
        /// <param name="config">Invoice configuration, which will be updated.</param>
        /// <param name="connection">CConnect object.</param>
        public static void UpdateInvoiceConfiguration(UpdateInvoiceConfigurationDataVO config, CConnect connection)
        {
            if (config.PayTermId.HasValue)
            {
                if (!config.PayTermStartDate.HasValue)
                    throw new ArgumentNullException();
            }

            StringBuilder sbXml = new StringBuilder();
            sbXml.Append("<TransportObject>");
            sbXml.Append("<RequestData>");

            if (config.InvConfNo.HasValue)
                sbXml.AppendFormat("<InvConfNo>{0}</InvConfNo>", config.InvConfNo);
            if (config.PayTermId.HasValue)
                sbXml.AppendFormat("<PayTermId>{0}</PayTermId>", config.PayTermId);
            if (config.PayTermStartDate.HasValue)
                sbXml.AppendFormat("<PayTermStartDate>{0}</PayTermStartDate>", config.PayTermStartDate.Value.ToString());
            if (config.InvoicePeriodId.HasValue)
                sbXml.AppendFormat("<InvoicePeriodId>{0}</InvoicePeriodId>", config.InvoicePeriodId);
            if (config.InvoiceGroupId.HasValue)
                sbXml.AppendFormat("<InvoiceGroupId>{0}</InvoiceGroupId>", config.InvoiceGroupId);
            if (config.InvoiceSendMethod.HasValue)
                sbXml.AppendFormat("<InvoiceSendMethod>{0}</InvoiceSendMethod>", config.InvoiceSendMethod);
            if (config.IsAdvanceNoticeCalc.HasValue)
                sbXml.AppendFormat("<IsAdvanceNoticeCalc>{0}</IsAdvanceNoticeCalc>", config.IsAdvanceNoticeCalc);
            if (config.InvoicingAddress != null)
                sbXml.Append(AddInvoiceConfigurationAddressData(config.InvoicingAddress, "InvoicingAddress"));
            if (config.ReminderAddress != null)
                sbXml.Append(AddInvoiceConfigurationAddressData(config.ReminderAddress, "ReminderAddress"));
            if (config.DebtCollectionAddress != null)
                sbXml.Append(AddInvoiceConfigurationAddressData(config.DebtCollectionAddress, "DebtCollectionAddress"));
            if (config.TemporaryAddressList != null)
                sbXml.Append(AddInvoiceConfigurationTemporaryAddressData(config.TemporaryAddressList));

            sbXml.Append("</RequestData>");
            sbXml.Append("</TransportObject>");

            ComCWCFCall("UpdateInvoiceConfiguration", sbXml.ToString(), connection, ComCWCFSite.Invoicing);
        }
    }
}
