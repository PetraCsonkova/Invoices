using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using TietoEnator.WebFW;
using TietoEnator.WebFW.CM;
using TietoEnator.WebFW.CM.Controls;

public partial class Pages_invoice_Invoices : CMPage
{
    //20080514 marttmik : added a variable used for getting company specific images / styles etc.
    public string COMPANY_CODE; 
    private const string PROFILE_ERRANDLIST = "GridViewColumnOrders.ErrandListGridView";
    private const string DDL_ERRAND_TYPES = "ErrandTypes";
    private const string DDL_ERRAND_STATUSES = "ErrandStatuses";
    private const string DDL_ERRAND_CHANNELS = "ErrandChannels";
    private const string DDL_RESPONSIBLE_AREAS = "ResponsibleAreas";

    private ColumnOrderList columnOrderToBeUsed;    

    protected override void PageLoadAction(object sender, EventArgs e)
    {
        COMPANY_CODE = Session["__CompanyCode__"].ToString();
        cache.Session["Errand"] = null;
        // If the column order list is found from the page cache then we use that one,
        // otherwise we'll fetch it from the profile.
        ColumnOrderList sessionColumnOrderlist = (cache.Page[PROFILE_ERRANDLIST]) as ColumnOrderList;
        if (sessionColumnOrderlist == null)
        {
            columnOrderToBeUsed = columnOrderCustomizer.GetColumnOrderFromProfile(PROFILE_ERRANDLIST);
            UpdateColumnOrderInCache(columnOrderToBeUsed);
        }
        else columnOrderToBeUsed = sessionColumnOrderlist;

        FillDropDownMenus();
        PopulateErrandParameters();
    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        DoDataBindingForErrandList(this.columnOrderToBeUsed);        
    }
    
    private void DoDataBindingForErrandList(ColumnOrderList columnOrderList)
    {
        List<SearchErrandResultVO> errandlist = cache.Page["ErrandList"] as List<SearchErrandResultVO>;
        if (errandlist != null)
        {
            if (columnOrderList == null || columnOrderList.Count < 1) AddColumnsInCustomOrder(gvErrandList, GetDefaultColumnOrder());
            else AddColumnsInCustomOrder(gvErrandList, columnOrderList);
            
            gvErrandList.DataKeyNames = new String[] { "ErrandId" };
            gvErrandList.DataSource = errandlist;
            gvErrandList.DataBind();
            gvErrandList.Visible = true;           
            
            // if the column order customizer is visible then we add data for it
            if (columnOrderCustomizer.Visible)
            {
                columnOrderCustomizer.ColumnOrderList = columnOrderCustomizer.GetColumnOrder(gvErrandList);
            }
            //marttmik 151107 : added check for showing search results ( SCROLL | DEFAULT )
            int maxNumOfInvoices;
            int.TryParse(ConfigurationManager.AppSettings["MaxNumberOfInvoices"], out maxNumOfInvoices);
            CMCommonUtils.CheckGVListStyle(gvErrandList, maxNumOfInvoices);
        }
    }

    /// <summary>
    /// Fetches and fills data to drop down lists.
    /// </summary>
    private void FillDropDownMenus()
    {
        // Errand type
        Hashtable errandTypes = cache.Process[DDL_ERRAND_TYPES] as Hashtable;
        if (errandTypes == null || ddlErrandType.Items.Count<1)
        {
            errandTypes = new Hashtable();
            errandTypes.Add("1", "");
            errandTypes.Add("2", "Type1");
            errandTypes.Add("3", "Type2");
            ddlErrandType.Items.Clear();
            ddlErrandType.DataTextField = "value";
            ddlErrandType.DataValueField = "key";
            ddlErrandType.DataSource = errandTypes;
            ddlErrandType.DataBind();
            cache.Process[DDL_ERRAND_TYPES] = errandTypes;
        } 

        // Errand status
        Hashtable errandStatuses = cache.Process[DDL_ERRAND_STATUSES] as Hashtable;
        if (errandStatuses == null || ddlErrandStatus.Items.Count < 1)
        {
            errandStatuses = new Hashtable();
            errandStatuses.Add("1", "");
            errandStatuses.Add("2", "Active");
            errandStatuses.Add("3", "Closed");
            ddlErrandStatus.Items.Clear();
            ddlErrandStatus.DataTextField = "value";
            ddlErrandStatus.DataValueField = "key";
            ddlErrandStatus.DataSource = errandStatuses;
            ddlErrandStatus.DataBind();
            cache.Process[DDL_ERRAND_STATUSES] = errandStatuses;
        }

        // Errand channels
        Hashtable errandChannels = cache.Process[DDL_ERRAND_CHANNELS] as Hashtable;
        if (errandChannels == null || ddlChannel.Items.Count < 1)
        {
            errandChannels = new Hashtable();
            errandChannels.Add("1", "");
            errandChannels.Add("2", "E-mail");
            errandChannels.Add("3", "Telephone");
            ddlChannel.Items.Clear();
            ddlChannel.DataTextField = "value";
            ddlChannel.DataValueField = "key";
            ddlChannel.DataSource = errandChannels;
            ddlChannel.DataBind();
            cache.Process[DDL_ERRAND_CHANNELS] = errandChannels;
        }

        // Responsible areas
        Hashtable reponsibleAreas = cache.Process[DDL_RESPONSIBLE_AREAS] as Hashtable;
        if (reponsibleAreas == null || ddlResponsibleArea.Items.Count < 1)
        {
            reponsibleAreas = new Hashtable();
            reponsibleAreas.Add("1", "");
            reponsibleAreas.Add("2", "Area1");
            reponsibleAreas.Add("3", "Area2");
            ddlResponsibleArea.Items.Clear();
            ddlResponsibleArea.DataTextField = "value";
            ddlResponsibleArea.DataValueField = "key";
            ddlResponsibleArea.DataSource = reponsibleAreas;
            ddlResponsibleArea.DataBind();
            cache.Process[DDL_RESPONSIBLE_AREAS] = reponsibleAreas;
        }
    }

