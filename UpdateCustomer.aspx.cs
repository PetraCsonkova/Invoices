using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using TietoEnator.Common.CustomerInfoValidator;
using TietoEnator.WebFW;
using TietoEnator.WebFW.CM;
using TietoEnator.WebFW.CM.Controls;
using TietoEnator.WebFW.CM.MessageHandling;

public partial class Pages_contract_NewContract_MaintainCustomer_UpdateCustomer : CMPage
{
    //20080514 marttmik : added a variable used for getting company specific images / styles etc.
    public string COMPANY_CODE;
    private const string CACHE_INTERESTS = "_INTERESTS_";
    private const string NO_ADDRESS_CHANGE = "NoChanges";
    private const string SITE_ADDRESS_TO_MAIN = "SiteToMain";
    private const string NEW_MAIN_ADDRESS = "NewAddress";

    private bool FutureAddrsLicensed
    {
        get
        {
           return IsLicensed("FutureAddresses");
           // return false;
        }
    }

    protected override void PageLoadAction(object sender, EventArgs e)
    {       
        COMPANY_CODE = Session["__CompanyCode__"].ToString();
        // 20100604 dyrdamar: set if country selection drop down list is enabled (CMONDEV-207)
        ddlCountry.Enabled = CMConstants.ALLOW_ADDRESS_COUNTRY_CHANGING;
        if (FutureAddrsLicensed)
        {
            if (!IsPostBack)
            {
                string defaultAccordion = SteeringInfoControl.FetchSteeringInfoByIdentifyCode(CMConstants.STEER_COMMON_SETTINGS,
                                                                            "MAKE_FUTURE_ADDRESS_DEFAULT_SHOWN_IN_ACCORDION",
                                                                           int.Parse(Session["LanguageID"].ToString()), false);
                if (defaultAccordion != null && defaultAccordion.Equals("true"))
                    accAddresses.SelectedIndex = 1;
            }
            apFutureAddress.Visible = true;

        }
        if (app.Process.Compare(typeof(PRAcceptContract)))
        {
            bSecurity.Visible = true;
        }
        // if process is PREditCustomer then hide continue and back buttons and show save and cancel
        if (app.Process.Compare(typeof(PREditCustomer)) || app.Process.Compare(typeof(PRContractingParties)))
        {
            bNext.Visible = false;
            bBack.Visible = false;
            bSave.Visible = true;
            bCancel.Visible = true;
            rblMainAddress.Visible = false;
            // 20120410 RaVa - disable hiding
            //lblMainAddress.Visible = false;

            // 20110104 dyrdamar : Allow longer async post back for long updates
            ScriptManager sm = ScriptManager.GetCurrent(this);
            sm.AsyncPostBackTimeout = 300;
        }
        else
        {
            bNext.Visible = true;
            bBack.Visible = true;
            bSave.Visible = false;
            bCancel.Visible = false;
        }
        // show invoice configuration update options
        if (app.Process.Compare(typeof(PREditCustomer)))
        {
            cmUpdateInvoiceConfigAddress.Visible = true;
        }
        // do not validate following fields in Contracting Parties process
        if (app.Process.Compare(typeof(PRContractingParties)))
        {
            TxtCustomerFirstNameValidator.ValidateEmptyText = false;
            TxtOrgNoValidator.ValidateEmptyText = false;
            TxtOrgNoValidator2.ValidateEmptyText = false;
        }

        if (!IsPostBack)
        {
            DeliverySiteDataVO deliverysite = cache.Process["DeliverySite"] as DeliverySiteDataVO;
            Z03DataVO z03data = cache.Process["Z03Data"] as Z03DataVO;

            FillDropDownLists();

            // 20100312 LKo: Check whick bank account format is used
            BankAccountFormat bafFormat = InvoiceControl.FetchBankAccountFormatSteeringValue(Session);

            // Set the visibility of national format
            trBankAccountNational.Visible = ((bafFormat == BankAccountFormat.National) || (bafFormat == BankAccountFormat.Both));
            trBankAccountIBAN.Visible = ((bafFormat == BankAccountFormat.IBAN) || (bafFormat == BankAccountFormat.Both));

            cache = WebFWUtil.GetCache(Context);
            // Setting customer information block fields
            if (cache.Session["Customer"] != null)
            {
                CustomerVO customer = (CustomerVO)cache.Session["Customer"];

                if (app.Process.Compare(typeof(PRContractingParties)))
                {
                    CustomerVO mainCustomer = null;
                    if (cache.Session["__StartProcess__"] == null)
                    {
                        AddInfoMessage(String.Format(GLR("RelationshipCreatedSuccessfuly"), customer.CustomerName));

                        string suggestedName = customer.CustomerName;

                        cache.Process["MemberCustomer"] = cache.Session["Customer"];
                        mainCustomer = (CustomerVO)cache.Process["PreviousCustomer"];
                        if (!mainCustomer.CustomerCode.Equals(cache.Session["MainCustomer"].ToString()))
                            mainCustomer = CustomerControl.FetchCustomer(cache.Session["MainCustomer"].ToString(), this.Connect);

                        txtSuggestedCustomerName.Text = (mainCustomer.CustomerName + ", " + suggestedName).Trim();
                        if (txtSuggestedCustomerName.Text.Length > 80) txtSuggestedCustomerName.Text = txtSuggestedCustomerName.Text.Substring(0, 80);
                        tblSuggestedCustomerName.Visible = true;
                    }
                    else
                    {
                        if (!customer.CustomerCode.Equals(cache.Session["MainCustomer"].ToString()))
                        {
                            mainCustomer = CustomerControl.FetchCustomer(cache.Session["MainCustomer"].ToString(), this.Connect);
                            cache.Process["MemberCustomer"] = cache.Session["Customer"];
                        }
                        else cache.Process["MainCustomerUpdate"] = true;
                        cache.Session.Remove("__StartProcess__");
                    }
                    if (mainCustomer != null)
                    {
                        cache.Session["Customer"] = mainCustomer;
                        customer = mainCustomer;
                    }
                }

                if (customer.CustomerType == CMConstants.CUSTOMERTYPE_PRIVATE)
                {
                    trCompanyCustomer.Visible = false;
                    trPrivateCustomer.Visible = true;
                    trGender.Visible = true;
                    txtCustomerFirstName.Text = customer.FirstName;
                    txtCustomerLastName.Text = customer.LastName;
                }
                else
                {
                    trCompanyCustomer.Visible = true;
                    trPrivateCustomer.Visible = false;
                    trGender.Visible = false;
                    txtCustomerName.Text = customer.CustomerName;
                }
                if (customer.PhoneNumbers != null && customer.PhoneNumbers.Count > 0)
                {
                    if (customer.MainPhone != null)
                    {
                        txtDefaultPhone.Text = customer.MainPhone.Number;
                    }
                }
                txtEMail.Text = customer.EmailAddress;
                chkReservedAgainstInfo.Checked = (customer.CommercialsInEmail == 1);
                chkForeignCustomer.Checked = (bool)customer.Foreign;
                if (app.Process.Compare(typeof(PREditCustomer)) || app.Process.Compare(typeof(PRContractingParties)))
                {
                    txtStreetAddress.Text = customer.MainAddress.StreetName;
                    txtStreetNumber.Text = customer.MainAddress.StreetNumber;
                    txtFlatNr.Text = customer.MainAddress.FlatId;
                    txtFloor.Text = customer.MainAddress.Floor;
                    txtPostalCode.Text = customer.MainAddress.PostCode;
                    txtPostOffice.Text = customer.MainAddress.PostalPlace;
                    txtCOAddress.Text = customer.MainAddress.CoAddress;
                    txtAttention.Text = customer.MainAddress.Attention;
                    ddlCountry.SelectedValue = customer.MainAddress.CountryCode;
                    //ddlLanguage.SelectedValue = DropDownListControl.FetchListContents(DropDownListsEnum.LanguageCodes, false, false, Connect).LanguageCodes.Find(delegate(LanguageCodeVO c) { return c.Code.CompareTo(customer.CustomerLanguage) == 0; }).Id.Value.ToString();

                    // 20100604 dyrdamar: unhide customer address fields in customer edit process
                    NewCustomerAddressFieldsVisibility(true);
                }

                // 20101220 dyrdamar : Set update invoice configurations address label (CMONDEV-1104)
                if (app.Process.Compare(typeof(PREditCustomer)))
                    cmUpdateInvoiceConfigAddress.SetCurrentAddressLabel(customer.MainAddressString);

                //dyrdamar 20101105 : Added setting customer language (CMONDEV-927)
                if (!String.IsNullOrEmpty(customer.CustomerLanguage))
                    ddlLanguage.SelectedValue = DropDownListControl.FetchListContents(DropDownListsEnum.LanguageCodes, false, false, Connect).LanguageCodes.Find(delegate(LanguageCodeVO c) { return c.Code.CompareTo(customer.CustomerLanguage) == 0; }).Id.Value.ToString();

                // 20100312 LKo: Check the visibility of the national bank account
                //20110518 bastirob CMONDEV-1595  if ((trBankAccountNational.Visible) &&
                if (!String.IsNullOrEmpty(customer.BankAccNo))//)
                {
                    try
                    {
                        tbCreditAccountNational.Text = BankAccountNoValidator.FormatBankAccountNo(customer.BankAccNo, GetAppSetting("ComC_CountryCode"));
                    }
                    catch (CIValidatorException cive)
                    {
                        tbCreditAccountNational.Text = customer.BankAccNo;
                        this.AddErrorMessage(GLR(cive.ErrMsg));
                    }
                }

                // Set the IBAN
                //20110518 bastirob CMONDEV-1595   if ((trBankAccountIBAN.Visible) &&
                if (!String.IsNullOrEmpty(customer.IBAN))//)
                    tbCreditAccountIBAN.Text = customer.IBAN;

                // Select the BIC
                //20110518 bastirob CMONDEV-1595  if ((trBankAccountIBAN.Visible) &&
                if (!String.IsNullOrEmpty(customer.BIC))//)
                    ddlBIC.SelectedIndex = DropDownListControl.DropDownListVO.BICCodes.FindIndex(delegate(BICCodeVo oBic) { return oBic.BIC == customer.BIC; });

                txtOrgNo.Text = customer.OrgNo;

                if (!String.IsNullOrEmpty(customer.Gender))
                    ddlGender.SelectedValue = customer.Gender;

                if (FutureAddrsLicensed)
                {
                    tblFutureAddresses.Visible = true;
                    PrepareNewFAFirstTime();
                }
                else
                {
                    tblFutureAddresses.Visible = false;
                }

                GetCustomersGroups(customer);
                if (IsLicensed("EnableCustomerNotifications"))
                {
                    GetCustomersNotes(customer);
                    GetCustomersInterests(customer);
                }

                ddlCustomerSegment.SelectedValue = customer.CustomerSegment;

                rblMainAddress.Items.Add(new ListItem(String.Format(GLR(NO_ADDRESS_CHANGE), customer.MainAddressString), NO_ADDRESS_CHANGE, true));
                //legerjir - CMONDEV-2340
                if (app.Process.Compare(typeof(PRNewContract)) || app.Process.Compare(typeof(PRNewCustomer)) || app.Process.Compare(typeof(PRAcceptContract)))
                {
                    if (deliverysite != null)
                    {
                        rblMainAddress.Items.Add(new ListItem(String.Format(GLR(SITE_ADDRESS_TO_MAIN), deliverysite.SiteAddress), SITE_ADDRESS_TO_MAIN, true));
                    }
                    else
                    {
                        rblMainAddress.Items.Add(new ListItem(String.Format(GLR(SITE_ADDRESS_TO_MAIN), z03data.SDelSiteAddress), SITE_ADDRESS_TO_MAIN, true));
                    }
                }
                rblMainAddress.Items.Add(new ListItem(GLR(NEW_MAIN_ADDRESS), NEW_MAIN_ADDRESS, true));
                if (app.Process.Compare(typeof(PRMoveOut)) && FutureAddrsLicensed)
                {
                    rblMainAddress.SelectedIndex = 1;
                    NewCustomerAddressFieldsVisibility(true);
                }
                else
                    rblMainAddress.SelectedIndex = 0;

                // copy customergroups to new list
                List<CustomerGroupVO> tempList = new List<CustomerGroupVO>();
                if (customer.Groups != null && customer.Groups.Count > 0)
                {
                    foreach (CustomerGroupVO group in customer.Groups)
                    {
                        CustomerGroupVO tempGroup = new CustomerGroupVO();
                        tempGroup = group.Copy();
                        tempList.Add(tempGroup);
                    }
                }

                //cache.Page["groups"] = customer.Groups;
                cache.Page["groups"] = tempList;
            }

            if (!IsLicensed("EnableCustomerNotifications"))
            {
                trNotifications.Visible = false;
                trInterests.Visible = false;
                cpeCustomerGroups.Collapsed = false;
            }

            // dyrdamar 20100524 - load customer address settings from cache
            if (cache.Process["UpdateCustomer_AddressRblSelection"] != null)
            {
                string selection = cache.Process["UpdateCustomer_AddressRblSelection"].ToString();

                rblMainAddress.SelectedValue = selection;
                if (selection == NEW_MAIN_ADDRESS)
                {
                    LoadNewCustomerAddress();
                    NewCustomerAddressFieldsVisibility(true);
                }
            }
        }
    }

