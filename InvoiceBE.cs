using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DCS.Data.TransferObjects;
using DCS.Data.Controls;
using DCS.Common;
using System.Collections;

namespace DCS.Business.Entities
{
    internal class InvoiceconfigurationBE
    {
        internal static void Save(InvoiceconfigurationTO invConf)
        {
            InvoiceControl.Save(invConf);
        }

        internal static List<InvoiceGroupsTO> GetInvoiceGroups(double consumptionEstimation, string CustomerType)
        {
            // list of all invoice groups from CAB
            List<InvoiceGroupsTO> allInvoiceGroups = InvoiceControl.GetInvoiceGroups();
            Hashtable input = new Hashtable();
            input["YearConsumptionMin"] = consumptionEstimation;
            input["YearConsumptionMax"] = consumptionEstimation;
            Hashtable output;
            try
            {
                output = RuleManager.GetRule("InvoiceGroupsConsumption", input, RuleManagerError.ThrowAnError);
            }
            catch (Exception exc)
            {
                LogManager.Error(exc.Message);
                return null;
            }            
            // list of invoice group names corresponding to the consumption estimation criteria
            List<string> invoiceGroupsFromRules = null;
            // marttmik 20120203 : cmondev-2327
            if (output == null || output["InvoiceGroups"] == null)
                invoiceGroupsFromRules = new List<string>();
            else
                invoiceGroupsFromRules = output["InvoiceGroups"].ToString().Split(',').ToList<string>(); // marttmik 20120203 : cmondev-2327
            //List<string> invoiceGroupsFromRules = output["InvoiceGroups"].ToString().Split(',').ToList<string>();            
            
            // result in "list" contains only those invoice group names which are both in CAB and in rules
            List<InvoiceGroupsTO> invoiceGroupsList = (from p in allInvoiceGroups where invoiceGroupsFromRules.Contains(p.Name) select p).ToList();
            Hashtable invoicesPerYearInput;
            Hashtable invoicesPerYearoutput;
            List<InvoiceGroupsTO> resultList = new List<InvoiceGroupsTO>();
            foreach (InvoiceGroupsTO invoiceGroup in invoiceGroupsList)
            {
                invoicesPerYearInput = new Hashtable();
                invoicesPerYearInput["Name"] = invoiceGroup.Name;
                invoicesPerYearInput["CustomerType"] = CustomerType;
                try
                {
                    invoicesPerYearoutput = new Hashtable();
                    invoicesPerYearoutput = RuleManager.GetRule("InvoiceGroups", invoicesPerYearInput, RuleManagerError.ThrowAnError);
                }
                catch (Exception exc)
                {
                    LogManager.Error(exc.Message);
                    return null;
                }
                if (invoicesPerYearoutput != null)
                {
                    InvoiceGroupsTO resultInvoiceGroup = new InvoiceGroupsTO();
                    resultInvoiceGroup.Id = invoiceGroup.Id;
                    resultInvoiceGroup.Name = invoiceGroup.Name;
                    string invoicesPerYearText;
                    if (invoicesPerYearoutput["Text_" + Utils.GetLangCode()] != null)
                        invoicesPerYearText = invoicesPerYearoutput["Text_" + Utils.GetLangCode()].ToString();
                    else
                        invoicesPerYearText = invoicesPerYearoutput["Text_ENG"].ToString();
                    resultInvoiceGroup.InvoicesPerYearText= invoicesPerYearText;
                    resultList.Add(resultInvoiceGroup);
                }
            }
            
            return resultList;
        }


        internal static List<InvoiceCountTO> GetInvoiceCount(int productType, double consumptionEstimation)
        {
            List<InvoiceCountTO> result = new List<InvoiceCountTO>();
            Hashtable input = new Hashtable();
            input["YearConsumptionMin"] = consumptionEstimation;
            input["YearConsumptionMax"] = consumptionEstimation;
            input["ProductType"] = productType;
            List<Hashtable> output;
            try
            {
                output = RuleManager.GetRules("InvoiceCount", input, RuleManagerError.ThrowAnError, RuleSortOrder.Match);
            }
            catch (Exception exc)
            {
                LogManager.Error(exc.Message);
                return null;
            }
           
            if (output != null)
            {
                foreach (Hashtable rule in output)
                {
                    InvoiceCountTO invoiceCount = new InvoiceCountTO();
                    int invoices;
                    if (int.TryParse(rule["Invoices"].ToString(), out invoices))
                        invoiceCount.Invoices = invoices;
                    else
                        return null;
                    string text;
                    if (rule["Text_" + Utils.GetLangCode()] != null)
                        text = rule["Text_" + Utils.GetLangCode()].ToString();
                    else
                        text = rule["Text_ENG"].ToString();
                    invoiceCount.Text = text;
                    result.Add(invoiceCount);
                }
                return result;
            }
            return null;
        }

        internal static List<InvoiceGroupsTO> GetInvoiceCountInvoiceGroup(string invoices, string gridArea, string productType, bool oddMonth)
        {
            List<InvoiceGroupsTO> allInvoiceGroups = InvoiceControl.GetInvoiceGroups();
            string monthType;
            if (oddMonth)
                monthType = "Odd";
            else
                monthType = "Even";
            List<InvoiceGroupsTO> resultList = new List<InvoiceGroupsTO>();            
            string result = string.Empty;
            Hashtable input = new Hashtable();
            input["Invoices"] = invoices;
            input["GridArea"] = gridArea;
            input["ProductType"] = productType;
            input["StartMonth"] = monthType;
            Hashtable output;
            try
            {
                output = RuleManager.GetRule("InvoiceCountInvoiceGroup", input, RuleManagerError.ThrowAnError);
            }
            catch (Exception exc)
            {
                LogManager.Error(exc.Message);
                return null;
            }
            string  invoiceGroupsFromRules = string.Empty;
            if (! (output == null ))
                invoiceGroupsFromRules = output["InvoiceGroups"].ToString();
            //else
              //  invoiceGroupsFromRules = output["InvoiceGroups"].ToString().Split(',').ToList<string>();
            //foreach (string invoiceGroupName in invoiceGroupsFromRules)
            if (!string.IsNullOrEmpty(invoiceGroupsFromRules))
            {
                InvoiceGroupsTO invoiceGroup = allInvoiceGroups.Find(x => x.Name == invoiceGroupsFromRules);
                if (invoiceGroup != null)
                    resultList.Add(invoiceGroup);
            }
            return resultList;                        
        }
        
    }
}