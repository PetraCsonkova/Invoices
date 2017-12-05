using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DCS.Data.TransferObjects
{
    public enum InvoicesPerYearMode
    {
        InvoiceGroupsConsumption = 0,
        InvoiceCountInvoiceGroup = 1
    }
    
    public class InvoiceconfigurationTO
    {
        public int? Id { get; set; }
        public int? PayTermId { get; set; }
        public string PayTermName { get; set; }
        public string PeriodTypeCode { get; set; }
        public string PeriodTypeDesc { get; set; }
        public int? InvoiceGroupId { get; set; }
        public string InvoiceGroupName { get; set; }
        public int? InvoicePeriodRuleId { get; set; }
        public string InvoicePeriodRuleName { get; set; }
        public int? InvoiceSendMethod { get; set; }
        public int? IsAdvanceNoticeCalc { get; set; }
        public AddressTO InvoicingAddress { get; set; }
        public AddressTO ReminderAddress { get; set; }
        public AddressTO DebtCollectionAddress { get; set; }

        public void Save()
        {
            Controls.InvoiceControl.Save(this);
        }
    }

    public class InvoiceInformationTO
    {
        public int InvoiceGroupId { get; set; }
        public string InvoicesPerYear { get; set; }
        public string PayTermCode { get; set; }
        public string DueDateName { get; set; }
        public AddressTO Address { get; set; }
        public bool UpdateAllContactInfo { get; set; }
        public string PossibleAdditionalInfo { get; set; }
        public string SBonus { get; set; }
    }

    public class InvoiceGroupsTO 
    {
        public int Id { get; set; }
        public string Name { get; set; }       
        public string InvoicesPerYearText { get; set; }
    }

    public class InvoiceCountTO
    {
        public int Invoices { get; set; }
        public string Text { get; set; }
        
    }
}