    protected void BSecurity_Click(object sender, EventArgs e)
    {
        //bool paramOk;
        //paramOk = StoreInvoicingInfo();
        //if (paramOk)
        //{
        app.Process.GoPage(CMConstants.PR_MOVEIN_SECURITYINFO, Context);
        //}
    }

    protected void BNext_Click(object sender, EventArgs e)
    {        
        if (Page.IsValid && CheckValues())
        {
            // save changes to customer information
            CustomerVO customer = cache.Session["Customer"] as CustomerVO;
            CustomerInputVO updateCustomer = new CustomerInputVO();
            List<CustomerGroupVO> groups = cache.Page["groups"] as List<CustomerGroupVO>;
            List<NoteVO> notes = cache.Page["notes"] as List<NoteVO>;
            List<NoteVO> interests = cache.Page[CACHE_INTERESTS] as List<NoteVO>;
            UpdateInvoiceConfAddress updateInvConfAddressOptions = null;

            bool bChanged = false;
            bool bAddressChanged = false;
            updateCustomer.CustomerCode = customer.CustomerCode;
            updateCustomer.CustomerType = customer.CustomerType;
            //updateCustomer.Address = customer.MainAddress;
            if (customer.CustomerType == CMConstants.CUSTOMERTYPE_PRIVATE)
            {
                updateCustomer.CustomerFirstName = txtCustomerFirstName.Text;
                updateCustomer.CustomerLastName = txtCustomerLastName.Text;
            }
            else
            {
                updateCustomer.CustomerName = txtCustomerName.Text;
            }
            updateCustomer.OrgNo = txtOrgNo.Text;
            updateCustomer.MainPhoneNo = txtDefaultPhone.Text;
            updateCustomer.EmailAddress = txtEMail.Text;
            updateCustomer.CommercialsInEmail = chkReservedAgainstInfo.Checked;

            // dyrdamar 20100524 - clear new customer address tmp cache and save the radio button selection
            cache.Process["UpdateCustomer_NewAddressTmp"] = null;
            cache.Process["UpdateCustomer_AddressRblSelection"] = rblMainAddress.SelectedValue;

            bool updateMainAddress = true;
            if (accAddresses.SelectedIndex == 1 && (app.Process.Compare(typeof(PRNewContract)) || app.Process.Compare(typeof(PRMoveOut))))
            {
                updateMainAddress = false;
                updateCustomer.Address = customer.MainAddress;
            }

            // New main address
            if (updateMainAddress)
            {
                if (rblMainAddress.SelectedValue == NEW_MAIN_ADDRESS || app.Process.Compare(typeof(PREditCustomer)) || app.Process.Compare(typeof(PRContractingParties)))
                {
                    updateCustomer.Address = new AddressVO();
                    updateCustomer.Address.StreetName = txtStreetAddress.Text;
                    updateCustomer.Address.StreetNumber = txtStreetNumber.Text;
                    updateCustomer.Address.FlatId = txtFlatNr.Text;
                    updateCustomer.Address.Floor = txtFloor.Text;
                    updateCustomer.Address.PostCode = txtPostalCode.Text;
                    updateCustomer.Address.CountryCode = ddlCountry.SelectedValue;
                    updateCustomer.Address.PostalPlace = txtPostOffice.Text;
                    updateCustomer.Address.CoAddress = txtCOAddress.Text;
                    updateCustomer.Address.Attention = txtAttention.Text;

                    // save new customer address to cache - needed when going back to this page
                    cache.Process["UpdateCustomer_NewAddressTmp"] = updateCustomer.Address;
                }
                // Site address as main address
                else if (rblMainAddress.SelectedValue == SITE_ADDRESS_TO_MAIN)
                {
                    DeliverySiteDataVO deliverysite = cache.Process["DeliverySite"] as DeliverySiteDataVO;
                    Z03DataVO z03data = cache.Process["Z03Data"] as Z03DataVO;
                    if (deliverysite != null)
                    {
                        updateCustomer.Address = new AddressVO();
                        updateCustomer.Address.StreetName = deliverysite.Address.StreetName;
                        updateCustomer.Address.StreetNumber = deliverysite.Address.StreetNumber;
                        updateCustomer.Address.FlatId = deliverysite.Address.FlatId;
                        updateCustomer.Address.Floor = deliverysite.Address.Floor;
                        updateCustomer.Address.PostCode = deliverysite.Address.PostCode;
                        updateCustomer.Address.CountryCode = deliverysite.Address.CountryCode;
                        updateCustomer.Address.PostalPlace = deliverysite.Address.PostalPlace;
                        updateCustomer.Address.CoAddress = deliverysite.Address.CoAddress;
                        updateCustomer.Address.Attention = deliverysite.Address.Attention;
                    }
                    else
                    {
                        updateCustomer.Address = new AddressVO();
                        updateCustomer.Address.StreetName = z03data.DelSiteAddress.StreetName;
                        updateCustomer.Address.StreetNumber = z03data.DelSiteAddress.StreetNumber;
                        updateCustomer.Address.FlatId = z03data.DelSiteAddress.FlatId;
                        updateCustomer.Address.Floor = z03data.DelSiteAddress.Floor;
                        updateCustomer.Address.PostCode = z03data.DelSiteAddress.PostCode;
                        updateCustomer.Address.CountryCode = z03data.DelSiteAddress.CountryCode;
                        updateCustomer.Address.PostalPlace = z03data.DelSiteAddress.PostalPlace;
                        updateCustomer.Address.CoAddress = z03data.DelSiteAddress.CoAddress;
                        updateCustomer.Address.Attention = z03data.DelSiteAddress.Attention;
                    }
                    bChanged = true;
                }
                else
                {
                    updateCustomer.Address = customer.MainAddress;
                }
            }
            // 20100312 LKo: Check the visibility of the bank account fields
            //20110518 bastirob CMONDEV-1595 if (trBankAccountNational.Visible)
            updateCustomer.BankAccount = tbCreditAccountNational.Text;

            // 20100322 LKo: Check if we need to handle the IBAN and BIC values
            //20110518 bastirob CMONDEV-1595  if (trBankAccountIBAN.Visible)
            //20110518 bastirob CMONDEV-1595  {
            string sBIC = DropDownListControl.DropDownListVO.BICCodes[ddlBIC.SelectedIndex].BIC;

            // Check if both values have been provided
            if ((!String.IsNullOrEmpty(tbCreditAccountIBAN.Text)) && (!String.IsNullOrEmpty(sBIC)))
            {
                updateCustomer.IBAN = InvoiceControl.FormatIBAN(tbCreditAccountIBAN.Text);
                updateCustomer.BIC = sBIC;
            } // Check if only 1 of the values is provided and show an error message
            else if ((!String.IsNullOrEmpty(tbCreditAccountIBAN.Text)) && (String.IsNullOrEmpty(sBIC)) ||
                     (String.IsNullOrEmpty(tbCreditAccountIBAN.Text)) && (!String.IsNullOrEmpty(sBIC)))
            {
                this.AddErrorMessage(GLR("In case of an IBAN account both IBAN and BIC must be provided"));
                return;
            }
            //20110518 bastirob CMONDEV-1595  }

            updateCustomer.CustomerSegment = ddlCustomerSegment.SelectedValue;
            updateCustomer.Foreign = chkForeignCustomer.Checked;
            updateCustomer.LanguageID = ddlLanguage.SelectedValue;
            updateCustomer.Gender = ddlGender.SelectedValue;

            if (customer.FirstName.CompareTo(txtCustomerFirstName.Text) != 0)
            {
                bChanged = true;
            }
            if (customer.LastName.CompareTo(txtCustomerLastName.Text) != 0)
            {
                bChanged = true;
            }
            if (customer.CustomerName.CompareTo(txtCustomerName.Text) != 0 && customer.CustomerType == CMConstants.CUSTOMERTYPE_COMPANY)
            {
                bChanged = true;
            }
            if (customer.OrgNo.CompareTo(txtOrgNo.Text) != 0)
            {
                bChanged = true;
            }
            if (customer.MainPhone != null)
            {
                if (customer.MainPhone.Number.CompareTo(txtDefaultPhone.Text) != 0)
                {
                    bChanged = true;
                }
            }
            if (customer.MainPhone == null && !string.IsNullOrEmpty(txtDefaultPhone.Text))
            {
                bChanged = true;
            }
            if (customer.EmailAddress.CompareTo(txtEMail.Text) != 0)
            {
                bChanged = true;
            }
            if ((customer.CommercialsInEmail == 1) != chkReservedAgainstInfo.Checked)
            {
                bChanged = true;
            }

            if (updateMainAddress)
            {
                if (rblMainAddress.SelectedValue == NEW_MAIN_ADDRESS || app.Process.Compare(typeof(PREditCustomer)) || app.Process.Compare(typeof(PRContractingParties)))
                {
                    if (customer.MainAddress.StreetName.CompareTo(txtStreetAddress.Text) != 0)
                    {
                        bAddressChanged = true;
                    }
                    if (customer.MainAddress.StreetNumber.CompareTo(txtStreetNumber.Text) != 0)
                    {
                        bAddressChanged = true;
                    }
                    if (customer.MainAddress.FlatId.CompareTo(txtFlatNr.Text) != 0)
                    {
                        bAddressChanged = true;
                    }
                    if (customer.MainAddress.Floor.CompareTo(txtFloor.Text) != 0)
                    {
                        bAddressChanged = true;
                    }
                    if (customer.MainAddress.PostCode.CompareTo(txtPostalCode.Text) != 0)
                    {
                        bAddressChanged = true;
                    }
                    if (customer.MainAddress.PostalPlace.CompareTo(txtPostOffice.Text) != 0)
                    {
                        bAddressChanged = true;
                    }
                    if (customer.MainAddress.CountryCode.CompareTo(ddlCountry.SelectedValue) != 0)
                    {
                        bAddressChanged = true;
                    }
                    if (customer.MainAddress.CoAddress.CompareTo(txtCOAddress.Text) != 0)
                    {
                        bAddressChanged = true;
                    }
                    if (customer.MainAddress.Attention.CompareTo(txtAttention.Text) != 0)
                    {
                        bAddressChanged = true;
                    }
                    if (bAddressChanged)
                        bChanged = true;
                }
            }
            if (customer.BankAccNo.CompareTo(tbCreditAccountNational.Text) != 0)
            {
                bChanged = true;
            }
            if (customer.IBAN.CompareTo(tbCreditAccountIBAN.Text) != 0)
            {
                bChanged = true;
            }
            if (customer.CustomerSegment.CompareTo(ddlCustomerSegment.SelectedValue) != 0)
            {
                bChanged = true;
            }
            if (customer.Foreign != chkForeignCustomer.Checked)
            {
                bChanged = true;
            }
            if (customer.CustomerLanguage != DropDownListControl.FetchListContents(DropDownListsEnum.LanguageCodes, false, false, Connect).LanguageCodes.Find(delegate(LanguageCodeVO c) { return c.Id.Value.ToString() == ddlLanguage.SelectedValue; }).Code)
            {
                bChanged = true;
            }
            if (customer.Gender != ddlGender.SelectedValue)
            {
                bChanged = true;
            }

            // 20101216 dyrdamar : Store update invoice configurations options according to selected values (CMONDEV-1104)
            if (app.Process.Compare(typeof(PREditCustomer)))
            {
                updateInvConfAddressOptions = cmUpdateInvoiceConfigAddress.GetOptions(updateCustomer.Address, customer.MainAddress);
                if (!bAddressChanged && updateInvConfAddressOptions != null && updateInvConfAddressOptions.OnlyMatchingAddress) // do not update in this case because address haven't changed
                    updateInvConfAddressOptions = null;
            }

            if ((bChanged) || (sender.Equals(bConfirmSave)))
            {
                // format bank account
                if (!String.IsNullOrEmpty(updateCustomer.BankAccount))
                {
                    updateCustomer.BankAccount = BankAccountNoValidator.GetBankAccountNoToStoreToDB(updateCustomer.BankAccount, GetAppSetting("ComC_CountryCode"));
                }
                else // 20042009 LKo: Req 192599 It should be possible to clear the bank account
                    updateCustomer.BankAccount = "";

                // 20091117 LKo: CM1.4 fix - Check if we need to check the social security code
                if ((!sender.Equals(bConfirmSave)) && (!chkForeignCustomer.Checked)) // Confirm means any social security code is Ok
                {
                    // Variable for the customer list
                    ControlList<CustomerResultVO> clCustomers = null;

                    try
                    {
                        // 20091116 LKo: CM1.4 fix - Method returns the customers with same SSC
                        clCustomers = CustomerControl.CheckForExistingOrgNo(updateCustomer.OrgNo, this.Connect, updateCustomer.CustomerCode, updateCustomer.CustomerType, updateCustomer.Foreign);
                    }
                    catch (CompanyIdException exc)
                    {
                        AddErrorMessage(GLR(exc.Message));
                        return;
                    }

                    // marttmik 2008103 : added check whether the given SSC already exists in CAB (Req #177793)
                    if ((clCustomers != null) && (clCustomers.Count > 0))
                    {
                        // Collect the names of the customers with same social security code
                        string sCustomerNames = CustomerControl.CollectCustomerNameString(clCustomers);

                        // 20091120 LKo: CM1.4 fix - Check if we need to prevent the use of same social security code (VE)
                        if (!CustomerControl.PreventSameSSCEnabled(Convert.ToInt32(Session["LanguageID"])))
                        {
                            // Show the confirm-button instead of next
                            bConfirmSave.Visible = true;
                            bNext.Visible = false;
                            bSave.Visible = false; // 20101220 dyrdamar : this button also should be hidden while Confirm Save is shown
                        }

                        // Set the error message
                        if (updateCustomer.CustomerType == CMConstants.CUSTOMERTYPE_PRIVATE)
                            this.AddErrorMessage(GLR("Customer with the given personal id already exists.") + sCustomerNames);
                        else
                            this.AddErrorMessage(GLR("Company customer with the given company code already exists.") + sCustomerNames);

                        // Get out before the save
                        return;
                    }
                }

                //bastirob CMONDEV-207 2010-05-27 try catch block added
                string newCustomerCode;
                try
                {
                    newCustomerCode = CustomerControl.UpdateCustomer(updateCustomer, this.Connect);
                }
                catch (CMException exc)
                {
                    if (exc.ErrorCode == CMExceptionCodes.COMBINATION_OF_POSTPLACE_AND_CODE_DOESNT_EXIST)
                        AddErrorMessage(GLR("CombinationOfPostcodeAndPlaceDoesntExist"));
                    else
                        AddErrorMessage(GLR("CouldNotUpdateCustomer"));
                    return;
                }
                catch (Exception exc)
                {
                    AddErrorMessage(GLR("CouldNotUpdateCustomer") + ": " + exc.Message);
                    return;
                }
                //bastirob CMONDEV-207 2010-05-27

                // DEBUG
                //string newCustomerCode = customer.CustomerCode;

                // 20101216 dyrdamar : Pass old customer address to next process (CMONDEV-1104)
                if (app.Process.Compare(typeof(PRNewContract)) || app.Process.Compare(typeof(PRNewCustomer)) || app.Process.Compare(typeof(PRMoveOut)))
                {
                    cache.Process["UpdateCustomer_OldCustomerAddress"] = customer.MainAddress;
                }

                customer = CustomerControl.FetchCustomer(newCustomerCode, this.Connect);
                CustomerControl.FetchCustomerGroupList(customer, this.Connect);
                CustomerControl.FetchCustomerNotes(customer, this.Connect);
            }

            // 20101216 dyrdamar : Update invoice configurations according to selected values (CMONDEV-1104)
            if (updateInvConfAddressOptions != null && app.Process.Compare(typeof(PREditCustomer)))
            {
                InvoiceControl.UpdateInvoiceConfAddresses(customer, null, updateInvConfAddressOptions.NewAddress, updateInvConfAddressOptions.PrevCustAddress,
                    updateInvConfAddressOptions.OnlyMatchingAddress, updateInvConfAddressOptions.UpdReminderAndDebt, false, string.Empty, this.Connect);
            }

            if (GroupListChanged(customer.Groups, groups))
            {
                CustomerControl.UpdateCustomerGroups(updateCustomer.CustomerCode, groups, this.Connect);

                // fetch customergroups again because it's become invalid.
                CustomerControl.FetchCustomerGroupList(customer, this.Connect);
            }

            if (gvNotifications.Visible && CustomerControl.NoteListChanged(customer.Notes, notes))
            {
                CustomerControl.UpdateCustomerNotes(updateCustomer.CustomerCode, notes, this.Connect);

                // fetch customernotes again because it's become invalid.
                CustomerControl.FetchCustomerNotes(customer, this.Connect);
            }

            if (trInterests.Visible && CustomerControl.NoteListChanged(customer.Interests, interests))
            {
                CustomerControl.UpdateCustomerInterests(customer, interests, this.Connect);

                customer.Interests = CustomerControl.FetchCustomerInterests(customer, connect);
            }

            //change the customer to member customer in Contracting Parties process
            if (app.Process.Compare(typeof(PRContractingParties)) && cache.Process["MainCustomerUpdate"] == null)
                cache.Session["Customer"] = cache.Process["MemberCustomer"];
            else
                cache.Session["Customer"] = customer;

            if (app.Process.Compare(typeof(PREditCustomer)) || app.Process.Compare(typeof(PRContractingParties)))
            {
                cache.Session[CMConstants.CACHE_MOVING_INFO_MESSAGE] = GLR("CustomerEditedSuccessfully");
            }

            bool updateFutureAddress = true;
            if (accAddresses.SelectedIndex == 0 && (app.Process.Compare(typeof(PRNewContract)) || app.Process.Compare(typeof(PRMoveOut))))
                updateFutureAddress = false;
            if (updateFutureAddress)
                if (!cmFADetailCustomer.InvokeOkClick())
                    return;

            app.Process.GoNextPage(Context);
        }
    }

