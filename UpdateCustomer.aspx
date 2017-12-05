<%@ Page Language="C#" MasterPageFile="~/Default.master" CodeFile="UpdateCustomer.aspx.cs"
    Inherits="Pages_contract_NewContract_MaintainCustomer_UpdateCustomer" StylesheetTheme="GridViewTheme"
    meta:resourcekey="PageResource1" %>

<%@ Register TagPrefix="cm" TagName="ColumnOrderCustomizer" Src="~/Pages/common/ColumnOrderCustomizer.ascx" %>
<%@ Register TagPrefix="cm" TagName="SelectPostcodeUC" Src="~/Pages/contract/NewContract/SelectPostcodeUC.ascx" %>
<%@ Register TagPrefix="ajax" Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" %>
<%@ Register TagPrefix="cm" TagName="TextBox" Src="~/Pages/common/TextBox.ascx" %>
<%@ Register TagPrefix="cm" TagName="MessageBox" Src="~/Pages/common/MessageBox.ascx" %>
<%@ Register TagPrefix="cm" TagName="SelectionButton" Src="~/Pages/common/SelectionButton.ascx" %>
<%@ Register TagPrefix="cm" TagName="UpdateInvoiceConfigAddress" Src="~/Pages/invoice/UpdateInvoiceConfigAddress/UpdateInvoiceConfigAddress.ascx" %>
<%@ Register TagPrefix="cm" TagName="FADetailCustomer" Src="~/Pages/common/FutureAddresses/FADetailCustomer.ascx" %>
<%@ Register TagPrefix="cm" TagName="FAList" Src="~/Pages/common/FutureAddresses/FAList.ascx" %>
<asp:Content ID="Content" ContentPlaceHolderID="MainContent" runat="Server">
    <cm:MessageBox ID="mbConfirmDeleteGroup" runat="server" OnOkClick="mbConfirmDeleteGroup_OkClick"
        Title="Confirm delete" Message="Are you sure you want to remove the customer from the group?" meta:resourcekey="mbConfirmDeleteGroupResource1" />
    <cm:MessageBox ID="mbConfirmDelete" runat="server" OnOkClick="mbConfirmDelete_OkClick"
        Title="Confirm delete" Message="Are you sure you want to delete this row?" meta:resourcekey="mbConfirmDeleteResource1" />
    <ajax:CollapsiblePanelExtender ID="cpeCustomerGroups" TargetControlID="pnlCustomerGroups"
        ImageControlID="imgCustomerGroupsColPan" CollapsedSize="0" CollapseControlID="imgCustomerGroupsColPan"
        ExpandControlID="imgCustomerGroupsColPan" CollapsedImage="/Resources/img/expand.jpg"
        ExpandedImage="/Resources/img/collapse.jpg" runat="server" Enabled="True" Collapsed="True" />
    <ajax:CollapsiblePanelExtender ID="cpeNotifications" TargetControlID="pnlNotifications"
        ImageControlID="imgNotificationsColPan" CollapsedSize="0" CollapseControlID="imgNotificationsColPan"
        ExpandControlID="imgNotificationsColPan" CollapsedImage="/Resources/img/expand.jpg"
        ExpandedImage="/Resources/img/collapse.jpg" runat="server" Enabled="True" Collapsed="True" />
    <ajax:CollapsiblePanelExtender ID="cpeInterests" TargetControlID="pnlInterests" ImageControlID="imgInterestsColPan"
        CollapsedSize="0" CollapseControlID="imgInterestsColPan" ExpandControlID="imgInterestsColPan"
        CollapsedImage="/Resources/img/expand.jpg" ExpandedImage="/Resources/img/collapse.jpg"
        runat="server" Enabled="True" Collapsed="true" />
    <cm:SelectPostcodeUC ID="selectPostcodeUC" runat="server" />
    <asp:HiddenField ID="hidMsgShown" runat="server" Value="false" />
    <fieldset>
        <legend class="HdrText">
            <asp:Label runat="server" ID="lblUpdateCustomer" meta:resourcekey="lblUpdateCustomerResource1"
                Text="Update customer information"></asp:Label></legend>
        <table id="tblSuggestedCustomerName" runat="server" visible="false" style="width: 100%;
            background-color: #EEEEEE">
            <tr>
                <td style="height: 20px" align="left">
                    <br />
                </td>
            </tr>
            <tr>
                <td class="normTxt" align="left">
                    <asp:Label runat="server" ID="lblSuggestedCustomerName" Text="Suggested main customer name:"
                        meta:resourcekey="lblSuggestedCustomerNameResource1" />
                </td>
            </tr>
            <tr>
                <td class="normTxt" align="left">
                    <cm:SelectionButton ID="sbSuggestedCustomerName" runat="server" OnClick="sbSuggestedCustomerName_Click" />
                    <asp:Label runat="server" ID="txtSuggestedCustomerName" Style="font-weight: bold;" />
                </td>
            </tr>
        </table>
        <table style="width: 100%; background-color: #EEEEEE">
            <tr>
                <td colspan="5" style="height: 20px" align="left">
                    <br />
                </td>
            </tr>
            <tr id="trPrivateCustomer" runat="server" visible="false">
                <td class="normTxt" style="width: 147px">
                    <asp:Label runat="server" ID="lblCustomerFirstName" meta:resourcekey="lblCustomerFirstNameResource1"
                        Text="Customer first name"></asp:Label>
                </td>
                <td style="width: 120px" align="left">
                    <asp:TextBox ID="txtCustomerFirstName" runat="server" CssClass="TextInput" meta:resourcekey="txtCustomerFirstNameResource1"
                        MaxLength="30" />
                    <asp:CustomValidator ID="TxtCustomerFirstNameValidator" runat="server" ControlToValidate="txtCustomerFirstName"
                        ErrorMessage="Customer first name is required" Display="None" meta:resourcekey="RequiredFieldValidator2Resource1"
                        OnServerValidate="TxtCustomerFirstNameValidator_ServerValidate"></asp:CustomValidator>
                </td>
                <td class="normTxt" style="width: 147px">
                    <asp:Label runat="server" ID="lblCustomerLastName" meta:resourcekey="lblCustomerLastNameResource1"
                        Text="Customer last name"></asp:Label>
                </td>
                <td style="width: 120px" align="left">
                    <asp:TextBox ID="txtCustomerLastName" runat="server" CssClass="TextInput" meta:resourcekey="txtCustomerLastNameResource1"
                        MaxLength="50" />
                    <asp:CustomValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtCustomerLastName"
                        ErrorMessage="Customer last name is required" Display="None" meta:resourcekey="RequiredFieldValidator3Resource1"
                        OnServerValidate="TxtCustomerLastNameValidator_ServerValidate" ValidateEmptyText="True"></asp:CustomValidator>
                </td>
                <td align="left" style="width: 5px">
                </td>
            </tr>
            <tr id="trCompanyCustomer" runat="server" visible="true">
                <td class="normTxt" style="width: 147px">
                    <asp:Label runat="server" ID="lblCustomerName" meta:resourcekey="lblCustomerNameResource1"
                        Text="Customer name"></asp:Label>
                </td>
                <td align="left" colspan="3">
                    <asp:TextBox ID="txtCustomerName" runat="server" CssClass="TextInput" meta:resourcekey="txtCustomerNameResource1"
                        Width="371px" />
                    <asp:CustomValidator ID="TxtCustomerNameValidator" runat="server" ControlToValidate="txtCustomerName"
                        ErrorMessage="Customer name is required" Display="None" meta:resourcekey="TxtCustomerNameValidatorResource1"
                        OnServerValidate="TxtCustomerNameValidator_ServerValidate" ValidateEmptyText="True"></asp:CustomValidator>
                </td>
                <td align="left" style="width: 5px">
                </td>
            </tr>
            <tr id="trGender" runat="server" visible="false">
                <td class="normTxt" style="width: 147px" align="left">
                    <asp:Label runat="server" ID="lblGender" meta:resourcekey="lblGenderResource1" Text="Gender"></asp:Label>
                </td>
                <td style="width: 120px" colspan="3" align="left">
                    <asp:DropDownList ID="ddlGender" runat="server" meta:resourcekey="ddlGenderResource1" />
                </td>
                <td id="td11" style="width: 5px;" colspan="1">
                    <br />
                </td>
            </tr>
            <tr>
                <td class="normTxt" style="width: 147px" align="left">
                    <asp:Label runat="server" ID="lblCustomerID" meta:resourcekey="lblCustomerIDResource1"
                        Text="Personal/Company ID"></asp:Label>
                </td>
                <td style="width: 120px" colspan="1" align="left">
                    <asp:TextBox ID="txtOrgNo" runat="server" CssClass="TextInput" meta:resourcekey="txtOrgNoResource1" />
                    <asp:CustomValidator ID="TxtOrgNoValidator" runat="server" ControlToValidate="txtOrgNo"
                        ErrorMessage="Personal ID is required" Display="None" meta:resourcekey="TxtOrgNoValidator1Resource1"
                        OnServerValidate="TxtOrgNoValidator_ServerValidate" ValidateEmptyText="True"></asp:CustomValidator>
                    <asp:CustomValidator ID="TxtOrgNoValidator2" runat="server" ControlToValidate="txtOrgNo"
                        ErrorMessage="Personal ID is invalid" Display="None" meta:resourcekey="TxtOrgNoValidator2Resource1"
                        OnServerValidate="TxtOrgNoValidator2_ServerValidate"></asp:CustomValidator>
                </td>
                <td class="normTxt" style="width: 147px" align="left">
                    <asp:Label runat="server" ID="lblForeignCustomer" meta:resourcekey="lblForeignCustomerResource1"
                        Text="Foreign customer"></asp:Label>
                </td>
                <td style="width: 120px" colspan="1" align="left">
                    <asp:CheckBox ID="chkForeignCustomer" runat="server" meta:resourcekey="chkForeignCustomerResource1" />
                </td>
                <td align="left" style="width: 5px">
                </td>
            </tr>
            <tr>
                <td class="normTxt" style="width: 147px; height: 13px">
                    <asp:Label runat="server" ID="lblDefaultPhone" meta:resourcekey="lblDefaultPhoneResource1"
                        Text="Default phone no"></asp:Label>
                </td>
                <td style="width: 120px; height: 13px;" align="left">
                    <asp:TextBox ID="txtDefaultPhone" runat="server" CssClass="TextInput" meta:resourcekey="txtDefaultPhoneResource1" />
                </td>
                <td class="normTxt" style="width: 147px">
                    <asp:Label runat="server" ID="lblEMail" meta:resourcekey="lblEMailResource1" Text="E-mail"></asp:Label>
                </td>
                <td align="left" style="width: 120px">
                    <asp:TextBox ID="txtEMail" runat="server" CssClass="TextInput" meta:resourcekey="EMailResource1" />
                    <asp:CustomValidator ID="TxtEmailCustomValidator" runat="server" ControlToValidate="txtEMail"
                        ErrorMessage="Email address is invalid." OnServerValidate="TxtEmailValidator_ServerValidate"
                        Display="None" meta:resourcekey="TxtEmailCustomValidatorResource1"></asp:CustomValidator>
                </td>
                <td id="td1" style="width: 5px; height: 13px;" colspan="1">
                    <br />
                </td>
            </tr>
            <tr>
                <td class="normTxt" style="width: 147px; height: 13px">
                    <asp:Label runat="server" ID="lblReservedAgainstInfo" meta:resourcekey="lblReservedAgainstInfoResource1"
                        Text="Reserved against info"></asp:Label>
                </td>
                <td style="width: 120px" colspan="1" align="left">
                    <asp:CheckBox ID="chkReservedAgainstInfo" runat="server" />
                </td>
            </tr>
            <!-- RaVa note -->
            <tr>
                <td colspan="5" style="height: 20px" align="left">
                    <br />
                </td>
            </tr>
            <tr>
                <td class="normTxt" style="width: 147px" align="left">
                    <asp:Label runat="server" ID="Label1" meta:resourcekey="lblLangResource1" Text="Language"></asp:Label>
                </td>
                <td style="width: 120px" colspan="3" align="left">
                    <asp:DropDownList ID="ddlLanguage" runat="server" meta:resourcekey="ddlLangResource1" />
                </td>
                <td id="td2" style="width: 5px;" colspan="1">
                    <br />
                </td>
            </tr>
            <!-- 20100312 LKo: We need 2 bank account fields -->
            <tr id="trBankAccountNational" runat="server" visible="false">
                <td style="width: 147px" class="normTxt">
                    <asp:Label runat="server" ID="lblCreditAccountNational" meta:resourcekey="lblCreditAccountNationalResource1"
                        Text="Credit account (national)"></asp:Label>
                </td>
                <td align="left" style="width: 121px;" colspan="3">
                    <asp:TextBox ID="tbCreditAccountNational" runat="server" CssClass="TextInput" meta:resourcekey="tbCreditAccountNationalResource1" />
                    <asp:CustomValidator ID="CreditAccountValidator1" runat="server" ControlToValidate="tbCreditAccountNational"
                        Display="None" ErrorMessage="Credit account is invalid" OnServerValidate="TxtCreditAccountValidator_ServerValidate"
                        meta:resourcekey="CreditAccountValidatorResource1"></asp:CustomValidator>
                </td>
                <td id="td10" style="width: 5px;" colspan="1">
                    <br />
                </td>
            </tr>
            <tr id="trBankAccountIBAN" runat="server" visible="false">
                <td style="width: 147px" class="normTxt">
                    <asp:Label runat="server" ID="lblCreditAccountIBAN" meta:resourcekey="lblCreditAccountIBANResource1"
                        Text="Credit account (IBAN)"></asp:Label>
                </td>
                <td align="left" style="width: 120px;">
                    <asp:TextBox ID="tbCreditAccountIBAN" runat="server" CssClass="TextInput" meta:resourcekey="tbCreditAccountIBANResource1" />
                    <asp:CustomValidator ID="CreditAccountValidator2" runat="server" ControlToValidate="tbCreditAccountIBAN"
                        Display="None" ErrorMessage="Credit account is invalid" OnServerValidate="TxtCreditAccountValidator_ServerValidate"
                        meta:resourcekey="CreditAccountValidatorResource1"></asp:CustomValidator>
                </td>
                <td class="normTxt" style="width: 147px" align="left">
                    <asp:Label runat="server" ID="lblCreditAccountBIC" meta:resourcekey="lblCreditAccountBICResource1"
                        Text="BIC"></asp:Label>
                </td>
                <td align="left" style="width: 121px;">
                    <asp:DropDownList ID="ddlBIC" runat="server" OnSelectedIndexChanged="ddlBIC_OnSelected"
                        AutoPostBack="True" meta:resourcekey="ddlBICResource1" />
                    &nbsp;<asp:Label runat="server" ID="lblBICBankName" meta:resourcekey="lblBICBankNameResource1"></asp:Label>
                </td>
                <td id="td9" style="width: 5px;" colspan="1">
                    <br />
                </td>
            </tr>
            <tr>
                <td class="normTxt" style="width: 147px" align="left">
                    <asp:Label runat="server" ID="lblCustomerSegment" meta:resourcekey="lblCustomerSegmentResource1"
                        Text="Customer segment"></asp:Label>
                </td>
                <td style="width: 120px" colspan="3" align="left">
                    <asp:DropDownList ID="ddlCustomerSegment" runat="server" CssClass="TextInput" meta:resourcekey="ddlCustomerSegmentResource1" />
                </td>
                <td id="td4" style="width: 5px">
                    <br />
                </td>
            </tr>
        </table>
        <ajax:Accordion ID="accAddresses" runat="server" AutoSize="None" HeaderCssClass="accordionHeader"
            ContentCssClass="accordionContent" SelectedIndex="0" RequireOpenedPane="true"
            FramesPerSecond="40">
            <Panes>
                <ajax:AccordionPane ID="apActualAddress" runat="server">
                    <Header>
                        <asp:Label runat="server" ID="lblMainAddress" meta:resourcekey="lblMainAddressResource1"
                            Text="Mainaddress"></asp:Label>
                    </Header>
                    <Content>
                        <table style="background-color: #EEEEEE; width: 100%">
                            <tr>
                                <td class="normTxtBold" colspan="5">
                                    <asp:Label runat="server" ID="txtFutureAddressInfo" Visible="false" meta:resourcekey="txtFutureAddressInfoResource1"
                                        Text="Future address"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="normTxt" colspan="5">
                                    <asp:RadioButtonList ID="rblMainAddress" runat="server" meta:resourcekey="rblMainAddressResource1"
                                        OnSelectedIndexChanged="rblMainAddress_SelectedIndexChanged" AutoPostBack="true" />
                                </td>
                            </tr>
                            <tr id="CustomerNemAddress_row1" runat="server" visible="false">
                                <td class="normTxt" style="width: 147px;">
                                    <asp:Label runat="server" ID="lblStreetAddress" meta:resourcekey="lblStreetAddressResource1"
                                        Text="Street name"></asp:Label>
                                </td>
                                <td style="width: 120px;" align="left">
                                    <asp:TextBox ID="txtStreetAddress" runat="server" CssClass="TextInput" meta:resourcekey="txtStreetAddressResource1" />
                                </td>
                                <td class="normTxt" colspan="2" align="center">
                                    <asp:Label runat="server" ID="lblStreetNr" meta:resourcekey="lblStreetNrResource1"
                                        Text="Number"></asp:Label>
                                    <asp:TextBox ID="txtStreetNumber" runat="server" Width="40px" CssClass="TinyTextInput"
                                        meta:resourcekey="txtStreetNumberResource1" />
                                    <asp:Label runat="server" ID="lblStreetFlat" meta:resourcekey="lblStreetFlatResource1"
                                        Text="Flat"></asp:Label>
                                    <asp:TextBox ID="txtFlatNr" runat="server" CssClass="TinyTextInput" meta:resourcekey="txtFlatNrResource1"
                                        Width="40px" />
                                    <asp:Label runat="server" ID="lblStreetFloor" meta:resourcekey="lblStreetFloorResource1"
                                        Text="Floor"></asp:Label>
                                    <asp:TextBox ID="txtFloor" runat="server" CssClass="TinyTextInput" meta:resourcekey="txtFloorResource1"
                                        Width="40px" />
                                </td>
                                <td id="td7" style="width: 5px;">
                                    <br />
                                </td>
                            </tr>
                            <tr id="CustomerNemAddress_row2" runat="server" visible="false">
                                <td class="normTxt" style="width: 147px;">
                                    <asp:Label runat="server" ID="lblPostCode" meta:resourcekey="lblPostCodeResource1"
                                        Text="Postal code"></asp:Label>
                                </td>
                                <td style="width: 120px;" align="left">
                                    <asp:TextBox ID="txtPostalCode" runat="server" CssClass="TextInput" AutoPostBack="True"
                                        OnTextChanged="txtPostalCode_TextChanged" meta:resourcekey="txtPostalCodeResource1" />
                                </td>
                                <td class="normTxt" style="width: 147px;">
                                    <asp:Label runat="server" ID="lblPostOffice" meta:resourcekey="lblPostOfficeResource1"
                                        Text="Post office"></asp:Label>
                                </td>
                                <td align="left" style="width: 120px;">
                                    <asp:TextBox ID="txtPostOffice" runat="server" CssClass="TextInput" meta:resourcekey="txtPostOfficeResource1" />
                                </td>
                                <td id="td3" style="width: 5px;" colspan="1">
                                    <br />
                                </td>
                            </tr>
                            <tr id="CustomerNemAddress_row3" runat="server" visible="false">
                                <td class="normTxt" style="width: 147px" align="left">
                                    <asp:Label runat="server" ID="lblCountry" meta:resourcekey="lblCountryResource1"
                                        Text="Country"></asp:Label>
                                </td>
                                <td style="width: 120px" colspan="3" align="left">
                                    <asp:DropDownList ID="ddlCountry" runat="server" meta:resourcekey="ddlCountryResource1" />
                                </td>
                                <td id="td6" style="width: 5px;" colspan="1">
                                    <br />
                                </td>
                            </tr>
                            <tr id="CustomerNemAddress_row4" runat="server" visible="false">
                                <td class="normTxt" style="width: 147px;">
                                    <asp:Label ID="lblCOAddress" runat="server" Text="C/O" meta:resourcekey="lblCOAddressResource1" />
                                </td>
                                <td style="width: 120px;" align="left">
                                    <asp:TextBox ID="txtCOAddress" runat="server" CssClass="TextInput" meta:resourcekey="txtCOAddressResource1" />
                                </td>
                                <td class="normTxt" style="width: 147px;">
                                    <asp:Label ID="lblAttention" runat="server" Text="Notification" meta:resourcekey="lblAttentionResource1" />
                                </td>
                                <td style="width: 120px;" align="left">
                                    <asp:TextBox ID="txtAttention" runat="server" CssClass="TextInput" meta:resourcekey="txtAttentionResource1" />
                                </td>
                                <td id="td8" style="width: 5px;">
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="5">
                                    <cm:UpdateInvoiceConfigAddress ID="cmUpdateInvoiceConfigAddress" runat="server" Visible="false" />
                                </td>
                            </tr>
                        </table>
                    </Content>
                </ajax:AccordionPane>
                <ajax:AccordionPane ID="apFutureAddress" Visible="false" runat="server">
                    <Header>
                        <asp:Label runat="server" ID="lblFutureAddress" meta:resourcekey="lblFutureAddressResource1"
                            Text="Future address"></asp:Label>
                    </Header>
                    <Content>
                        <ajax:CollapsiblePanelExtender ID="cpeFutureAddresses" TargetControlID="pnlFutureAddresses"
                            ImageControlID="imgFutureAddressesColPan" CollapsedSize="0" CollapseControlID="imgFutureAddressesColPan"
                            ExpandControlID="imgFutureAddressesColPan" CollapsedImage="/Resources/img/expand.jpg"
                            ExpandedImage="/Resources/img/collapse.jpg" runat="server" Enabled="True" Collapsed="True" />
                        <table id="tblFutureAddresses" runat="server" width="100%">
                            <tr>
                                <td>
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <asp:Image ID="imgFutureAddressesColPan" runat="server" ImageUrl="/Resources/img/collapse.jpg"
                                                    meta:resourcekey="imgFutureAddressesColPanResource1" />&nbsp;<asp:Label ID="lblFutureAddresses"
                                                        meta:resourcekey="lblFutureAddressesResource1" Text="Future addresses" runat="server" />&nbsp;
                                                <b>
                                                    <asp:Label ID="lblFutureAddressesRowsCount" runat="server" Text='<%# Eval("gvFutureAddresses.Rows.Count") %>' /></b>&nbsp;<asp:Label
                                                        runat="server" ID="lblFutureAddressesCount" meta:resourcekey="lblFutureAddressesCountResource1"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                    <br />
                                    <asp:Panel ID="pnlFutureAddresses" runat="server">
                                        <cm:FAList runat="server" ID="cmFAListCustomer" FutureAddrRefType="CustomerAddress"
                                            OnFutureAddresses_AddNew="cmFAListCustomer_AddNew" OnFutureAddresses_EditItem="cmFAListCustomer_EditItem"
                                            OnFutureAddresses_DeleteItem="cmFAListCustomer_DeleteItem" />
                                    </asp:Panel>
                                    <asp:Panel ID="pnlFutureAddressesDetail" runat="server" Visible="true">
                                        <cm:FADetailCustomer runat="server" ID="cmFADetailCustomer" OnFADetailCustomer_Ok="cmFADetailCustomer_Ok"
                                            OnFADetailCustomer_Cancel="cmFADetailCustomer_Cancel"></cm:FADetailCustomer>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </Content>
                </ajax:AccordionPane>
            </Panes>
        </ajax:Accordion>
        <br />
        <!-- 20100806 dyrdamar: divided into separate tables (CMONDEV-523) -->
        <table style="background-color: #EEEEEE; width: 100%">
            <tr>
                <td class="normTxt" colspan="4" align="left">
                    <!-- 20110331 stejspet: added table for future addresses-->
                </td>
            </tr>
            <tr>
                <td class="normTxt" colspan="4" align="left">
                    <table id="tblCustomerGroups" runat="server" width="100%">
                        <tr>
                            <td>
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <asp:Image ID="imgCustomerGroupsColPan" runat="server" ImageUrl="/Resources/img/collapse.jpg"
                                                meta:resourcekey="imgCustomerGroupsColPanResource1" />&nbsp;<asp:Label ID="lblCustomerGroups"
                                                    meta:resourcekey="lblCustomerGroupsResource1" Text="Customer groups" runat="server" />&nbsp;
                                            <b>
                                                <asp:Label ID="lblCustomerGroupRowsCount" runat="server" Text='<%# Eval("gvCustomerGroups.Rows.Count") %>'
                                                    meta:resourcekey="lblCustomerGroupRowsCountResource1" /></b>&nbsp;<asp:Label runat="server"
                                                        Text="Items" ID="lblCustomerGroupCount" meta:resourcekey="lblCustomerGroupCountResource1"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <asp:Panel ID="pnlCustomerGroups" runat="server" meta:resourcekey="pnlCustomerGroupsResource1">
                                    <asp:Label runat="server" ID="lblCustomerGroup" meta:resourcekey="lblCustomerGroupResource1"
                                        Text="Customer group"></asp:Label>&nbsp;
                                    <asp:DropDownList ID="ddlNewCustomerGroups" runat="server" AutoPostBack="True" meta:resourcekey="ddlNewCustomerGroupsResource1" />
                                    &nbsp;
                                    <asp:Label runat="server" ID="lblMembershipCode" meta:resourcekey="lblMembershipCodeResource1"
                                        Text="Membership code"></asp:Label>&nbsp;
                                    <!-- 20100901 dyrdamar: added maxlength property (CMONDEV-619) -->
                                    <asp:TextBox ID="txtNewMembershipCode" runat="server" MaxLength="19" CssClass="TextInput"
                                        meta:resourcekey="txtMembershipCodeResource1" />&nbsp;
                                    <asp:Button ID="Button3" Text="Add group" runat="server" OnClick="CmdAddGroup_Click"
                                        meta:resourcekey="cmdAddResource1" />
                                    <asp:GridView ID="gvCustomerGroups" runat="server" AutoGenerateColumns="False" UseAccessibleHeader="False"
                                        OnRowDeleting="gvCustomerGroups_RowDeleting" OnRowEditing="gvCustomerGroups_RowEditing"
                                        BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" BackColor="White"
                                        OnRowCancelingEdit="gvCustomerGroups_RowCancelingEdit" OnRowUpdating="gvCustomerGroups_RowUpdating"
                                        meta:resourcekey="gvCustomerGroupsResource1">
                                        <Columns>
                                            <asp:CommandField ShowEditButton="True" CancelImageUrl="~/Resources/img/cancel.gif"
                                                DeleteImageUrl="~/Resources/img/delete.gif" EditImageUrl="~/Resources/img/tablearrows/<%=COMPANY_CODE%>/right_arrow.gif"
                                                CausesValidation="False" ShowDeleteButton="True" meta:resourcekey="CommandFieldResource1">
                                            </asp:CommandField>
                                            <asp:TemplateField HeaderText="Customer group" meta:resourcekey="TemplateFieldResource1">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblGroupName" runat="server" Text='<%# Bind("CustomerGroupName") %>'
                                                        meta:resourcekey="lblGroupNameResource1"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Membership code" meta:resourcekey="TemplateFieldResource2">
                                                <EditItemTemplate>
                                                    <!-- 20100901 dyrdamar: added maxlength property (CMONDEV-619) -->
                                                    <asp:TextBox ID="txtGroupCode" runat="server" MaxLength="19" Text='<%# Bind("MemberNo") %>'
                                                        meta:resourcekey="txtGroupCodeResource1"></asp:TextBox>
                                                </EditItemTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblGroupCode" runat="server" Text='<%# Bind("MemberNo") %>' meta:resourcekey="lblGroupCodeResource1"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    <br />
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </td>
                <td id="td5" style="width: 5px">
                    <br />
                </td>
            </tr>
            <tr runat="server" id="trNotifications">
                <td class="normTxt" colspan="4" align="left">
                    <table id="tblNotifications" runat="server" width="100%">
                        <tr>
                            <td>
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <asp:Image ID="imgNotificationsColPan" runat="server" ImageUrl="/Resources/img/collapse.jpg"
                                                meta:resourcekey="imgNotificationsColPanResource1" />&nbsp;<asp:Label ID="lblNotifications"
                                                    meta:resourcekey="lblNotificationsResource1" Text="Notifications" runat="server" />&nbsp;
                                            <b>
                                                <asp:Label ID="lblNotificationRowsCount" runat="server" meta:resourcekey="lblNotificationRowsCountResource1" /></b>&nbsp;<asp:Label
                                                    runat="server" Text="Items" ID="lblNotificationCount" meta:resourcekey="lblNotificationCountResource1"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <asp:Panel ID="pnlNotifications" runat="server" meta:resourcekey="pnlNotificationsResource1">
                                    <table width="100%">
                                        <tr valign="top">
                                            <td style="width: 60px">
                                                <asp:Label runat="server" ID="lblNewNotificationType" meta:resourcekey="lblNewNotificationTypeResource1"
                                                    Text="Notification type"></asp:Label>
                                            </td>
                                            <td style="width: 80px">
                                                <asp:DropDownList ID="ddlNewNotificationTypes" runat="server" AutoPostBack="True"
                                                    meta:resourcekey="ddlNewNotificationTypesResource1" />
                                                &nbsp;
                                            </td>
                                            <td style="width: 60px">
                                                <asp:Label runat="server" ID="lblNewNotificationText" meta:resourcekey="lblNewNotificationTextResource1"
                                                    Text="Notification"></asp:Label>&nbsp;
                                            </td>
                                            <td style="width: 320px">
                                                <asp:TextBox ID="txtNewNotification" runat="server" TextMode="MultiLine" Width="320px"
                                                    Height="48px" meta:resourcekey="txtNewNotificationResource1" />&nbsp;
                                            </td>
                                            <td style="width: 60px">
                                                <asp:Label runat="server" ID="lblNewValidThru" meta:resourcekey="lblNewValidThruResource1"
                                                    Text="Valid thru"></asp:Label>&nbsp;
                                            </td>
                                            <td style="width: 160px">
                                                <cm:TextBox ID="txtNewValidThru" runat="server" ValidationType="Date" MaxLength="10" />
                                            </td>
                                            <td style="width: 80px">
                                                <asp:Button ID="Button1" Text="Add notification" runat="server" OnClick="cmdAddNotification_Click"
                                                    meta:resourcekey="cmdAddNoteResource1" />
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:GridView ID="gvNotifications" runat="server" AutoGenerateColumns="False" UseAccessibleHeader="False"
                                        OnRowDeleting="gvNotifications_RowDeleting" OnRowEditing="gvNotifications_RowEditing"
                                        BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" BackColor="White"
                                        OnRowCancelingEdit="gvNotifications_RowCancelingEdit" OnRowUpdating="gvNotifications_RowUpdating"
                                        meta:resourcekey="gvNotificationsResource1">
                                        <Columns>
                                            <asp:CommandField ShowEditButton="True" CancelImageUrl="~/Resources/img/cancel.gif"
                                                DeleteImageUrl="~/Resources/img/delete.gif" EditImageUrl="~/Resources/img/tablearrows/<%=COMPANY_CODE%>/right_arrow.gif"
                                                CausesValidation="False" ShowDeleteButton="True" meta:resourcekey="CommandFieldResource2">
                                                <ItemStyle Width="32px" />
                                            </asp:CommandField>
                                            <asp:TemplateField HeaderText="Notification type" meta:resourcekey="lblNotificationTypeResource1">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNotificationType" runat="server" Text='<%# Bind("NoteTypesName") %>'
                                                        meta:resourcekey="lblNotificationTypeResource1"></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="120px" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Notification" meta:resourcekey="lblNotificationResource1">
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="txtNotification" runat="server" Text='<%# Bind("Note") %>' TextMode="MultiLine"
                                                        Width="320px" Height="48px" meta:resourcekey="txtNotificationResource1"></asp:TextBox>
                                                </EditItemTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNotification" runat="server" Text='<%# Bind("Note") %>' meta:resourcekey="lblNotificationResource1"></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="320px" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Valid thru" meta:resourcekey="lblValidThruResource1">
                                                <EditItemTemplate>
                                                    <cm:TextBox ID="txtValidThru" runat="server" ValidationType="Date" MaxLength="10"
                                                        meta:resourcekey="txtValidThruResource1" />
                                                </EditItemTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblValidThru" runat="server" Text='<%# Bind("SValidUntilDate") %>'
                                                        meta:resourcekey="lblValidThruResource1"></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="120px" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Create Date" meta:resourcekey="lblCreateDateResource1">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCreateDate" runat="server" Text='<%# Bind("SCreateDate") %>' meta:resourcekey="lblCreateDateResource1"></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="120px" />
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </td>
                <td align="left" style="width: 5px">
                </td>
            </tr>
            <tr id="trInterests" runat="server">
                <td class="normTxt" colspan="4" align="left">
                    <table id="tblInterests" runat="server" width="100%">
                        <tr>
                            <td>
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <asp:Image ID="imgInterestsColPan" runat="server" ImageUrl="/Resources/img/collapse.jpg" />&nbsp;<asp:Label
                                                ID="lblInterests" meta:resourcekey="lblInterestsResource1" Text="Customer interests"
                                                runat="server" />&nbsp; <b>
                                                    <asp:Label ID="lblInterestsRowsCount" runat="server" /></b>&nbsp;<asp:Label runat="server"
                                                        meta:resourcekey="lblInterestsCountResource1" Text="Items" ID="lblInterestsCount"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <asp:Panel ID="pnlInterests" runat="server">
                                    <asp:Label runat="server" ID="lblInterestType" meta:resourcekey="lblInterestTypeResource1"
                                        Text="Interest type"></asp:Label>&nbsp;
                                    <asp:DropDownList ID="ddlInterestTypes" runat="server" />
                                    &nbsp;
                                    <asp:Label runat="server" ID="lblInterestValidFrom" meta:resourcekey="lblInterestValidFromResource1"
                                        Text="Valid from"></asp:Label>&nbsp;
                                    <cm:TextBox ID="txtInterestValidFrom" runat="server" ValidationType="Date" MaxLength="10" />
                                    &nbsp;
                                    <asp:Label runat="server" ID="lblInterestValidThru" meta:resourcekey="lblInterestValidThruResource1"
                                        Text="Valid thru"></asp:Label>&nbsp;
                                    <cm:TextBox ID="txtInterestValidThru" runat="server" ValidationType="Date" MaxLength="10" />
                                    <asp:Button ID="btnAddInterest" meta:resourcekey="btnAddInterestResource1" Text="Add"
                                        OnClick="btnAddInterest_Click" runat="server" />
                                    <asp:GridView ID="gvInterests" runat="server" AutoGenerateColumns="False" UseAccessibleHeader="False"
                                        BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" BackColor="White"
                                        OnRowDeleting="gvInterests_RowDeleting" OnRowEditing="gvInterests_RowEditing"
                                        OnRowCancelingEdit="gvInterests_RowCancelingEdit" OnRowUpdating="gvInterests_RowUpdating">
                                        <Columns>
                                            <asp:CommandField ShowEditButton="True" CancelImageUrl="~/Resources/img/cancel.gif"
                                                DeleteImageUrl="~/Resources/img/delete.gif" EditImageUrl="~/Resources/img/tablearrows/<%=COMPANY_CODE%>/right_arrow.gif"
                                                CausesValidation="False" ShowDeleteButton="True" meta:resourcekey="CommandFieldResource1">
                                            </asp:CommandField>
                                            <asp:TemplateField HeaderText="Interest type" meta:resourcekey="tfInterestTypeResource1">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblInterestType" runat="server" Text='<%# Bind("NoteTypesName") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Valid from" meta:resourcekey="tfValidFromResource1">
                                                <EditItemTemplate>
                                                    <cm:TextBox ID="txtValidFrom" runat="server" ValidationType="Date" MaxLength="10"
                                                        Text='<%# Bind("SValidFromDate") %>' />
                                                </EditItemTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblValidFrom" runat="server" Text='<%# Bind("SValidFromDate") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Valid thru" meta:resourcekey="tfValidThruResource1">
                                                <EditItemTemplate>
                                                    <cm:TextBox ID="txtValidThru" runat="server" ValidationType="Date" MaxLength="10"
                                                        Text='<%# Bind("SValidUntilDate") %>' />
                                                </EditItemTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblValidThru" runat="server" Text='<%# Bind("SValidUntilDate") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </td>
                <td align="left" style="width: 5px">
                </td>
            </tr>
            <tr>
                <td colspan="5">
                    <br />
                </td>
            </tr>
        </table>
        <br />
    </fieldset>
    <br />
    <table style="width: 100%">
        <tr>
            <td align="right">
                <asp:Button ID="bSecurity" runat="server" Text="Request for Security" OnClick="BSecurity_Click"
                    meta:resourcekey="bSecurityResource1" Visible="False" />
                <asp:Button ID="bBack" Text="Back" runat="server" OnClick="BBack_Click" CausesValidation="false"
                    meta:resourcekey="bBackResource1" />&nbsp;
                <asp:Button ID="bNext" Text="Continue" runat="server" OnClick="BNext_Click" meta:resourcekey="bNextResource1" />
                <asp:Button ID="bConfirmSave" Text="Confirm" runat="server" OnClick="bConfirmSave_Click"
                    meta:resourcekey="bConfirmSaveResource1" Visible="False" />
                <asp:Button ID="bCancel" Text="Cancel" runat="server" OnClick="BCancel_Click" CausesValidation="false"
                    meta:resourcekey="bCancelResource1" />&nbsp;<asp:Button ID="bSave" Text="Save" runat="server"
                        OnClick="BNext_Click" meta:resourcekey="bSaveResource1" />
            </td>
        </tr>
    </table>
</asp:Content>