    /// <summary>
    /// Populates the errand parameters table.
    /// </summary>
    private void PopulateErrandParameters()
    {
        // If type is not selected then we'll exit this method
        if (ddlErrandType.SelectedIndex < 1) return;

        int j = 1;
        for (int i = 0; i < 2; i++)
        {                               
            TableRow tr = new TableRow();
            
            TableCell paramCode1 = new TableCell();
            paramCode1.Text = "Test parameter "+j;
            TableCell paramValue1 = new TableCell();           
            TextBox txtParamValue1 = new TextBox();
            txtParamValue1.CssClass = "TextInput";
            paramValue1.Controls.Add(txtParamValue1);
            tr.Cells.Add(paramCode1);
            tr.Cells.Add(paramValue1);
            j++;

            TableCell paramCode2 = new TableCell();
            paramCode2.Text = "Test parameter "+j;
            TableCell paramValue2 = new TableCell();
            TextBox txtParamValue2 = new TextBox();
            txtParamValue2.CssClass = "TextInput";
            paramValue2.Controls.Add(txtParamValue2);
            tr.Cells.Add(paramCode2);
            tr.Cells.Add(paramValue2);
            j++;

            ErrandParameterTable.Rows.Add(tr);
        }
    }

    protected void CmdFetch_Click(object sender, EventArgs e)
    {
        //List<SearchErrandResultVO> errands = ErrandControl.FetchErrandList(this);
        //cache.Page["ErrandList"] = errands;
        //DoDataBindingForErrandList(columnOrderCustomizer.GetColumnOrderFromProfile(PROFILE_ERRANDLIST));
    }  
    
    protected void gvErrandList_SelectedIndexChanged(object sender, EventArgs e)
    {      
        long? selectedErrandId = (long?)gvErrandList.SelectedValue;
        ErrandDataVO errandVO = ErrandControl.FetchErrand(this.connect, selectedErrandId);
        cache.Page["Errand"] = errandVO;
        if (errandVO != null)
        {
            app.Process.GoNextPage(Context);
        }
        else
        {
            // TODO: Localize message
            AddErrorMessage("The selected errand was not found. Another user may have deleted it.");
        }        
    }