    private bool GroupListChanged(ControlList<CustomerGroupVO> oldGroups, List<CustomerGroupVO> newGroups)
    {
        bool bRet = false;

        // newGroups should never be null
        if (newGroups == null)
        {
            return false;
        }

        // 20091102 LKo: Req 206478 - The deleted group is not handled correctly IF it didn't originally exist in the database
        if (newGroups.Exists(delegate(CustomerGroupVO cgVo) { return cgVo.Delete == true; }))
        {
            // Variable for the customer group
            CustomerGroupVO cgCustomerGroup;

            // Loop the new items through
            for (int i = 0; i < newGroups.Count; i++)
            {
                // Get the group
                cgCustomerGroup = newGroups[i];

                if (cgCustomerGroup != null)
                {
                    // Check if it has the deleted flag
                    if ((cgCustomerGroup.Delete.HasValue) && (cgCustomerGroup.Delete.Value))
                    {
                        // Remove the flagged item from the list IF it didn't exist in the original
                        if (!oldGroups.Exists(delegate(CustomerGroupVO cgVo) { return cgVo.CustomerGroupName == cgCustomerGroup.CustomerGroupName; }))
                            newGroups.Remove(cgCustomerGroup);
                    }
                }
            }
        }

        // if old groups and new groups count differ we must update
        if (oldGroups.Count != newGroups.Count)
        {
            return true;
        }

        // if the memberno in groups is changed we must update
        foreach (CustomerGroupVO group in oldGroups)
        {
            if (!newGroups.Exists(delegate(CustomerGroupVO cgVo) { return cgVo.CustomerGroupName == group.CustomerGroupName && cgVo.MemberNo == group.MemberNo; }))
            {
                bRet = true;
            }
        }

        // if there's delete flag up in some of the groups we must udpate
        if (oldGroups.Exists(delegate(CustomerGroupVO cgVo) { return cgVo.Delete == true; }) ||
            newGroups.Exists(delegate(CustomerGroupVO cgVo) { return cgVo.Delete == true; }))
        {
            bRet = true;
        }

        return bRet;
    }

