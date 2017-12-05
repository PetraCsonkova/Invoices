using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCS.Data.TransferObjects;
using DCS.Common;

namespace DCS.Data.Controls
{
    internal interface IInvoiceControl
    {
        List<InvoiceconfigurationTO> GetInvoiceconfigurations(CustomerTO customer);
        InvoiceconfigurationTO GetInvoiceconfiguration(int invoiceConfigId);
        void Save(InvoiceconfigurationTO invConf);
        List<InvoiceGroupsTO> GetInvoiceGroups();
        List<PayTermTO> GetPayTerms();
    }

    internal class InvoiceFactory
    {
        internal static IInvoiceControl GetInterface()
        {
            string system = (string)ConfigurationManager.GetConfiguration("CISSystem");
            if (String.IsNullOrEmpty(system))
                throw new Exception("CISSystem is not configured in App.Config");
            switch (system.ToUpper())
            {
                case "CAB":
                    return new DCS.Data.Controls.CAB.InvoiceControl();
                default:
                    throw new Exception(String.Format("Unknown backend system {0} for ContractControl", system));
            }
        }
    }
    
    public class InvoiceControl
    {
        public static List<InvoiceconfigurationTO> GetInvoiceconfigurations(CustomerTO customer)
        {
            IInvoiceControl control = InvoiceFactory.GetInterface();
            return control.GetInvoiceconfigurations(customer);
        }

        public static InvoiceconfigurationTO GetInvoiceconfiguration(int invoiceConfigId)
        {
            IInvoiceControl control = InvoiceFactory.GetInterface();
            return control.GetInvoiceconfiguration(invoiceConfigId);
        }

        public static void Save(InvoiceconfigurationTO invConf)
        {
            IInvoiceControl control = InvoiceFactory.GetInterface();
            control.Save(invConf);
        }

        public static List<InvoiceGroupsTO> GetInvoiceGroups()
        {
            IInvoiceControl control = InvoiceFactory.GetInterface();
            return control.GetInvoiceGroups();
        }

        public static List<PayTermTO> GetPayTerms()
        {
            IInvoiceControl control = InvoiceFactory.GetInterface();
            return control.GetPayTerms();
        }

    }
}