    /// <summary>
    /// Adds columns to GridView in custom order. 
    /// This method has to be implemented for every GridView that has user customizable column order.
    /// </summary>
    /// <param name="colOrderList">Defines the order of columns.</param>
    private void AddColumnsInCustomOrder(GridView gridview, ColumnOrderList colOrderList)
    {               
        // TODO: Localization to header texts

        gridview.Columns.Clear();
                
        // Static fields
        CustomButtonField cbtField = new CustomButtonField();
        cbtField.HeaderID = null; // not a user customizable field       
        //cbtField.HeaderText = "<a href=\"javascript:showOrderCustomizer('divColumnOrderCustomizer')\"><img src='/Resources/img/properties.gif' style='border-width:0px' alt='Properties'/></a>";
        cbtField.HeaderText = "<a href=\"javascript:showOrderCustomizer('" + columnOrderCustomizer.showColumnOrderCustomizer + "', '" + columnOrderCustomizer.ID + "')\"><img src='/Resources/img/properties.gif' style='border-width:0px' alt='Properties'/></a>";
        cbtField.ButtonType = ButtonType.Link; // Image button cannot be used because of the ASP.NET bug (action handler get's called twice)
        cbtField.CommandName = "Select";       
        cbtField.Visible = true;
        cbtField.Text = "<img src='/Resources/img/tablearrows/" + COMPANY_CODE + "/right_arrow.gif' style='border-width:0px'/>";
        gridview.Columns.Add(cbtField);      
        
        // User customizable fields
        foreach (ColumnOrderInfo colOrder in colOrderList)
        {
            switch (colOrder.ID)
            {                               
                case "NUMBER":                           
                    CustomBoundField cbf1 = new CustomBoundField();                 
                    cbf1.HeaderID = "NUMBER";
                    cbf1.HeaderText = "Number";
                    cbf1.DataField = "ErrandId";
                    cbf1.SortExpression = "Number";
                    cbf1.Visible = colOrder.Visible;
                    gridview.Columns.Add(cbf1);                    
                    break;

                case "TYPE":
                    CustomBoundField cbf2 = new CustomBoundField();
                    cbf2.HeaderID = "TYPE";
                    cbf2.HeaderText = "Type";
                    cbf2.DataField = "ErrandType";
                    cbf2.SortExpression = "Type";
                    cbf2.Visible = colOrder.Visible;
                    gridview.Columns.Add(cbf2);
                    break;
                
                case "STATUS":
                    CustomBoundField cbf3 = new CustomBoundField();
                    cbf3.HeaderID = "STATUS";
                    cbf3.HeaderText = "Status";
                    cbf3.DataField = "Status";
                    cbf3.SortExpression = "Status";
                    cbf3.Visible = colOrder.Visible;
                    gridview.Columns.Add(cbf3);
                    break;

                case "CHANNEL":
                    CustomBoundField cbf4 = new CustomBoundField();
                    cbf4.HeaderID = "CHANNEL";
                    cbf4.HeaderText = "Channel";
                    cbf4.DataField = "Channel";
                    cbf4.SortExpression = "Channel";
                    cbf4.Visible = colOrder.Visible;
                    gridview.Columns.Add(cbf4);
                    break;

                case "CUSTNAME":
                    CustomBoundField cbf5 = new CustomBoundField();
                    cbf5.HeaderID = "CUSTNAME";
                    cbf5.HeaderText = "Cust. name";
                    cbf5.DataField = "CustomerName";
                    cbf5.SortExpression = "Cust. name";
                    cbf5.Visible = colOrder.Visible;
                    gridview.Columns.Add(cbf5);
                    break;

                case "HANDLER":
                    CustomBoundField cbf6 = new CustomBoundField();
                    cbf6.HeaderID = "HANDLER";
                    cbf6.HeaderText = "Handler";
                    cbf6.DataField = "Handler";
                    cbf6.SortExpression = "Handler";
                    cbf6.Visible = colOrder.Visible;
                    gridview.Columns.Add(cbf6);
                    break;

                case "CREATION_DATE":
                    CustomBoundField cbf7 = new CustomBoundField();
                    cbf7.HeaderID = "CREATION_DATE";
                    cbf7.HeaderText = "Cre. date";
                    cbf7.DataField = "ErrandDate";
                    cbf7.SortExpression = "Cre. date";
                    cbf7.Visible = colOrder.Visible;
                    gridview.Columns.Add(cbf7);
                    break;
                
                case "DUE_DATE":
                    CustomBoundField cbf8 = new CustomBoundField();
                    cbf8.HeaderID = "DUE_DATE";
                    cbf8.HeaderText = "Due date";
                    cbf8.DataField = "TimeoutDate";
                    cbf8.SortExpression = "Due date";
                    cbf8.Visible = colOrder.Visible;
                    gridview.Columns.Add(cbf8);
                    break;

                case "RESPONSIBLE_AREA":
                    CustomBoundField cbf9 = new CustomBoundField();
                    cbf9.HeaderID = "RESPONSIBLE_AREA";
                    cbf9.HeaderText = "Resp. area";
                    cbf9.DataField = "ResponsibleArea";
                    cbf9.SortExpression = "Resp. area";
                    cbf9.Visible = colOrder.Visible;
                    gridview.Columns.Add(cbf9);
                    break;

                case "DELSITEADDRESS":
                    CustomBoundField cbf10 = new CustomBoundField();
                    cbf10.HeaderID = "DELSITEADDRESS";
                    cbf10.HeaderText = "Del. site addr.";
                    cbf10.DataField = "DeliverySiteAddress";
                    cbf10.SortExpression = "Del. site addr.";
                    cbf10.Visible = colOrder.Visible;
                    gridview.Columns.Add(cbf10);
                    break;

                case "ATTACHMENT":
                    CustomBoundField cbf11 = new CustomBoundField();
                    cbf11.HeaderID = "ATTACHMENT";
                    cbf11.HeaderText = "Appendix";
                    cbf11.DataField = "Filename";
                    cbf11.SortExpression = "Appendix";
                    cbf11.Visible = colOrder.Visible;
                    gridview.Columns.Add(cbf11);
                    break;

                case "CUSTNUM":
                    CustomBoundField cbf12 = new CustomBoundField();
                    cbf12.HeaderID = "CUSTNUM";
                    cbf12.HeaderText = "Cust. num";
                    cbf12.DataField = "CustomerNumber";
                    cbf12.SortExpression = "Cust. num";
                    cbf12.Visible = colOrder.Visible;
                    gridview.Columns.Add(cbf12);
                    break;
                
                case "HEADER":
                    CustomBoundField cbf13 = new CustomBoundField();
                    cbf13.HeaderID = "HEADER";
                    cbf13.HeaderText = "Header";
                    cbf13.DataField = "Header";
                    cbf13.SortExpression = "Header";
                    cbf13.Visible = colOrder.Visible;
                    gridview.Columns.Add(cbf13);
                    break;

                default:
                    throw new Exception("Unknown id in the column order list.");
            }
        }        
    }