    protected void BBack_Click(object sender, EventArgs e)
    {
        // dyrdamar 20100524 - store address information into cache
        ClearNewCustomerAddressCache();

        if (app.Process.Compare(typeof(PRNewCustomer)))
        {
            // 20042009 LKo: Req 192599 Go back to the deliverysite selection (skip create new customer)
            app.Process.GoPage(CMConstants.PR_NEWCONTRACT_DELIVERYSITE, Context);
        }
        else
        {
            app.Process.GoPreviousPage(Context);
        }
    }

    protected void BCancel_Click(object sender, EventArgs e)
    {
        if (app.Process.Compare(typeof(PRContractingParties)) && cache.Process["MainCustomerUpdate"] == null)
            cache.Session["Customer"] = cache.Process["MemberCustomer"];
        // add message about canceling
        StartProcess("PRCustomerInfo");
    }

    protected void sbSuggestedCustomerName_Click(object sender, EventArgs e)
    {
        CustomerVO customer = (CustomerVO)cache.Session["Customer"];
        if (customer.CustomerType == CMConstants.CUSTOMERTYPE_PRIVATE)
        {
            txtCustomerFirstName.Text = String.Empty;
            txtCustomerLastName.Text = String.Empty;

            if (txtSuggestedCustomerName.Text.Length <= 50)
            {
                txtCustomerLastName.Text = txtSuggestedCustomerName.Text;
            }
            else
            {
                string[] names = txtSuggestedCustomerName.Text.Replace("  ", " ").Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string name in names)
                {
                    if ((txtCustomerLastName.Text.Length + name.Length) <= 50) txtCustomerLastName.Text += name + ", ";
                    else txtCustomerFirstName.Text += name + ", ";
                }
                if (txtCustomerFirstName.Text.Length > 30) txtCustomerFirstName.Text = txtCustomerFirstName.Text.Substring(0, 30);
                txtCustomerLastName.Text = txtCustomerLastName.Text.TrimEnd(' ').TrimEnd(',');
            }
        }
        else
            txtCustomerName.Text = txtSuggestedCustomerName.Text;
    }

    // 25032009 LKo: Check if parameter group name is a SBonus group
    private bool IsSBonusGroup(string aGroupName)
    {
        // Check that the string is Ok
        if (!String.IsNullOrEmpty(aGroupName))
        {
            // Get the steering data for the SBonus groups
            ControlList<SteeringInfoVO> clSBonusGroups = this.GetSBonusSteeringInfo();

            // Check if string is found from the list
            if (clSBonusGroups != null)
                return (clSBonusGroups.Find(delegate(SteeringInfoVO aSteeringInfo) { return aSteeringInfo.Identifycode == aGroupName; }) != null);
        }

        // Return false by default
        return false;
    }

    // 25032009 LKo: Fetch the steering data for the SBonus groups
    private ControlList<SteeringInfoVO> GetSBonusSteeringInfo()
    {
        // Result variable
        ControlList<SteeringInfoVO> clSBonusInfoList = null;

        // Fetch the SBonus steering group information
        SteeringInfoData sidSBonusInfo = SteeringInfoControl.FetchSteeringInfoValues(CMConstants.STEER_SBONUS_GROUPS, "", "", Convert.ToInt32(Session["LanguageID"]), true, false);

        // Create the result list for the info
        if ((sidSBonusInfo != null) && (sidSBonusInfo.SteeringInfo != null))
            clSBonusInfoList = sidSBonusInfo.SteeringInfo;

        // Return the result list
        return clSBonusInfoList;
    }

    private bool CheckSBonusValues(string aGroupCode)
    {
        // Clear the spaces
        string sValueToCheck = aGroupCode.Replace(" ", "");

        // Check the length of the string
        if (sValueToCheck.Length != 10)
            this.AddInfoMessage(GLR("SBonus_IdNumberFormatError"));
        else
        {
            if (!ControlUtil.IsNumeric(sValueToCheck))
                this.AddInfoMessage(GLR("SBonus_IdNumberFormatError"));
            else
            {
                // Get the local S-market rules
                IWebFWRule rSMarketRule = GetRule("LocalSMarkets");

                if ((rSMarketRule != null) && (rSMarketRule.Valid))
                {
                    // Get the SMarket rules
                    DataRow[] drSMarketRules = rSMarketRule.Query(sValueToCheck.Substring(0, 4));

                    if (drSMarketRules != null)
                    {
                        // Check if returned array is empty
                        if (drSMarketRules.Length == 0)
                            this.AddInfoMessage(GLR("SBonus_LocalStoreIdNotFound"));
                        else
                            return true;	// Syntax is Ok and local area was found
                    }
                    else
                        this.AddInfoMessage(GLR("SBonus_LocalRulesMissing"));
                }
                else
                    this.AddInfoMessage(GLR("SBonus_LocalRulesMissing"));
            }
        }

        // Return false by default
        return false;
    }

    protected void CmdAddGroup_Click(object sender, EventArgs e)
    {
        // 24032009 LKo: Check if the group is for S-bonus OR S-bonus main (28042009 LKo: Req 192218)
        if (IsSBonusGroup(ddlNewCustomerGroups.SelectedItem.Text))
            if (!CheckSBonusValues(txtNewMembershipCode.Text.Trim()))	// Check the values
                return;

        List<CustomerGroupVO> groups = cache.Page["groups"] as List<CustomerGroupVO>;

        CustomerGroupVO group = null;

        if (groups != null)
            group = groups.Find(delegate(CustomerGroupVO cg) { return cg.CustomerGroupName == ddlNewCustomerGroups.SelectedItem.Text; });

        if (group == null)
        {
            CustomerGroupVO newGroup = new CustomerGroupVO();
            newGroup.CustomerGroupName = ddlNewCustomerGroups.SelectedItem.Text;
            newGroup.CustomerGroupValue = ddlNewCustomerGroups.SelectedValue.ToString();
            newGroup.MemberNo = txtNewMembershipCode.Text;
            groups.Add(newGroup);
            gvCustomerGroups.DataSource = groups;
            gvCustomerGroups.DataBind();
            for (int i = 0; i < gvCustomerGroups.Rows.Count; i++)
            {
                if (groups[i].Delete == true)
                {
                    gvCustomerGroups.Rows[i].Visible = false;
                }
            }
        }
        else
        {
            if (group.Delete != null && group.Delete == true)
            {
                group.Delete = false;
                group.MemberNo = txtNewMembershipCode.Text;
                gvCustomerGroups.DataSource = groups;
                gvCustomerGroups.DataBind();
                for (int i = 0; i < gvCustomerGroups.Rows.Count; i++)
                {
                    if (groups[i].Delete == true)
                    {
                        gvCustomerGroups.Rows[i].Visible = false;
                    }
                }
            }
            else
            {
                this.AddInfoMessage(GLR("Group is already in customer's groups"));
            }
        }

        lblCustomerGroupRowsCount.Text = gvCustomerGroups.Rows.Count.ToString();
    }

    protected void gvCustomerGroups_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        //CMONDEV-822 : Delete confirmation dialog for deleting customer group : marttmik 20120618
        gvCustomerGroups.SelectedIndex = e.RowIndex;
        mbConfirmDeleteGroup.Show();
    }

    protected void gvCustomerGroups_RowEditing(object sender, GridViewEditEventArgs e)
    {
        List<CustomerGroupVO> groups = cache.Page["groups"] as List<CustomerGroupVO>;
        gvCustomerGroups.EditIndex = e.NewEditIndex;
        gvCustomerGroups.DataSource = groups;
        gvCustomerGroups.DataBind();
        for (int i = 0; i < gvCustomerGroups.Rows.Count; i++)
        {
            if (groups[i].Delete == true)
            {
                gvCustomerGroups.Rows[i].Visible = false;
            }
        }
    }

    protected void gvCustomerGroups_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        // 07042009 LKo: We need to get the groups for the S-Bonus check
        List<CustomerGroupVO> lGroups = cache.Page["groups"] as List<CustomerGroupVO>;

        // Variable for the customer group name
        string sName = "";

        // Get the selected group
        CustomerGroupVO cgCustomerGroup = lGroups[e.RowIndex];

        // Get the group name
        if (cgCustomerGroup != null)
            sName = cgCustomerGroup.CustomerGroupName;

        TextBox txtGroupCode = gvCustomerGroups.Rows[e.RowIndex].Cells[2].FindControl("txtGroupCode") as TextBox;
        string groupCode = txtGroupCode.Text;

        //CustomerVO customer = (CustomerVO)cache.Session["Customer"];
        List<CustomerGroupVO> groups = cache.Page["groups"] as List<CustomerGroupVO>;
        //ControlList<CustomerGroupVO> groups = customer.Groups;

        // Check if we are updating a S-Bonus group
        if (IsSBonusGroup(sName))
        {
            // Check the entered values
            if (!CheckSBonusValues(groupCode))
            {
                // 28042009 LKo: Req 192218 Restore the original value
                txtGroupCode.Text = groups[e.RowIndex].MemberNo;

                // Get out
                return;
            }
        }

        groups[e.RowIndex].MemberNo = groupCode;

        //customer.Groups = groups;

        //cache.Session["Customer"] = customer;
        gvCustomerGroups.EditIndex = -1;
        gvCustomerGroups.DataSource = groups;
        gvCustomerGroups.DataBind();
        for (int i = 0; i < gvCustomerGroups.Rows.Count; i++)
        {
            if (groups[i].Delete == true)
            {
                gvCustomerGroups.Rows[i].Visible = false;
            }
        }
    }

    private void GetCustomersGroups(CustomerVO customer)
    {
        try
        {
            bool bGroupsFetched = true;
            if (customer.Groups == null)
            {
                bGroupsFetched = CustomerControl.FetchCustomerGroupList(customer, this.Connect);
            }

            // set images to linkbuttons
            CommandField editCmd = (CommandField)gvCustomerGroups.Columns[0];
            editCmd.EditText = string.Format("<img src='/Resources/img/tablearrows/{0}/right_arrow.gif' style='border-width:0px'/>", COMPANY_CODE);
            editCmd.DeleteText = "<img src='/Resources/img/delete.gif' style='border-width:0px'/>";
            editCmd.CancelText = "<img src='/Resources/img/cancel.gif' style='border-width:0px'/>";
            editCmd.UpdateText = "<img src='/Resources/img/update.png' style='border-width:0px'/>";

            gvCustomerGroups.DataSource = customer.Groups;
            gvCustomerGroups.DataBind();
            gvCustomerGroups.DataKeyNames = new String[] { "CustomerGroupName" };
            gvCustomerGroups.DataBind();

            lblCustomerGroupRowsCount.Text = gvCustomerGroups.Rows.Count.ToString();
        }
        catch (Exception ex)
        {
            string s = ex.Message.ToString();
        }
    }

    protected void gvCustomerGroups_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        //CustomerVO customer = (CustomerVO)cache.Session["Customer"];
        List<CustomerGroupVO> groups = cache.Page["groups"] as List<CustomerGroupVO>;

        gvCustomerGroups.EditIndex = -1;
        gvCustomerGroups.DataSource = groups;
        gvCustomerGroups.DataBind();
        for (int i = 0; i < gvCustomerGroups.Rows.Count; i++)
        {
            if (groups[i].Delete == true)
            {
                gvCustomerGroups.Rows[i].Visible = false;
            }
        }
    }

    /// <summary>
    /// Fetches and fills data to drop down lists.
    /// </summary>
    private void FillDropDownLists()
    {
        DropDownListVO dropdownlists = cache.Session["__dropdownlists__"] as DropDownListVO;

        // Customer segments
        if (dropdownlists == null || dropdownlists.CustomerSegments == null || ddlCustomerSegment.Items.Count < 1)
        {
            dropdownlists = DropDownListControl.FetchListContents(DropDownListsEnum.CustomerSegments, this.Connect);
            ddlCustomerSegment.Items.Clear();
            ddlCustomerSegment.DataTextField = "Name";
            ddlCustomerSegment.DataValueField = "Code";
            ddlCustomerSegment.DataSource = dropdownlists.CustomerSegments;
            ddlCustomerSegment.DataBind();
        }

        // Customer groups
        if (dropdownlists == null || dropdownlists.CustomerGroups == null || ddlNewCustomerGroups.Items.Count < 1)
        {
            dropdownlists = DropDownListControl.FetchListContents(DropDownListsEnum.CustomerGroups, this.Connect);
            ddlNewCustomerGroups.Items.Clear();
            ddlNewCustomerGroups.DataTextField = "CustomerGroupName";
            ddlNewCustomerGroups.DataValueField = "CustomerGroupValue";
            ddlNewCustomerGroups.DataSource = dropdownlists.CustomerGroups;
            ddlNewCustomerGroups.DataBind();
        }

        // Countries
        if (dropdownlists == null || dropdownlists.Countries == null || ddlCountry.Items.Count < 1)
        {
            dropdownlists = DropDownListControl.FetchListContents(DropDownListsEnum.Countries, this.Connect);
            ddlCountry.Items.Clear();
            ddlCountry.DataTextField = "Name";
            ddlCountry.DataValueField = "Code";
            ddlCountry.DataSource = dropdownlists.Countries;
            ddlCountry.DataBind();
            ddlCountry.SelectedValue = GetAppSetting("ComC_CountryCode");
        }

        // Languages
        if (dropdownlists == null || dropdownlists.LanguageCodes == null || ddlLanguage.Items.Count < 1)
        {
            dropdownlists = DropDownListControl.FetchListContents(DropDownListsEnum.LanguageCodes, this.Connect);
            ddlLanguage.Items.Clear();
            ddlLanguage.DataTextField = "NameLocal";
            ddlLanguage.DataValueField = "Id";
            ddlLanguage.DataSource = dropdownlists.LanguageCodes;
            ddlLanguage.DataBind();
        }

        // Customer notifications
        if (dropdownlists == null || dropdownlists.CustomerNoteTypes == null || ddlNewNotificationTypes.Items.Count < 1)
        {
            dropdownlists = DropDownListControl.FetchListContents(DropDownListsEnum.CustomerNoteTypes, this.Connect);
            ddlNewNotificationTypes.Items.Clear();
            ddlNewNotificationTypes.DataTextField = "Name";
            ddlNewNotificationTypes.DataValueField = "Id";
            ddlNewNotificationTypes.DataSource = dropdownlists.CustomerNoteTypes;
            ddlNewNotificationTypes.DataBind();
        }

        // BIC codes
        if (dropdownlists == null || dropdownlists.BICCodes == null || ddlBIC.Items.Count < 1)
        {
            dropdownlists = DropDownListControl.FetchListContents(DropDownListsEnum.BICCodes, this.Connect);
            ddlBIC.Items.Clear();
            ddlBIC.DataTextField = "BIC";
            ddlBIC.DataValueField = "Id";
            ddlBIC.DataSource = dropdownlists.BICCodes;
            ddlBIC.DataBind();
        }

        // Available interests
        if (dropdownlists == null || dropdownlists.InterestNoteTypes == null || ddlInterestTypes.Items.Count < 1)
        {
            dropdownlists = DropDownListControl.FetchListContents(DropDownListsEnum.InterestNoteTypes, this.connect);
            ddlInterestTypes.Items.Clear();
            ddlInterestTypes.DataTextField = "Name";
            ddlInterestTypes.DataValueField = "Id";
            ddlInterestTypes.DataSource = dropdownlists.InterestNoteTypes;
            ddlInterestTypes.DataBind();
        }

        // Gender
        if (dropdownlists == null || dropdownlists.Gender == null || ddlGender.Items.Count < 1)
        {
            dropdownlists = DropDownListControl.FetchListContents(DropDownListsEnum.Gender, this.connect);
            ddlGender.Items.Clear();
            ddlGender.DataTextField = "Name";
            ddlGender.DataValueField = "Code";
            ddlGender.DataSource = dropdownlists.Gender;
            ddlGender.DataBind();
        }
    }

    protected void txtPostalCode_TextChanged(object sender, EventArgs e)
    {
        selectPostcodeUC.FetchPostPlaces(ddlCountry.SelectedValue, ref txtPostalCode, ref txtPostOffice);
    }

    protected void TxtOrgNoValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (cache.Session["Customer"] != null)
        {
            CustomerVO customer = cache.Session["Customer"] as CustomerVO;
            if (customer.CustomerType == CMConstants.CUSTOMERTYPE_PRIVATE && String.IsNullOrEmpty(args.Value))
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }
    }

    protected void TxtOrgNoValidator2_ServerValidate(object source, ServerValidateEventArgs args)
    {
        CustomerVO customer = cache.Session["Customer"] as CustomerVO;
        if (customer.CustomerType == CMConstants.CUSTOMERTYPE_PRIVATE)
        {
            args.IsValid = CMCommonUtils.IsPersonalIdValid(CMConstants.CUSTOMERTYPE_PRIVATE, args.Value, chkForeignCustomer.Checked);
        }
        else
        {
            args.IsValid = CMCommonUtils.IsPersonalIdValid(CMConstants.CUSTOMERTYPE_COMPANY, args.Value, chkForeignCustomer.Checked);
        }
    }

    protected void TxtCustomerFirstNameValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (cache.Session["Customer"] != null)
        {
            CustomerVO customer = cache.Session["Customer"] as CustomerVO;
            if (customer.CustomerType == CMConstants.CUSTOMERTYPE_PRIVATE && String.IsNullOrEmpty(args.Value))
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }
    }

    protected void TxtCustomerLastNameValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (cache.Session["Customer"] != null)
        {
            CustomerVO customer = cache.Session["Customer"] as CustomerVO;
            if (customer.CustomerType == CMConstants.CUSTOMERTYPE_PRIVATE && String.IsNullOrEmpty(args.Value))
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }
    }

    protected void TxtCustomerNameValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (cache.Session["Customer"] != null)
        {
            CustomerVO customer = cache.Session["Customer"] as CustomerVO;
            if (customer.CustomerType == CMConstants.CUSTOMERTYPE_COMPANY && String.IsNullOrEmpty(args.Value))
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }
    }

    protected void TxtCreditAccountValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            if (source.Equals(CreditAccountValidator1))
            {
                args.IsValid = BankAccountNoValidator.ValidateBankAccountNo(args.Value, GetAppSetting("ComC_CountryCode"));
            }
            else
            {
                // 20100312 LKo: Check for the possible IBAN
                args.IsValid = IBANBankAccountFormatCheck.IBANBankAccountFormatChecker.CheckIBAN(args.Value);
            }
        }
        catch (CIValidatorException civ)
        {
            //TODO: how to translate messages returned from 3rd side components, where all is in ENG ? not possible, only solution is to make translation per possible value or
            // for all exit codes, simpliest solution is to hide additional validation information

            // Add the error message into the page
            //this.AddErrorMessage(GLR(civ.ErrMsg));
            args.IsValid = false;
        }
        catch (Exception e)
        {
            //TODO: how to translate messages returned from 3rd side components, where all is in ENG ? not possible, only solution is to make translation per possible value or
            // for all exit codes, simpliest solution is to hide additional validation information

            // Add the error message into the page
            //this.AddErrorMessage(e.Message);
            args.IsValid = false;
        }
    }

    protected void TxtEmailValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = CMCommonUtils.IsValidEmail(args.Value);
    }

    private bool CheckValues()
    {
        bool validationOk = true;
        if (accAddresses.SelectedIndex == 1 && (app.Process.Compare(typeof(PRNewContract)) || app.Process.Compare(typeof(PRMoveOut))))
        {
            return validationOk;
        }
        // R#152553
        if (txtDefaultPhone.Text.Length > 0 && !CMCommonUtils.checkPhoneNumberWithLen(txtDefaultPhone.Text, 0))
        {
            AddErrorMessage(GLR("Phonenumber is not valid."));
            validationOk = false;
        }
        //marttmik 080805 : added check if user has chosen not to change ones address information (Req #146899)
        //marttmik 080826 : fixed checking of postal code (Req #176528)
        //dyrdamar 20100524 : added street name and number validation (JIRA CMONDEV-201)
        if (app.Process.Compare(typeof(PREditCustomer)) || (rblMainAddress.SelectedItem.Value.ToString().ToUpper() != "NOCHANGES"
            && rblMainAddress.SelectedItem.Value.ToString().ToUpper() != "SITETOMAIN"))
        {
            // validate street
            txtStreetAddress.Text = txtStreetAddress.Text.Trim();
            if (string.IsNullOrEmpty(txtStreetAddress.Text))
            {
                AddErrorMessage(GLR("Street name is required"));
                validationOk = false;
            }

            // validate street number
            txtStreetNumber.Text = txtStreetNumber.Text.Trim();
            // vankorad 257846: Street Number is not mandatory field
            /*
            if (string.IsNullOrEmpty(txtStreetNumber.Text))
            {
                AddErrorMessage(GLR("Street number is required"));
                validationOk = false;
            }
            */

            // validate postcode
            if (string.IsNullOrEmpty(txtPostalCode.Text))
            {
                AddErrorMessage(GLR("Postal code is required"));
                validationOk = false;
            }
            else
            {
                //dyrdamar 20100527 : first if fixes the postal place input value if user changed it directly before button next click (JIRA CMONDEV-201)
                ControlList<PostCodeVO> postcodes = PostCodeControl.FetchPostCodes(txtPostalCode.Text, null, true);
                if (postcodes.Count == 1)
                {
                    txtPostalCode.Text = postcodes[0].Code;
                    txtPostOffice.Text = postcodes[0].Postalplace;
                }
                // CMONDEV-2639 - Resign from using country code validation (validation done on CAB side)
                //else // if (postcodes == null || postcodes.Count == 0)
                //{
                //    if (String.Compare(ddlCountry.SelectedValue, GetAppSetting("ComC_CountryCode")) == 0)
                //    {
                //        AddErrorMessage(GLR("Postal code is invalid"));
                //        validationOk = false;
                //    }
                //}
            }
        }

        return validationOk;
    }

    //20081028 marttmik : added new button(used for confirmation if customer exists with given ssc in moveIn -process)
    protected void bConfirmSave_Click(object sender, EventArgs e)
    {
        // 20091120 LKo: CM1.4 fix - Call the next button click again
        this.BNext_Click(sender, e);
    }

    protected void cmdAddNotification_Click(object sender, EventArgs e)
    {
        List<NoteVO> notes = cache.Page["notes"] as List<NoteVO>;

        NoteVO newNote = new NoteVO();
        newNote.NoteTypesId = int.Parse(ddlNewNotificationTypes.SelectedValue);
        newNote.NoteTypesName = ddlNewNotificationTypes.SelectedItem.Text;
        newNote.Note = txtNewNotification.Text;
        try
        {
            newNote.ValidUntilDate = DateTime.Parse(txtNewValidThru.Text);
        }
        catch
        {
            newNote.ValidUntilDate = null;
        }
        newNote.CreateDate = DateTime.Now;

        notes.Add(newNote);
        gvNotifications.DataSource = notes;
        gvNotifications.DataBind();
        for (int i = 0; i < gvNotifications.Rows.Count; i++)
        {
            if (notes[i].Delete == true)
            {
                gvNotifications.Rows[i].Visible = false;
            }
        }

        lblNotificationRowsCount.Text = gvNotifications.Rows.Count.ToString();
    }

    protected void gvNotifications_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        List<NoteVO> notes = cache.Page["notes"] as List<NoteVO>;
        notes[e.RowIndex].Delete = true;
        gvNotifications.EditIndex = -1;
        gvNotifications.DataSource = notes;
        gvNotifications.DataBind();
        for (int i = 0; i < gvNotifications.Rows.Count; i++)
        {
            if (notes[i].Delete == true)
            {
                gvNotifications.Rows[i].Visible = false;
            }
        }
        cache.Page["notes"] = notes;

        lblNotificationRowsCount.Text = gvNotifications.Rows.Count.ToString();
    }

    protected void gvNotifications_RowEditing(object sender, GridViewEditEventArgs e)
    {
        List<NoteVO> notes = cache.Page["notes"] as List<NoteVO>;
        gvNotifications.EditIndex = e.NewEditIndex;
        gvNotifications.DataSource = notes;
        gvNotifications.DataBind();
        for (int i = 0; i < gvNotifications.Rows.Count; i++)
        {
            if (notes[i].Delete == true)
            {
                gvCustomerGroups.Rows[i].Visible = false;
            }
        }
    }

    protected void gvNotifications_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        List<NoteVO> notes = cache.Page["notes"] as List<NoteVO>;

        gvNotifications.EditIndex = -1;
        gvNotifications.DataSource = notes;
        gvNotifications.DataBind();
        for (int i = 0; i < gvNotifications.Rows.Count; i++)
        {
            if (notes[i].Delete == true)
            {
                gvNotifications.Rows[i].Visible = false;
            }
        }
    }

    protected void gvNotifications_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        List<NoteVO> notes = cache.Page["notes"] as List<NoteVO>;

        TextBox txtNotification = gvNotifications.Rows[e.RowIndex].Cells[2].FindControl("txtNotification") as TextBox;
        notes[e.RowIndex].Note = txtNotification.Text;
        try
        {
            //ASP._pages_common_textbox_ascx txtValidThru = gvNotifications.Rows[e.RowIndex].Cells[3].FindControl("txtValidThru") as ASP._pages_common_textbox_ascx;
            Pages_common_TextBox_ascx txtValidThru = gvNotifications.Rows[e.RowIndex].Cells[3].FindControl("txtValidThru") as Pages_common_TextBox_ascx;
            notes[e.RowIndex].ValidUntilDate = DateTime.Parse(txtValidThru.Text);
        }
        catch (Exception)
        {
            notes[e.RowIndex].ValidUntilDate = null;
        }

        gvNotifications.EditIndex = -1;
        gvNotifications.DataSource = notes;
        gvNotifications.DataBind();
        for (int i = 0; i < gvNotifications.Rows.Count; i++)
        {
            if (notes[i].Delete == true)
            {
                gvNotifications.Rows[i].Visible = false;
            }
        }
    }

    private void GetCustomersNotes(CustomerVO customer)
    {
        try
        {
            bool bNotesFetched = true;
            if (customer.Notes == null)
            {
                bNotesFetched = CustomerControl.FetchCustomerNotes(customer, this.Connect);
            }

            // set images to linkbuttons
            CommandField editCmd = (CommandField)gvNotifications.Columns[0];
            editCmd.EditText = string.Format("<img src='/Resources/img/tablearrows/{0}/right_arrow.gif' style='border-width:0px'/>", COMPANY_CODE);
            editCmd.DeleteText = "<img src='/Resources/img/delete.gif' style='border-width:0px'/>";
            editCmd.CancelText = "<img src='/Resources/img/cancel.gif' style='border-width:0px'/>";
            editCmd.UpdateText = "<img src='/Resources/img/update.png' style='border-width:0px'/>";

            gvNotifications.DataSource = customer.Notes;
            gvNotifications.DataKeyNames = new String[] { "NoteTypesName", "Note", "SValidUntilDate", "SCreateDate" };
            gvNotifications.DataBind();

            lblNotificationRowsCount.Text = gvNotifications.Rows.Count.ToString();
        }
        catch (Exception ex)
        {
            string s = ex.Message.ToString();
        }

        // copy customernotes to new list
        List<NoteVO> tempList = new List<NoteVO>();
        if (customer.Notes != null)
            customer.Notes.ForEach(x => tempList.Add(x.Copy()));
        cache.Page["notes"] = tempList;
    }

    protected void ddlBIC_OnSelected(object sender, EventArgs e)
    {
        // Check that the list is Ok
        if ((DropDownListControl.DropDownListVO.BICCodes != null) && (DropDownListControl.DropDownListVO.BICCodes.Count > 0))
        {
            // Get the list
            ControlList<BICCodeVo> clBicCodes = DropDownListControl.DropDownListVO.BICCodes;

            // Get the selected item
            BICCodeVo bcBicCode = clBicCodes[ddlBIC.SelectedIndex];

            // Set the name
            if (bcBicCode != null)
                lblBICBankName.Text = bcBicCode.BankName;
        }
    }

    // dyrdamar 20100524 - main address radio button list selected index changed event handler (JIRA CMONDEV-201)
    //                   - hide and unhide new address input fields according to user selection
    //                   - store and clear address cache

    #region CustomerNewAddressFieldsHandling

    protected void rblMainAddress_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rblMainAddress.SelectedValue == NEW_MAIN_ADDRESS)
        {
            ClearNewCustomerAddresValues();
            NewCustomerAddressFieldsVisibility(true);
        }
        else NewCustomerAddressFieldsVisibility(false);
    }

    protected void ClearNewCustomerAddresValues()
    {
        txtStreetAddress.Text = string.Empty;
        txtStreetNumber.Text = string.Empty;
        txtFlatNr.Text = string.Empty;
        txtFloor.Text = string.Empty;
        txtPostalCode.Text = string.Empty;
        txtPostOffice.Text = string.Empty;
        txtCOAddress.Text = string.Empty;
        txtAttention.Text = string.Empty;
    }

    protected void NewCustomerAddressFieldsVisibility(bool value)
    {
        CustomerNemAddress_row1.Visible = value;
        CustomerNemAddress_row2.Visible = value;
        CustomerNemAddress_row3.Visible = value;
        CustomerNemAddress_row4.Visible = value;
    }

    protected void LoadNewCustomerAddress()
    {
        if (cache.Process["UpdateCustomer_NewAddressTmp"] != null)
        {
            AddressVO address = cache.Process["UpdateCustomer_NewAddressTmp"] as AddressVO;

            txtStreetAddress.Text = address.StreetName;
            txtStreetNumber.Text = address.StreetNumber;
            txtFlatNr.Text = address.FlatId;
            txtFloor.Text = address.Floor;
            txtPostalCode.Text = address.PostCode;
            txtPostOffice.Text = address.PostalPlace;
            txtCOAddress.Text = address.CoAddress;
            txtAttention.Text = address.Attention;
        }
    }

    protected void ClearNewCustomerAddressCache()
    {
        cache.Process["UpdateCustomer_NewAddressTmp"] = null;
        cache.Process["UpdateCustomer_AddressRblSelection"] = null;
    }

    #endregion CustomerNewAddressFieldsHandling

    #region Customers Interests

    private void BindNotifications()
    {
        List<NoteVO> interests = cache.Page[CACHE_INTERESTS] as List<NoteVO>;
        if (interests != null)
        {
            gvInterests.DataSource = interests;
            gvInterests.DataBind();

            int count = 0;
            for (int i = 0; i < gvInterests.Rows.Count; i++)
            {
                if (interests[i].Delete == true)
                    gvInterests.Rows[i].Visible = false;
                else
                    count++;
            }
            lblInterestsRowsCount.Text = count.ToString();
        }
    }

    private void GetCustomersInterests(CustomerVO customer)
    {
        try
        {
            if (customer.Interests == null)
                customer.Interests = CustomerControl.FetchCustomerInterests(customer, connect);
        }
        catch
        {
            AddErrorMessage(GLR("FetchingCustomersInterestsFailed"));
            return;
        }

        CommandField editCmd = (CommandField)gvInterests.Columns[0];
        editCmd.EditText = string.Format("<img src='/Resources/img/tablearrows/{0}/right_arrow.gif' style='border-width:0px'/>", COMPANY_CODE);
        editCmd.DeleteText = "<img src='/Resources/img/delete.gif' style='border-width:0px'/>";
        editCmd.CancelText = "<img src='/Resources/img/cancel.gif' style='border-width:0px'/>";
        editCmd.UpdateText = "<img src='/Resources/img/update.png' style='border-width:0px'/>";

        gvInterests.DataSource = customer.Interests;
        gvInterests.DataBind();
        lblInterestsRowsCount.Text = customer.Interests.Count.ToString();

        // copy customers interests to new list
        List<NoteVO> tempList = new List<NoteVO>();
        if (customer.Interests != null)
            customer.Interests.ForEach(x => tempList.Add(x.Copy()));
        cache.Page[CACHE_INTERESTS] = tempList;
    }

    protected void btnAddInterest_Click(object sender, EventArgs e)
    {
        List<NoteVO> interests = cache.Page[CACHE_INTERESTS] as List<NoteVO>;
        if (interests != null)
        {
            int noteTypeId;
            if (int.TryParse(ddlInterestTypes.SelectedItem.Value, out noteTypeId))
            {
                DateTime dateTmp;
                NoteVO interest = new NoteVO();
                interest.Id = null; // new item doesn't have id yet
                interest.NoteTypesName = ddlInterestTypes.SelectedItem.Text;
                interest.NoteTypesId = noteTypeId;
                interest.ValidFromDate = DateTime.TryParse(txtInterestValidFrom.Text, out dateTmp) ? (DateTime?)dateTmp : null;
                interest.ValidUntilDate = DateTime.TryParse(txtInterestValidThru.Text, out dateTmp) ? (DateTime?)dateTmp : null;
                interests.Add(interest);
            }
            gvInterests.EditIndex = -1;
            BindNotifications();
        }
    }

    protected void gvInterests_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        List<NoteVO> interests = cache.Page[CACHE_INTERESTS] as List<NoteVO>;
        if (interests != null)
        {
            gvInterests.EditIndex = -1;
            gvInterests.SelectedIndex = e.RowIndex;
            mbConfirmDelete.Show();
        }
    }

    protected void gvInterests_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvInterests.EditIndex = e.NewEditIndex;
        BindNotifications();
    }

    protected void gvInterests_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvInterests.EditIndex = -1;
        BindNotifications();
    }

    protected void gvInterests_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        List<NoteVO> interests = cache.Page[CACHE_INTERESTS] as List<NoteVO>;
        if (interests != null)
        {
            NoteVO interest = interests[e.RowIndex];

            Pages_common_TextBox_ascx txtValidFrom = gvInterests.Rows[e.RowIndex].FindControl("txtValidFrom") as Pages_common_TextBox_ascx;
            Pages_common_TextBox_ascx txtValidThru = gvInterests.Rows[e.RowIndex].FindControl("txtValidThru") as Pages_common_TextBox_ascx;

            DateTime tmpDate;
            DateTime? validFrom = DateTime.TryParse(txtValidFrom.Text, out tmpDate) ? (DateTime?)tmpDate : null;
            DateTime? validThru = DateTime.TryParse(txtValidThru.Text, out tmpDate) ? (DateTime?)tmpDate : null;

            interest.ValidFromDate = validFrom;
            interest.ValidUntilDate = validThru;

            gvInterests.EditIndex = -1;
            BindNotifications();
        }
    }

    #endregion Customers Interests

    protected void mbConfirmDelete_OkClick(object sender, EventArgs e)
    {
        List<NoteVO> interests = cache.Page[CACHE_INTERESTS] as List<NoteVO>;
        if (interests != null)
        {
            interests[gvInterests.SelectedIndex].Delete = true;
            gvInterests.SelectedIndex = -1;
            BindNotifications();
        }
    }


    protected void mbConfirmDeleteGroup_OkClick(object sender, EventArgs e)
    {
        //CustomerVO customer = (CustomerVO)cache.Session["Customer"];
        List<CustomerGroupVO> groups = cache.Page["groups"] as List<CustomerGroupVO>;
        //groups.RemoveAt(e.RowIndex);
        groups[gvCustomerGroups.SelectedIndex].Delete = true;
        gvCustomerGroups.EditIndex = -1;
        gvCustomerGroups.DataSource = groups;
        gvCustomerGroups.DataBind();
        //gvCustomerGroups.Rows[e.RowIndex].Visible = false;
        for (int i = 0; i < gvCustomerGroups.Rows.Count; i++)
        {
            if (groups[i].Delete == true)
            {
                gvCustomerGroups.Rows[i].Visible = false;
            }
        }
        //cache.Session["Customer"] = customer;
        // 20091030 LKo Req 206478: Set the updated list into the cache to prevent the row magically reappearing
        cache.Page["groups"] = groups;
        lblCustomerGroupRowsCount.Text = gvCustomerGroups.Rows.Count.ToString();
}


    #region Future Address

    private void PrepareNewFAFirstTime()
    {
        bool showFAdetail = false;
        if (app.Process.Compare(typeof(PRNewContract)) || app.Process.Compare(typeof(PRMoveOut)))
        {
            
            cmFADetailCustomer.HideOkCancelButtons();
            cmFAListCustomer.HideAddNewButton();
            pnlFutureAddresses.Visible = false;
            imgFutureAddressesColPan.Visible = false;
            lblFutureAddressesRowsCount.Visible=false;
            lblFutureAddresses.Visible = false;
            lblFutureAddressesCount.Visible = false;

            CustomerVO customer = cache.Session["Customer"] as CustomerVO;
            bool? addrAlreadySaved = cache.Process["MoveOutInCustFutureAddrSaved"] as bool?;
            //if (!addrAlreadySaved.HasValue || !addrAlreadySaved.Value) // User didn't filled default opened (proposed) future address yet.
            {
                if (app.Process.Compare(typeof(PRNewContract)))
                {
                    AddressVO newFAAddress = null;
                    DeliverySiteDataVO delSiteAddr = cache.Process["DeliverySite"] as DeliverySiteDataVO;
                    if (delSiteAddr != null) // We will try to get address from delivery site address.
                    {
                        newFAAddress = delSiteAddr.Address;
                    }
                    if (newFAAddress == null) // User perhaps selected external site.
                    {
                        Z03DataVO z03 = cache.Process["Z03Data"] as Z03DataVO;
                        if (z03 != null)
                            newFAAddress = z03.DelSiteAddress;
                    }
                    if (newFAAddress != null)
                    {
                        showFAdetail = true;
                        cmFADetailCustomer.FillFADetailCustomerControl(GLR("Move in"), customer, newFAAddress, null, null, false);
                    }
                }
                else if (app.Process.Compare(typeof(PRMoveOut)))
                {
                    string sEndDate = cache.Process["ContractEndDate"] as string;
                    DateTime endDate;
                    if (sEndDate != null && DateTime.TryParse(sEndDate, out endDate))
                    {
                        //if (endDate > DateTime.Today && customer != null && customer.MainAddress != null)
                        //{ 
                            DateTime from = endDate;                            
                            showFAdetail = true;
                            cmFADetailCustomer.FillFADetailCustomerControl(GLR("Move out"), customer, null, from, null, true);
                        //}
                    }
                }
            }
        }
        if (showFAdetail)
            FAShowDetail();
        else
            FAShowList();
        GetFutureAddresses();
    }

    private void GetFutureAddresses()
    {
        CustomerVO customer = cache.Session["Customer"] as CustomerVO;
        ControlList<FutureAddressVO> futureAddrs = CustomerControl.GetCustomerFutureAddresses(customer, Connect);
        cmFAListCustomer.LoadFutureAddresses(futureAddrs);
        lblFutureAddressesRowsCount.Text = futureAddrs.Count.ToString();

        //legerjir - CMONDEV-2340
        if (futureAddrs.Count > 0)
        {
            string text = txtFutureAddressInfo.Text;
            txtFutureAddressInfo.Text = string.Format(text, futureAddrs[0].FromDate.Value.ToShortDateString(), futureAddrs[0].Address.AddressString);
            txtFutureAddressInfo.Visible = true;
        }
        else
        {
            txtFutureAddressInfo.Visible = false;
        }
    }

    protected void cmFAListCustomer_AddNew(object sender, EventArgs e)
    {
        FAShowDetail();
        CustomerVO customer = cache.Session["Customer"] as CustomerVO;
        cmFADetailCustomer.FillFADetailCustomerControl(string.Empty, customer);
    }

    protected void cmFAListCustomer_EditItem(object sender, FAListOperationEventArgs e)
    {
        FAShowDetail();
        CustomerVO customer = cache.Session["Customer"] as CustomerVO;
        cmFADetailCustomer.FillFADetailCustomerControl(string.Empty,customer, e.FutureAddress);
    }

    protected void SaveFutureAddress()
    {              
       // FutureAddressResponseVO response = CustomerControl.AppendFutureAddress(e.FutureAddress, this.Connect);
       // if (response.ResultState == FutureAddressResultState.Faulty)
      //      AddErrorMessage(response.Message);
       // else
       // {                
         //   if (app.Process.Compare(typeof(PRNewContract)) || app.Process.Compare(typeof(PRMoveOut)))
        //    {
           //     bool? addrAlreadySaved = cache.Process["MoveOutInCustFutureAddrSaved"] as bool?;
           //     if (!addrAlreadySaved.HasValue || !addrAlreadySaved.Value) // User fills default opened (proposed) future address just now, CM won't show it again.
             //   {
              //      cache.Process["MoveOutInCustFutureAddrSaved"] = true;
             //       if (app.Process.Compare(typeof(PRNewContract))) // Let's store default new contract start date.
               //         cache.Process["ProposedContractStartDate"] = e.FutureAddress.FromDate;
               // }
           // }
        //}           
    }

    protected void cmFADetailCustomer_Ok(object sender, OkFAEventArgs e)
    {
        if (e.Success)
        {
            FutureAddressResponseVO response = CustomerControl.AppendFutureAddress(e.FutureAddress, this.Connect);
            if (response.ResultState == FutureAddressResultState.Faulty)
                AddErrorMessage(response.Message);
            else
            {
                pnlFutureAddressesDetail.Visible = false; // Hide the panel only in case no error occured
                
                GetFutureAddresses();
                FAShowList();
                if (e.FutureAddress.Id.HasValue)
                    AddInfoMessage(GLR("ItemEditedOK"));
                else
                    AddInfoMessage(GLR("ItemAddedOK"));

                e.SavedOk = true;

                if (app.Process.Compare(typeof(PRNewContract)) || app.Process.Compare(typeof(PRMoveOut)))
                {
                    bool? addrAlreadySaved = cache.Process["MoveOutInCustFutureAddrSaved"] as bool?;
                    if (!addrAlreadySaved.HasValue || !addrAlreadySaved.Value) // User fills default opened (proposed) future address just now, CM won't show it again.
                    {
                        cache.Process["MoveOutInCustFutureAddrSaved"] = true;
                        if (app.Process.Compare(typeof(PRNewContract))) // Let's store default new contract start date.
                            cache.Process["ProposedContractStartDate"] = e.FutureAddress.FromDate;
                    }
                }
            }
        }
        else
        {
            foreach (string msg in e.Message)
            {
                AddErrorMessage(msg);
            }
        }
    }

    private void FAShowDetail()
    {                
        pnlFutureAddresses.Visible = false;
        pnlFutureAddressesDetail.Visible = true;        
    }

    private void FAShowList()
    {        
        pnlFutureAddresses.Visible = true;
        pnlFutureAddressesDetail.Visible = false;        
    }

    protected void cmFADetailCustomer_Cancel(object sender, EventArgs e)
    {
        pnlFutureAddresses.Visible = true;
        pnlFutureAddressesDetail.Visible = false;
        cpeFutureAddresses.Collapsed = false;
        if (app.Process.Compare(typeof(PRNewContract)) || app.Process.Compare(typeof(PRMoveOut)))
        {
            bool? addrAlreadySaved = cache.Process["MoveOutInCustFutureAddrSaved"] as bool?;
            if (!addrAlreadySaved.HasValue || !addrAlreadySaved.Value) // User cancels default opened (proposed) future address just now, CM won't show it again.
            {
                cache.Process["MoveOutInCustFutureAddrSaved"] = true;
                cache.Process.Remove("ProposedContractStartDate");
            }
        }
    }

    protected void cmFAListCustomer_DeleteItem(object sender, FAListOperationEventArgs e)
    {
        if (e.FutureAddress.Id.HasValue)
        {
            RemoveFutureAddressesResponseVO removeFAresponse;
            removeFAresponse = CustomerControl.RemoveFutureAddress(e.FutureAddress.Id.Value, this.Connect);
            if (removeFAresponse.ResultState == FutureAddressResultState.Faulty)
                AddErrorMessage(removeFAresponse.Message);
            else
            {
                GetFutureAddresses();
                AddInfoMessage(GLR("ItemDeletedOK"));
            }
        }
        else
        {
            AddErrorMessage(GLR("FAInvalidID"));
        }
    }

    #endregion Future Address
    
   
}
