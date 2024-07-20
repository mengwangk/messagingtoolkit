<%@ Page  MasterPageFile="~/MasterPage.master" Title="New GrpMain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>New GrpMain</h1>      

<asp:DetailsView runat="server" DefaultMode="Insert" PageIndex="-1" DataSourceID="GrpMainsDataSource" EmptyDataText="The requested record was not found." ID="GrpMainsDetailsView" AutoGenerateRows="False" DataKeyNames="Groupid"><Fields>
<asp:TemplateField SortExpression="Username" HeaderText="Username"><InsertItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllUsers" ID="User_UsernameDataSource" DataObjectTypeName="bulksms.User" UpdateMethod="Update" TypeName="bulksms.User" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="User_UsernameList" DataTextField="Username" DataValueField="Username" SelectedValue='<%# Bind("Username") %>' DataSourceID="User_UsernameDataSource" >
</asp:DropDownList>
</InsertItemTemplate>
</asp:TemplateField>
<asp:BoundField ReadOnly="True" DataField="Groupid" HeaderText="Groupid"></asp:BoundField>
<asp:BoundField DataField="Groupname" HeaderText="Groupname"></asp:BoundField>
<asp:BoundField DataField="Detail" HeaderText="Detail"></asp:BoundField>
<asp:BoundField DataField="Grouporder" HeaderText="Grouporder"></asp:BoundField>
<asp:HyperLinkField Text="View GrpDetails" DataNavigateUrlFormatString="~/GrpDetails.aspx?Table=GrpMain_GrpDetails&amp;GrpDetails_Groupid={0}" DataNavigateUrlFields="Groupid,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View MsgDetails" DataNavigateUrlFormatString="~/MsgDetails.aspx?Table=GrpMain_MsgDetails&amp;MsgDetails_Groupid={0}" DataNavigateUrlFields="Groupid,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View MsgHistories" DataNavigateUrlFormatString="~/MsgHistories.aspx?Table=GrpMain_MsgHistories&amp;MsgHistories_Groupid={0}" DataNavigateUrlFields="Groupid,"></asp:HyperLinkField>
<asp:CommandField CancelText="Clear Values" ShowInsertButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetGrpMain" ID="GrpMainsDataSource" DataObjectTypeName="bulksms.GrpMain" UpdateMethod="Update" TypeName="bulksms.GrpMain" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="Groupid" QueryStringField="Groupid"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>