    /// <summary>
    /// Returns the default order of the user customizable columns in the GridView.
    /// This method has to be implemented for every GridView that has user customizable column order.
    /// </summary>
    /// <returns>ColumnOrderList (generic list) containing ColumnOrderInfo objects.</returns>
    private ColumnOrderList GetDefaultColumnOrder()
    {
        ColumnOrderList colOrderList = new ColumnOrderList();
        colOrderList.Add(new ColumnOrderInfo("NUMBER", true));
        colOrderList.Add(new ColumnOrderInfo("TYPE", true));
        colOrderList.Add(new ColumnOrderInfo("STATUS", true));
        colOrderList.Add(new ColumnOrderInfo("HEADER", true));
        colOrderList.Add(new ColumnOrderInfo("HANDLER", true));
        colOrderList.Add(new ColumnOrderInfo("CUSTNUM", true));
        colOrderList.Add(new ColumnOrderInfo("CUSTNAME", true));
        colOrderList.Add(new ColumnOrderInfo("CHANNEL", true));
        colOrderList.Add(new ColumnOrderInfo("RESPONSIBLE_AREA", true));
        colOrderList.Add(new ColumnOrderInfo("CREATION_DATE", true));
        colOrderList.Add(new ColumnOrderInfo("DUE_DATE", true));
        colOrderList.Add(new ColumnOrderInfo("DELSITEADDRESS", true));
        colOrderList.Add(new ColumnOrderInfo("ATTACHMENT", true));        
        return colOrderList;
    }
    
    protected void ColumnOrderCustomizer_Load(object sender, EventArgs e)
    {
        columnOrderCustomizer.SaveButtonPressedEventHandler += new EventHandler(columnOrderCustomizer_SaveButtonPressedEventHandler);
        columnOrderCustomizer.DefaultsButtonPressedEventHandler += new EventHandler(columnOrderCustomizer_DefaultsButtonPressedEventHandler);
    }

    protected void ImbCustomizeOrder_Click(object sender, EventArgs e)
    {        
        columnOrderCustomizer.Visible = true;
    }

    void columnOrderCustomizer_SaveButtonPressedEventHandler(object sender, EventArgs e)
    {
        ColumnOrderCustomizerEventArgs colOrderEventArgs = e as ColumnOrderCustomizerEventArgs;
        ColumnOrderList columnOrderList = colOrderEventArgs.ColOrderList;
        if (columnOrderList == null || columnOrderList.Count < 1) columnOrderList = GetDefaultColumnOrder();
        columnOrderCustomizer.SaveColumnOrderToProfile(columnOrderList, PROFILE_ERRANDLIST);
        UpdateColumnOrderInCache(columnOrderList);
    }

    void columnOrderCustomizer_DefaultsButtonPressedEventHandler(object sender, EventArgs e)
    {
        AddColumnsInCustomOrder(gvErrandList, GetDefaultColumnOrder());
        ColumnOrderList columnOrderList = GetDefaultColumnOrder();
        columnOrderCustomizer.SaveColumnOrderToProfile(columnOrderList, PROFILE_ERRANDLIST);
        UpdateColumnOrderInCache(columnOrderList);        
    }

    private void UpdateColumnOrderInCache(ColumnOrderList columnOrderList)
    {
        cache.Page[PROFILE_ERRANDLIST] = columnOrderList;
        columnOrderToBeUsed = columnOrderList;
    }

    protected void ddlErrandType_SelectedIndexChanged(object sender, EventArgs e)
    {
        // No need to do anything here at the moment. Exists just to make sure that postback will be done.
    }
}