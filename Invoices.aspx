<%@ Page Language="C#" MasterPageFile="~/Default.master" AutoEventWireup="true" CodeFile="Invoices.aspx.cs" Inherits="Pages_invoice_Invoices" StyleSheetTheme="GridViewTheme"%>
<%@ Register TagPrefix="cm" TagName="ColumnOrderCustomizer" Src="~/Pages/common/ColumnOrderCustomizer.ascx" %>
<%@ Register TagPrefix="cm" TagName="Calendar" Src="~/Pages/common/CalendarControl.ascx" %>
<asp:Content ID="Content" ContentPlaceHolderID="MainContent" Runat="Server">
    <fieldset>
    <legend class="HdrText"><asp:Label runat="server" ID="lblSearchCustomer" Text="Search errand"></asp:Label></legend>
    <table width="100%" class="SearchParameterTable">
        <tr><td colspan="4"><br/></td></tr>
        <tr><td colspan="4" class="SearchParameterTableSubHeader"><asp:Label ID="lblErrandHeader" runat="server" Text="Errand"></asp:Label></td></tr>
        <tr>
            <td class="normTxt"><asp:Label runat="server" ID="lblErrandNumber" Text="Errand number"></asp:Label></td>
            <td><asp:TextBox ID="txtErrandNumber" runat="server" CssClass="TextInput"/></td>
            <td><asp:Label runat="server" ID="lblErrandStatus" Text="Errand status"></asp:Label></td>
            <td><asp:DropDownList ID="ddlErrandStatus" runat="server"></asp:DropDownList></td>
        </tr>
        <tr>
            <td class="normTxt"><asp:Label runat="server" ID="lblErrandType" Text="Errand type"></asp:Label></td>
            <td><asp:DropDownList ID="ddlErrandType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlErrandType_SelectedIndexChanged"/></td>
            <td><asp:Label runat="server" ID="lblHandler" Text="Handler"></asp:Label></td>
            <td>           
                <asp:TextBox ID="txtHandler" runat="server" CssClass="TextInput"></asp:TextBox>
                <asp:HyperLink ID="hlSelectHandler" runat="server" NavigateUrl="javascript:createWindowAtMouse('Select handler','/Pages/errand/SelectHandler.aspx');">...</asp:HyperLink>
            </td>            
        </tr>
        <tr>
            <td class="normTxt"><asp:Label runat="server" ID="lblResponsibleArea" Text="Responsible area"></asp:Label></td>
            <td><asp:DropDownList ID="ddlResponsibleArea" runat="server"/></td>
            <td><asp:Label runat="server" ID="lblChannel" Text="Channel"></asp:Label></td>
            <td><asp:DropDownList ID="ddlChannel" runat="server"/></td>
        </tr>        
        <tr>
            <td class="normTxt"><asp:Label runat="server" ID="lblCreationDateEarliest" Text="Creation date, earliest"></asp:Label></td>
            <td>
                <asp:TextBox ID="txtCreationDateEarliest" runat="server" CssClass="TextInput" Columns="10" MaxLength="10"></asp:TextBox>
                <cm:Calendar ID="calCreationDateEarliest" runat="server" DataBindField="txtCreationDateEarliest"/>
            </td>
            <td>
                <asp:Label runat="server" ID="lblCreationDateLatest" Text="Creation date, latest"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtCreationDateLatest" runat="server" CssClass="TextInput" Columns="10" MaxLength="10"></asp:TextBox>
                <cm:Calendar ID="calCreationDateLatest" runat="server" DataBindField="txtCreationDateLatest"/>
            </td>                          
        </tr>        
        <tr>
            <td class="normTxt"><asp:Label runat="server" ID="lblDueDateEarliest" Text="Due date, earliest"></asp:Label></td>
            <td>
                <asp:TextBox ID="txtDueDateEarliest" runat="server" CssClass="TextInput" Columns="10" MaxLength="10"></asp:TextBox>
                <cm:Calendar ID="calDueDateEarliest" runat="server" DataBindField="txtDueDateEarliest"/>
            </td>
            <td>
                <asp:Label runat="server" ID="lblDueDateLatest" Text="Due date, latest"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtDueDateLatest" runat="server" CssClass="TextInput" Columns="10" MaxLength="10"></asp:TextBox>
                <cm:Calendar ID="calDueDateLatest" runat="server" DataBindField="txtDueDateLatest"/>
            </td>                          
        </tr>
        <tr>
            <td class="normTxt"><asp:Label runat="server" ID="lblConnectionCode" Text="Connection code"></asp:Label></td>
            <td><asp:TextBox ID="txtConnectionCode" runat="server" CssClass="TextInput"/></td>
            <td><asp:Label runat="server" ID="lblSalesproject" Text="Salesproject in CAB"></asp:Label></td>
            <td><asp:TextBox ID="txtSalesproject" runat="server" CssClass="TextInput"/></td>
        </tr>        
        <tr>
            <td class="normTxt"><asp:Label runat="server" ID="lblContract" Text="Contract"></asp:Label></td>
            <td><asp:TextBox ID="txtContract" runat="server" CssClass="TextInput"/></td>
            <td colspan="2"></td>
        </tr>
        <tr><td><br/></td></tr>
        <tr><td colspan="4" class="SearchParameterTableSubHeader"><asp:Label ID="lblCustomerHeader" runat="server" Text="Customer"></asp:Label></td></tr>
        <tr>
            <td class="normTxt"><asp:Label runat="server" ID="lblCustomerNumber" Text="Customer number"></asp:Label></td>
            <td><asp:TextBox ID="txtCustomerNumber" runat="server" CssClass="TextInput"/></td>
            <td><asp:Label runat="server" ID="lblCustomerName" Text="Customer name"></asp:Label></td>
            <td><asp:TextBox ID="txtCustomerName" runat="server" CssClass="TextInput"/></td>
        </tr>
        <tr>
            <td class="normTxt"><asp:Label runat="server" ID="lblContactPersonCode" Text="Contact person code"></asp:Label></td>
            <td><asp:TextBox ID="txtContactPersonCode" runat="server" CssClass="TextInput"/></td>
            <td><asp:Label runat="server" ID="labelContactPersonName" Text="Contact person name"></asp:Label></td>
            <td><asp:TextBox ID="txtContactPersonName" runat="server" CssClass="TextInput"/></td>
        </tr>
        <tr>
            <td class="normTxt"><asp:Label runat="server" ID="lblContactPersonPhone" Text="Contact person phone"></asp:Label></td>
            <td><asp:TextBox ID="txtContactPersonPhone" runat="server" CssClass="TextInput"/></td>
            <td><asp:Label runat="server" ID="lblContactPersonEmail" Text="Contact person email"></asp:Label></td>
            <td><asp:TextBox ID="txtContactPersonEmail" runat="server" CssClass="TextInput"/></td>
        </tr>
        <tr><td><br/></td></tr>
        <tr><td colspan="4" class="SearchParameterTableSubHeader"><asp:Label ID="lblDeliverySiteHeader" runat="server" Text="Deliverysite"></asp:Label></td></tr>
        <tr>
            <td class="normTxt"><asp:Label runat="server" ID="lblDelSiteCode" Text="Delivery site code"></asp:Label></td>
            <td><asp:TextBox ID="txtlDelSiteCode" runat="server" CssClass="TextInput"/></td>
            <td colspan="2"></td>
        </tr>       
        <tr>
            <td class="normTxt"><asp:Label runat="server" ID="lblStreetAddress" Text="Street address"></asp:Label></td>
            <td><asp:TextBox ID="txtStreetAddress" runat="server" CssClass="TextInput"/></td>
            <td  colspan="2">
                <asp:Label runat="server" ID="lblStreetNr" Text="Number"></asp:Label> <asp:TextBox ID="txtStreetNumber" runat="server" CssClass="TinyTextInput"/>
                <asp:Label runat="server" ID="lblStreetFlat" Text="Flat"></asp:Label> <asp:TextBox ID="TxtFlatNr" runat="server" CssClass="TinyTextInput"/>
                <asp:Label runat="server" ID="lblStreetFloor" Text="Floor"></asp:Label> <asp:TextBox ID="TxtFloor" runat="server" CssClass="TinyTextInput"/>
            </td>
            <td><br/></td>
        </tr>
        <tr>
            <td class="normTxt"><asp:Label runat="server" ID="lblPostCode" Text="Postal code"></asp:Label></td>
            <td><asp:TextBox ID="txtPostalCode" runat="server" CssClass="TextInput"/></td>
            <td><asp:Label runat="server" ID="lblPostOffice" Text="City"></asp:Label></td>
            <td><asp:TextBox ID="txtPostOffice" runat="server" CssClass="TextInput"/></td>
        </tr>        
        <tr><td><br/></td></tr>
    </table>
    <asp:Table ID="ErrandParameterTable" runat="server" CssClass="SearchParameterTable" Width="100%">
        <asp:TableRow>
            <asp:TableCell ColumnSpan="4" CssClass="SearchParameterTableSubHeader">
                <asp:Label ID="Label1" runat="server" Text="Parameters"></asp:Label>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>   
    <table width="100%" class="SearchParameterTable">
        <tr align="right">
            <td>
                <asp:Button ID="cmdFetch" Text="Search" runat="server" OnClick="CmdFetch_Click" />
                <asp:Button ID="cmdClear" Text="Clear" runat="server" />          
            </td>
        </tr>
    </table>        
    <br />    
    <cm:ColumnOrderCustomizer ID="columnOrderCustomizer" runat="server" OnLoad="ColumnOrderCustomizer_Load"/>
    <asp:GridView ID="gvErrandList" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" EnableSortingAndPagingCallbacks="True" SkinID="gvResultList" meta:resourcekey="gvCustomerListResource1" Visible="false" OnSelectedIndexChanged="gvErrandList_SelectedIndexChanged"></asp:GridView>
     
    </fieldset> 
</asp:Content>